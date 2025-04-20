import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { UserInterface } from 'src/app/core/interfaces';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';

@Component({
  selector: 'mus-top-playlists',
  templateUrl: './top-playlists.component.html',
  styleUrls: ['./top-playlists.component.scss']
})
export class TopPlaylistsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public user: UserInterface;
  public statusEnum = StatusEnum;
  public playingSong: SongInterface ;
  public topPlaylists: PlaylistInterface[] = [];

  constructor(
    private readonly notificationService: NotificationService,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly audioPlayer: AudioPlayerService,
    private readonly playlistApiService: PlaylistApiService,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService,

  ) {}

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.getCurrentUser();
    }
    this.getPlayingSong();
    this.getTopPlaylists();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getTopPlaylists(): void {
    this.playlistApiService.getTopPlaylists()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((playlists): void => {
        this.topPlaylists = playlists;
        this.topPlaylists.length =   this.topPlaylists.length > 6 ?  6 :  this.topPlaylists.length;
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

  public sharePlaylist(playlist: PlaylistInterface){
    let url: string = `${document.location.origin}/browse/playlist/${playlist.id}`;
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

  public playPlaylist(playlist: PlaylistInterface): void{
    this.audioPlayer.setSongs(playlist?.songs);
    this.audioPlayer.setCurrentSong(playlist?.songs[0]);
  } 

  public goToPlaylist(id: string): void{
    this.router.navigate(['browse/playlist', id.toString()]);
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
}
