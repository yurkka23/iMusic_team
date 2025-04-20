import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ArtistsRoutingModule } from './artists-routing.module';
import { ArtistsComponent } from './components/artists/artists.component';
import { CoreModule } from '../core/core.module';


@NgModule({
  declarations: [
    ArtistsComponent
  ],
  imports: [
    CommonModule,
    ArtistsRoutingModule,
    CoreModule
  ]
})
export class ArtistsModule { }
