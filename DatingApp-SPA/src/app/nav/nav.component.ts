import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(private authService: AuthService) {}

  ngOnInit() {}

  login() {
    /* subscribe to Obserable
    // use overload function that has next, error and complete */
    this.authService.login(this.model).subscribe(
      next => {
        console.log('Logged in successfully');
      },
      error => {
        console.log(error);
      }
    );
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token; /*if something in the token, return true; if empty, return false*/
  }

  logout() {
    localStorage.removeItem('token');
    console.log('logged out');
  }
}
