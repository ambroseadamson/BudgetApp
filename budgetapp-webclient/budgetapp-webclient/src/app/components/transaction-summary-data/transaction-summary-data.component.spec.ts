import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionSummaryDataComponent } from './transaction-summary-data.component';

describe('TransactionSummaryDataComponent', () => {
  let component: TransactionSummaryDataComponent;
  let fixture: ComponentFixture<TransactionSummaryDataComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
    imports: [TransactionSummaryDataComponent]
})
    .compileComponents();
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
