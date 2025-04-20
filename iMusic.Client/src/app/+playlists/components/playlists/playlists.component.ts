import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';
import { FormPlaylistComponent } from '../form-playlist/form-playlist.component';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';

@Component({
  selector: 'mus-playlists',
  templateUrl: './playlists.component.html',
  styleUrls: ['./playlists.component.scss']
})
export class PlaylistsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public statusEnum = StatusEnum;
  public playlists: PlaylistInterface[] = [];
  public playingSong: SongInterface;


  constructor(
    private readonly dialog: MatDialog,
    private readonly playlistApiService: PlaylistApiService,
    private readonly notificationService: NotificationService,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly audioPlayer: AudioPlayerService,

  ) { }

  ngOnInit(): void {
    this.getPlaylists();
    this.getPlayingSong();

  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private getPlayingSong(): void {
    this.audioPlayer.currentSong$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((song): void => {
      this.playingSong = song;
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

  public addPlaylist(): void {
    const dialogRef = this.dialog.open(FormPlaylistComponent, {
      width: '350px',
      data: {
        isPlayListUpdate: false
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.getPlaylists();
          this.playlistApiService.needToupdate$.next(true);

        }
      });
  }

  public editPlaylist(playlist: PlaylistInterface): void {
    let dataToDialog ={
      id: playlist.id,
      title: playlist.title,
      status: playlist.status,
      playlistImgUrl: playlist.playlistImgUrl
    }
    const dialogRef = this.dialog.open(FormPlaylistComponent, {
      width: '350px',
      data: {
        playList: dataToDialog,
        isPlayListUpdate: true
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.getPlaylists();
          this.playlistApiService.needToupdate$.next(true);

        }
      });
  }

  public addPlaylistSongs() : void {
    this.router.navigate(['search']);
  }

  public deletePlaylist(id: string): void {
    this.playlistApiService.deletePlaylist(id)
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((val): void => {
      if(val){
        this.playlistApiService.needToupdate$.next(true);
        this.getPlaylists();
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

  public playPlaylist(playlist: PlaylistInterface): void{
    this.audioPlayer.setSongs(playlist?.songs);
    this.audioPlayer.setCurrentSong(playlist?.songs[0]);
  } 


}
