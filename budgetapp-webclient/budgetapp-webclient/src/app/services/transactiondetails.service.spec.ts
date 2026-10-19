import { TestBed, inject } from '@angular/core/testing';

import { TransactiondetailsService } from './transactiondetails.service';

describe('TransactiondetailsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TransactiondetailsService]
    });
  });

  it('should be created', inject([TransactiondetailsService], (service: TransactiondetailsService) => {
    expect(service).toBeTruthy();
  }));
});
