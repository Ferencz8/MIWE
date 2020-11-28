import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedMaterialModule } from '../shared.module';
import { SnackbarService } from '../shared/snackbar.service';
import { AddJobScheduleComponent } from './components/add-job-schedule/add-job-schedule.component';
import { JobPickerComponent } from './components/job-picker/job-picker.component';


@NgModule({
  declarations: [
    AddJobScheduleComponent,
    JobPickerComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedMaterialModule,
    RouterModule
  ],
  entryComponents: [
    JobPickerComponent
  ],
  exports: [
    AddJobScheduleComponent,
    JobPickerComponent
  ],
  providers: [
    SnackbarService
  ]
})
export class JobScheduleModule { }
