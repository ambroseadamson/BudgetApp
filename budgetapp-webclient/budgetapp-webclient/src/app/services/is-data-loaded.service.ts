import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class IsDataLoadedService {
  private dataSource = new BehaviorSubject(false);
  currentData = this.dataSource.asObservable();

  constructor() {}

  changeIsDataLoaded(message: boolean) {
    this.dataSource.next(message);
  }
}
