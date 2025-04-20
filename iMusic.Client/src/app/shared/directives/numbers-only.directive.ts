import {Directive, HostListener} from '@angular/core';
import {NgControl} from '@angular/forms';


@Directive({
  selector: '[musNumbersOnly]'
})
export class NumberOnlyDirective {
  constructor(private element: NgControl) {
  }

  @HostListener('input', ['$event.target.value'])
  public onInput(value: string): void {
    this.element.control.patchValue(value.replace(/[^0-9]/g, ''));
  }
}
