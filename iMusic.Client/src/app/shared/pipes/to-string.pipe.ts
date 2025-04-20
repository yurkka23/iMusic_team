import {Pipe, PipeTransform} from '@angular/core';


@Pipe({
  name: 'toString'
})
export class ToStringPipe implements PipeTransform {
  public transform(value: number): string {
    return value.toString();
  }
}
