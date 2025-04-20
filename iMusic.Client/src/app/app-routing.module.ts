import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

import {AboutUsModule} from './+about-us/about-us.module';
import {AuthModule} from './+auth/auth.module';

import {ForbiddenComponent} from './core/components/forbidden/forbidden.component';
import {NotFoundComponent} from './core/components/not-found/not-found.component';

import {RoleGuard} from './core/helpers/role.guard';

import {applicationRoleConstant} from './core/constants';
import { AcountModule } from './+acount/acount.module';
import { AlbumsModule } from './+albums/albums.module';
import { ArtistsModule } from './+artists/artists.module';
import { BrowseModule } from './+browse/browse.module';
import { FavoriteListModule } from './+favorite-list/favorite-list.module';
import { PlaylistsModule } from './+playlists/playlists.module';
import { RecentlyAddedModule } from './+recently-added/recently-added.module';
import { SongsModule } from './+songs/songs.module';
import { SingerModule } from './+singer/singer.module';
import { AdminModule } from './+admin/admin.module';
import { AuthGuard } from './core/helpers/auth.guard';
import { AcountComponent } from './+acount/components/acount/acount.component';
import { SearchModule } from './+search/search.module';



const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'browse'
  }, 
  {
    path: 'about-us',
    loadChildren: (): Promise<AboutUsModule> => import('./+about-us/about-us.module').then((m): AboutUsModule => m.AboutUsModule),
  },
  {
    path: 'acount',
    loadChildren: () : Promise<AcountModule>  => import('./+acount/acount.module').then((m) : AcountModule => m.AcountModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'admin',
    loadChildren: (): Promise<AdminModule> => import('./+admin/admin.module').then((m): AdminModule => m.AdminModule),
    canActivate: [RoleGuard],
    data: {roles: [applicationRoleConstant.AdminRole]}
  },
  {
    path: 'auth',
    loadChildren: (): Promise<AuthModule> => import('./+auth/auth.module').then((m): AuthModule => m.AuthModule)
  },
  {
    path: 'albums',
    loadChildren: (): Promise<AlbumsModule> => import('./+albums/albums.module').then((m): AlbumsModule => m.AlbumsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'artists',
    loadChildren: (): Promise<ArtistsModule> => import('./+artists/artists.module').then((m): ArtistsModule => m.ArtistsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'browse',
    loadChildren: (): Promise<BrowseModule> => import('./+browse/browse.module').then((m): BrowseModule => m.BrowseModule)
  },
  {
    path: 'search',
    loadChildren: (): Promise<SearchModule> => import('./+search/search.module').then((m): SearchModule => m.SearchModule)
  },
  {
    path: 'favorite-list',
    loadChildren: (): Promise<FavoriteListModule> => import('./+favorite-list/favorite-list.module').then((m): FavoriteListModule => m.FavoriteListModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'playlists',
    loadChildren: (): Promise<PlaylistsModule> => import('./+playlists/playlists.module').then((m): PlaylistsModule => m.PlaylistsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'recently-added',
    loadChildren: (): Promise<RecentlyAddedModule> => import('./+recently-added/recently-added.module').then((m): RecentlyAddedModule => m.RecentlyAddedModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'singer',
    loadChildren: (): Promise<SingerModule> => import('./+singer/singer.module').then((m): SingerModule => m.SingerModule),
    canActivate: [RoleGuard],
    data: {roles: [applicationRoleConstant.SingerRole]}
  },
  {
    path: 'songs',
    loadChildren: (): Promise<SongsModule> => import('./+songs/songs.module').then((m): SongsModule => m.SongsModule),
    canActivate: [AuthGuard]
  },
  {
    path: '403',
    component: ForbiddenComponent
  },
  {
    path: '404',
    component: NotFoundComponent
  },
  {
    path: '**',
    redirectTo: '404'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
