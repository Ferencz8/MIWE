import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Job } from 'src/app/models/job';
import { PluginType } from 'src/app/models/plugin,type';
import { JobService } from 'src/app/services/job.service';

@Component({
  selector: 'app-job-picker',
  templateUrl: './job-picker.component.html',
  styleUrls: ['./job-picker.component.css']
})
export class JobPickerComponent implements OnInit {

  selected: Job;
  selectedId: string;

  pluginType: PluginType;

  jobs$: Observable<Job[]>;

  constructor(private jobService: JobService) { }

  ngOnInit(): void {
    this.jobService.getAll().subscribe((data) => {

      this.jobs$ = of(data.filter(n => n.pluginType === this.pluginType));
      //this.jobs$ = of(data);
      if (this.selectedId !== undefined) {
        this.selected = data.find(n => n.id === this.selectedId);
      }
    });
  }
}
