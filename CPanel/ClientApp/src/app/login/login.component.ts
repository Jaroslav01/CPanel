import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialog, MatSnackBar } from '@angular/material';
import { HttpClient } from '@angular/common/http';
import { LocalStorageService } from '../local-storage.service';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  hide = true;
  token: Token;
  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') public baseUrl: string,
    public dialog: MatDialog,
    private _snackBar: MatSnackBar,
    public localStorag: LocalStorageService,
    public appComponent: AppComponent) {
  }
  ngOnInit() {
  }
  Login() {
    var loginemail = <HTMLInputElement>document.getElementById("loginemail");
    var loginpassword = <HTMLInputElement>document.getElementById("loginpassword");
    this.http.post<Token>(this.baseUrl + "Account/token?username=" + loginemail.value + "&Password=" + loginpassword.value, {}).subscribe((result) => {
      this.token = result;
      this.localStorag.set("access_token", this.token.access_token);
      this.localStorag.set("email", this.token.email);
      this.localStorag.set("username", this.token.username);
      this.localStorag.set("role", this.token.role);
      this.appComponent.UpdateUserTitle();
      this._snackBar.open("Welcome " + this.token.username, "Undo", {
        duration: 2000,
      });
    });
  }
  Register() {
    var firstname = <HTMLInputElement>document.getElementById("firstname");
    var lastname = <HTMLInputElement>document.getElementById("lastname");
    var registeremail = <HTMLInputElement>document.getElementById("registeremail");
    var registerpassword = <HTMLInputElement>document.getElementById("registerpassword");
    this.http.get<any>(this.baseUrl + "Account/Register?login=" + registeremail.value + "&password=" + registerpassword.value + "&firstName=" + firstname.value + "&lastName=" + lastname.value).toPromise()
      .then(function (response) {
        return response.text();
      });
    this._snackBar.open("Registered", "Undo", {
        duration: 2000,
      });
  }
}
interface Token {
  access_token: string;
  email: string;
  username: string;
  role: string;
}
