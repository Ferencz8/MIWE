import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedMaterialModule } from '../shared.module';
import { SnackbarService } from '../shared/snackbar.service';
import { SessionsListComponent } from './components/sessions-list/sessions-list.component';


@NgModule({
  declarations: [
    SessionsListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedMaterialModule,
    RouterModule
  ],
  exports: [
    SessionsListComponent
  ],
  providers: [
    SnackbarService
  ]
})
export class JobSessionModule { }
