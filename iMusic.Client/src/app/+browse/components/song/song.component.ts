import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { UserInterface } from 'src/app/core/interfaces';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';

@Component({
  selector: 'mus-song',
  templateUrl: './song.component.html',
  styleUrls: ['./song.component.scss']
})
export class SongComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public songId: string;
  public song: SongInterface;
  public playingSong: SongInterface;
  public playlists: PlaylistInterface[] = [];
  public user: UserInterface;

  constructor(
    private readonly router : Router,
    private readonly route: ActivatedRoute,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly audioPlayerService: AudioPlayerService,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService,
    private readonly playlistApiService: PlaylistApiService,
    private readonly authService: AuthService,
  ) { }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.getCurrentUser();
      this.getPlaylists();
    }
    this.getSongId();
    this.getSong();
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

  getSongId(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id !== null) {
      this.songId = id;
    }
  }

  public getSong(): void {
    this.songApiService.getSongById(this.songId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((song): void => {
        this.song = song;
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
    if(!id){
      return;
    }
    this.router.navigate(['browse/album/', id.toString()]);
  }

  public playSong(song: SongInterface): void {
    this.audioPlayerService.setSongs([song]);
    this.audioPlayerService.setCurrentSong(song);
  }

  public shareSong(song: SongInterface){
    let url: string = `${document.location.origin}/browse/song/${song.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
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
