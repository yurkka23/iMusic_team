import {Pipe, PipeTransform} from '@angular/core';


@Pipe({
  name: 'trimString'
})
export class TrimStringPipe implements PipeTransform {
  public transform(text: string, star: number, end: number): string {
    return text.substring(star, end);
  }
}
