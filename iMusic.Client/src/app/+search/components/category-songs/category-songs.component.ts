import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { CategoryInterface } from 'src/app/core/interfaces/category/category.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { UserInterface } from 'src/app/core/interfaces';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';

@Component({
  selector: 'mus-category-songs',
  templateUrl: './category-songs.component.html',
  styleUrls: ['./category-songs.component.scss']
})
export class CategorySongsComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public categoryId: string;
  public category: CategoryInterface;
  public songs: SongInterface[] = [];
  public statusEnum = StatusEnum;
  public totalLength: number = 0;
  public playingSong: SongInterface;
  public playlists: PlaylistInterface[] = [];
  public user: UserInterface;

  constructor(
    private readonly router : Router,
    private readonly route: ActivatedRoute,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly audioPlayerService: AudioPlayerService,
    private readonly categoryApiService: CategoryApiService,
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
    this.getCategoryId();
    this.getCategory();
    this.getPlayingSong();
    this.getSongs();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getSongs(): void{
    this.songApiService.getSongsByCategory(this.categoryId)
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((songs): void => {
      this.songs = songs;
      this.songs.forEach( e => this.totalLength += e.duration);
    }, (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);
      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
  }

  public shareSong(song: SongInterface){
    let url: string = `${document.location.origin}/browse/song/${song.id}`;
    this.clipboard.copy(url);
    this.notificationService.showNotification("Url is copied!", );
  }

  private getPlayingSong(): void {
    this.audioPlayerService.currentSong$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((song): void => {
      this.playingSong = song;
    });
  }

  getCategoryId(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id !== null) {
      this.categoryId = id;
    }
  }

  public getCategory(): void {
    this.categoryApiService.getCategory(this.categoryId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((catedory): void => {
        this.category = catedory;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }


  public playCategory(): void {
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

  public goToAlbum(id: string): void{
    if(!id){
      return;
    }
    this.router.navigate(['browse/album', id.toString()]);
  }

  public goToSinger(id: string): void{
    this.router.navigate(['browse/singer/', id.toString()]);
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
