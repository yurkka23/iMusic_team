import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FavoriteListComponent } from './components/favorite-list/favorite-list.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: FavoriteListComponent
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FavoriteListRoutingModule { }
