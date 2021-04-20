import { Component,Inject, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialog, MatSnackBar } from '@angular/material';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  hide = true;
  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public dialog: MatDialog, private _snackBar: MatSnackBar) {

  }
  ngOnInit() {
  }
  Register() {
    var firstname = <HTMLInputElement>document.getElementById("firstname");
    var lastname = <HTMLInputElement>document.getElementById("lastname");
    var registeremail = <HTMLInputElement>document.getElementById("registeremail");
    var registerpassword = <HTMLInputElement>document.getElementById("registerpassword");
    this.http.post<any>(this.baseUrl + "Account/Register?login=" + registeremail.value + "&password=" + registerpassword.value + "&firstName=" + firstname.value + "&lastName=" + lastname.value).toPromise()
      .then(function (response) {
        return response.text();
      });
  this._snackBar.open("Registered", "Undo", {
      duration: 2000,
    });
  }
}
