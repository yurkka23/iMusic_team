import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { UserInterface } from 'src/app/core/interfaces';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { FavoriteListInterface } from 'src/app/core/interfaces/favoritelist/FavoriteList.interface';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { NotificationService } from 'src/app/core/services';
import {Clipboard} from '@angular/cdk/clipboard';
import { AdminApiService } from 'src/app/core/services/api/admin-api.service';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'mus-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public userId: string;
  public user: UserInterface;
  public addedSongs: SongInterface[] = [];
  public addedAlbums: AlbumInterface[] = [];
  public favoritelist: FavoriteListInterface;
  public userPlaylists: PlaylistInterface[] = [];

  constructor(
    private readonly router : Router,
    private readonly route: ActivatedRoute,
    private readonly notificationService: NotificationService,
    private clipboard: Clipboard,
    private readonly adminApiService: AdminApiService,
  ) { }

  ngOnInit(): void {
    this.getUserId();
    this.getUser();
    this.getAddedSongs();
    this.getAddedAlbums();
    this.getFavoritelist();
    this.getPlaylists();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getUserId(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id !== null) {
      this.userId = id;
    }
  }

  public getUser(): void {
    this.adminApiService.getUserInfo(this.userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((user): void => {
        this.user = user;
      });
  }

  public getAddedSongs(): void {
    this.adminApiService.getUserAddedSongs(this.userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.addedSongs = songs;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getAddedAlbums(): void {
    this.adminApiService.getUserAddedAlbums(this.userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((albums): void => {
        this.addedAlbums = albums;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getFavoritelist(): void {
    this.adminApiService.getUserFavoriteList(this.userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((list): void => {
        this.favoritelist = list;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getPlaylists(): void {
    this.adminApiService.getUserPlaylists(this.userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((playlists): void => {
        this.userPlaylists = playlists;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

  public goToSinger(id: string): void{
    this.router.navigate(['browse/singer/', id.toString()]);
  }
  public goToAlbum(id: string): void{
    this.router.navigate(['browse/album/', id.toString()]);
  }

  public goToSong(id: string): void{
    this.router.navigate(['browse/song/', id.toString()]);
  }

  public goToPlaylist(id: string): void{
    this.router.navigate(['browse/playlist/', id.toString()]);
  }


}
