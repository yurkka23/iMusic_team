import {Component, OnInit} from '@angular/core';
import {
  Event as RouterEvent,
  NavigationCancel,
  NavigationEnd,
  NavigationError,
  NavigationStart,
  Router
} from '@angular/router';

import {merge, Observable} from 'rxjs';
import {map} from 'rxjs/operators';

import {SpinnerService} from '../../services';


@Component({
  selector: 'mus-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss']
})
export class SpinnerComponent implements OnInit {
  public isLoadingHttp$: Observable<boolean>;
  public isLoading$: Observable<boolean>;

  constructor(private readonly spinnerService: SpinnerService,
              private readonly router: Router) {
  }

  private navigationInterceptor(event: RouterEvent): boolean {
    if (event instanceof NavigationStart) {
      return true;
    }
    if (event instanceof NavigationEnd) {
      return false;
    }

    if (event instanceof NavigationCancel) {
      return false;
    }
    if (event instanceof NavigationError) {
      return false;
    }

    return false;
  }

  public ngOnInit(): void {
    this.isLoadingHttp$ = this.spinnerService.onLoadingChanged$;

    this.isLoading$ = merge(this.router.events
      .pipe(map((event: RouterEvent): boolean => this.navigationInterceptor(event))), this.isLoadingHttp$);
  }
}
