import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {SharedModule} from '../shared/shared.module';

import {AboutUsRoutingModule} from './about-us-routing.module';

import {AboutUsComponent} from './about-us.component';


@NgModule({
  declarations: [AboutUsComponent],
  imports: [
    CommonModule,
    SharedModule,
    AboutUsRoutingModule
  ]
})
export class AboutUsModule {
}
