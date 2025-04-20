import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { UserInterface } from 'src/app/core/interfaces';
import { NotificationService } from 'src/app/core/services';
import { UserApiService } from 'src/app/core/services/api/user-api.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'mus-artists',
  templateUrl: './artists.component.html',
  styleUrls: ['./artists.component.scss']
})
export class ArtistsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public singers: UserInterface[] = [];

  constructor(
    private readonly notificationService: NotificationService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly userApiService: UserApiService,
    private clipboard: Clipboard
  ) { }

  ngOnInit(): void {
    this.getSingers();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }


  private getSingers(): void {
      this.userApiService.getUserSingers()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((singers): void => {
        this.singers = singers;
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public shareSinger(singer: UserInterface){
    let url: string = `${document.location.origin}/browse/singer/${singer.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }


  public goToSinger(id: string): void{
    this.router.navigate(['browse/singer', id.toString()]);
  }

}
