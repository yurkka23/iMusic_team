import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserInterface } from 'src/app/core/interfaces';
import { NotificationService } from 'src/app/core/services';
import { AdminApiService } from 'src/app/core/services/api/admin-api.service';

@Component({
  selector: 'mus-become-singer',
  templateUrl: './become-singer.component.html',
  styleUrls: ['./become-singer.component.scss']
})
export class BecomeSingerComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public  usersRequests: UserInterface[];

  constructor(
    private readonly adminApiService: AdminApiService,
    private readonly notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    this.getUserRequests();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getUserRequests(): void {
    this.adminApiService.getBecomeusersRequests()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((requests): void => {
        this.usersRequests = requests;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public approveSinger( id: string ): void {
    this.adminApiService.approveSinger(id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.getUserRequests();
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public rejectSinger( id: string ): void {
    this.adminApiService.rejectSinger(id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.getUserRequests();
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

}
