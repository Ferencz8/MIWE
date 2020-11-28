import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedMaterialModule } from './shared.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HomeModule } from './home/home.module';
import { JobsModule } from './jobs/jobs.module';
import { HttpClientModule } from '@angular/common/http';
import { JobScheduleModule } from './job-schedule/job-schedule.module';
import { JobSessionModule } from './job-sessions/job-sessions.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    SharedMaterialModule,
    HomeModule,
    JobsModule,
    JobScheduleModule,
    JobSessionModule,
    AppRoutingModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
