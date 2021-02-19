import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { getBaseUrl } from 'src/main';
import {MatDialog, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import * as signalR from "@microsoft/signalr";

export interface DialogData {
  animal: 'panda' | 'unicorn' | 'lion';
}
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})

export class HomeComponent {
  showFiller = false;
  value = 'Clear me';
  public connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .build();


  public currentCount = 0;
  public i = 1;
  public topic: string;
  public response: Parameter[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, public dialog: MatDialog, private _snackBar: MatSnackBar) {
    this.connection.start().catch(err => document.write(err));

    this.connection.on("mqttsyncres", (response: Parameter[]) => {
      this.response = response;
      console.log(response);
    });
    http.get<Parameter[]>(baseUrl + 'mqtt/GetParameters').subscribe(result => {
      this.response = result;
      this.connection.send("MqttSync", this.response);
      console.log(this.response);
    }, error => console.error(error));

  }
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

  public OnOff2(topic2: string) {
    this.currentCount++;
    this.i = 1;
    if (this.currentCount % 2 == 0) { this.i = 0; }
    else { this.i = 1; }
    var request = new Request(getBaseUrl() + "mqtt/set?topic=" + topic2 + "&value=" + this.i);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
      console.log(text.substring(0, 30));
    });
  }
  public Update(id: string) {
    var topic = <HTMLInputElement>document.getElementById(id + "topic");
    var name = <HTMLInputElement>document.getElementById(id + "name");
    console.log(topic.value);
   

    console.log(this.response);
    this.response[0]["name"] = name.value;
    console.log(this.response);
this.connection.send("MqttSync",this.response);

console.log(this.response);
    this.openSnackBar("Updated","Hide");
  }
  public Delete(id: number) {
    var name = <HTMLInputElement>document.getElementById(id + "name");

    var request = new Request(getBaseUrl() + "mqtt/Delete?id=" + id);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
    });
    this.connection.on("mqttsyncres", (response: Parameter[]) => {
      this.response = response;
    });
    this.openSnackBar("Deleted " + name.value, "Hide");
  }
}

interface Parameter {
  id: number;
  deviseId: number;
  name: string;
  topic: string;
  data: string;
}

@Component({
  selector: 'dialog-data-example-dialog',
  templateUrl: 'dialog-data-example-dialog.html',
})
export class DialogElementsExampleDialog {
  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData, @Inject('BASE_URL') baseUrl: string, private _snackBar: MatSnackBar) { }

  public Add() {
    var topic = <HTMLInputElement>document.getElementById("topic");
    var name = <HTMLInputElement>document.getElementById("name");
    console.log(topic.value);
    var request = new Request(getBaseUrl() + "mqtt/update?topic=" + topic.value + "&name=" + name.value);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
      console.log(document.getElementById("topic"));

    });
    this.openSnackBar("Saccesfull add " + name.value, "Hide");
  }

  public openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }
}
