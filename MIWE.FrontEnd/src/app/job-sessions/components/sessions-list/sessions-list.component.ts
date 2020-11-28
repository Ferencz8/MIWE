import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { JobSession } from 'src/app/models/job.session';
import { JobSessionService } from 'src/app/services/job.session.service';
import { SnackbarService } from 'src/app/shared/snackbar.service';

@Component({
  selector: 'app-sessions-list',
  templateUrl: './sessions-list.component.html',
  styleUrls: ['./sessions-list.component.css']
})
export class SessionsListComponent implements OnInit {

  displayedColumns: string[] = ['dateStart', 'dateEnd', 'instance', 'jobPipeline', 'actions'];

  dataSource$: Observable<JobSession[]>;

  constructor(private jobSessionService: JobSessionService, private snackbar: SnackbarService) { }

  ngOnInit(): void {

    this.dataSource$ = this.jobSessionService.getAll();
  }

  downloadResult(jobSession: JobSession): void {

    //https://stackoverflow.com/questions/52154874/angular-6-downloading-file-from-rest-api
    this.snackbar.open('Download started.');
    this.jobSessionService.downloadResult(jobSession.id).subscribe(res => {
        const newBlob = new Blob([res], { type: jobSession.resultContentType });
        const downloadUrl = window.URL.createObjectURL(newBlob);
        window.open(downloadUrl);
    });
  }
}
