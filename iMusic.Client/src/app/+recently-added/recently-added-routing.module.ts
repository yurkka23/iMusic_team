import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RecentlyAddedComponent } from './components/recently-added/recently-added.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: RecentlyAddedComponent
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RecentlyAddedRoutingModule { }
