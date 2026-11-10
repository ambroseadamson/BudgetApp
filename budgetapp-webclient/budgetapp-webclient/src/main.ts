import { enableProdMode, importProvidersFrom } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';


import { environment } from './environments/environment';
import 'hammerjs';
import { GrouptypeconfigService } from './app/services/grouptypeconfig.service';
import { TransactiondetailsService } from './app/services/transactiondetails.service';
import { IsDataLoadedService } from './app/services/is-data-loaded.service';
import { MinMaxDateService } from './app/services/min-max-date.service';
import { ExpEntryCountService } from './app/services/exp-entry-count.service';
import { MockGrouptypeconfigService } from './app/services/mock-grouptypeconfig.service';
import { MockTransactiondetailsService } from './app/services/mock-transactiondetails.service';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app/app-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { AppComponent } from './app/app.component';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
    providers: [
        importProvidersFrom(BrowserModule, BrowserAnimationsModule, FormsModule, ReactiveFormsModule, AppRoutingModule, HttpClientModule, MatButtonModule, MatListModule, MatTableModule, MatInputModule, MatPaginatorModule, MatProgressSpinnerModule, MatSortModule, MatSidenavModule, MatToolbarModule, MatIconModule, MatCheckboxModule, FlexLayoutModule, MatDatepickerModule, MatSelectModule),
        GrouptypeconfigService,
        TransactiondetailsService,
        IsDataLoadedService,
        MinMaxDateService,
        ExpEntryCountService,
        MockGrouptypeconfigService,
        MockTransactiondetailsService
    ]
})
  .catch(err => console.log(err));
