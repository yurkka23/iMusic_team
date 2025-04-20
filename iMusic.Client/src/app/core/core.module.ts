// modules
import {CommonModule} from '@angular/common';
import {HTTP_INTERCEPTORS, HttpClient, HttpClientModule} from '@angular/common/http';
import {NgModule} from '@angular/core';

import {NotifierModule} from 'angular-notifier';
import {NgxMaskModule} from 'ngx-mask';

import {SharedModule} from '../shared/shared.module';

// components
import {ForbiddenComponent} from './components/forbidden/forbidden.component';
import {HeaderComponent} from './components/header/header.component';
import {NotFoundComponent} from './components/not-found/not-found.component';
import {SpinnerComponent} from './components/spinner/spinner.component';
import {SpinnerInterceptor} from './helpers/spinner.interceptor';
import {SnackbarComponent} from './components/snackbar/snackbar.component';
import {SidebarComponent} from './components/sidebar/sidebar.component';
import {FooterComponent} from './components/footer/footer.component';


import {AuthorizationInterceptor} from './helpers/authorization-interceptor';
import { MainAudioPlayerComponent } from './components/main-audio-player/main-audio-player.component';
import { ImageCropperModule } from 'ngx-image-cropper';
import { BufferSidebarComponent } from './components/buffer-sidebar/buffer-sidebar.component';

@NgModule({
  declarations: [
    ForbiddenComponent,
    NotFoundComponent,
    HeaderComponent,
    FooterComponent,
    SpinnerComponent,
    SnackbarComponent,
    SidebarComponent,
    MainAudioPlayerComponent,
    BufferSidebarComponent,
    BufferSidebarComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    HttpClientModule,
    ImageCropperModule,
    NotifierModule.withConfig({
      position: {
        horizontal: {
          position: 'right',
          distance: 12
        },
        vertical: {
          position: 'top',
          distance: 12,
          gap: 10
        },
      }
    }),
    NgxMaskModule.forRoot(),
  ],
  exports: [
    CommonModule,
    ForbiddenComponent,
    NotFoundComponent,
    HeaderComponent,
    FooterComponent,
    SpinnerComponent,
    NotifierModule,
    SidebarComponent,
    MainAudioPlayerComponent,
    SharedModule,
    ImageCropperModule,
    BufferSidebarComponent
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: SpinnerInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthorizationInterceptor,
      multi: true
    }
  ]
})
export class CoreModule {
}
