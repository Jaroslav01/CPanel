import * as signalR from "@microsoft/signalr";

import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-counter',
  templateUrl: './counter.component.html',
  styleUrls: ['./StyleSheet.css']
})

export class CounterComponent {
  public connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .build();
  constructor(http: HttpClient, @Inject('BASE_URL') public baseUrl: string) {
    this.start();
    var divMessages = <HTMLDivElement>document.getElementById("divMessages");
    var tbMessage = <HTMLInputElement>document.getElementById("tbMessage");
    var btnSend = <HTMLButtonElement>document.getElementById("btnSend");
    var username = new Date().getTime();

  }
  public async  start() {
  try {
    await this.connection.start();
    console.assert(this.connection.state === signalR.HubConnectionState.Connected);
    console.log("SignalR Connected.");
  } catch (err) {
    console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
    console.log(err);
    setTimeout(() => this.start(), 5000);
  }
};
  public send() {
    var divMessages = <HTMLDivElement>document.getElementById("divMessages");
    var tbMessage = <HTMLInputElement>document.getElementById("tbMessage");
    var btnSend = <HTMLButtonElement>document.getElementById("btnSend");
    var username = new Date().getTime();

    if (this.connection.start.apply) {
      this.connection.on("messageReceived", (username: string, message: string) => {
        let messages = document.createElement("div");
        messages.innerHTML =
          `<div class="message-author">${username}</div><div>${message}</div>`;
        divMessages.appendChild(messages);
        divMessages.scrollTop = divMessages.scrollHeight;
      });
    }

    this.connection.send("newMessage", username, tbMessage.value)
      .then(() => tbMessage.value = "");
    

  }
}
