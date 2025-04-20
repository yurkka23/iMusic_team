import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PlaylistsRoutingModule } from './playlists-routing.module';
import { PlaylistsComponent } from './components/playlists/playlists.component';
import { CoreModule } from '../core/core.module';
import { FormPlaylistComponent } from './components/form-playlist/form-playlist.component';
import { PlaylistComponent } from './components/playlist/playlist.component';


@NgModule({
  declarations: [
    PlaylistsComponent,
    FormPlaylistComponent,
    PlaylistComponent
  ],
  imports: [
    CommonModule,
    PlaylistsRoutingModule,
    CoreModule
  ]
})
export class PlaylistsModule { }
