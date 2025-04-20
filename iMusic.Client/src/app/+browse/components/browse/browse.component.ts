import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { UserInterface } from 'src/app/core/interfaces';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { CategoryInterface } from 'src/app/core/interfaces/category/category.interface';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import { environment } from 'src/environments/environment';
import {Clipboard} from '@angular/cdk/clipboard';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';


@Component({
  selector: 'mus-browse',
  templateUrl: './browse.component.html',
  styleUrls: ['./browse.component.scss']
})
export class BrowseComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public user: UserInterface;
  public statusEnum = StatusEnum;
  public albums: AlbumInterface[] = [];
  public topSongs: SongInterface[] = [];
  public newSongs: SongInterface[] = [];
  public recommendedSongs: SongInterface[] = [];
  public playingSong: SongInterface ;
  public categories: CategoryInterface[];
  public topPlaylists: PlaylistInterface[] = [];
  public playlists: PlaylistInterface[] = [];

  constructor(
    private readonly albumApiService: AlbumApiService,
    private readonly songApiService: SongApiService,
    private readonly activatedRoute: ActivatedRoute,
    private readonly notificationService: NotificationService,
    private readonly authService: AuthService,
    private readonly categoryApiService: CategoryApiService,
    private readonly router: Router,
    private readonly audioPlayer: AudioPlayerService,
    private readonly playlistApiService: PlaylistApiService,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService
  ) {}

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.getCurrentUser();
      this.getPlaylists();
    }
    this.getTopAlbums();
    this.getPlayingSong();
    this.getTopSongs();
    this.getNewSongs();
    this.getCategories();
    this.getTopPlaylists();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getCategories(): void {
    this.categoryApiService.getAllCategories()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((categories): void => {
        this.categories = categories;
        this.categories.length =   this.categories.length > 6 ?  6 :  this.categories.length;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
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

  public playTopSong(song: SongInterface): void{
    this.audioPlayer.setSongs(this.topSongs);
    this.audioPlayer.setCurrentSong(song);
  } 

  public getTopSongs(): void {
    this.songApiService.getTopSongs()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.topSongs = songs;
        this.topSongs.length =   this.topSongs.length > 12 ?  12 :  this.topSongs.length;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getNewSongs(): void {
    this.songApiService.getNewSongs()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.newSongs = songs;
        this.newSongs.length =   this.newSongs.length > 10 ?  10 :  this.newSongs.length;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getRecommendedSongs(): void {
    if(!this.user?.id){
      return;
    }
    this.songApiService.getRecommendedSongs(this.user.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.recommendedSongs = songs;
        this.recommendedSongs.length =   this.recommendedSongs.length > 10 ?  10 :  this.recommendedSongs.length;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public getTopAlbums(): void {
    this.albumApiService.getTopAlbums()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((albums): void => {
        this.albums = albums;
        this.albums.length =   this.albums.length > 10 ?  10 :  this.albums.length;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
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

  public sharePlaylist(playlist: PlaylistInterface){
    let url: string = `${document.location.origin}/browse/playlist/${playlist.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  public playRecommendedALbums(album: AlbumInterface): void{
    this.audioPlayer.setSongs(album?.songs);
    this.audioPlayer.setCurrentSong(album?.songs[0]);
  } 

  public playPlaylist(playlist: PlaylistInterface): void{
    this.audioPlayer.setSongs(playlist?.songs);
    this.audioPlayer.setCurrentSong(playlist?.songs[0]);
  } 

  public playNewSongs(song: SongInterface): void{
    this.audioPlayer.setSongs(this.newSongs);
    this.audioPlayer.setCurrentSong(song);
  } 

  public playRecommendedForYou(song: SongInterface): void{
    this.audioPlayer.setSongs(this.recommendedSongs);
    this.audioPlayer.setCurrentSong(song);
  }

  public goToAlbum(id: string): void{
    if(!id){
      return;
    }
    this.router.navigate(['browse/album', id.toString()]);
  }

  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

  public goToPlaylist(id: string): void{
    this.router.navigate(['browse/playlist', id.toString()]);
  }


  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.user = currentUser;
        this.getRecommendedSongs();
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
