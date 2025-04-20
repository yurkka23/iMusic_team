import {Location} from '@angular/common';
import {Component} from '@angular/core';


@Component({
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.scss', './not-found-responsive.component.scss']
})
export class NotFoundComponent {
  constructor(private readonly location: Location) {
  }

  public goBack(): void {
    this.location.back();
  }
}
