import {Component, Inject, ViewChild} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import {FormControl, FormGroup} from "@angular/forms";
import {NgbAlert, NgbDate} from "@ng-bootstrap/ng-bootstrap";
import {Subject} from "rxjs";
import {debounceTime} from "rxjs/operators";

@Component({
  selector: 'room-list',
  templateUrl: './room-list.component.html'
})

export class RoomListComponent {
  public roomList: BookedRoom[] = [];
  public fetching: boolean = false;
  model: any;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router) {
    let today = new Date();
    today.setHours(0,0,0,0);
    this.roomListServiceReq(today);
  }
  private _fetching = new Subject<string>();

  @ViewChild('fetchingMessageAlert', {static: false}) fetchingMessageAlert: NgbAlert | undefined;

  fetchingMessage = '';

  ngOnInit(): void {
    this._fetching.subscribe(message => this.fetchingMessage = message);
    this._fetching.pipe(debounceTime(5000)).subscribe(() => {
      if (this.fetchingMessageAlert) {
        this.fetchingMessageAlert.close();
      }
    });
  }

  getToDay(){
    return new Date().toLocaleDateString()
  }

  roomListServiceReq(date: Date){
    this._fetching.next("fetching...");
    this.http.get<BookedRoom[]>(`${this.baseUrl}api/v1/Room?date=${Math.floor(date.getTime()/1000)}`).subscribe(result => {
      this.roomList = result;
    },error => console.error(error));
    this._fetching.next("Room(s) fetched successfully");
  }

  goToAddRoom($myParam: string = ''): void {
    const navigationDetails: string[] = ['/room/new'];
    if($myParam.length) {
      navigationDetails.push($myParam);
    }
    this.router.navigate(navigationDetails).then();
  }

  roomTypeGetString(key: number) {
    let types = ["Single", "Double"];
    return types[key];
  }

  getRoomStatus(booked: boolean){
    return booked ? "Booked" : "Not Booked"
  }

  roomListFilter = new FormGroup({
    date: new FormControl(new NgbDate(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())),
  });

  getRoomList(){
    let selectDate = this.roomListFilter.get("date")?.value;
    if(!selectDate)
      return
    let dateObj = new Date();
    dateObj.setFullYear(selectDate.year, selectDate.month - 1, selectDate.day);
    dateObj.setHours(0,0,0,0);
    this.roomListServiceReq(dateObj);
  }
}

export enum RoomType {
  SINGLE,
  DOUBLE
}

export interface Room {
  id: string;
  number: string;
  type: RoomType;
}

export interface BookedRoom {
  id: string;
  number: string;
  type: RoomType;
  booked: boolean;
}
