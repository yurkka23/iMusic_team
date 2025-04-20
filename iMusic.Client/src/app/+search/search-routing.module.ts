import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SearchAllComponent } from './components/searchAll/searchAll.component';
import { CategorySongsComponent } from './components/category-songs/category-songs.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: SearchAllComponent
  },
  {
    path: 'category-songs/:id',
    component: CategorySongsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SearchRoutingModule { }
