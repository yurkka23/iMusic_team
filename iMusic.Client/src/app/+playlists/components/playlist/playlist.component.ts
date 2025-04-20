import { Component, OnInit } from '@angular/core';
import {Clipboard} from '@angular/cdk/clipboard';
import { Subject } from 'rxjs';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { StatusEnum } from 'src/app/core/enums';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { NotificationService } from 'src/app/core/services';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';

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

  constructor(
    private readonly router : Router,
    private readonly route: ActivatedRoute,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly audioPlayerService: AudioPlayerService,
    private readonly playlistApiService: PlaylistApiService,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService
    ) 
  { 
    this.route.paramMap.subscribe((params) => {
      if (this.playlistId) {
        this.ngOnDestroy();
        this.ngOnInit();
      }
    });
  }

  ngOnInit(): void {
    this.totalLength = 0;
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

  private getPlaylistId(): void {
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

  public addToFavotitePlaylists(playlist: PlaylistInterface): void{
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

  public deletePlaylist(playlist: PlaylistInterface): void{
    this.playlistApiService.deletePlaylist(playlist.id)
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((val): void => {
      if(val){
        this.playlistApiService.needToupdate$.next(true);
        this.router.navigate(['playlists/all']);
        this.notificationService.showNotification("Thanks playlist was deleted.");
      }else{
        this.notificationService.showNotification("You can't delete this playlist.", 'snack-bar-error');
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

  public removeSongFromPlaylist(song: SongInterface): void{
    this.playlistApiService.removeSongFromPlaylist(song.id, this.playlistId)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification("You removed song");
        this.getPlaylist();
      }else{
        this.notificationService.showNotification("You can't removed.", 'snack-bar-error');
      }
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);

      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

}
