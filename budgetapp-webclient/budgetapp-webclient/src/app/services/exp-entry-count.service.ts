import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class ExpEntryCountService {
  private dataSource = new BehaviorSubject(0);
  currentData = this.dataSource.asObservable();

  constructor() {}

  changeExpEntryCount(message: number) {
    this.dataSource.next(message);
  }
}
