import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';
import { UserInterface } from 'src/app/core/interfaces';

@Component({
  selector: 'mus-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.scss']
})
export class PlaylistComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public playlistId: string;
  public songs: SongInterface[] = [];
  public statusEnum = StatusEnum;
  public totalLength: number = 0;
  public playingSong: SongInterface;
  public playlist: PlaylistInterface;
  public user: UserInterface;
  public playlists: PlaylistInterface[] = [];

  constructor(
    private readonly router : Router,
    private readonly route: ActivatedRoute,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly audioPlayerService: AudioPlayerService,
    private readonly playlistApiService: PlaylistApiService,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService,
    private readonly authService: AuthService,

  ) { }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.getCurrentUser();
      this.getPlaylists();
    }
    this.getPlaylistId();
    this.getPlaylist();
    this.getPlayingSong();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private getPlayingSong(): void {
    this.audioPlayerService.currentSong$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((song): void => {
      this.playingSong = song;
    });
  }

  getPlaylistId(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id !== null) {
      this.playlistId = id;
    }
  }

  public sharePlaylist(playlist: PlaylistInterface){
    let url: string = `${document.location.origin}/browse/playlist/${playlist.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  public shareSong(song: SongInterface){
    let url: string = `${document.location.origin}/browse/song/${song.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  public getPlaylist(): void {
    this.playlistApiService.getPlaylist(this.playlistId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((playlist): void => {
        this.playlist = playlist;
        this.songs = playlist?.songs;
        this.songs.forEach(x => this.totalLength += x.duration);
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

  public playPlaylist(): void {
    if(this.songs.length < 1){
      return;
    }
    this.audioPlayerService.setSongs(this.songs);
    this.audioPlayerService.setCurrentSong(this.songs[0]);
  }

  public playSong(song: SongInterface): void {
    this.audioPlayerService.setSongs(this.songs);
    this.audioPlayerService.setCurrentSong(song);
  }

  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.user = currentUser;
      });
  }

  public getPlaylists(): void {
    this.playlistApiService.getUserPlaylists()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((playlists): void => {
        this.playlists = playlists;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }


  public addToFavotitePlaylist(playlist: PlaylistInterface): void{
    this.favoriteApiService.addPlaylist(playlist.id)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification("You liked playlist");
      }else{
        this.notificationService.showNotification("You can't like. You've already liked.", 'snack-bar-error');
      }
    },
    
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

 
  
  public addToFavotiteSong(song: SongInterface): void{
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

  public addToSongs(song: SongInterface): void{
    this.songApiService.addSongToUser(song.id)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification("You added song");
      }else{
        this.notificationService.showNotification("You can't add. You've already added.", 'snack-bar-error');
      }
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

  public addToPlaylist(song: SongInterface, playlist: PlaylistInterface): void{
    this.playlistApiService.addSongToPlaylist(song.id, playlist.id)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification(`You added song to playlist ${playlist.title}`);
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
