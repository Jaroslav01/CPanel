import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
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
  public response: Devices[];
  public res: Devices[];
  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public dialog: MatDialog, private _snackBar: MatSnackBar, public localStorag: LocalStorageService, public appComponent: AppComponent) {
    http.post<Devices[]>(baseUrl + 'mqtt/GetDevices', {}).subscribe((result) => {
      this.response = result;
      console.log(result);
    },
      err => console.log(err.status));
  }
  ngOnInit() {
    this.start();
    this.connection.on("DevicesGet", (
      action: string,
      devices: Devices
    ) => {
      console.log(devices);
      console.log(action);
      if (action == "update") {
        for (var i = 0; i < this.response.length; i++) {
          if (this.response[i]["id"] == devices.id) {
            this.response[i]["id"] = devices.id;
            this.response[i]["ip"] = devices.ip;
            this.response[i]["mac"] = devices.mac;
            this.response[i]["freeMem"] = devices.freeMem;
            this.response[i]["name"] = devices.name;
            this.response[i]["topic"] = devices.topic;
            this.response[i]["rssi"] = devices.rssi;
            this.response[i]["uptime"] = devices.uptime;
            this.response[i]["userId"] = devices.userId;
          }
        }
      }
      else if (action == "add") {
        this.response.push(devices);
      }
      else if (action == "delete") {
        for (var i = 0; i < this.response.length; i++) {
          if (this.response[i]["id"] == devices.id) {
            this.response.splice(i, 1);
          }
        }
      }
    });
    this.connection.on("ParametersGet", (
      action: string,
      parameter: Parameter
    ) => {
      console.log(parameter);
      console.log(action);
      if (action == "update") {
        for (var i = 0; i < this.response.length; i++) {
          for (var j = 0; j < this.response[i].parameters.length; j++) {
            if (this.response[i].parameters[j]["id"] == parameter.id) {
              this.response[i].parameters[j]["id"] = parameter.id;
              this.response[i].parameters[j]["deviseId"] = parameter.deviseId;
              this.response[i].parameters[j]["userId"] = parameter.userId;
              this.response[i].parameters[j]["name"] = parameter.name;
              this.response[i].parameters[j]["topic"] = parameter.topic;
              this.response[i].parameters[j]["data"] = parameter.data;
              this.response[i].parameters[j]["type"] = parameter.type;
            }
          }
        }
      }
      else if (action == "add") {
        for (var i = 0; i < this.response.length; i++) {
          if (this.response[i].id == parameter.deviseId) {
            this.response[i].parameters.push(parameter);
          }
        }
      }
      else if (action == "delete") {
        for (var i = 0; i < this.response.length; i++) {
          for (var j = 0; j < this.response[i].parameters.length; j++) {
            if (this.response[i].parameters[j]["id"] == parameter.id) {
              this.response[i].parameters.splice(j, 1);
            }
          }
        }
      }
    });
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
    console.log(value);
    console.log(type);
    console.log(topic);

    if (type == "button") {
      if (value == "0") {
        value = "1";
      }
      else {
        value = "0";
      }
    }
    console.log(value);
    this.http.post<any>(this.baseUrl + "mqtt/set?topic=" + topic + "&value=" + value, {}).subscribe((result) => {
      console.log(result);
    });
    this.openSnackBar("Updated", "Hide");
  }
  type: string;
  public Update(id: string) {
    var name = <HTMLInputElement>document.getElementById(id + "name");
    this.http.post<any>(this.baseUrl + "mqtt/UpdateParameter?id=" + id + "&name=" + name.value + "&type=" + this.type, {}).subscribe((result) => {
      console.log(result);
      this.openSnackBar("Updated", "Hide");
    });
  }
  public Delete(id: string) {
    var name = <HTMLInputElement>document.getElementById(id + "name");
    this.http.post<any>(this.baseUrl + "mqtt/Delete?id=" + id, {}).subscribe((result) => {
      console.log(result);
      this.openSnackBar("Deleted", "Hide");
    });
  }
}
interface Devices {
  id: number;
  userId: number;
  uptime: number;
  rssi: number;
  name: string;
  topic: string;
  ip: string;
  mac: string;
  freeMem: number;
  parameters: Parameter[];
}
interface Parameter {
  id: number;
  deviseId: number;
  userId: number;
  name: string;
  topic: string;
  data: string;
  type: string;
}

@Component({
  selector: 'dialog-data-example-dialog',
  templateUrl: 'dialog-data-example-dialog.html',
})
export class DialogElementsExampleDialog {
  public response: Devices[];
  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private _snackBar: MatSnackBar, public localStorag: LocalStorageService) {
    http.post<Devices[]>(baseUrl + 'mqtt/GetDevices', {}).subscribe((result) => {
      this.response = result;
      console.log(result);
    });
  }
  type: string;
  deviceId: string;
  public AddDevice() {
    var topic = <HTMLInputElement>document.getElementById("topicDevice");
    var name = <HTMLInputElement>document.getElementById("nameDevice");
    this.http.post<any[]>(this.baseUrl + "mqtt/AddDevices?topic=" + topic.value + "&name=" + name.value, {}).subscribe((result) => {
      this.response = result;
      console.log(result);
    });
    this.openSnackBar("Saccesfull add " + name.value, "Hide");
  }
  public Add(type: string) {
    var topic = <HTMLInputElement>document.getElementById("topic");
    var name = <HTMLInputElement>document.getElementById("name");
    this.http.post<any[]>(this.baseUrl + "mqtt/AddParameter?topic=" + topic.value + "&type" + type + "&name=" + name.value + "&deviceId=" + this.deviceId, {}).subscribe((result) => {
      this.response = result;
      console.log(result);
    });
    this.openSnackBar("Saccesfull add " + name.value, "Hide");
  }
  public openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 1000,
    });
  }

}
