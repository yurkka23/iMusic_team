import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder } from 'ngx-strongly-typed-forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserInterface } from 'src/app/core/interfaces';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { CategoryInterface } from 'src/app/core/interfaces/category/category.interface';
import { SearchInterface } from 'src/app/core/interfaces/general/search.interface';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { NotificationService, AuthService } from 'src/app/core/services';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { UserApiService } from 'src/app/core/services/api/user-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';
import { FavoriteListInterface } from 'src/app/core/interfaces/favoritelist/FavoriteList.interface';

@Component({
  selector: 'mus-favorite-list',
  templateUrl: './favorite-list.component.html',
  styleUrls: ['./favorite-list.component.scss']
})
export class FavoriteListComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public favoritelist: FavoriteListInterface;
  public playingSong: SongInterface ;
  
  constructor(
    private readonly notificationService: NotificationService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly audioPlayer: AudioPlayerService,
    private readonly favoritelistApiService: FavoritelistApiService,
    private clipboard: Clipboard
  ) { }

  ngOnInit(): void {
    this.getPlayingSong();
    this.getFavoritelist();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
 
  private getFavoritelist(): void{
    this.favoritelistApiService.getFavoritelist()
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((list): void => {
  
      this.favoritelist = list;
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

  public removePlaylist(playlist: PlaylistInterface){
    this.favoritelistApiService.removePlaylist(playlist.id)
    .subscribe((res): void => {
      if(res){
        this.favoritelist.playlists = this.favoritelist.playlists.filter(x => x.id != playlist.id) 
      }else{
        this.notificationService.showNotification("You can't delete", 'snack-bar-error');
      }
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

  public removeSong(song: SongInterface){
    this.favoritelistApiService.removeSong(song.id)
    .subscribe((res): void => {
      if(res){
        this.favoritelist.songs = this.favoritelist.songs.filter(x => x.id != song.id) 
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
    this.favoritelistApiService.removeAlbum(album.id)
    .subscribe((res): void => {
      if(res){
        this.favoritelist.albums = this.favoritelist.albums.filter(x => x.id != album.id) 
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

  public shareSong(song: SongInterface){
    let url: string = `${document.location.origin}/browse/song/${song.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  public sharePlaylist(playlist: PlaylistInterface){
    let url: string = `${document.location.origin}/browse/playlist/${playlist.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

 
  public playSong(song: SongInterface): void{
    this.audioPlayer.setSongs(this.favoritelist.songs);
    this.audioPlayer.setCurrentSong(song);
  }
  
  public playAlbum(album: AlbumInterface): void{
    this.audioPlayer.setSongs(album?.songs);
    this.audioPlayer.setCurrentSong(album?.songs[0]);
  }

  public playPlaylist(playlist: PlaylistInterface): void{
    this.audioPlayer.setSongs(playlist?.songs);
    this.audioPlayer.setCurrentSong(playlist?.songs[0]);
  }

  public goToAlbum(id: string): void{
    this.router.navigate(['browse/album', id.toString()]);
  }

  public goToPlaylist(id: string): void{
    this.router.navigate(['browse/playlist', id.toString()]);
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

}
