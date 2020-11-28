import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppSettings } from '../shared/app.settings';
import { JobSession } from '../models/job.session';

@Injectable({
  providedIn: 'root'
})
export class JobSessionService {

  constructor(private httpClient: HttpClient) {}

  public getAll(): Observable<JobSession[]> {
    return this.httpClient.get<JobSession[]>(`${AppSettings.API_ENDPOINT}/JobSession`);
  }

  public downloadResult(jobSessionId: string): Observable<Blob> {
    return this.httpClient.get(`${AppSettings.API_ENDPOINT}/JobSession/DownloadResult/${jobSessionId}`, { responseType: 'blob' });
  }
}

