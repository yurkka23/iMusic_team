import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';

import {Observable} from 'rxjs';
import {finalize} from 'rxjs/operators';

import {SpinnerService} from '../services';

@Injectable()
export class SpinnerInterceptor implements HttpInterceptor {

  constructor(private readonly spinnerService: SpinnerService) {
  }

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.spinnerService.onStarted(request);

    return next.handle(request).pipe(finalize((): void => this.spinnerService.onFinished(request)));
  }
}
