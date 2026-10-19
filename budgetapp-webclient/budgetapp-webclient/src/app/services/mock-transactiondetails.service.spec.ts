import { TestBed, inject } from '@angular/core/testing';

import { MockTransactiondetailsService } from './mock-transactiondetails.service';

describe('MockTransactiondetailsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MockTransactiondetailsService]
    });
  });

  it('should be created', inject([MockTransactiondetailsService], (service: MockTransactiondetailsService) => {
    expect(service).toBeTruthy();
  }));
});
