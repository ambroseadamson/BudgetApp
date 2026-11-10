import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { Observable, BehaviorSubject ,  of } from 'rxjs';
import { catchError, finalize, map } from 'rxjs/operators';

export class ImportDataSource implements DataSource<string> {
  private importDataSubject = new BehaviorSubject<string[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();

  constructor() {}

  connect(collectionViewer: CollectionViewer): Observable<string[]> {
    return this.importDataSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.importDataSubject.complete();
    this.loadingSubject.complete();
  }

  genImportData(dataSet: string[]): void {
    this.loadingSubject.next(true);
    of(dataSet)
      .pipe(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
      )
      .subscribe(balanceSummary => this.importDataSubject.next(balanceSummary));
  }
}
