import { Component, OnDestroy, OnInit } from '@angular/core';
import {Validators} from '@angular/forms';
import {ActivatedRoute, Params, Router} from '@angular/router';
import {HttpErrorResponse, HttpStatusCode} from '@angular/common/http';

import {FormBuilder, FormGroup} from 'ngx-strongly-typed-forms';
import {Subject} from 'rxjs';

import {first, takeUntil} from 'rxjs/operators';

import {SignInInterface, SignUpInterface} from '../../../core/interfaces';

import {
  AuthApiService,
  AuthService,
  NotificationService,
  SidebarService,
  StorageService,
  ValidationService
} from '../../../core/services';

@Component({
  selector: 'mus-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit, OnDestroy {

  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public showPassword: boolean = false;
  public signUpFormGroup: FormGroup<SignUpInterface>;

  constructor(private readonly formBuilder: FormBuilder,
    private readonly validationService: ValidationService,
    private readonly router: Router,
    private readonly authApiService: AuthApiService,
    private readonly authService: AuthService,
    private readonly notificationService: NotificationService,
    private readonly sidebarService: SidebarService
    ) {
  }

  public ngOnInit(): void {
    this.setForms();
    this.sidebarService.isSidebarOpened$.next(false);
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
    this.sidebarService.isSidebarOpened$.next(true);

  }
  
  private setForms(): void {
    this.signUpFormGroup = this.formBuilder.group<SignUpInterface>({
      email: [null, [Validators.required, this.validationService.emailValidator]],
      firstName: [null, [Validators.required, this.validationService.whitespaceValidator]],
      lastName: [null, [Validators.required, this.validationService.whitespaceValidator]],
      userName: [null, [Validators.required, this.validationService.whitespaceValidator]],
      password: [null, [this.validationService.newPasswordValidator]],
      confirmPassword: [null, [Validators.required]],
    }, {validator: this.validationService.passwordsEqualCheck});
  }

  public signUpFormSubmit(): void {
    if (this.signUpFormGroup.invalid) {
      return;
    }

    this.authApiService.sendSignUpForm(this.signUpFormGroup.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.notificationService.showNotification("Thanks for signing up. Please sign in");
        this.router.navigate(["auth/sign-in"]);
      },
      (error: HttpErrorResponse): void => {
        let errorMessage = "Error request";
        
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

}
