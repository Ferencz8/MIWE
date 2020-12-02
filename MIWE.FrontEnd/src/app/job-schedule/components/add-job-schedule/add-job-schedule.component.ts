import { AfterViewInit, Component, ComponentFactoryResolver, ComponentRef, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { JobSchedule } from 'src/app/models/job.schedule';
import { JobScheduleService } from 'src/app/services/job.schedule.service';
import { JobPickerComponent } from '../job-picker/job-picker.component';

@Component({
  selector: 'add-jobSchedule',
  templateUrl: './add-job-schedule.component.html',
  styleUrls: ['./add-job-schedule.component.css']
})
export class AddJobScheduleComponent implements OnInit, AfterViewInit {

  @ViewChild('containerJobPicker', {read: ViewContainerRef})
  containerJobPicker: ViewContainerRef;
  // @ViewChild('containerJobPicker', { static: true }) containerJobPicker: ViewContainerRef;
  components = [];

  isEdit = false;
  jobSchedules$: Observable<JobSchedule>;

  constructor(private componentFactoryResolver: ComponentFactoryResolver, private jobScheduleService: JobScheduleService,
    private router: Router) { }

  ngOnInit(): void {
    this.jobSchedules$ = of(new JobSchedule());
  }

  ngAfterViewInit() {
    console.log("ngAfterViewInit", this.containerJobPicker);
  }

  addJobPicker() {

    // Create component dynamically inside the ng-template
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(JobPickerComponent);
    const component = this.containerJobPicker.createComponent(componentFactory);

    // Push the component so that we can keep track of which components are created
    this.components.push(component);
  }

  removeJobPicker() {
    // Find the component
    const component = this.components.find((component) => component.instance instanceof JobPickerComponent);
    const componentIndex = this.components.indexOf(component);

    if (componentIndex !== -1) {
      // Remove component from both view and array
      this.containerJobPicker.remove(this.containerJobPicker.indexOf(component));
      this.components.splice(componentIndex, 1);
    }
  }

  save(jobSchedule: JobSchedule) {

    //let component: ComponentRef<JobPickerComponent> = this.components.find((component) => component.instance instanceof JobPickerComponent);
    const mainComponent = this.components[0] as ComponentRef<JobPickerComponent>;
    const mainJobId = mainComponent.instance.selected.id;

    jobSchedule.mainJob = mainJobId;
    jobSchedule.nextJobs = '';

    for (let index = 1; index < this.components.length; index++){
      const refComponent = this.components[index] as ComponentRef<JobPickerComponent>;
      jobSchedule.nextJobs += refComponent.instance.selected.id + ',';
    }
    jobSchedule.nextJobs.slice(0, -1);

    this.jobScheduleService.add(jobSchedule).subscribe(res => {
      this.router.navigate(['../home/schedules']);
    });
  }

  cancel() {
    this.router.navigate(['../home/schedules']);
  }
}
