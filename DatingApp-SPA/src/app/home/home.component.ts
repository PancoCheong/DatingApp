import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  values: any;
  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getValues();
  }

  registerToggle() {
    this.registerMode = true; /* always set to true */
  }

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }

  /*demo code for parent to child component communication*/
  getValues() {
    /*@return is an Observable, so we need to subscribe to get content*/
    this.http.get('http://localhost:5000/api/values').subscribe(
      response => {
        this.values = response;
      },
      error => {
        console.log(error);
      }
    );
  }
}
