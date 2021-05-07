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
import {FormControl} from '@angular/forms';


export interface DialogData {
  animal: 'panda' | 'unicorn' | 'lion';
}
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  myControl = new FormControl();
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
  panelOpenState = false;
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
    var val:string;
    console.log(value);
    console.log(type);
    if (type == "button") {
      if (value == "0") {
        val = "1";
      }
      else {
        val = "0";
      } 
    }
    else{
      val = value;
    }
    this.http.post<any>(this.baseUrl + "mqtt/set?topic=" + topic + "&value=" + val, {}).subscribe((error) => {
      console.log(error);
    });
  }
  public UpdateParameter(id: string, name: string, topic: string, type: string) {
    this.http.post<any>(this.baseUrl + "mqtt/UpdateParameter?id=" + id + "&name=" + name + "&type=" + type, {}).subscribe((result) => {
      this.openSnackBar("Updated", "Hide");
    });
  }
  public UpdateDevice(id: string, name: string, topic: string) {
    this.http.post<any>(this.baseUrl + "mqtt/UpdateDevices?id=" + id + "&name=" + name + "&topic=" + topic, {}).subscribe((result) => {
      this.openSnackBar("Updated", "Hide");
    });
  }
  public DeleteParameter(id: string) {
    this.http.post<any>(this.baseUrl + "mqtt/Delete?id=" + id, {}).subscribe((result) => {
    });
  }
  public DeleteDevice(id: string) {
    this.http.post<any>(this.baseUrl + "mqtt/DeleteDeviceWithParameters?id=" + id, {}).subscribe((result) => {
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
  myControl = new FormControl();
  public response: Devices[];
  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private _snackBar: MatSnackBar, public localStorag: LocalStorageService) {
    http.post<Devices[]>(baseUrl + 'mqtt/GetDevices', {}).subscribe((result) => {
      this.response = result;
      console.log(result);
    });
  }
  type: string;
  deviceId: string;
  public AddDevice(name: string, topic: string) {
    this.http.post<any[]>(this.baseUrl + "mqtt/AddDevices?topic=" + topic + "&name=" + name, {}).subscribe((result) => {
      this.response = result;
      console.log(result);
    });
    this.openSnackBar("Saccesfull add " + name, "Hide");
  }
  public Add(name:string, topic:string, deviceId:string, type: string) {
    this.http.post<any[]>(this.baseUrl + "mqtt/AddParameter?topic=" + topic + "&type" + type + "&name=" + name + "&deviceId=" + deviceId, {}).subscribe((result) => {
      this.response = result;
      console.log(result);
    });
    this.openSnackBar("Saccesfull add " + name, "Hide");
  }
  public openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 1000,
    });
  }

}
