import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AcountRoutingModule } from './acount-routing.module';
import { AcountComponent } from './components/acount/acount.component';
import { ImageCropperComponent } from './components/image-cropper/image-cropper.component';
import { CoreModule } from '../core/core.module';


@NgModule({
  declarations: [
    AcountComponent,
    ImageCropperComponent
  ],
  imports: [
    CommonModule,
    AcountRoutingModule,
    CoreModule
  ]
})
export class AcountModule { }
