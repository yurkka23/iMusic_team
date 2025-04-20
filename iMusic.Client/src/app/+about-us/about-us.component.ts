import {Component} from '@angular/core';
import { Router } from '@angular/router';


@Component({
  templateUrl: './about-us.component.html',
  styleUrls: ['./about-us.component.scss', './about-us-responsive.component.scss']
})
export class AboutUsComponent {

  constructor(private readonly router: Router) { }

  openSongs():void{
    this.router.navigate(['all-articles']);
  }
  openSignUp():void{
    this.router.navigate(['register']);
  }
}
