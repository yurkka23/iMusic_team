import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { ManageUsersComponent } from './components/manage-users/manage-users.component';
import { ManageSingersComponent } from './components/manage-singers/manage-singers.component';
import { ManageCategoriesComponent } from './components/manage-categories/manage-categories.component';
import { CoreModule } from '../core/core.module';
import { CategoryComponent } from './components/manage-categories/category/category.component';
import { AddCategoryComponent } from './components/manage-categories/add-category/add-category.component';
import { SharedModule } from '../shared/shared.module';
import { BecomeSingerComponent } from './components/become-singer/become-singer.component';
import { UserInfoComponent } from './components/user-info/user-info.component';


@NgModule({
  declarations: [
    ManageUsersComponent,
    ManageSingersComponent,
    ManageCategoriesComponent,
    CategoryComponent,
    AddCategoryComponent,
    BecomeSingerComponent,
    UserInfoComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    CoreModule
  ]
})
export class AdminModule { }
