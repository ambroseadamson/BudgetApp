// This file is required by karma.conf.js and loads recursively all the .spec and framework files

import 'zone.js/testing';
import { getTestBed } from '@angular/core/testing';
import {
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting
} from '@angular/platform-browser-dynamic/testing';

// First, initialize the Angular testing environment.
getTestBed().initTestEnvironment(
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting()
);

import './app/app.component.spec';
import './app/components/config/config.component.spec';
import './app/components/details/details.component.spec';
import './app/components/import/import.component.spec';
import './app/components/transaction-summary-data/transaction-summary-data.component.spec';
import './app/services/exp-entry-count.service.spec';
import './app/services/grouptypeconfig.service.spec';
import './app/services/is-data-loaded.service.spec';
import './app/services/min-max-date.service.spec';
import './app/services/mock-grouptypeconfig.service.spec';
import './app/services/mock-transactiondetails.service.spec';
import './app/services/transactiondetails.service.spec';
