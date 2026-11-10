import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Inputdata } from '../models/inputdata';
import {
  Transactiondetails,
  TransactionSummaryData
} from '../models/transactiondetails';

@Injectable({
  providedIn: 'root'
})
export class MockTransactiondetailsService {
  private httpClient = inject(HttpClient);

  serverPort = '4200';

  // POST
  // GET
  summaryURL = `http://localhost:${this.serverPort}/api/TransactionSummary`;

  transactionsByGroupTypeRoute = 'TransactionsByGroupType';
  summaryByDateRangeRoute = 'SummaryByDateRange';

  /** Inserted by Angular inject() migration for backwards compatibility */
  constructor(...args: unknown[]);

  constructor() {}

  getTransactionDetails(
    detailsType: string,
    startDate: string,
    endDate: string
  ): Observable<Transactiondetails> {
    const options = {
      params: new HttpParams()
        .set('grouptype', detailsType)
        .set('startdate', startDate)
        .set('enddate', endDate)
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
        .set('grouptype', detailsType)
        .set('startdate', startDate)
        .set('enddate', endDate)
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
