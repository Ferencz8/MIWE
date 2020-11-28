import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppSettings } from '../shared/app.settings';
import { JobSchedule } from '../models/job.schedule';

@Injectable({
  providedIn: 'root'
})
export class JobScheduleService {

  constructor(private httpClient: HttpClient) {}

  public getAll(): Observable<JobSchedule[]> {
    return this.httpClient.get<JobSchedule[]>(`${AppSettings.API_ENDPOINT}/JobSchedule`);
  }

  public get(id): Observable<JobSchedule> {
    return this.httpClient.get<JobSchedule>(`${AppSettings.API_ENDPOINT}/JobSchedule/${id}`);
  }

  public add(JobSchedule: JobSchedule) {
    return this.httpClient.post(`${AppSettings.API_ENDPOINT}/JobSchedule`, JobSchedule);
  }

  public update(JobSchedule: JobSchedule) {
    return this.httpClient.put(`${AppSettings.API_ENDPOINT}/JobSchedule/${JobSchedule.id}`, JobSchedule);
  }
}

