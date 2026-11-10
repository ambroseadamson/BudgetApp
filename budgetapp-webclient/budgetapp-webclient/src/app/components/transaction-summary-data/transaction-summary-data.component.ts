import { Component, OnChanges, OnInit, SimpleChange, inject, input } from '@angular/core';
import { TransactionSummaryDataSource } from '../../datasources/transaction-summary-data-source';
import { TransactionSummaryTotalDataSource } from '../../datasources/transaction-summary-total-data-source';
import { ExpEntryCountService } from '../../services/exp-entry-count.service';
import { TransactiondetailsService } from '../../services/transactiondetails.service';
import { AsyncPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatTable, MatColumnDef, MatHeaderCellDef, MatHeaderCell, MatCellDef, MatCell, MatHeaderRowDef, MatHeaderRow, MatRowDef, MatRow } from '@angular/material/table';

@Component({
    selector: 'budgetapp-transaction-summary-data',
    templateUrl: './transaction-summary-data.component.html',
    styleUrls: ['./transaction-summary-data.component.css'],
    imports: [MatProgressSpinner, MatTable, MatColumnDef, MatHeaderCellDef, MatHeaderCell, MatCellDef, MatCell, MatHeaderRowDef, MatHeaderRow, MatRowDef, MatRow, AsyncPipe, CurrencyPipe, DatePipe]
})
export class TransactionSummaryDataComponent implements OnInit, OnChanges {
  private transactionDetailsService = inject(TransactiondetailsService);
  private expEntryCountService = inject(ExpEntryCountService);

  displayedColumnsData = ['date', 'type', 'description', 'amount'];
  dataSourceData: TransactionSummaryDataSource;

  displayedColumnsTotal = ['total'];
  dataSourceTotal: TransactionSummaryTotalDataSource;

  readonly summaryType = input<string>(undefined);
  readonly detailsType = input<string>(undefined);
  readonly startDate = input<string>(undefined);
  readonly endDate = input<string>(undefined);

  isInitialised = false;

  /** Inserted by Angular inject() migration for backwards compatibility */
  constructor(...args: unknown[]);

  constructor() {}

  ngOnInit() {
    this.dataSourceData = new TransactionSummaryDataSource(
      this.transactionDetailsService,
      this.expEntryCountService
    );

    this.dataSourceTotal = new TransactionSummaryTotalDataSource(
      this.transactionDetailsService
    );
    this.isInitialised = true;
    this.onRefresh();
  }

  ngOnChanges(changes: { [propKey: string]: SimpleChange }): void {
    if (
      'detailsType' in changes ||
      'startDate' in changes ||
      'endDate' in changes
    ) {
      this.onRefresh();
    }
  }

  onRefresh() {
    if (this.isInitialised !== true) {
      return;
    }

    const detailsType = this.detailsType();
    const summaryType = this.summaryType();
    const startDate = this.startDate();
    const endDate = this.endDate();
    this.dataSourceData.genTransactionSummaries(
      detailsType,
      summaryType,
      startDate,
      endDate
    );

    this.dataSourceTotal.genTransactionSummaryTotals(
      detailsType,
      summaryType,
      startDate,
      endDate
    );
  }
}
