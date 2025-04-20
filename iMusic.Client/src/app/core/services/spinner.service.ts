import {HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';

import {BehaviorSubject} from 'rxjs';


const excludeRequests: string[] = ['assets', 'autocomplete', 'read', 'chat', 'message', 'negotiate', '.svg'];

@Injectable({
  providedIn: 'root',
})
export class SpinnerService {
  public readonly onLoadingChanged$: BehaviorSubject<boolean> = new BehaviorSubject(false);

  private requests: Array<HttpRequest<unknown>> = [];

  public onStarted(request: HttpRequest<unknown>): void {
    if (excludeRequests.some((urlSegment): boolean => request.url.includes(urlSegment))) {
      return;
    }
    this.requests.push(request);
    this.notify();
  }

  public onFinished(request: HttpRequest<unknown>): void {
    const index = this.requests.indexOf(request);

    if (index !== -1) {
      this.requests.splice(index, 1);
      this.notify();
    }
  }

  private notify(): void {
    this.onLoadingChanged$.next(this.requests.length !== 0);
  }
}
