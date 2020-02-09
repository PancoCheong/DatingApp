import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})

// MemberCardComponent is the child of MemberListComponent
export class MemberCardComponent implements OnInit {
  // pass down the user into card component
  @Input() user: User;

  constructor() {}

  ngOnInit() {}
}
