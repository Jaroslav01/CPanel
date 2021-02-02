
import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { getBaseUrl } from '../../main';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'

})

export class FetchDataComponent {
  public response: Parameter[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Parameter[]>(baseUrl + 'mqtt/GetParameters').subscribe(result => {
      this.response = result;
    }, error => console.error(error));
  }
  public Update(id: string) {
    var topic = <HTMLInputElement>document.getElementById(id + "topic");
    var name = <HTMLInputElement>document.getElementById(id + "name");
    console.log(topic.value);
    var request = new Request(getBaseUrl() + "mqtt/update?topic=" + topic.value + "&name=" + name.value);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
      console.log(document.getElementById(id + "topic"));
    });
  }
  public Add(id: string) {
    var topic = <HTMLInputElement>document.getElementById(id + "topic");
    var name = <HTMLInputElement>document.getElementById(id + "name");
    console.log(topic.value);
    var request = new Request(getBaseUrl() + "mqtt/update?topic=" + topic.value + "&name=" + name.value);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
      console.log(document.getElementById(id + "topic"));
    });
    topic.value = "topic";
    name.value = "name";
  }
}

interface Parameter {
  id: number;
  deviseId: number;
  name: string;
  topic: string;
  data: string;
}
