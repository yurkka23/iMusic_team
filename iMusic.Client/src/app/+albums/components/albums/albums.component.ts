import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { UserInterface } from 'src/app/core/interfaces';
import { NotificationService } from 'src/app/core/services';
import { UserApiService } from 'src/app/core/services/api/user-api.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';

@Component({
  selector: 'mus-albums',
  templateUrl: './albums.component.html',
  styleUrls: ['./albums.component.scss']
})
export class AlbumsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public albums: AlbumInterface[] = [];
  public playingSong: SongInterface ;
  
  constructor(
    private readonly notificationService: NotificationService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly audioPlayer: AudioPlayerService,
    private readonly albumApiService: AlbumApiService,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService

  ) { }

  ngOnInit(): void {
    this.getPlayingSong();
    this.getAlbums();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
 
  private getAlbums(): void{
    this.albumApiService.getUserAlbums()
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((albums): void => {
      this.albums = albums;
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

  public removeAlbum(album: AlbumInterface){
    this.albumApiService.removeAlbumFromUser(album.id)
    .subscribe((res): void => {
      if(res){
        this.albums = this.albums.filter(x => x.id != album.id) 
      }else{
        this.notificationService.showNotification("You can't delete", 'snack-bar-error');
      }
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

  public shareAlbum(album: AlbumInterface){
    let url: string = `${document.location.origin}/browse/album/${album.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

  public playAlbum(album: AlbumInterface): void{
    this.audioPlayer.setSongs(album?.songs);
    this.audioPlayer.setCurrentSong(album?.songs[0]);
  }

  public goToAlbum(id: string): void{
    this.router.navigate(['browse/album', id.toString()]);
  }

  public goToSinger(id: string): void{
    this.router.navigate(['browse/singer', id.toString()]);
  }

  private getPlayingSong(): void {
    this.audioPlayer.currentSong$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((song): void => {
      this.playingSong = song;
    });
  } 

  public addToFavoriteAlbum(album: AlbumInterface): void{
    this.favoriteApiService.addAlbum(album.id)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification("You liked song");
      }else{
        this.notificationService.showNotification("You can't like. You've already liked.", 'snack-bar-error');
      }
    },
    
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }
}
