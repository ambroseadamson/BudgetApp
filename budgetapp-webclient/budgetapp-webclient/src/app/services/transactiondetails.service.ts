import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { concatMap } from "rxjs/operators";
import { Inputdata } from "../models/inputdata";
import {
  Transactiondetails,
  TransactionSummaryData
} from "../models/transactiondetails";

@Injectable()
export class TransactiondetailsService {
  //  serverPort = "4200";
  serverPort = "5000";

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
    const summaryByTypeURL = `${this.summaryURL}/${
      this.transactionsByGroupTypeRoute
    }`;

    const options = {
      params: new HttpParams()
        .set("grouptype", detailsType)
        .set("startdate", startDate)
        .set("enddate", endDate)
    };

    // const summaryByTypeURL = `${this.summaryURL}/${detailsType}.json`;
    return this.httpClient.get<Transactiondetails>(summaryByTypeURL, options);
  }

  getTransactionSummaryByDateRange(
    detailsType: string,
    startDate: string,
    endDate: string
  ): Observable<TransactionSummaryData> {
    const summaryByDateRangeURL = `${this.summaryURL}/${
      this.summaryByDateRangeRoute
    }`;

    const options = {
      params: new HttpParams()
        .set("grouptype", detailsType)
        .set("startdate", startDate)
        .set("enddate", endDate)
    };

    // const summaryByDateRangeURL = `${this.summaryURL}.json`;
    return this.httpClient.get<TransactionSummaryData>(
      summaryByDateRangeURL,
      options
    );
  }

  getTransactionSummary(): Observable<TransactionSummaryData> {
    const url = `${this.summaryURL}`;
    // const url = `${this.summaryURL}.json`;
    return this.httpClient.get<TransactionSummaryData>(url);
  }

  importData(data: Inputdata): Observable<TransactionSummaryData> {
    const httpOptions = {
      headers: new HttpHeaders()
        .set("Content-Type", "application/json")
        .set("Accept", "application/json")
    };

    const url = `${this.summaryURL}`;
    return this.httpClient
      .post<Inputdata>(url, data, httpOptions)
      .pipe(concatMap(val => this.httpClient.get<TransactionSummaryData>(url)));

    // const url = `${this.summaryURL}.json`;
    // return this.httpClient.get<TransactionSummaryData>(url);
  }
}
