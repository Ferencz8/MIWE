import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppSettings } from '../shared/app.settings';
import { Instance } from '../models/instance';

@Injectable({
  providedIn: 'root'
})
export class InstanceService {

  constructor(private httpClient: HttpClient) {}

  public getAll(): Observable<Instance[]> {
    return this.httpClient.get<Instance[]>(`${AppSettings.API_ENDPOINT}/Instance`);
  }
}

