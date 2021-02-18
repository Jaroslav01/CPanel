import * as signalR from "@microsoft/signalr";

import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { getBaseUrl } from 'src/main';

@Component({
  selector: 'app-counter',
  templateUrl: './counter.component.html',
  styleUrls: ['./StyleSheet.css']
})





export class CounterComponent {
 public connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
  .build();
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
   
 this.connection.start().catch(err => document.write(err));
  }



  
  public send() {
    var divMessages = <HTMLDivElement>document.getElementById("divMessages");
    var tbMessage = <HTMLInputElement>document.getElementById("tbMessage");
    var btnSend = <HTMLButtonElement>document.getElementById("btnSend");
  var username = new Date().getTime();
  
   

 this.connection.send("newMessage", username, tbMessage.value)
    .then(() => tbMessage.value = "");
    this.connection.on("messageReceived", (username: string, message: string) => {
  let messages = document.createElement("div");

  messages.innerHTML =
    `<div class="message-author">${username}</div><div>${message}</div>`;

  divMessages.appendChild(messages);
  divMessages.scrollTop = divMessages.scrollHeight;
});

}
}








