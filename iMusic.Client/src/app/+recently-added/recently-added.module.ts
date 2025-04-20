import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RecentlyAddedRoutingModule } from './recently-added-routing.module';
import { RecentlyAddedComponent } from './components/recently-added/recently-added.component';
import { CoreModule } from '../core/core.module';


@NgModule({
  declarations: [
    RecentlyAddedComponent
  ],
  imports: [
    CommonModule,
    RecentlyAddedRoutingModule,
    CoreModule
  ]
})
export class RecentlyAddedModule { }
