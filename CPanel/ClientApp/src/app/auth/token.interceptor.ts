import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { LocalStorageService } from '../local-storage.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private localStorage: LocalStorageService) { }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${this.localStorage.get("access_token")}`
      }
    });
    return next.handle(request);
  }
}
