import {Component, OnDestroy, OnInit} from '@angular/core';

import {Subject} from 'rxjs';
import {takeUntil} from 'rxjs/operators';

import {applicationRoleConstant} from '../../constants';

import {UserInterface} from '../../interfaces';

import {AuthService, SidebarService, StorageService} from '../../services';


@Component({
  selector: 'mus-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss', './header-responsive.component.scss']
})
export class HeaderComponent implements OnDestroy, OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public readonly applicationRole = applicationRoleConstant;

  public currentUser: UserInterface;
  public currentLanguageName: string;

  constructor(private readonly authService: AuthService,
    private readonly sidebarService: SidebarService,
    private readonly storageService: StorageService) {
  }

  public ngOnInit(): void {
    this.getCurrentUser();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }


  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.currentUser = currentUser;
      });
  }

  public logout(): void {
    this.authService.logOut();

    if (this.sidebarService.isSidebarOpened$.value) {
      this.sidebarService.isSidebarOpened$.next(false);
    }
  }

  public openSidebar(): void {
    this.sidebarService.isSidebarOpened$.next(true);
  }

}
