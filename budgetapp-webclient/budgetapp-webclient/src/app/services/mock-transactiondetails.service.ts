import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { Inputdata } from "../models/inputdata";
import {
  Transactiondetails,
  TransactionSummaryData
} from "../models/transactiondetails";

@Injectable({
  providedIn: "root"
})
export class MockTransactiondetailsService {
  serverPort = "4200";

  // POST
  // GET
  summaryURL = `http://localhost:${this.serverPort}/api/TransactionSummary`;

  transactionsByGroupTypeRoute = "TransactionsByGroupType";
  summaryByDateRangeRoute = "SummaryByDateRange";

  constructor(private httpClient: HttpClient) {}

  getTransactionDetails(
    detailsType: string,
    startDate: string,
    endDate: string
  ): Observable<Transactiondetails> {
    const options = {
      params: new HttpParams()
        .set("grouptype", detailsType)
        .set("startdate", startDate)
        .set("enddate", endDate)
    };

    const summaryByTypeURL = `${this.summaryURL}/${detailsType}.json`;
    return this.httpClient.get<Transactiondetails>(summaryByTypeURL, options);
  }

  getTransactionSummaryByDateRange(
    detailsType: string,
    startDate: string,
    endDate: string
  ): Observable<TransactionSummaryData> {
    const options = {
      params: new HttpParams()
        .set("grouptype", detailsType)
        .set("startdate", startDate)
        .set("enddate", endDate)
    };

    const summaryByDateRangeURL = `${this.summaryURL}.json`;
    return this.httpClient.get<TransactionSummaryData>(
      summaryByDateRangeURL,
      options
    );
  }

  getTransactionSummary(): Observable<TransactionSummaryData> {
    const url = `${this.summaryURL}.json`;
    return this.httpClient.get<TransactionSummaryData>(url);
  }

  importData(data: Inputdata): Observable<TransactionSummaryData> {
    const url = `${this.summaryURL}.json`;
    return this.httpClient.get<TransactionSummaryData>(url);
  }
}
