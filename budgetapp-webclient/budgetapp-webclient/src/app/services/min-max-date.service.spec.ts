import { TestBed, inject } from '@angular/core/testing';

import { MinMaxDateService } from './min-max-date.service';

describe('MinMaxDateService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MinMaxDateService]
    });
  });

  it('should be created', inject([MinMaxDateService], (service: MinMaxDateService) => {
    expect(service).toBeTruthy();
  }));
});
