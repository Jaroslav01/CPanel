import { Component } from '@angular/core';

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {
  public currentCount = 0;
  public i = 0;



  public incrementCounter() {
    this.currentCount++;
    this.i = 1;
    if (this.currentCount % 2 == 0) { this.i = 0; }

    else { this.i = 1;}
      

    var request = new Request('https://localhost:44333/Mqtt/'+this.i);
    fetch(request).then(function (response) {
      return response.text();
    }).then(function (text) {
      console.log(text.substring(0, 30));
    });

  }
}
