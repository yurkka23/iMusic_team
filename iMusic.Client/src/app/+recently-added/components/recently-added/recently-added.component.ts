import { Component, OnInit } from '@angular/core';
import {Clipboard} from '@angular/cdk/clipboard';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { NotificationService } from 'src/app/core/services';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';

@Component({
  selector: 'mus-recently-added',
  templateUrl: './recently-added.component.html',
  styleUrls: ['./recently-added.component.scss']
})
export class RecentlyAddedComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public albums: AlbumInterface[] = [];
  public songs: SongInterface[] = [];
  public playingSong: SongInterface ;

  constructor(
    private readonly albumApiService: AlbumApiService,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly router: Router,
    private readonly audioPlayer: AudioPlayerService,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService
  ) {}

  ngOnInit(): void {
    this.getPlayingSong();
    this.getSongs();
    this.getAlbums();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public addToFavoriteSong(song: SongInterface): void{
    this.favoriteApiService.addSong(song.id)
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

  
  public removeSong(song: SongInterface){
    this.songApiService.removeSongFromUser(song.id)
    .subscribe((res): void => {
      if(res){
        this.songs = this.songs.filter(x => x.id != song.id) 
      }else{
        this.notificationService.showNotification("You can't delete", 'snack-bar-error');
      }
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

  private getPlayingSong(): void {
    this.audioPlayer.currentSong$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((song): void => {
      this.playingSong = song;
    });
  }

  public playSong(song: SongInterface): void{
    this.audioPlayer.setSongs(this.songs);
    this.audioPlayer.setCurrentSong(song);
  } 


  public getSongs(): void {
    this.songApiService.getUserRecentlySongs()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.songs = songs;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getAlbums(): void {
    this.albumApiService.getUserRecentlyAlbums()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((albums): void => {
        this.albums = albums;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public shareAlbum(album: AlbumInterface){
    let url: string = `${document.location.origin}/browse/album/${album.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  public shareSong(song: SongInterface){
    let url: string = `${document.location.origin}/browse/song/${song.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  public playAlbums(album: AlbumInterface): void{
    this.audioPlayer.setSongs(album?.songs);
    this.audioPlayer.setCurrentSong(album?.songs[0]);
  } 

  public goToAlbum(id: string): void{
    if(!id){
      return;
    }
    this.router.navigate(['browse/album', id.toString()]);
  }

  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

}
