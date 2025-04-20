import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { NotificationService } from 'src/app/core/services';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { FormSongComponent } from '../../your-songs/form-song/form-song.component';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';

@Component({
  selector: 'mus-edit-album-songs',
  templateUrl: './edit-album-songs.component.html',
  styleUrls: ['./edit-album-songs.component.scss']
})
export class EditAlbumSongsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public albumId: string;
  public album: AlbumInterface;
  public statusEnum = StatusEnum;
  public totalLength: number = 0;
  public playingSong: SongInterface;

  constructor(
    private readonly router : Router,
    private readonly route: ActivatedRoute,
    private readonly dialog: MatDialog,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly albumApiService: AlbumApiService,
    private readonly audioPlayerService: AudioPlayerService
  ) { }

  ngOnInit(): void {
    this.getAlbumId();
    this.getAlbum();
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

  getAlbumId(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id !== null) {
      this.albumId = id;
    }
  }

  public getAlbum(): void {
    this.albumApiService.getAlbumById(this.albumId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((album): void => {
        this.album = album;
       this.album.songs.forEach( e => this.totalLength += e.duration);
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public addSong(): void {
    const dialogRef = this.dialog.open(FormSongComponent, {
      width: '500px',
      data: {
        song: {
          albumId: this.albumId
        },
        isSongUpdate: false
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.getAlbum();
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
          this.getAlbum();
        }
      });
  }

  public deleteSong(id: string): void {
    this.songApiService.deleteSong(id)
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((val): void => {
      if(val){
        this.getAlbum();
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

  public playAlbum(): void {
    this.audioPlayerService.setSongs(this.album.songs);
    this.audioPlayerService.setCurrentSong(this.album?.songs[0]);
  }

  public playSong(song: SongInterface): void {
    this.audioPlayerService.setSongs(this.album.songs);
    this.audioPlayerService.setCurrentSong(song);
  }
 
}
