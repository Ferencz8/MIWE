import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { JobsComponent } from './components/jobs-list/jobs.component';
import { AddJobComponent } from './components/add-job/add-job.component';
import { SharedMaterialModule } from '../shared.module';
import { NgxFileDropModule } from 'ngx-file-drop';
import { SnackbarService } from '../shared/snackbar.service';

@NgModule({
  declarations: [
    JobsComponent,
    AddJobComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedMaterialModule,
    RouterModule,
    NgxFileDropModule
  ],
  exports: [
    JobsComponent,
    AddJobComponent
  ],
  providers: [
    SnackbarService
  ]
})
export class JobsModule { }
