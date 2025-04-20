import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Router} from '@angular/router';

import {Observable, throwError} from 'rxjs';
import {catchError, filter, first, switchMap} from 'rxjs/operators';

import {TokenInformationInterface} from '../interfaces';

import {AuthApiService, AuthService, StorageService} from '../services';

@Injectable()
export class AuthorizationInterceptor implements HttpInterceptor {

  constructor(private readonly authService: AuthService,
              private readonly authApiService: AuthApiService,
              private readonly storageService: StorageService,
              private readonly router: Router) {
  }

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<never | HttpEvent<unknown>> {
    const userToken = this.storageService.getToken();

    const excludeRequests = ['assets', 'login'];

    if (excludeRequests.some((urlSegment): boolean => request.url.includes(urlSegment))) {
      return next.handle(request);
    }

    return next.handle(this.addTokenToRequest(request, userToken)).pipe(
      catchError((error): Observable<never | HttpEvent<unknown>> => {
        if (error instanceof HttpErrorResponse) {
          switch ((error as HttpErrorResponse).status) {
            case 401:
              return this.handleUnauthorizedError(request, next);

            case 403:

              return this.handleForbiddenError();
            default:
              return throwError(error);
          }
        }
      })
    );
  }

  private addTokenToRequest(request: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
    return request.clone({setHeaders: {authorization: `Bearer ${token}`}});
  }

  private handleForbiddenError(): Observable<never> {
    this.authService.user$
      .pipe(
        first(),
        filter((user): boolean => !!user),
      )
      .subscribe((user): void => {
        if (this.storageService.checkInitialRoles()) {
          if (!(user.userRoles.sort().join('') === this.storageService.getInitialRoles().sort().join(''))) {
            this.authService.logOut();
          } else {
            this.router.navigate(['403']);
          }
        } else {
          this.router.navigate(['403']);
        }
      });

    return throwError('');
  }

  private handleUnauthorizedError(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (this.storageService.getRefreshToken()) {
      return this.authApiService.refreshToken().pipe(
        switchMap((result: TokenInformationInterface): Observable<HttpEvent<unknown>> => {
          const {accessToken, refreshToken} = result;

          this.storageService.setToken(accessToken);
          this.storageService.setRefreshToken(refreshToken);

          return next.handle(this.addTokenToRequest(request, accessToken));
        }),
        catchError((error): Observable<never> => {
          this.authService.logOut();

          return throwError(error);
        })
      );
    } else {
      this.authService.logOut();

      return throwError('');
    }
  }
}
