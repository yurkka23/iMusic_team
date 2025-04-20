import {Component, OnDestroy, OnInit} from '@angular/core';
import {MatIconRegistry} from '@angular/material/icon';
import {DomSanitizer} from '@angular/platform-browser';
import {ActivatedRoute, Data, NavigationEnd, Router, RouterEvent} from '@angular/router';

import {Observable, Subject} from 'rxjs';

import {filter, map, mergeMap, takeUntil} from 'rxjs/operators';

import {AuthService, SidebarService, StorageService} from './core/services';


@Component({
  selector: 'mus-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();
  public isOpen: boolean = true;

  public isAuthorizationPage: boolean;

  constructor(
    private readonly authService: AuthService,
    private readonly sidebarService: SidebarService) {
  }

  public ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.authService.getCurrentUser();
    }
    this.handleSidebarState();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private handleSidebarState(): void {
    this.sidebarService.isSidebarOpened$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((isOpen): void => {
        this.isOpen = isOpen;
      });
  }

}
