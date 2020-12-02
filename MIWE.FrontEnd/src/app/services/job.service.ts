import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppSettings } from '../shared/app.settings';
import { Job } from '../models/job';

@Injectable({
  providedIn: 'root'
})
export class JobService {

  constructor(private httpClient: HttpClient) {}

  public getAll(): Observable<Job[]> {
    return this.httpClient.get<Job[]>(`${AppSettings.API_ENDPOINT}/Job`);
  }

  public getAllAssociatedJobs(ids: string[]): Observable<Job[]>{
    return this.httpClient.post<Job[]>(`${AppSettings.API_ENDPOINT}/Job/GetAssociatedJobs`, ids);
  }

  public get(id): Observable<Job> {
    return this.httpClient.get<Job>(`${AppSettings.API_ENDPOINT}/Job/${id}`);
  }

  public add(Job: Job) {
    return this.httpClient.post(`${AppSettings.API_ENDPOINT}/Job`, Job);
  }

  public update(Job: Job) {
    return this.httpClient.put(`${AppSettings.API_ENDPOINT}/Job/${Job.id}`, Job);
  }

  public upload(file: File, name: string){
    const formData = new FormData();
    formData.append('file', file);
    return this.httpClient.post(`${AppSettings.API_ENDPOINT}/Job/UploadFile?name=${name}`, formData);
  }

  public startjob(id) {
    return this.httpClient.get(`${AppSettings.API_ENDPOINT}/Job/Run/${id}`);
  }

  public stopjob(id) {
    return this.httpClient.get(`${AppSettings.API_ENDPOINT}/Job/Stop/${id}`);
  }
}

