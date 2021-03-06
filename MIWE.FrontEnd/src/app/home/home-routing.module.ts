import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home.component';
import { JobsComponent } from '../jobs/components/jobs-list/jobs.component';
import { AddJobComponent } from '../jobs/components/add-job/add-job.component';
import { AddJobScheduleComponent } from '../job-schedule/components/add-job-schedule/add-job-schedule.component';
import { SessionsListComponent } from '../job-sessions/components/sessions-list/sessions-list.component';
import { JobScheduleListComponent } from '../job-schedule/components/job-schedule-list/job-schedule-list.component';
import { InstancesComponent } from '../instances/components/instances/instances.component';

const routes: Routes = [
  {
    path: 'home', component: HomeComponent, children: [
      { path: 'jobs', component: JobsComponent },
      { path: 'addjob', component: AddJobComponent },
      { path: 'editJob/:id', component: AddJobComponent },
      { path: 'addSchedule', component: AddJobScheduleComponent },
      { path: 'editSchedule/:id', component: AddJobScheduleComponent },
      { path: 'schedules', component: JobScheduleListComponent },
      { path: 'sessions', component: SessionsListComponent },
      { path: 'instances', component: InstancesComponent },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
