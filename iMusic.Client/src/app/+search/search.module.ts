import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SearchRoutingModule } from './search-routing.module';
import { SearchAllComponent } from './components/searchAll/searchAll.component';
import { CoreModule } from '../core/core.module';
import { CategorySongsComponent } from './components/category-songs/category-songs.component';


@NgModule({
  declarations: [
    SearchAllComponent,
    CategorySongsComponent,
  ],
  imports: [
    CommonModule,
    SearchRoutingModule,
    CoreModule
  ]
})
export class SearchModule { }
