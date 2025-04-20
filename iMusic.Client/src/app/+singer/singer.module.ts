import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SingerRoutingModule } from './singer-routing.module';
import { OverviewComponent } from './components/overview/overview.component';
import { YourSongsComponent } from './components/your-songs/your-songs.component';
import { YourAlbumsComponent } from './components/your-albums/your-albums.component';
import { CoreModule } from '../core/core.module';
import { FormSongComponent } from './components/your-songs/form-song/form-song.component';
import { FormAlbumComponent } from './components/your-albums/form-album/form-album.component';
import { EditAlbumSongsComponent } from './components/your-albums/edit-album-songs/edit-album-songs.component';


@NgModule({
  declarations: [
    OverviewComponent,
    YourSongsComponent,
    YourAlbumsComponent,
    FormSongComponent,
    FormAlbumComponent,
    EditAlbumSongsComponent
  ],
  imports: [
    CommonModule,
    SingerRoutingModule,
    CoreModule
  ]
})
export class SingerModule { }
