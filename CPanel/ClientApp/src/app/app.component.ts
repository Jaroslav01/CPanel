import { Component, Inject, OnInit } from '@angular/core';

import { LocalStorageService } from './local-storage.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(public localStorag: LocalStorageService, @Inject('BASE_URL') public baseUrl: string) {

  }
  title = 'app';
  username = "Guest";
  ngOnInit() {
    console.log(this.username = this.localStorag.get("username"));
    if (this.localStorag.get("username") != null) {
      this.username = this.localStorag.get("username");
    }
  }
  UpdateUserTitle() {
    this.username = this.localStorag.get("username");
  }
  GuestUserTitle() {
    this.username = "Guest";
  }
}
