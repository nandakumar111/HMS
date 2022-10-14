import { Component, Inject, ViewChild } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {RoomType} from "../list/room-list.component";
import {Router} from "@angular/router";
import { UntypedFormGroup, UntypedFormControl, Validators } from '@angular/forms';
import {NgbAlert} from '@ng-bootstrap/ng-bootstrap';
import {Subject} from 'rxjs';
import {debounceTime} from 'rxjs/operators';

@Component({
  selector: 'room-add',
  templateUrl: './room-add.component.html',
})
export class AddRoomComponent {
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router) {}
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

  roomTypeKeys() : Array<any>{
    return [{text : "Single", key : 0}, {text : "Double", key : 1}];
  }

  addRoomForm = new UntypedFormGroup({
    number: new UntypedFormControl('', [Validators.required, Validators.maxLength(15)]),
    type: new UntypedFormControl(RoomType.SINGLE)
  });

  onFormSubmit(): void {
    this.http.post<ActionResponse>(this.baseUrl + 'api/v1/Room', this.addRoomForm.getRawValue()).subscribe(res => {
      if(res.data){
        this._success.next(res.data.message);
      }else{
        this._fail.next(res.errorInfo.message);
      }
    }, error => this._fail.next(error.errorInfo.message));
  }
}

export interface ActionResponse {
  data: Data;
  errorInfo: Data,
  errors: any,
  title: string,
}

export interface Data {
  message: string,
  statusCode: number
}
