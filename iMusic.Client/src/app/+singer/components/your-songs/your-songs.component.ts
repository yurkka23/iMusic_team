import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { UserInterface } from 'src/app/core/interfaces';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { FormSongComponent } from './form-song/form-song.component';
import { Router } from '@angular/router';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';

@Component({
  selector: 'mus-your-songs',
  templateUrl: './your-songs.component.html',
  styleUrls: ['./your-songs.component.scss']
})
export class YourSongsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public user: UserInterface;
  public statusEnum = StatusEnum;
  public songs: SongInterface[] = [];
  public readonly tableColumns = ['name', 'singer', 'album','status' ,'time', 'actions'];
  public playingSong: SongInterface;

  constructor(
    private readonly dialog: MatDialog,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly audioPlayer: AudioPlayerService
  ) { 
    
  }

  ngOnInit(): void {
    this.getCurrentUser();
    this.getSongs();
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

  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.user = currentUser;
      });
  }

  public playSong(song: SongInterface): void{
    this.audioPlayer.setSongs(this.songs);
    this.audioPlayer.setCurrentSong(song);
  } 
  public getSongs(): void {
    this.songApiService.getSongsBySinger(this.user.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.songs = songs;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public addSong(): void {
    const dialogRef = this.dialog.open(FormSongComponent, {
      width: '500px',
      data: {
        isSongUpdate: false
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.getSongs();
        }
      });
  }

  public editSong(song: SongInterface): void {
    song.songUrl = "../../../../../assets/images/old-music-file.jpg";

    const dialogRef = this.dialog.open(FormSongComponent, {
      width: '500px',
      data: {
        song: song,
        isSongUpdate: true
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.getSongs();
        }
      });
  }

  public deleteSong(id: string): void {
    this.songApiService.deleteSong(id)
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((val): void => {
      if(val){
        this.getSongs();
        this.notificationService.showNotification("Thanks song was deleted.");
      }else{
        this.notificationService.showNotification("You can't delete this song.", 'snack-bar-error');
      }
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);
      
      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
        
  }

}
