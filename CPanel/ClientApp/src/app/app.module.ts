import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent, DialogElementsExampleDialog } from './home/home.component';

import { CounterComponent } from './counter/counter.component';
import { PanelComponent } from './panel/panel.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';


import { MatSliderModule } from '@angular/material/slider';
import { MatListModule } from '@angular/material/list';
import { MatDialogModule } from '@angular/material/dialog'; 
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTabsModule } from '@angular/material/tabs';
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonToggleModule } from '@angular/material/button-toggle';

import * as signalR from "@microsoft/signalr";
import { LoginComponent } from './login/login.component';
import { AccountComponent } from './account/account.component';
import { TokenInterceptor } from './auth/token.interceptor';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PanelComponent,
    DialogElementsExampleDialog,
    CounterComponent,
    LoginComponent,
    AccountComponent,

  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    MatSliderModule,
    MatListModule,
    MatDialogModule,
    MatButtonModule,
    MatSidenavModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatSnackBarModule,
    MatSlideToggleModule,
    MatCardModule,
    MatFormFieldModule,
    MatBadgeModule,
    MatTabsModule,
    MatStepperModule,
    MatButtonToggleModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'panel', component: PanelComponent },
      { path: 'login', component: LoginComponent },
      { path: 'account', component: AccountComponent },
    ]),
    BrowserAnimationsModule
  ],
  entryComponents: [HomeComponent, DialogElementsExampleDialog],


  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: TokenInterceptor,
    multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
