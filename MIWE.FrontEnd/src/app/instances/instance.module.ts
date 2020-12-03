import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedMaterialModule } from '../shared.module';
import { SnackbarService } from '../shared/snackbar.service';
import { InstancesComponent } from './components/instances/instances.component';


@NgModule({
  declarations: [
    InstancesComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedMaterialModule,
    RouterModule
  ],
  entryComponents: [
  ],
  exports: [
    InstancesComponent
  ],
  providers: [
    SnackbarService
  ]
})
export class InstanceModule { }
