import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { NEVER } from 'rxjs';

import { DetailsComponent } from './details.component';
import { TransactiondetailsService } from '../../services/transactiondetails.service';

describe('DetailsComponent', () => {
  let component: DetailsComponent;
  let fixture: ComponentFixture<DetailsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [DetailsComponent],
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
    fixture = TestBed.createComponent(DetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
