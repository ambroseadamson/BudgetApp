import { TestBed, inject } from '@angular/core/testing';

import { IsDataLoadedService } from './is-data-loaded.service';

describe('IsDataLoadedService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [IsDataLoadedService]
    });
  });

  it('should be created', inject([IsDataLoadedService], (service: IsDataLoadedService) => {
    expect(service).toBeTruthy();
  }));
});
