import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Console } from 'console';
import { getBaseUrl } from 'src/main';
import {MatDialog} from '@angular/material/dialog';

@Component({
  selector: 'home-component',
  templateUrl: './home.component.html'
})

export class HomeComponent {

  

  
  public response: Parameter[];
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, public dialog: MatDialog) {
    http.get<Parameter[]>(baseUrl + 'mqtt/GetParameters').subscribe(result => {
      this.response = result;
    }, error => console.error(error));
  }
  openDialog() {
    this.dialog.open(DialogElementsExampleDialog);
  }
  public currentCount = 0;
  public i = 1;
  public topic: string;
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
}

interface Parameter {
  id: number;
  deviseId: number;
  name: string;
  topic: string;
  data: string;
}
@Component({
  selector: 'dialog-elements-example-dialog',
  templateUrl: 'dialog-elements-example-dialog.html',
})
export class DialogElementsExampleDialog {}