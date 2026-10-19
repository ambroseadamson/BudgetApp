import { TestBed, inject } from '@angular/core/testing';

import { MockGrouptypeconfigService } from './mock-grouptypeconfig.service';

describe('MockGrouptypeconfigService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MockGrouptypeconfigService]
    });
  });

  it('should be created', inject([MockGrouptypeconfigService], (service: MockGrouptypeconfigService) => {
    expect(service).toBeTruthy();
  }));
});
