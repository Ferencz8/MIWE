import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  title = 'miwe';
  navLinks = [
    { label: 'Jobs', path: '/home/jobs', isActive: true },
    { label: 'Schedules', path: '/home/schedules', isActive: false },
    { label: 'Sessions', path: '/home/sessions', isActive: false },
  ];
  constructor() { }

  ngOnInit(): void {
  }

}
