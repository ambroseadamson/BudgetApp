import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

export class MinMaxDate {
  minDate: Date;
  maxDate: Date;
}

@Injectable({
  providedIn: "root"
})
export class MinMaxDateService {
  private dataSource = new BehaviorSubject({
    minDate: new Date(),
    maxDate: new Date()
  });
  currentData = this.dataSource.asObservable();

  constructor() {}

  changeMinMaxDate(message: MinMaxDate) {
    this.dataSource.next(message);
  }
}
