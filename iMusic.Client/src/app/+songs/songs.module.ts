import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SongsRoutingModule } from './songs-routing.module';
import { SongsComponent } from './components/songs/songs.component';
import { CoreModule } from '../core/core.module';


@NgModule({
  declarations: [
    SongsComponent
  ],
  imports: [
    CommonModule,
    SongsRoutingModule,
    CoreModule
  ]
})
export class SongsModule { }
