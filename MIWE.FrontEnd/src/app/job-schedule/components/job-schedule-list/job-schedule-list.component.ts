import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { JobSchedule } from 'src/app/models/job.schedule';
import { JobSchedulePipeline } from 'src/app/models/job.schedule.pipeline';
import { JobScheduleService } from 'src/app/services/job.schedule.service';
import { SnackbarService } from 'src/app/shared/snackbar.service';

@Component({
  selector: 'app-job-schedule-list',
  templateUrl: './job-schedule-list.component.html',
  styleUrls: ['./job-schedule-list.component.css']
})
export class JobScheduleListComponent implements OnInit {

  displayedColumns: string[] = ['scheduling', 'pipeline', 'actions'];

  dataSource$: Observable<JobSchedulePipeline[]>;
  mydata: JobSchedulePipeline[] = [
    { id: '1', scheduling: '* * * * * * *', pipeline: 'scacas'}
  ];

  constructor(private jobScheduleService: JobScheduleService, private snackbar: SnackbarService, private router: Router) { }

  ngOnInit(): void {

    this.jobScheduleService.getAll().subscribe(res => {this.dataSource$ = of(res); });
    //this.dataSource$ = of(this.mydata);
  }

  editJobSchedule(id): void {
    this.router.navigate([`/home/editSchedule/${id}`]);
  }
}
