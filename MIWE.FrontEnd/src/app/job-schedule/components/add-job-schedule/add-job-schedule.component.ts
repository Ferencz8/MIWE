import { AfterViewInit, Component, ComponentFactoryResolver, ComponentRef, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Job } from 'src/app/models/job';
import { JobSchedule } from 'src/app/models/job.schedule';
import { PluginType } from 'src/app/models/plugin,type';
import { JobScheduleService } from 'src/app/services/job.schedule.service';
import { JobService } from 'src/app/services/job.service';
import { SnackbarService } from 'src/app/shared/snackbar.service';
import { JobPickerComponent } from '../job-picker/job-picker.component';

@Component({
  selector: 'add-jobSchedule',
  templateUrl: './add-job-schedule.component.html',
  styleUrls: ['./add-job-schedule.component.css']
})
export class AddJobScheduleComponent implements OnInit {

  @ViewChild('containerJobPicker', { read: ViewContainerRef })
  containerJobPicker: ViewContainerRef;
  // @ViewChild('containerJobPicker', { static: true }) containerJobPicker: ViewContainerRef;
  components = [];

  isEdit = false;
  jobSchedule$: Observable<JobSchedule>;


  constructor(private componentFactoryResolver: ComponentFactoryResolver, private jobScheduleService: JobScheduleService,
    private router: Router, private route: ActivatedRoute, private jobService: JobService, private snackbar: SnackbarService) { }

  ngOnInit(): void {
    this.jobSchedule$ = of(new JobSchedule());

    this.route.params.subscribe(params => {
      const id = params['id'];
      if (!id) {
        return;
      }

      this.isEdit = true;

      this.jobScheduleService.get(id).subscribe(data => {
        this.jobSchedule$ = of(data);

        const ids: string[] = [];
        ids.push(data.mainJob);
        const nextJobIds = data.nextJobs.split(',').filter(n => n !== '');
        nextJobIds.forEach(n => ids.push(n));

        ids.forEach(n => this.addJobPicker(n));
      });
    });
  }

  addJobPicker(jobId: string) {

    if (this.components.length >= 2){
      this.snackbar.open('Only one Crawler & Processor are allowed for now!');
      return;
    }
    // Create component dynamically inside the ng-template
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(JobPickerComponent);
    const component = this.containerJobPicker.createComponent(componentFactory);
    if (jobId !== undefined) {
      component.instance.selectedId = jobId;
    }
    component.instance.pluginType = this.components.length >= 1 ? PluginType.DataProcessor : PluginType.Crawler;
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

    const mainComponent = this.components[0] as ComponentRef<JobPickerComponent>;
    const mainJobId = mainComponent.instance.selected.id;

    jobSchedule.mainJob = mainJobId;
    jobSchedule.nextJobs = '';

    for (let index = 1; index < this.components.length; index++) {
      const refComponent = this.components[index] as ComponentRef<JobPickerComponent>;
      jobSchedule.nextJobs += refComponent.instance.selected.id + ',';
    }
    jobSchedule.nextJobs.slice(0, -1);

    if (this.isEdit) {
      this.jobScheduleService.update(jobSchedule).subscribe(res => {
        this.router.navigate(['../home/schedules']);
      });
    }
    else {
      this.jobScheduleService.add(jobSchedule).subscribe(res => {
        this.router.navigate(['../home/schedules']);
      });
    }
  }

  cancel() {
    this.router.navigate(['../home/schedules']);
  }
}
