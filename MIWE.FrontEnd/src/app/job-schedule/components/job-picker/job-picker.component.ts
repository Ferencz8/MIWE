import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Job } from 'src/app/models/job';
import { JobService } from 'src/app/services/job.service';

@Component({
  selector: 'app-job-picker',
  templateUrl: './job-picker.component.html',
  styleUrls: ['./job-picker.component.css']
})
export class JobPickerComponent implements OnInit {

  selected: Job;

  jobs$: Observable<Job[]>;


  constructor(private jobService: JobService) { }

  ngOnInit(): void {
    this.jobService.getAll().subscribe((data) => { this.jobs$ = of(data); });
    //this.jobs$ = this.jobService.getAll();
  }

}
