import {enableProdMode} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

import {AppModule} from './app/app.module';
import {environment} from './environments/environment';

if (environment.production) {
  enableProdMode();

  if (!location.protocol.includes('https')) {
    location.protocol = 'https:';
  }
}

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch((err): void => console.error(err));
