import {Directive, ElementRef, HostListener} from '@angular/core';


@Directive({
  selector: '[musBecomeVisibleOnLoaded]'
})
export class BecomeVisibleOnLoadedDirective {

  constructor(private element: ElementRef) {
  }

  @HostListener('load') private onLoad(): void {
    this.element.nativeElement.classList.remove('hidden');
  }
}
