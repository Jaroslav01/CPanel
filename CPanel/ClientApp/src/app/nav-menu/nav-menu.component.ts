import { Component, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';
import { LocalStorageService } from '../local-storage.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  constructor(public localStorag: LocalStorageService, public appComponent: AppComponent) {

  }
  isExpanded = false;
  hidden = false;
  user:string;
  ngOnInit() {
    if (this.user == null) this.user = "Guest";
    this.user = this.localStorag.get("username");
  }
  toggleBadgeVisibility() {
    this.hidden = !this.hidden;
  }
  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
