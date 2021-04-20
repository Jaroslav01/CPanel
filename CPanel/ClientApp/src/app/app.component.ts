import { Component, OnInit } from '@angular/core';

import { LocalStorageService } from './local-storage.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit{
  constructor(public localStorag: LocalStorageService) {

  }
  title = 'app';
  username = "Guest";
  ngOnInit() {
    this.username = this.localStorag.get("username");
  }
  UpdateUserTitle() {
    this.username = this.localStorag.get("username");
  }
  GuestUserTitle() {
    this.username = "Guest";
  }
}
