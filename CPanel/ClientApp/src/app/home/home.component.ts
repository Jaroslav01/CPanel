import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { getBaseUrl } from 'src/main';
import {MatDialog, MAT_DIALOG_DATA} from '@angular/material/dialog';

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


  
  public response: Parameter[];
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, public dialog: MatDialog) {
    http.get<Parameter[]>(baseUrl + 'mqtt/GetParameters').subscribe(result => {
      this.response = result;
    }, error => console.error(error));
  }
  
  
  openDialog() {
    this.dialog.open(DialogDataExampleDialog, {
      data: {
        animal: 'panda'
      }
    });}


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
  selector: 'dialog-data-example-dialog',
  templateUrl: 'dialog-data-example-dialog.html',
})
export class DialogDataExampleDialog {
  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData) {}
}
