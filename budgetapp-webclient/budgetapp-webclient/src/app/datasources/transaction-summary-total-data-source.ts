import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { Observable } from "rxjs/Observable";
import { of } from "rxjs/observable/of";
import { catchError, finalize } from "rxjs/operators";
import { Transactiondetails } from "../models/transactiondetails";
import { TransactiondetailsService } from "../services/transactiondetails.service";

export class TransactionSummaryTotalDataSource implements DataSource<number> {
  private transactionSummaryTotalSubject = new BehaviorSubject<number[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();

  constructor(private transactionDetailsService: TransactiondetailsService) {}

  connect(collectionViewer: CollectionViewer): Observable<number[]> {
    return this.transactionSummaryTotalSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.transactionSummaryTotalSubject.complete();
    this.loadingSubject.complete();
  }

  genTransactionSummaryTotals(
    cfgName: string,
    type: string,
    startDate: string,
    endDate: string
  ): void {
    this.loadingSubject.next(true);

    const genValue = (transDetails: any[] | Transactiondetails): number[] => {
      const _transDetails = <Transactiondetails>transDetails;
      return [
        type === "income"
          ? _transDetails.income.total
          : _transDetails.expenditure.total
      ];
    };

    this.transactionDetailsService
      .getTransactionDetails(cfgName, startDate, endDate)
      .pipe(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
      )
      .subscribe(transDetails =>
        this.transactionSummaryTotalSubject.next(genValue(transDetails))
      );
  }
}
