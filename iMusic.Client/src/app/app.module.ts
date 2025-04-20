import {NgModule} from '@angular/core';

import {AppRoutingModule} from './app-routing.module';

import {AppComponent} from './app.component';
import {CoreModule} from './core/core.module';
import { BrowseModule } from './+browse/browse.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowseRoutingModule } from './+browse/browse-routing.module';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    AppRoutingModule,
    BrowseModule,
    BrowserAnimationsModule,
    BrowseRoutingModule,
    CoreModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
}
