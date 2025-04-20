import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { UserInterface } from 'src/app/core/interfaces';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { FormAlbumComponent } from './form-album/form-album.component';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'mus-your-albums',
  templateUrl: './your-albums.component.html',
  styleUrls: ['./your-albums.component.scss']
})
export class YourAlbumsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public user: UserInterface;
  public statusEnum = StatusEnum;
  public albums: AlbumInterface[] = [];

  constructor(
    private readonly dialog: MatDialog,
    private readonly albumApiService: AlbumApiService,
    private readonly notificationService: NotificationService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) { }

  ngOnInit(): void {
    this.getCurrentUser();
    this.getAlbums();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.user = currentUser;
      });
  }

  public getAlbums(): void {//change
    this.albumApiService.getAlbumsBySinger(this.user.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((albums): void => {
        this.albums = albums;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public addAlbums(): void {
    const dialogRef = this.dialog.open(FormAlbumComponent, {
      width: '350px',
      data: {
        isAlbumUpdate: false
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.getAlbums();
        }
      });
  }

  public editAlbum(album: AlbumInterface): void {
    let dataToDialog ={
      id: album.id,
      title: album.title,
      status: album.status,
      categoryId: album.category.id,
      albumImgUrl: album.albumImgUrl
    }
    const dialogRef = this.dialog.open(FormAlbumComponent, {
      width: '350px',
      data: {
        album: dataToDialog,
        isAlbumUpdate: true
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.getAlbums();
        }
      });
  }

  public editAlbumSongs(id : string) : void {
    this.router.navigate(['singer/album', id.toString()]);
  }

  public deleteAlbum(id: string): void {
    this.albumApiService.deleteAlbum(id)
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((val): void => {
      if(val){
        this.getAlbums();
        this.notificationService.showNotification("Thanks album was deleted.");
      }else{
        this.notificationService.showNotification("You can't delete this album.", 'snack-bar-error');
      }
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);
      
      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
        
  }


}
