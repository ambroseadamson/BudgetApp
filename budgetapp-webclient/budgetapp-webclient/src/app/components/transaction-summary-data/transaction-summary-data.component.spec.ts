import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { NEVER } from 'rxjs';

import { TransactionSummaryDataComponent } from './transaction-summary-data.component';
import { TransactiondetailsService } from '../../services/transactiondetails.service';

describe('TransactionSummaryDataComponent', () => {
  let component: TransactionSummaryDataComponent;
  let fixture: ComponentFixture<TransactionSummaryDataComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [TransactionSummaryDataComponent],
      providers: [
        {
          provide: TransactiondetailsService,
          useValue: {
            getTransactionSummary: () => NEVER,
            getTransactionDetails: () => NEVER,
            getTransactionSummaryByDateRange: () => NEVER
          }
        }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TransactionSummaryDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
