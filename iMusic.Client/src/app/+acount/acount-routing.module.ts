import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AcountComponent } from './components/acount/acount.component';

const routes: Routes = [
  {
    path: '',
   // pathMatch: 'full',
    component: AcountComponent
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AcountRoutingModule { }
