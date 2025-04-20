import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BrowseRoutingModule } from './browse-routing.module';
import { BrowseComponent } from './components/browse/browse.component';
import { CoreModule } from '../core/core.module';
import { SiteRecommendsComponent } from './components/site-recommends/site-recommends.component';
import { TopSongsComponent } from './components/top-songs/top-songs.component';
import { NewMusicComponent } from './components/new-music/new-music.component';
import { TopPlaylistsComponent } from './components/top-playlists/top-playlists.component';
import { RecommendedForYouComponent } from './components/recommended-for-you/recommended-for-you.component';
import { AlbumComponent } from './components/album/album.component';
import { PlaylistComponent } from './components/playlist/playlist.component';
import { SingerComponent } from './components/singer/singer.component';
import { SongComponent } from './components/song/song.component';


@NgModule({
  declarations: [
    BrowseComponent,
    SiteRecommendsComponent,
    TopSongsComponent,
    NewMusicComponent,
    TopPlaylistsComponent,
    RecommendedForYouComponent,
    AlbumComponent,
    PlaylistComponent,
    SingerComponent,
    SongComponent
  ],
  imports: [
    CommonModule,
    BrowseRoutingModule,
    CoreModule
  ]
})
export class BrowseModule { }
