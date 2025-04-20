import {Component, OnDestroy, OnInit} from '@angular/core';
import {NavigationStart, Router} from '@angular/router';

import {filter, takeUntil} from 'rxjs/operators';
import {Subject} from 'rxjs';

import {AuthService, NotificationService, SidebarService} from '../../services';

import {UserInterface} from '../../interfaces';

import {applicationRoleConstant} from '../../constants';
import { PlaylistInterface } from '../../interfaces/playlist/playlist.interface';
import { PlaylistApiService } from '../../services/api/playlist-api.service';
import { HttpErrorResponse } from '@angular/common/http';


@Component({
  selector: 'mus-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit, OnDestroy {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public readonly applicationRole = applicationRoleConstant;
  public currentUser: UserInterface;
  public isOpen: boolean = true;
  public userPlaylists : PlaylistInterface[] = [];

  constructor(private readonly router: Router,
    private readonly sidebarService: SidebarService,
    private readonly authService: AuthService,
    private readonly playlistApiService: PlaylistApiService,
    private readonly notificationService : NotificationService) {
      
  }

  public ngOnInit(): void {
   // this.handleSidebarState();
    this.getCurrentUser();
    this.checkToUpdate();
  
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
        if(!!currentUser){
          this.getPlaylists();
        }
      });
  }

  public checkToUpdate(): void {
    this.playlistApiService.needToupdate$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val): void => {
        if(val){
          this.getPlaylists();
        }
      });
  }

  private handleSidebarState(): void {
    this.sidebarService.isSidebarOpened$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((isOpen): void => {
        this.isOpen = isOpen;
      });
  }

  public close(): void {
    this.sidebarService.isSidebarOpened$.next(false);
  }

  public logout(): void {
    this.authService.logOut();

    // if (this.sidebarService.isSidebarOpened$.value) {
    //   this.sidebarService.isSidebarOpened$.next(false);
    // }
  }

  public getPlaylists(): void {
    this.playlistApiService.getUserPlaylists()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((playlists): void => {
        this.userPlaylists = playlists;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }
}
