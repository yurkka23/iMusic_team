import {Directive, HostListener} from '@angular/core';
import {NgControl} from '@angular/forms';


@Directive({
  selector: '[musOneHundredPercentDirective]'
})
export class OneHundredPercentDirective {
  constructor(private element: NgControl) {
  }

  @HostListener('input', ['$event.target.value'])
  public onInput(value: string): void {
    this.element.control.patchValue(value.replace('', ''));
  }
}
