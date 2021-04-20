import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import {MatDialog, MAT_DIALOG_DATA} from '@angular/material/dialog';
import * as signalR from "@microsoft/signalr";
import { LocalStorageService } from '../local-storage.service';
import {
  MatSnackBar,
  MatSnackBarHorizontalPosition,
  MatSnackBarVerticalPosition,
} from '@angular/material/snack-bar';
import { AppComponent } from '../app.component';

export interface DialogData {
  animal: 'panda' | 'unicorn' | 'lion';
}
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  showFiller = false;
  value = 'Clear me';
  public connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .withAutomaticReconnect()
    .build();
  public currentCount = 0;
  public i = 1;
  public topic: string;
  public response: Parameter[];
  public res: Parameter[]; 
  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public dialog: MatDialog, private _snackBar: MatSnackBar, public localStorag: LocalStorageService, public appComponent: AppComponent) {
    http.post<Parameter[]>(baseUrl + 'mqtt/GetParameters', {}, this.httpOptions()).subscribe((result) => {
      this.response = result;
      console.log(result);
    });
  }
  ngOnInit() {
    this.start();
    this.connection.on("mqttsyncres", (action: string, id: number, deviseId: number, userId: number, name: string, topic: string, data: string, type: string) => {
      var item: Parameter;
      console.log(action)
      if (action == "update") {
        for (var i = 0; i < this.response.length; i++) {
          if (this.response[i]["id"] == id) {
            this.response[i]["deviseId"] = deviseId;
            this.response[i]["userId"] = userId;
            this.response[i]["name"] = name;
            this.response[i]["topic"] = topic;
            this.response[i]["data"] = data;
            this.response[i]["type"] = type;
          }
        }
      }
      else if (action == "add") {
        var item: Parameter;
        item = { id, deviseId, userId, name, topic, data, type };
        this.response.push(item);
      }
      else if (action == "delete") {
        for (var i = 0; i < this.response.length; i++) {
          if (this.response[i]["id"] == id) {
            this.response.splice(i, 1);
          }
        }
      }
    });
  }
  public httpOptions() {
    var headers_object = new HttpHeaders().set("Authorization", "Bearer " + this.localStorag.get("access_token"));
    headers_object.append('Content-Type', 'application/json');
    headers_object.append("Authorization", "Bearer " + this.localStorag.get("access_token"));
    const httpOptions = {
      headers: headers_object
    };
    return httpOptions;
  }
  public async start() {
    try {
      this.connection.start();
      console.assert(this.connection.state === signalR.HubConnectionState.Connected);
      console.log("SignalR Connected.");
    } catch (err) {
      console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
      console.log(err);
      setTimeout(() => this.start(), 5000);
    }
  };
  public openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }
  openDialog() {
    this.dialog.open(DialogElementsExampleDialog, {
      data: {
        animal: 'panda'
      }
    }); 
  }
  public OnOff(topic: string, value: string, type: string) {
    if (type == "button") {
      if (value == "1") value = "0";
      else value = "1";
    }
    this.http.post<any>(this.baseUrl + "mqtt/set?topic=" + topic + "&value=" + value, {}, this.httpOptions()).subscribe((result) => {
      console.log(result);
    });
    this.openSnackBar("Updated", "Hide");
  }
  type: string;
  public Update(id: string) {
    var name = <HTMLInputElement>document.getElementById(id + "name");
    this.http.post<any>(this.baseUrl + "mqtt/UpdateParameter?id=" + id + "&name=" + name.value + "&type=" + this.type, {}, this.httpOptions()).subscribe((result) => {
      console.log(result);
      this.openSnackBar("Updated", "Hide");
    });
  }
  public Delete(id: string) {
    var name = <HTMLInputElement>document.getElementById(id + "name");
    this.http.post<any>(this.baseUrl + "mqtt/Delete?id=" + id, {}, this.httpOptions()).subscribe((result) => {
      console.log(result);
      this.openSnackBar("Deleted", "Hide");
    });
  }
}

interface Parameter {
  id: number;
  deviseId: number;
  userId: number;
  name: string;
  topic: string;
  data: any;
  type: string;
}

@Component({
  selector: 'dialog-data-example-dialog',
  templateUrl: 'dialog-data-example-dialog.html',
})
export class DialogElementsExampleDialog {
  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private _snackBar: MatSnackBar) { }
  type: string;
  public Add() {
    var topic = <HTMLInputElement>document.getElementById("topic");
    var name = <HTMLInputElement>document.getElementById("name");
    this.http.get<any>(this.baseUrl + "mqtt/AddParameter?topic=" + topic.value + "&type" + this.type + "&name=" + name.value).toPromise()
      .then(function (response) {
        return response.text();
      });
    this.openSnackBar("Saccesfull add " + name.value, "Hide");
  }
  public openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 1000,
    });
  }
}
