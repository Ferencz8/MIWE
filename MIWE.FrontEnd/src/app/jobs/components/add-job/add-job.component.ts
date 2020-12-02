import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Job } from 'src/app/models/job';
import { FileSystemFileEntry, NgxFileDropEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { JobService } from 'src/app/services/job.service';
import { SnackbarService } from 'src/app/shared/snackbar.service';

@Component({
  selector: 'app-add-job',
  templateUrl: './add-job.component.html',
  styleUrls: ['./add-job.component.css']
})
export class AddJobComponent implements OnInit {
  selectedOS = '0';
  selectedType = '0';
  isEdit = false;
  job$: Observable<Job>;

  dllUrl: string;
  dllFile: File;
  acceptedFileExtensions = '.dll,.zip';

  constructor(
    private route: ActivatedRoute,
    private jobService: JobService,
    private router: Router,
    private snackbar: SnackbarService) { }

  ngOnInit(): void {
    this.job$ = of(new Job());
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (!id) {
        return;
      }

      this.isEdit = true;

      this.jobService.get(id).subscribe(data => {
        this.selectedOS = data.osType.toString();
        this.selectedType = data.pluginType.toString();
        this.job$ = of(data);
      });
    });
  }

  save(job: Job) {
    if (this.dllFile) {
      this.jobService.upload(this.dllFile, job.name).subscribe((res: any) => {

        if (!!res && !!res.fileUrl) {
          job.pluginPath = res.fileUrl;
          job.osType = Number(this.selectedOS);
          job.pluginType = Number(this.selectedType);
          job.isActive = true;
          if (this.isEdit) {
            this.jobService.update(job).subscribe(res2 => this.router.navigate(['../home/jobs']));
          }
          else {
            this.jobService.add(job).subscribe(res2 =>
              this.router.navigate(['../home/jobs'])
            );
          }
        }
      });
    }
    else {
      //display required dll
      this.snackbar.open('The dll is required.');
    }
  }

  cancel() {
    this.router.navigate(['../home/jobs']);
  }

  public dropped(files: NgxFileDropEntry[]) {

    for (const droppedFile of files) {

      // Is it a file?
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          this.dllFile = file;
          // Here you can access the real file
          console.log(droppedFile.relativePath, file);

          const reader = new FileReader();

          reader.onload = (event: any) => {
            this.dllUrl = event.target.result;
          };

          reader.onerror = (event: any) => {
            console.log('File could not be read: ' + event.target.error.code);
          };

          reader.readAsDataURL(file);
        });
      } else {
        // It was a directory (empty directories are added, otherwise only files)
        const fileEntry = droppedFile.fileEntry as FileSystemDirectoryEntry;
        console.log(droppedFile.relativePath, fileEntry);
      }
    }
  }
}
