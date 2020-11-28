import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddJobScheduleComponent } from './add-job-schedule.component';

describe('AddJobScheduleComponent', () => {
  let component: AddJobScheduleComponent;
  let fixture: ComponentFixture<AddJobScheduleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddJobScheduleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddJobScheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
