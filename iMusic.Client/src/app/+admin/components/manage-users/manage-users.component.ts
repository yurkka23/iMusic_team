import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserInterface } from 'src/app/core/interfaces';
import { NotificationService } from 'src/app/core/services';
import { AdminApiService } from 'src/app/core/services/api/admin-api.service';

@Component({
  selector: 'mus-manage-users',
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.scss']
})
export class ManageUsersComponent implements OnInit {

  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public  users: UserInterface[] = [];
  public bannedUsers: UserInterface[] = [];

  constructor(
    private readonly adminApiService: AdminApiService,
    private readonly notificationService: NotificationService,
    private readonly router: Router
  ) { }

  ngOnInit(): void {
    this.getUsers();
    this.getbannedUsers();

  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getUsers(): void {
    this.adminApiService.getUsers()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((users): void => {
        this.users = users;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getbannedUsers(): void {
    this.adminApiService.getBannedUsers()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((users): void => {
        this.bannedUsers = users;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public banUser( id: string ): void {
    this.adminApiService.ban(id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.getUsers();
        this.getbannedUsers();
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public unbanUser( id: string ): void {
    this.adminApiService.unban(id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.getUsers();
        this.getbannedUsers();
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public showMore(id: string): void{
    this.router.navigate(['admin/user-info', id.toString()]);
  }
}
