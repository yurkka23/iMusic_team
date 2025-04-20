import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserInterface } from 'src/app/core/interfaces';
import { NotificationService } from 'src/app/core/services';
import { AdminApiService } from 'src/app/core/services/api/admin-api.service';

@Component({
  selector: 'mus-manage-singers',
  templateUrl: './manage-singers.component.html',
  styleUrls: ['./manage-singers.component.scss']
})
export class ManageSingersComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public  users: UserInterface[];

  constructor(
    private readonly adminApiService: AdminApiService,
    private readonly notificationService: NotificationService,
    private readonly router: Router
  ) { }

  ngOnInit(): void {
    this.getUsers();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getUsers(): void {
    this.adminApiService.getSingers()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((users): void => {
        this.users = users;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public removeSingerRole( id: string ): void {
    this.adminApiService.removeSingerRole(id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.getUsers();
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public rejectSinger( id: string ): void {//cahnge
    this.adminApiService.rejectSinger(id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.getUsers();
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public showMore(id: string): void{
    this.router.navigate(['admin/user-info', id.toString()]);
  }
}
