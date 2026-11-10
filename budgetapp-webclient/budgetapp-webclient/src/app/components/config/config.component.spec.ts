import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { ConfigComponent } from './config.component';
import { GrouptypeconfigService } from '../../services/grouptypeconfig.service';

describe('ConfigComponent', () => {
  let component: ConfigComponent;
  let fixture: ComponentFixture<ConfigComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [ConfigComponent],
      providers: [
        provideRouter([]),
        {
          provide: GrouptypeconfigService,
          useValue: {
            getGroupTypeNames: () => of(['test']),
            getTypeItems: (groupName: string) =>
              of({ id: 1, groupName, groupType: 'expense', names: [] })
          }
        }
      ]
})
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
