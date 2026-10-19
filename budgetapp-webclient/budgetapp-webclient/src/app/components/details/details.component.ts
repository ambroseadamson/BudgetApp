import { Component, OnInit } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MomentDateAdapter } from "@angular/material-moment-adapter";
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE
} from "@angular/material/core";
// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from "moment";
// tslint:disable-next-line:no-duplicate-imports
import { default as _rollupMoment } from "moment";
import {
  DateFilter,
  TransactionSummaryEntry
} from "../../models/transactiondetails";
import { ExpEntryCountService } from "../../services/exp-entry-count.service";
import { IsDataLoadedService } from "../../services/is-data-loaded.service";
import { MinMaxDateService } from "../../services/min-max-date.service";
import { TransactiondetailsService } from "../../services/transactiondetails.service";

const moment = _rollupMoment || _moment;

// See the Moment.js docs for the meaning of these formats:
// https://momentjs.com/docs/#/displaying/format/
export const MY_FORMATS = {
  parse: {
    dateInput: "DD/MMM/YYYY"
  },
  display: {
    dateInput: "DD/MMM/YYYY",
    monthYearLabel: "MMM/YYYY",
    dateA11yLabel: "DD/MMM/YYYY",
    monthYearA11yLabel: "MMMM/YYYY"
  }
};

export interface DateRangeType {
  type: string;
  value: number;
}

@Component({
  selector: "budgetapp-details",
  templateUrl: "./details.component.html",
  styleUrls: ["./details.component.css"],
  providers: [
    // `MomentDateAdapter` can be automatically provided by importing `MomentDateModule` in your
    // application's root module. We provide it at the component level here, due to limitations of
    // our example generation script.
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE]
    },

    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS }
  ]
})
export class DetailsComponent implements OnInit {
  groups: TransactionSummaryEntry[] = [];
  selectedGroup: TransactionSummaryEntry = null;
  dateFilter: DateFilter = null;

  incSummaryType = "income";
  expSummaryType = "expenditure";
  expEntryCount = 0;

  fcStartDate: FormControl = null;
  fcDateTypeRange: FormControl = null;

  selectedRange: DateRangeType = null;

  minAndMaxDates = { minDate: null, maxDate: null };

  _isDataLoaded = false;

  dateRangeTypes: DateRangeType[] = [
    { type: "Day", value: 1 },
    { type: "2 Days", value: 2 },
    { type: "3 Days", value: 3 },
    { type: "4 Days", value: 4 },
    { type: "5 Days", value: 5 },
    { type: "6 Days", value: 6 },
    { type: "Week", value: 7 },
    { type: "Fortnight", value: 14 },
    { type: "Month", value: 30 }
  ];

  constructor(
    private transactionDetailsService: TransactiondetailsService,
    private expEntryCountService: ExpEntryCountService,
    private minMaxDateService: MinMaxDateService,
    private isDataLoadedService: IsDataLoadedService
  ) {}

  ngOnInit() {
    this.transactionDetailsService
      .getTransactionSummary()
      .subscribe(transDetails => {
        this.groups = transDetails.summaryData;
        this.dateFilter = transDetails.dateRange;
        this.selectedGroup = transDetails.summaryData[1];
        this.fcStartDate = new FormControl(
          moment(transDetails.dateRange.startDate)
        );

        this.fcDateTypeRange = new FormControl(this.dateRangeTypes[8]);
      });

    this.expEntryCountService.currentData.subscribe(
      data => (this.expEntryCount = data)
    );
    this.isDataLoadedService.currentData.subscribe(
      data => (this._isDataLoaded = data)
    );
    this.minMaxDateService.currentData.subscribe(
      data => (this.minAndMaxDates = data)
    );
  }

  onSelectGroup(group: TransactionSummaryEntry): void {
    this.selectedGroup = group;
  }

  onApplyFilter(event: any): void {
    this.dateFilter.startDate = moment(this.fcStartDate.value).toISOString();
    const selectedDateRange: DateRangeType = this.fcDateTypeRange.value;

    this.dateFilter.endDate = moment(this.fcStartDate.value)
      .add(selectedDateRange.value, "days")
      .toISOString();
  }

  isDataLoaded(): boolean {
    return (
      this._isDataLoaded === true &&
      this.groups.length > 0 &&
      this.selectedGroup !== null
    );
  }
}
