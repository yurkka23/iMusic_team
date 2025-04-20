import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { UserInterface } from 'src/app/core/interfaces';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';

@Component({
  selector: 'mus-site-recommends',
  templateUrl: './site-recommends.component.html',
  styleUrls: ['./site-recommends.component.scss']
})
export class SiteRecommendsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public albums: AlbumInterface[] = [];
  public playingSong: SongInterface;
  public user: UserInterface;

  constructor(
    private readonly albumApiService: AlbumApiService,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly audioPlayer: AudioPlayerService,
    private readonly router: Router,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService,
    private readonly authService: AuthService,


  ) {}

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.getCurrentUser();
    }
    this.getPlayingSong();
    this.getTopAlbums();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getTopAlbums(): void {
    this.albumApiService.getTopAlbums()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((albums): void => {
        this.albums = albums;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  private getPlayingSong(): void {
    this.audioPlayer.currentSong$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((song): void => {
      this.playingSong = song;
    });
  }

  public playSongs(album: AlbumInterface): void{
    this.audioPlayer.setSongs(album?.songs);
    this.audioPlayer.setCurrentSong(album?.songs[0]);
  } 
  public shareAlbum(album: AlbumInterface){
    let url: string = `${document.location.origin}/browse/album/${album.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }


  public goToAlbum(id: string): void{
    this.router.navigate(['browse/album', id.toString()]);
  }
  
  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.user = currentUser;
      });
  }

  
  public addToFavotiteAlbum(album: AlbumInterface): void{
    this.favoriteApiService.addAlbum(album.id)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification("You liked album");
      }else{
        this.notificationService.showNotification("You can't like. You've already liked.", 'snack-bar-error');
      }
    },
    
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

 
  public addToAlbums(album: AlbumInterface): void{
    this.albumApiService.addAlbumToUser(album.id)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification("You added album");
      }else{
        this.notificationService.showNotification("You can't add. You've already added.", 'snack-bar-error');
      }
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

}
