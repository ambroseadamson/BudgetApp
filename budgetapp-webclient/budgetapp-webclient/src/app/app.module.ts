// tslint:disable:quotemark
import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FlexLayoutModule } from "@angular/flex-layout";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import {
  MatButtonModule,
  MatCheckboxModule,
  MatInputModule,
  MatListModule,
  MatPaginatorModule,
  MatProgressSpinnerModule,
  MatSortModule,
  MatTableModule
} from "@angular/material";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatIconModule } from "@angular/material/icon";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatToolbarModule } from "@angular/material/toolbar";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ConfigComponent } from "./components/config/config.component";
import { DetailsComponent } from "./components/details/details.component";
import { ImportComponent } from "./components/import/import.component";
import { TransactionSummaryDataComponent } from "./components/transaction-summary-data/transaction-summary-data.component";
import { ExpEntryCountService } from "./services/exp-entry-count.service";
import { GrouptypeconfigService } from "./services/grouptypeconfig.service";
import { IsDataLoadedService } from "./services/is-data-loaded.service";
import { MinMaxDateService } from "./services/min-max-date.service";
import { MockGrouptypeconfigService } from "./services/mock-grouptypeconfig.service";
import { MockTransactiondetailsService } from "./services/mock-transactiondetails.service";
import { TransactiondetailsService } from "./services/transactiondetails.service";

@NgModule({
  declarations: [
    AppComponent,
    DetailsComponent,
    ConfigComponent,
    TransactionSummaryDataComponent,
    ImportComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    HttpClientModule,
    MatButtonModule,
    MatListModule,
    MatTableModule,
    MatInputModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatSortModule,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatCheckboxModule,
    FlexLayoutModule,
    MatDatepickerModule,
    MatSelectModule
  ],
  providers: [
    GrouptypeconfigService,
    TransactiondetailsService,
    IsDataLoadedService,
    MinMaxDateService,
    ExpEntryCountService,
    MockGrouptypeconfigService,
    MockTransactiondetailsService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
