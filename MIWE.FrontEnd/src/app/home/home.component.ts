import { Component, OnInit } from '@angular/core';
import { InstanceService } from '../services/instance.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  instanceType = '';
  title = 'miwe';
  navLinks = [
    { label: 'Jobs', path: '/home/jobs', isActive: true },
    { label: 'Schedules', path: '/home/schedules', isActive: false },
    { label: 'Sessions', path: '/home/sessions', isActive: false },
    { label: 'Instances', path: '/home/instances', isActive: false },
  ];
  constructor(private instanceService: InstanceService) { }

  ngOnInit(): void {
    this.instanceService.getCurrentInstanceType().subscribe(data => {
      this.instanceType = data;
    });
  }

}
