import {Component, Input} from '@angular/core';

import {AbstractControl} from 'ngx-strongly-typed-forms';

import {ValidationService} from '../../../core/services';


@Component({
  selector: 'mus-control-validation-message',
  styleUrls: ['./control-validation-message.component.scss'],
  templateUrl: './control-validation-message.component.html'
})
export class ControlValidationMessageComponent {
  @Input() public control: AbstractControl<unknown>;

  constructor(private readonly validationService: ValidationService) {
  }

  public get errorMessage(): string {
    return this.validationService.getValidatorErrorMessage(this.control);
  }
}
