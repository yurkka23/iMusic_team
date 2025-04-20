import {Injectable} from '@angular/core';
import {Router} from '@angular/router';

import {BehaviorSubject, Observable} from 'rxjs';

import {UserInterface} from '../interfaces';

import {AuthApiService, StorageService} from '../services';
import { UserApiService } from './api/user-api.service';
import { HttpErrorResponse } from '@angular/common/http';


@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly userSubject$: BehaviorSubject<UserInterface> = new BehaviorSubject<UserInterface>(null);
  public readonly user$: Observable<UserInterface> = this.userSubject$.asObservable();

  private user: UserInterface;

  constructor(private readonly storageService: StorageService,
              private readonly authApiService: AuthApiService,
              private readonly router: Router,
              private readonly userApiService: UserApiService) {
  }

  public setUser(user: UserInterface): void {
    this.userSubject$.next(user);
  }

  public isAuthenticated(): boolean {
    const token = this.storageService.getToken();

    return token !== null && token !== undefined;
  }

  public getCurrentUser(): void {
    this.userApiService.getCurrentUser()
      .subscribe((currentUser): void => {
        this.storageService.setCurrentUserRoles(currentUser.userRoles);
        this.setUser(currentUser);
        this.user = currentUser;
      },(error: HttpErrorResponse): void => {
        this.storageService.clear();
        this.userSubject$.next(null);
      });
  }

  public logOut(): void {
    this.storageService.clear();
    this.userSubject$.next(null);
    this.router.navigate(['about-us']);
  }
}
