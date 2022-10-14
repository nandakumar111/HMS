import {Component, Inject, ViewChild} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {FormControl, UntypedFormControl, UntypedFormGroup, Validators} from "@angular/forms";
import {RoomType} from "../room/list/room-list.component";
import {Router} from "@angular/router";
import {Subject} from "rxjs";
import {NgbAlert, NgbDate, NgbCalendar, NgbDateParserFormatter} from "@ng-bootstrap/ng-bootstrap";
import {debounceTime} from "rxjs/operators";
import {ActionResponse} from "../room/add/room-add.component";

@Component({
  selector: 'booking-add',
  templateUrl: './booking-add.component.html',
  styles: [`
    .dp-hidden {
      width: 0;
      margin: 0;
      border: none;
      padding: 0;
    }
    .custom-day {
      text-align: center;
      padding: 0.185rem 0.25rem;
      display: inline-block;
      height: 2rem;
      width: 2rem;
    }
    .custom-day.focused {
      background-color: #e6e6e6;
    }
    .custom-day.range, .custom-day:hover {
      background-color: rgb(2, 117, 216);
      color: white;
    }
    .custom-day.faded {
      background-color: rgba(2, 117, 216, 0.5);
    }
  `]
})
export class AddBookingComponent {
  hoveredDate: NgbDate | null = null;

  fromDate: NgbDate | null;
  toDate: NgbDate | null;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, private calendar: NgbCalendar, public formatter: NgbDateParserFormatter) {
    this.fromDate = calendar.getToday();
    this.toDate = calendar.getNext(calendar.getToday(), 'd', 10);
  }
  private _success = new Subject<string>();
  private _fail = new Subject<string>();

  @ViewChild('successMessageAlert', {static: false}) successMessageAlert: NgbAlert | undefined;
  @ViewChild('failMessageAlert', {static: false}) failMessageAlert: NgbAlert | undefined;

  successMessage = '';
  failedMessage = '';

  ngOnInit(): void {
    this._success.subscribe(message => this.successMessage = message);
    this._success.pipe(debounceTime(5000)).subscribe(() => {
      if (this.successMessageAlert) {
        this.successMessageAlert.close();
      }
    });
    this._fail.subscribe(message => this.failedMessage = message);
    this._fail.pipe(debounceTime(5000)).subscribe(() => {
      if (this.failMessageAlert) {
        this.failMessageAlert.close();
      }
    });
  }

  onDateSelection(date: NgbDate) {
    if (!this.fromDate && !this.toDate) {
      this.fromDate = date;
    } else if (this.fromDate && !this.toDate && date && date.after(this.fromDate)) {
      this.toDate = date;
    } else {
      this.toDate = null;
      this.fromDate = date;
    }
  }

  isHovered(date: NgbDate) {
    return this.fromDate && !this.toDate && this.hoveredDate && date.after(this.fromDate) &&
      date.before(this.hoveredDate);
  }

  isInside(date: NgbDate) { return this.toDate && date.after(this.fromDate) && date.before(this.toDate); }

  isRange(date: NgbDate) {
    return date.equals(this.fromDate) || (this.toDate && date.equals(this.toDate)) || this.isInside(date) ||
      this.isHovered(date);
  }

  validateInput(currentValue: NgbDate | null, input: string): NgbDate | null {
    const parsed = this.formatter.parse(input);
    return parsed && this.calendar.isValid(NgbDate.from(parsed)) ? NgbDate.from(parsed) : currentValue;
  }

  roomTypeKeys() : Array<any>{
    return [{text : "Single", key : 0}, {text : "Double", key : 1}];
  }

  addBookingForm = new UntypedFormGroup({
    userEmail: new UntypedFormControl('',[Validators.required, Validators.maxLength(50)]),
    from: new FormControl(),
    to: new FormControl(),
    roomType: new UntypedFormControl(RoomType.SINGLE)
  });

  dateFormatToUnixTime(date: NgbDate | null){
    if(!date)
      return Math.floor(new Date().getTime() / 1000)
    let dateObj = new Date();
    dateObj.setFullYear(date.year, date.month - 1, date.day);
    dateObj.setHours(0,0,0,0);
    return Math.floor(dateObj.getTime()/1000)
  }

  onFormSubmit(): void {
    let reqData = this.addBookingForm.getRawValue();
    if(!this.fromDate|| !this.toDate || !reqData.userEmail){
      this._fail.next("Fill the form");
      return;
    }

    reqData.from = this.dateFormatToUnixTime(this.fromDate);
    reqData.to = this.dateFormatToUnixTime(this.toDate);

    this.http.post<ActionResponse>(this.baseUrl + 'api/v1/Booking', reqData).subscribe(res => {
      if(res.data){
        this._success.next(res.data.message);
      }else{
        this._fail.next(res.errorInfo.message);
      }
    }, error => {
      if(error.error){
        this._fail.next(error.error.title);
      }else{
        this._fail.next(error.errorInfo.message);
      }
    });
  }
}
