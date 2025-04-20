import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { UserInterface } from 'src/app/core/interfaces';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { UserApiService } from 'src/app/core/services/api/user-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';

@Component({
  selector: 'mus-singer',
  templateUrl: './singer.component.html',
  styleUrls: ['./singer.component.scss']
})
export class SingerComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public singerId: string;
  public topSongs: SongInterface[] = [];
  public albums: AlbumInterface[] = [];

  public statusEnum = StatusEnum;
  public playingSong: SongInterface;
  public singer: UserInterface;
  public playlists: PlaylistInterface[] = [];
  public user: UserInterface;

  constructor(
    private readonly router : Router,
    private readonly route: ActivatedRoute,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly audioPlayerService: AudioPlayerService,
    private readonly albumApiService: AlbumApiService,
    private readonly userApiService: UserApiService,
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
    this.getSingerId();
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

  private getSingerId(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id !== null) {
      this.singerId = id;
    }
  }

  private getSinger(): void {
    this.userApiService.getUser(this.singerId)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe((singer): void => {
          this.singer = singer;
        }, (error: HttpErrorResponse): void => {
          const errorMessage = this.notificationService.getErrorMessage(error);
          this.notificationService.showNotification(errorMessage, 'snack-bar-error');
        });
  }

  public getTopSongs(): void {
    this.songApiService.getTopSingerSongs(this.singerId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.topSongs = songs;
        this.topSongs.length =   this.topSongs.length > 12 ?  12 :  this.topSongs.length;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getAlbums(): void {
    this.albumApiService.getAlbumsBySinger(this.singerId)
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

  public addToFavotiteAlbum(album: AlbumInterface): void{
    this.favoriteApiService.addAlbum(album.id)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification("You liked album");
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

  public addToAlbums(album: AlbumInterface): void{
    this.albumApiService.addAlbumToUser(album.id)
    .subscribe((res): void => {
      if(res){
        this.notificationService.showNotification("You added album");
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
