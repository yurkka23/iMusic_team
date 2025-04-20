import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OverviewComponent } from './components/overview/overview.component';
import { YourSongsComponent } from './components/your-songs/your-songs.component';
import { YourAlbumsComponent } from './components/your-albums/your-albums.component';
import { EditAlbumSongsComponent } from './components/your-albums/edit-album-songs/edit-album-songs.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'overview',
    pathMatch: 'full',
  },
  {
    path: 'overview',
    component: OverviewComponent,
  },
  {
    path: 'your-songs',
    component: YourSongsComponent,
  },
  {
    path: 'your-albums',
    component: YourAlbumsComponent,
  },
  {
    path: 'album/:id',
    component: EditAlbumSongsComponent
  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SingerRoutingModule { }
