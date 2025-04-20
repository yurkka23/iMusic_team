import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FavoriteListRoutingModule } from './favorite-list-routing.module';
import { FavoriteListComponent } from './components/favorite-list/favorite-list.component';
import { CoreModule } from '../core/core.module';


@NgModule({
  declarations: [
    FavoriteListComponent
  ],
  imports: [
    CommonModule,
    FavoriteListRoutingModule,
    CoreModule
  ]
})
export class FavoriteListModule { }
