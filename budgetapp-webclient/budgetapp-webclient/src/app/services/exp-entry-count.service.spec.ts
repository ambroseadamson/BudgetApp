import { TestBed, inject } from '@angular/core/testing';

import { ExpEntryCountService } from './exp-entry-count.service';

describe('ExpEntryCountService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ExpEntryCountService]
    });
  });

  it('should be created', inject([ExpEntryCountService], (service: ExpEntryCountService) => {
    expect(service).toBeTruthy();
  }));
});
