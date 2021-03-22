import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { getBaseUrl } from 'src/main';
import {MatDialog, MAT_DIALOG_DATA} from '@angular/material/dialog';
import * as signalR from "@microsoft/signalr";
import {
  MatSnackBar,
  MatSnackBarHorizontalPosition,
  MatSnackBarVerticalPosition,
} from '@angular/material/snack-bar';

export interface DialogData {
  animal: 'panda' | 'unicorn' | 'lion';
}
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
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
    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, public dialog: MatDialog, private _snackBar: MatSnackBar) {
    http.get<Parameter[]>(baseUrl + 'mqtt/GetParameters').subscribe(result => {
      this.response = result;
      console.log(this.response);
    }, error => console.error(error));
  }
  ngOnInit() {
    this.start();
    this.connection.on("mqttsyncres", (action: string, id: number, deviseId: number, name: string, topic: string, data: string, type: string) => {
      var item: Parameter;
      console.log(action)
      if (action == "update") {
        for (var i = 0; i < this.response.length; i++) {
          if (this.response[i]["id"] == id) {
            this.response[i]["deviseId"] = deviseId;
            this.response[i]["name"] = name;
            this.response[i]["topic"] = topic;

                this.response[i]["data"] = Boolean(Number(data));
            
            this.response[i]["type"] = type;
          }
        }
      }
      else if (action == "add") {
        var item: Parameter;
        item = { id, deviseId, name, topic, data, type };
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
  public OnOff(topic: string, value:string) {
    
    var request = new Request(getBaseUrl() + "mqtt/set?topic=" + topic + "&value=" + value);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
      console.log(document.getElementById("mqtt/set?topic=" + topic + "&value=" + value));
    });
    this.openSnackBar("Updated", "Hide");
  }
  public Update(id: string) {
    var topic = <HTMLInputElement>document.getElementById(id + "topic");
    var name = <HTMLInputElement>document.getElementById(id + "name");
    var type: string;
    var request = new Request(getBaseUrl() + "mqtt/UpdateParameter?id=" + id + "&name=" + name.value + "&type" + type);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
      console.log(document.getElementById(id + "topic"));
    });
    this.openSnackBar("Updated", "Hide");
  }
  public Delete(id: string) {
    var name = <HTMLInputElement>document.getElementById(id + "name");

    var request = new Request(getBaseUrl() + "mqtt/Delete?id=" + id);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
    });
    this.openSnackBar("Deleted " + name.value, "Hide");
  }
}

interface Parameter {
  id: number;
  deviseId: number;
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
  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData, @Inject('BASE_URL') baseUrl: string, private _snackBar: MatSnackBar) { }
  public Add() {
    var type:string;
    var topic = <HTMLInputElement>document.getElementById("topic");
    var name = <HTMLInputElement>document.getElementById("name");
    console.log(topic.value);
    var request = new Request(getBaseUrl() + "mqtt/AddParameter?topic=" + topic.value + "&type" + type + "&name=" + name.value);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
      console.log(document.getElementById("topic"));
    });
    this.openSnackBar("Saccesfull add " + name.value, "Hide");
  }
  public openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 1000,
    });
  }
}
