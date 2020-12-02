import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobScheduleListComponent } from './job-schedule-list.component';

describe('JobScheduleListComponent', () => {
  let component: JobScheduleListComponent;
  let fixture: ComponentFixture<JobScheduleListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ JobScheduleListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JobScheduleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
