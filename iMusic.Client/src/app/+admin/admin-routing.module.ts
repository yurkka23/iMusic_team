import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageUsersComponent } from './components/manage-users/manage-users.component';
import { ManageSingersComponent } from './components/manage-singers/manage-singers.component';
import { ManageCategoriesComponent } from './components/manage-categories/manage-categories.component';
import { BecomeSingerComponent } from './components/become-singer/become-singer.component';
import { UserInfoComponent } from './components/user-info/user-info.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'manage-categories',
    pathMatch: 'full',
  },
  {
    path: 'manage-users',
    component: ManageUsersComponent,
  },
  {
    path: 'manage-singers',
    component: ManageSingersComponent,
  },
  {
    path: 'manage-categories',
    component: ManageCategoriesComponent,
  },
  {
    path: 'become-singer',
    component: BecomeSingerComponent,
  },
  {
    path: 'user-info/:id',
    component: UserInfoComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
