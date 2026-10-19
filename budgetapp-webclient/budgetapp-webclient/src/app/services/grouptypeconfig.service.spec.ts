import { TestBed, inject } from '@angular/core/testing';

import { GrouptypeconfigService } from './grouptypeconfig.service';

describe('GrouptypeconfigService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [GrouptypeconfigService]
    });
  });

  it('should be created', inject([GrouptypeconfigService], (service: GrouptypeconfigService) => {
    expect(service).toBeTruthy();
  }));
});
