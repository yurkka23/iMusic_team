import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';

@Component({
  selector: 'mus-songs',
  templateUrl: './songs.component.html',
  styleUrls: ['./songs.component.scss']
})
export class SongsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public statusEnum = StatusEnum;
  public songs: SongInterface[] = [];
  public readonly tableColumns = ['name', 'singer', 'album' ,'time', 'actions'];
  public playingSong: SongInterface;

  constructor(
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly audioPlayer: AudioPlayerService,
    private readonly favoriteApiService: FavoritelistApiService
  ) { 
    
  }

  ngOnInit(): void {
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

  public playSong(song: SongInterface): void{
    this.audioPlayer.setSongs(this.songs);
    this.audioPlayer.setCurrentSong(song);
  } 
  
  public getSongs(): void {
    this.songApiService.getUserSongs()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.songs = songs;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

 
  public addToFavorite(song: SongInterface): void{
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


}
