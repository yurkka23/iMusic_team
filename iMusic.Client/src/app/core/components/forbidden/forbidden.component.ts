import {Location} from '@angular/common';
import {Component} from '@angular/core';


@Component({
  templateUrl: './forbidden.component.html',
  styleUrls: ['./forbidden.component.scss', './forbidden-responsive.component.scss']
})
export class ForbiddenComponent {
  constructor(private readonly location: Location) {
  }

  public goBack(): void {
    this.location.back();
  }
}
