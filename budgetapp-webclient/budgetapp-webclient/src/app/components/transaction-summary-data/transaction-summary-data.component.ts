import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChange
} from "@angular/core";
import { TransactionSummaryDataSource } from "../../datasources/transaction-summary-data-source";
import { TransactionSummaryTotalDataSource } from "../../datasources/transaction-summary-total-data-source";
import { ExpEntryCountService } from "../../services/exp-entry-count.service";
import { TransactiondetailsService } from "../../services/transactiondetails.service";

@Component({
  selector: "budgetapp-transaction-summary-data",
  templateUrl: "./transaction-summary-data.component.html",
  styleUrls: ["./transaction-summary-data.component.css"]
})
export class TransactionSummaryDataComponent implements OnInit, OnChanges {
  displayedColumnsData = ["date", "type", "description", "amount"];
  dataSourceData: TransactionSummaryDataSource;

  displayedColumnsTotal = ["total"];
  dataSourceTotal: TransactionSummaryTotalDataSource;

  @Input()
  summaryType: string;
  @Input()
  detailsType: string;
  @Input()
  startDate: string;
  @Input()
  endDate: string;

  isInitialised = false;

  constructor(
    private transactionDetailsService: TransactiondetailsService,
    private expEntryCountService: ExpEntryCountService
  ) {}

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
    let hasDetailsTypeChanged = false;
    let hasDateRangeChanged = false;
    for (let propName in changes) {
      const changedProp = changes[propName];
      if (propName === "detailsType") {
        this.detailsType = changedProp.currentValue;
        hasDetailsTypeChanged = true;
      } else if (propName === "startDate") {
        this.startDate = changedProp.currentValue;
        hasDateRangeChanged = true;
      } else if (propName === "endDate") {
        this.endDate = changedProp.currentValue;
        hasDateRangeChanged = true;
      }
      if (hasDateRangeChanged || hasDetailsTypeChanged) {
        this.onRefresh();
      }
    }
  }

  onRefresh() {
    if (this.isInitialised !== true) {
      return;
    }

    this.dataSourceData.genTransactionSummaries(
      this.detailsType,
      this.summaryType,
      this.startDate,
      this.endDate
    );

    this.dataSourceTotal.genTransactionSummaryTotals(
      this.detailsType,
      this.summaryType,
      this.startDate,
      this.endDate
    );
  }
}
