import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Instance } from 'src/app/models/instance';
import { InstanceService } from 'src/app/services/instance.service';
import { SnackbarService } from 'src/app/shared/snackbar.service';

@Component({
  selector: 'app-instances',
  templateUrl: './instances.component.html',
  styleUrls: ['./instances.component.css']
})
export class InstancesComponent implements OnInit {

  displayedColumns: string[] = ['ip', 'cpuThreshold', 'isAvailable', 'isMaster', 'isDown'];

  dataSource$: Observable<Instance[]>;
  mydata: Instance[] = [
    { id: '1', ip: 'sava', cpuThreshold: 50, isAvailable: true, isMaster: true, isDown: false}
  ];

  constructor(private instanceService: InstanceService, private snackbar: SnackbarService, private router: Router) { }

  ngOnInit(): void {

    this.instanceService.getAll().subscribe(res => {this.dataSource$ = of(res); });
  }
}
