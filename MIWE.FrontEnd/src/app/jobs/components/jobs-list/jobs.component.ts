import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { OSType } from 'src/app/models/os.type';
import { PluginType } from 'src/app/models/plugin,type';
import { Job } from 'src/app/models/job';
import { JobService } from 'src/app/services/job.service';
import { SnackbarService } from 'src/app/shared/snackbar.service';

@Component({
  selector: 'jobs-list',
  templateUrl: './jobs.component.html',
  styleUrls: ['./jobs.component.css']
})
export class JobsComponent implements OnInit {

  displayedOSType = OSType;
  displayedPluginType = PluginType;

  displayedColumns: string[] = ['name', 'os', 'pluginType', 'isActive', 'isRunning', 'actions'];

  isJobStarted: boolean = false;

  dataSource$: Observable<Job[]>;
  mydata: Job[] = [
    { id: '1', name: 'first', description: 'scacas', osType: 1, isActive: true, isRunning: false, pluginPath: null, pluginType: 0 }
  ];

  constructor(private jobService: JobService, private snackbar: SnackbarService) { }

  ngOnInit(): void {

    this.jobService.getAll().subscribe(res => {this.dataSource$ = of(res); });
    //this.dataSource$ = of(this.mydata);
  }

  startJob(jobId): void {
    if (this.isJobStarted) { return; }
    this.isJobStarted = true;

    this.snackbar.open('Started job.');
    this.dataSource$.subscribe(n => {
      const element = n.find(f => f.id === jobId);
      if (element !== undefined){
        element.isRunning = true;
        this.jobService.startjob(jobId).subscribe();
      }
      this.isJobStarted = false;
      this.dataSource$ = of(n);
    });
  }

  stopJob(jobId): void {
    if (this.isJobStarted) { return; }
    this.isJobStarted = true;

    this.snackbar.open('Stopped job.');
    this.dataSource$.subscribe(n => {
      const element = n.find(f => f.id === jobId);
      if (element !== undefined){
        element.isRunning = false;
        this.jobService.stopjob(jobId).subscribe();
      }
      this.isJobStarted = false;
      this.dataSource$ = of(n);
    });
  }
}
