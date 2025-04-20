import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BrowseComponent } from './components/browse/browse.component';
import { SiteRecommendsComponent } from './components/site-recommends/site-recommends.component';
import { TopSongsComponent } from './components/top-songs/top-songs.component';
import { NewMusicComponent } from './components/new-music/new-music.component';
import { RecommendedForYouComponent } from './components/recommended-for-you/recommended-for-you.component';
import { TopPlaylistsComponent } from './components/top-playlists/top-playlists.component';
import { AlbumComponent } from './components/album/album.component';
import { PlaylistComponent } from './components/playlist/playlist.component';
import { SingerComponent } from './components/singer/singer.component';
import { SongComponent } from './components/song/song.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: BrowseComponent
  },
  {
    path: 'site-recommends',
    component: SiteRecommendsComponent
  },
  {
    path: 'top-songs',
    component: TopSongsComponent
  },
  {
    path: 'new-music',
    component: NewMusicComponent
  },
  {
    path: 'recommended-for-you',
    component: RecommendedForYouComponent
  },
  {
    path: 'top-playlists',
    component: TopPlaylistsComponent
  },
  {
    path: 'album/:id',
    component: AlbumComponent
  },
  {
    path: 'playlist/:id',
    component: PlaylistComponent
  },
  {
    path: 'singer/:id',
    component: SingerComponent
  },
  {
    path: 'song/:id',
    component: SongComponent
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BrowseRoutingModule { }
