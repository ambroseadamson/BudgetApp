import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { Observable } from "rxjs/Observable";
import { of } from "rxjs/observable/of";
import { catchError, finalize } from "rxjs/operators";
import { TransactionSummaryData } from "../models/transactiondetails";
import { TransactiondetailsService } from "../services/transactiondetails.service";

export class TransactionSummaryEntryVal {
  name: string;
  pct: string;
}

export class BalanceSummaryDataSource
  implements DataSource<TransactionSummaryEntryVal> {
  private balanceSummarySubject = new BehaviorSubject<
    TransactionSummaryEntryVal[]
  >([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();

  constructor(private transactionDetailsService: TransactiondetailsService) {}

  connect(
    collectionViewer: CollectionViewer
  ): Observable<TransactionSummaryEntryVal[]> {
    return this.balanceSummarySubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.balanceSummarySubject.complete();
    this.loadingSubject.complete();
  }

  genBalanceSummary(): void {
    this.loadingSubject.next(true);

    const genValue = (
      summary: any | TransactionSummaryData
    ): TransactionSummaryEntryVal[] => {
      const summaryEntries = (<TransactionSummaryData>summary).summaryData;
      return summaryEntries.map(entry => {
        return {
          name: entry.name,
          pct: `${entry.pct}%`
        };
      });
    };

    this.transactionDetailsService
      .getTransactionSummary()
      .pipe(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
      )
      .subscribe(transDetails =>
        this.balanceSummarySubject.next(genValue(transDetails))
      );
  }
}
