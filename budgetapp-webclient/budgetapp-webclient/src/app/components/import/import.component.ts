import { Location, AsyncPipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import moment from 'moment';
import { tap } from 'rxjs/operators';
import { ImportDataSource } from '../../datasources/import-data-source';
import { Inputdata } from '../../models/inputdata';
import { TransactionSummaryData } from '../../models/transactiondetails';
import { IsDataLoadedService } from '../../services/is-data-loaded.service';
import { MinMaxDateService } from '../../services/min-max-date.service';
import { TransactiondetailsService } from '../../services/transactiondetails.service';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatTable, MatColumnDef, MatHeaderCellDef, MatHeaderCell, MatCellDef, MatCell, MatHeaderRowDef, MatHeaderRow, MatRowDef, MatRow } from '@angular/material/table';
@Component({
    selector: 'budgetapp-import',
    templateUrl: './import.component.html',
    styleUrls: ['./import.component.css'],
    imports: [MatProgressSpinner, MatTable, MatColumnDef, MatHeaderCellDef, MatHeaderCell, MatCellDef, MatCell, MatHeaderRowDef, MatHeaderRow, MatRowDef, MatRow, AsyncPipe]
})
export class ImportComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private location = inject(Location);
  private transactiondetailsService = inject(TransactiondetailsService);
  private minMaxDateService = inject(MinMaxDateService);
  private isDataLoadedService = inject(IsDataLoadedService);

  displayedColumns = ['data'];
  dataSource: ImportDataSource;
  inputData: Inputdata = {
    type: 'Unknown',
    data: ['']
  };
  summaryData: TransactionSummaryData;

  /** Inserted by Angular inject() migration for backwards compatibility */
  constructor(...args: unknown[]);

  constructor() {}

  ngOnInit() {
    this.dataSource = new ImportDataSource();

    this.onRefresh();
  }

  onRefresh() {
    this.dataSource.genImportData(this.inputData.data);
  }

  genDisplayRowsStrippedOfQuotes(rows: string[]): string[] {
    return rows.map(row => row.replace(/['']+/g, ''));
  }

  handleFileInput(files: FileList) {
    const file = files.item(0);
    const reader = new FileReader();

    const that = this;

    const obtainFileType = (fileName: string): string => {
      const fileTypes = fileName.match(/(cashReceipts|Statement Download)/g);
      return fileTypes !== null ? fileTypes[0] : '';
    };

    const parseContent = (content: any): void => {
      const fileType = obtainFileType(file.name);
      const validFileDetected = fileType.length > 0;

      this.inputData = {
        type:
          validFileDetected === true ? fileType : 'Invalid File Type Detected',
        data: validFileDetected === true ? content.split('\n') : ['']
      };

      if (validFileDetected === true) {
        this.transactiondetailsService
          .importData(this.inputData)
          .pipe(
            tap(summary =>
              this.isDataLoadedService.changeIsDataLoaded(
                summary.summaryData.length > 0
              )
            )
          )
          .subscribe(summaryData => {
            this.summaryData = summaryData;

            const minAndMaxDates = {
              minDate: moment(summaryData.dateRange.startDate).toDate(),
              maxDate: moment(summaryData.dateRange.endDate).toDate()
            };
            this.minMaxDateService.changeMinMaxDate(minAndMaxDates);
          });
      } else {
        that.isDataLoadedService.changeIsDataLoaded(false);
      }
    };

    reader.onload = function(e) {
      parseContent(reader.result);
      that.onRefresh();
    };

    reader.onerror = function(err) {
      //      console.log(err, err.loaded, err.loaded === 0, file);
      that.isDataLoadedService.changeIsDataLoaded(false);
      that.inputData = {
        type: 'Invalid File Type Detected',
        data: ['']
      };
    };

    reader.readAsText(file);
  }
}
