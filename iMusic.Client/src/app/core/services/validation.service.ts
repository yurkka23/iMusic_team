import {Injectable} from '@angular/core';
import {ValidationErrors} from '@angular/forms';


import {AbstractControl, FormControl, FormGroup, ValidatorFn} from 'ngx-strongly-typed-forms';
import {ChangePasswordInterface, ConfirmationPasswordInterface} from '../interfaces';


@Injectable({
  providedIn: 'root',
})
export class ValidationService {
  private config = {};

  constructor() {   
  }


  

  public emailValidator(control: AbstractControl<string>): unknown {
    const emailRegex = new RegExp('^(([^<>()\\[\\]\\\\.,;:\\s@"]+(\\.[^<>()\\[\\]\\\\.,;:\\s@"]+)*)|(".+"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$');

    if (control.value === null || control.value === '') {
      return null;
    }

    if (!emailRegex.test(control.value)) {
      return {emailIncorrect: true};
    }

    return null;
  }

  public newPasswordValidator(control: FormControl<string>): unknown {
    const newPasswordRegex = new RegExp(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*?&_\-^#(){}\[\]<>=+\\|/;:,.'"`~№₴\d])([A-Za-z\d@$!%*?&_\-^#(){}\[\]<>=+\\|/;:,.'"`~№₴]){8,}$/);

    if (control.value === null) {
      return null;
    }

    if (!newPasswordRegex.test(control.value)) {
      control.markAsTouched();

      return {passwordIncorrect: true};
    }

    return null;
  }

  public passwordsEqualCheck(formGroup: AbstractControl<ConfirmationPasswordInterface>): ValidatorFn<unknown> {
    const {confirmPassword, password} = formGroup.value;

    if (!confirmPassword || !password) {
      return;
    }

    if (confirmPassword !== password) {
      (formGroup as FormGroup<ConfirmationPasswordInterface>).controls.confirmPassword.setErrors({'wrongConfirmPassword': true});
    } else if (confirmPassword === password) {
      (formGroup as FormGroup<ConfirmationPasswordInterface>).controls.confirmPassword.setErrors(null);
    }
  }

  public changePasswordValidation(formGroup: FormGroup<ChangePasswordInterface>): ValidatorFn<ChangePasswordInterface> {
    const {newPassword, confirmNewPassword} = formGroup.value;

    if (!newPassword || !confirmNewPassword) {
      return;
    }

    if (confirmNewPassword !== newPassword) {
      formGroup.controls.confirmNewPassword.setErrors({'wrongConfirmPassword': true});
    } else if (confirmNewPassword === newPassword) {
      formGroup.controls.confirmNewPassword.setErrors(null);
    }

    return;
  }

  public getValidatorErrorMessage(control: AbstractControl<unknown>, config = this.config): string {
    if (control) {
      for (const propertyName in control.errors) {
        if (control.errors.hasOwnProperty(propertyName)) {
          return config[propertyName];
        }
      }
    }

    return null;
  }

  public whitespaceValidator(control: FormControl<string>): ValidationErrors {
    if (control.value) {
      const value = Array.isArray(control.value) ? control.value.join('') : control.value;

      if (!value.trim().length) {
        return {whitespace: true};
      }
    }

    return null;
  }

  public autocompleteValueSelectedCheck(control: AbstractControl<unknown>): ValidationErrors {
    if (typeof control.value === 'string') {
      return {autocompleteValue: true};
    }

    return null;
  }

  public urlValidator(control: AbstractControl<string>): ValidationErrors {
    const urlRegex = new RegExp('^[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*$');

    if (control.value === null || control.value === '') {
      return null;
    }

    if (!urlRegex.test(control.value)) {
      return {urlInvalid: true};
    }

    return null;
  }

  private static isEmpty(value: number | null | undefined | string): boolean {
    return value === null || value === undefined || value.toString().trim() === '';
  }
}
