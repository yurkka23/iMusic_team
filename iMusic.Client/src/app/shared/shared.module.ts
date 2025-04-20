// modules
import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {TranslateModule} from '@ngx-translate/core';
import {InfiniteScrollModule} from 'ngx-infinite-scroll';
import {NgxMaskModule} from 'ngx-mask';

import {MaterialModule} from './modules/material.module';

// components
import {
  ControlValidationMessageComponent
} from './components/control-validation-message/control-validation-message.component';


// directives
import {NumberOnlyDirective} from './directives/numbers-only.directive';
import {OneHundredPercentDirective} from './directives/one-hundred-percent.directive';
import {BecomeVisibleOnLoadedDirective} from './directives/become-visible-on-loaded.directive';

// pipes
import {IsInvalidPipe} from './pipes/is-invalid.pipe';
import {CheckRolePipe} from './pipes/check-role.pipe';
import {TrimStringPipe} from './pipes/trim-string.pipe';
import {ToStringPipe} from './pipes/to-string.pipe';
import { SearchComponent } from './components/search/search.component';

@NgModule({
  declarations: [
    ControlValidationMessageComponent,
    NumberOnlyDirective,
    OneHundredPercentDirective,
    BecomeVisibleOnLoadedDirective,
    CheckRolePipe,
    TrimStringPipe,
    ToStringPipe,
    IsInvalidPipe,
    SearchComponent
  ],
  imports: [
    MaterialModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    RouterModule,
    NgxMaskModule,
    InfiniteScrollModule
  ],
  exports: [
    CommonModule,
    RouterModule,
    FormsModule,
    TranslateModule,
    ReactiveFormsModule,
    MaterialModule,
    NgxMaskModule,
    InfiniteScrollModule,
    ControlValidationMessageComponent,
    NumberOnlyDirective,
    OneHundredPercentDirective,
    IsInvalidPipe,
    CheckRolePipe,
    TrimStringPipe,
    ToStringPipe,
    BecomeVisibleOnLoadedDirective,
    SearchComponent
  ],
  providers: []
})
export class SharedModule {
}
