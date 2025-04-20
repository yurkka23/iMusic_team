import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { applicationRoleConstant } from 'src/app/core/constants';
import { ChangePasswordInterface, PersonalInfoInterface, UserInterface } from 'src/app/core/interfaces';
import { ImageCropperComponent } from '../image-cropper/image-cropper.component';
import { filter, takeUntil, tap } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService, NotificationService, StorageService, ValidationService } from 'src/app/core/services';
import {FormBuilder, FormGroup} from 'ngx-strongly-typed-forms';
import { Validators } from '@angular/forms';
import { UserApiService } from 'src/app/core/services/api/user-api.service';

@Component({
  selector: 'mus-acount',
  templateUrl: './acount.component.html',
  styleUrls: ['./acount.component.scss']
})
export class AcountComponent implements OnInit , OnDestroy {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();
  public readonly applicationRole = applicationRoleConstant;
  public user: UserInterface;
  public personalInfoForm: FormGroup<PersonalInfoInterface>;
  public changePasswordForm: FormGroup<ChangePasswordInterface>;
  public showOldPassword: boolean = true;
  public showNewPassword: boolean = true;
  public showConfirmNewPassword: boolean = true;

  constructor(    
    private readonly dialog: MatDialog,
    private readonly notificationService: NotificationService,
    private readonly formBuilder: FormBuilder,
    private readonly validationService: ValidationService,
    private readonly storageService: StorageService,
    private readonly authService: AuthService,
    private readonly userApiService: UserApiService
    ) { }

  ngOnInit(): void {
    this.getCurrentUser();
    this.setPersonalInfoForm();
    this.setChangePasswordForm();
     
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.user = currentUser;
      });
  }

  private setPersonalInfoForm(): void {
    this.personalInfoForm = this.formBuilder.group<PersonalInfoInterface>({
      firstName: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      aboutMe: [null],
      email: [null, [Validators.required, this.validationService.emailValidator]],
      userName: [null, [Validators.required]],
    });
    this.setPersonalInfoFormValue();
  }
  
  private setChangePasswordForm(): void {
    this.changePasswordForm = this.formBuilder.group<ChangePasswordInterface>({
      oldPassword: [null, Validators.required],
      newPassword: [null, [Validators.required, this.validationService.newPasswordValidator]],
      confirmNewPassword: [null, Validators.required],
    }, {validator: this.validationService.changePasswordValidation});
  }


  private setPersonalInfoFormValue(): void {
    this.personalInfoForm.patchValue({...this.user});
  }
  
  public selectAvatar(event: Event): void {
    if (!(event.target as HTMLInputElement).files[0]) {
      return;
    }
    const dialogRef = this.dialog.open(ImageCropperComponent, {
      data: event,
      maxHeight: '90vh'
    });

    dialogRef.afterClosed()
      .pipe(
        tap((): void => (event.target as HTMLInputElement).value = null),
        filter((value): boolean => !!value),
        takeUntil(this.unsubscribe$)
      )
      .subscribe((avatar): void => {
        this.uploadAvatar(avatar);
      });
  }

  private uploadAvatar(avatar: Blob): void {
    const fileReader = new FileReader();

    fileReader.readAsDataURL(avatar);

    const formData = new FormData();

    formData.append('userProfileImage', avatar);
    
    this.userApiService.updateUserProfileImage(formData)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.authService.getCurrentUser()
        this.notificationService.showNotification("Avatar updated");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public updatePersonalInfo(): void {
    if (this.personalInfoForm.invalid || this.personalInfoForm.pristine) {
      return;
    }

    this.userApiService.updatePersonalInfo(this.personalInfoForm.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.authService.getCurrentUser()

        this.personalInfoForm.markAsPristine();
        this.notificationService.showNotification("Personal info updated");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public changePassword(): void {
    if (this.changePasswordForm.invalid || this.changePasswordForm.pristine) {
      return;
    }

    this.userApiService.changePassword(this.changePasswordForm.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.changePasswordForm.reset();
        this.changePasswordForm.markAsPristine();
        this.notificationService.showNotification("Password changed");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });

  }

  public becomeSinger(): void { 
    this.userApiService.becomeSinger(this.user.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.authService.getCurrentUser();
        this.getCurrentUser();
        this.notificationService.showNotification("Request sended.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });

  }
  public deleteAcount(): void { 
    this.userApiService.deleteAcount()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.authService.logOut();
        this.notificationService.showNotification("Acount deleted.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });

  }

}
