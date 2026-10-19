import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { Observable } from "rxjs/Observable";
import { of } from "rxjs/observable/of";
import { catchError, finalize } from "rxjs/operators";
import { Transactiondetails } from "../models/transactiondetails";
import { ExpEntryCountService } from "../services/exp-entry-count.service";
import { TransactiondetailsService } from "../services/transactiondetails.service";

export class TransactionSummary {
  dateStamp: string;
  transType: string;
  description: string;
  amount: number;
}

export class TransactionSummaryDataSource
  implements DataSource<TransactionSummary> {
  private transactionSummarySubject = new BehaviorSubject<TransactionSummary[]>(
    []
  );
  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();

  public expEntryCount = 0;

  constructor(
    private transactionDetailsService: TransactiondetailsService,
    private expEntryCountService: ExpEntryCountService
  ) {}

  connect(
    collectionViewer: CollectionViewer
  ): Observable<TransactionSummary[]> {
    return this.transactionSummarySubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.transactionSummarySubject.complete();
    this.loadingSubject.complete();
  }

  genTransactionSummaries(
    cfgName: string,
    type: string,
    startDate: string,
    endDate: string
  ): void {
    this.loadingSubject.next(true);

    const genValue = (
      transDetails: any[] | Transactiondetails
    ): TransactionSummary[] => {
      const _transDetails = <Transactiondetails>transDetails;

      if (type === "expenditure") {
        this.expEntryCountService.changeExpEntryCount(
          _transDetails.expenditure.transactions.length
        );
      }

      const expData =
        type === "income"
          ? _transDetails.income.transactions
          : _transDetails.expenditure.transactions;
      return expData.map(transaction => {
        return {
          dateStamp: transaction.dateStamp,
          transType: transaction.transType,
          description: transaction.description,
          amount: transaction.amount
        };
      });
    };

    this.transactionDetailsService
      .getTransactionDetails(cfgName, startDate, endDate)
      .pipe(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
      )
      .subscribe(transDetails => {
        this.transactionSummarySubject.next(genValue(transDetails));
      });
  }
}
