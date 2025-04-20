import {Component, OnDestroy, OnInit} from '@angular/core';
import {Validators} from '@angular/forms';
import {ActivatedRoute, Params, Router} from '@angular/router';
import {HttpErrorResponse, HttpStatusCode} from '@angular/common/http';

import {FormBuilder, FormGroup} from 'ngx-strongly-typed-forms';
import {Subject} from 'rxjs';

import {first, takeUntil} from 'rxjs/operators';

import {SignInInterface} from '../../../core/interfaces';

import {
  AuthApiService,
  AuthService,
  NotificationService,
  SidebarService,
  StorageService,
  ValidationService
} from '../../../core/services';
import { UserApiService } from 'src/app/core/services/api/user-api.service';


@Component({
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss', './sign-in-responsive.component.scss']
})
export class SignInComponent implements OnInit, OnDestroy {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  private redirectPath: string = null;

  public showPassword: boolean = false;
  public signInFormGroup: FormGroup<SignInInterface>;

  constructor(private readonly formBuilder: FormBuilder,
    private readonly validationService: ValidationService,
    private readonly router: Router,
    private readonly storageService: StorageService,
    private readonly authApiService: AuthApiService,
    private readonly userApiService: UserApiService,
    private readonly authService: AuthService,
    private readonly notificationService: NotificationService,
    private readonly activatedRoute: ActivatedRoute,
    private readonly sidebarService: SidebarService
  ) {
  }

  public ngOnInit(): void {
    this.setForms();

    this.activatedRoute.params
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((params: Params): void => {
        this.redirectPath = params.redirectPath;
      });
      this.sidebarService.isSidebarOpened$.next(false);

  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
    this.sidebarService.isSidebarOpened$.next(true);
  }

  private setForms(): void {
    this.signInFormGroup = this.formBuilder.group<SignInInterface>({
      email: [null, [Validators.required, this.validationService.emailValidator]],
      password: [null, [Validators.required, this.validationService.newPasswordValidator]]
    });
  }

  public signInFormSubmit(): void {
    if (this.signInFormGroup.invalid) {
      return;
    }

    this.authApiService.signIn(this.signInFormGroup.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((result): void => {

        this.storageService.setToken(result.accessToken);
        this.storageService.setRefreshToken(result.refreshToken);

        this.userApiService.getCurrentUser().pipe(first())
          .subscribe((currentUser): void => {
            this.authService.setUser(currentUser);
            this.storageService.setCurrentUserRoles(currentUser.userRoles);
            this.storageService.setInitialRolesRoles(currentUser.userRoles);
            this.router.navigate([""]);
          });

        this.notificationService.showNotification("Success login")
      },
      (error: HttpErrorResponse): void => {
        let errorMessage: string;

        switch (error.status) {
          case HttpStatusCode.NotFound:
            errorMessage = "Login or Password Incorrect";
            break;
          case HttpStatusCode.Forbidden:
            errorMessage = "User Not Verified";
            break;
          default:
            errorMessage = "You can't sign in";
        }

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }
}
