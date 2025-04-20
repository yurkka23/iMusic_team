import {Pipe, PipeTransform} from '@angular/core';

import {FormGroup} from 'ngx-strongly-typed-forms';


@Pipe({
  name: 'isInvalid',
  pure: false
})
export class IsInvalidPipe implements PipeTransform {
  public transform(group: FormGroup<unknown>): boolean {
    return group.invalid || group.pristine;
  }
}
