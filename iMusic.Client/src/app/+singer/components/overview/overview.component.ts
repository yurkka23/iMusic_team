import { Component, OnInit } from '@angular/core';
import {Clipboard} from '@angular/cdk/clipboard';
import { Subject } from 'rxjs';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { UserInterface } from 'src/app/core/interfaces';
import { ActivatedRoute, Router } from '@angular/router';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AuthService, NotificationService, StorageService } from 'src/app/core/services';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { UserApiService } from 'src/app/core/services/api/user-api.service';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'mus-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public topSongs: SongInterface[] = [];
  public albums: AlbumInterface[] = [];

  public playingSong: SongInterface;
  public singer: UserInterface;

  constructor(
    private readonly router : Router,
    private readonly route: ActivatedRoute,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly audioPlayerService: AudioPlayerService,
    private readonly albumApiService: AlbumApiService,
    private readonly userApiService: UserApiService,
    private clipboard: Clipboard,
    private readonly storageService: StorageService,
    private readonly authService: AuthService
  ) { }

  ngOnInit(): void {
    this.getSinger();
    this.getTopSongs();
    this.getAlbums();
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

  private getSinger(): void {
    this.authService.user$
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe((user): void => {
          this.singer = user;
        }, (error: HttpErrorResponse): void => {
          const errorMessage = this.notificationService.getErrorMessage(error);
          this.notificationService.showNotification(errorMessage, 'snack-bar-error');
        });
  }

 

  public getTopSongs(): void {
    this.songApiService.getTopSingerSongs(this.singer.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.topSongs = songs;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getAlbums(): void {
    this.albumApiService.getAlbumsBySinger(this.singer.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((albums): void => {
        this.albums = albums;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

  public playSongs(): void {
    if(this.topSongs.length < 1){
      return;
    }
    this.audioPlayerService.setSongs(this.topSongs);
    this.audioPlayerService.setCurrentSong(this.topSongs[0]);
  }
  
  public playAlbum(album: AlbumInterface): void{
    this.audioPlayerService.setSongs(album?.songs);
    this.audioPlayerService.setCurrentSong(album?.songs[0]);
  } 

  public playTopSong(song: SongInterface): void {
    this.audioPlayerService.setSongs(this.topSongs);
    this.audioPlayerService.setCurrentSong(song);
  }

  public goToAlbum(id: string): void{
    this.router.navigate(['browse/album', id.toString()]);
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

  public shareSinger(singer: UserInterface){
    let url: string = `${document.location.origin}/browse/singer/${singer.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }


}
