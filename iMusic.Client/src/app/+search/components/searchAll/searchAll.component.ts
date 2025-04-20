import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup } from 'ngx-strongly-typed-forms';
import { Subject } from 'rxjs';
import { debounceTime, switchMap, takeUntil } from 'rxjs/operators';
import { StatusEnum } from 'src/app/core/enums';
import { UserInterface } from 'src/app/core/interfaces';
import { AlbumInterface } from 'src/app/core/interfaces/album/album.interface';
import { CategoryInterface } from 'src/app/core/interfaces/category/category.interface';
import { SearchInterface } from 'src/app/core/interfaces/general/search.interface';
import { PlaylistInterface } from 'src/app/core/interfaces/playlist/playlist.interface';
import { SongInterface } from 'src/app/core/interfaces/songs/song.interface';
import { AuthService, NotificationService } from 'src/app/core/services';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { UserApiService } from 'src/app/core/services/api/user-api.service';
import { AudioPlayerService } from 'src/app/core/services/audio-player.service';
import {Clipboard} from '@angular/cdk/clipboard';
import { FavoritelistApiService } from 'src/app/core/services/api/favoritelist-api.service';

@Component({
  selector: 'mus-search-all',
  templateUrl: './searchAll.component.html',
  styleUrls: ['./searchAll.component.scss']
})
export class SearchAllComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public user: UserInterface;
  public statusEnum = StatusEnum;
  public albums: AlbumInterface[] = [];
  public songs: SongInterface[] = [];
  public playingSong: SongInterface ;
  public categories: CategoryInterface[] = [];
  public playlists: PlaylistInterface[] = [];
  public singers: UserInterface[] = [];
  public playlistsuser: PlaylistInterface[] = [];


  public searchFormGroup: FormGroup<SearchInterface>;

  constructor(
    private readonly albumApiService: AlbumApiService,
    private readonly songApiService: SongApiService,
    private readonly notificationService: NotificationService,
    private readonly authService: AuthService,
    private readonly categoryApiService: CategoryApiService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly audioPlayer: AudioPlayerService,
    private readonly playlistApiService: PlaylistApiService,
    private readonly formBuilder: FormBuilder,
    private readonly userApiService: UserApiService,
    private clipboard: Clipboard,
    private readonly favoriteApiService: FavoritelistApiService,
  ) { }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.getCurrentUser();
      this.getPlaylistsuser();
    }
    this.setForms();
    this.getPlayingSong();
    this.getCategories();
    this.updateSearch();
    this.listenForQueryParametersChange();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private listenForQueryParametersChange(): void {
    this.activatedRoute.queryParams
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((params): void => {
          this.searchFormGroup.patchValue({
            searchTerm: params.searchTerm || null,
          });
        if(!!this.searchFormGroup.value.searchTerm){
          this.getSearchResult();
        }else{
          this.songs = [];
          this.playlists = [];
          this.albums = [];
          this.singers = [];
        }
      });
  }

  private getSearchResult(): void {
      //songs
      this.songApiService.getSearchedSongs(this.searchFormGroup.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((songs): void => {
        this.songs = songs;
        this.songs.length =   this.songs.length > 15 ?  15 :  this.songs.length;
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });

      //albums
      this.albumApiService.getSearchedAlbums(this.searchFormGroup.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((albums): void => {
        this.albums = albums;
        this.albums.length =   this.albums.length > 5 ?  5 :  this.albums.length;
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });

      //playlists
      this.playlistApiService.getSearchedPlaylists(this.searchFormGroup.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((playlists): void => {
        this.playlists = playlists;
        this.playlists.length = this.playlists.length > 5 ?  5 :  this.playlists.length;
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });

      //singers
      this.userApiService.getSearchedSingers(this.searchFormGroup.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((singers): void => {
        this.singers = singers;
        this.singers.length =   this.singers.length > 5 ?  5 :  this.singers.length;
      },
      (error: HttpErrorResponse): void => {
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
  public goToCategory(id: string): void{
    this.router.navigate(['search/category-songs', id.toString()]);
  }

  private setForms(): void {
    this.searchFormGroup = this.formBuilder.group<SearchInterface>({
      searchTerm: null
    });
  }

  public playSong(song: SongInterface): void{
    this.audioPlayer.setSongs(this.songs);
    this.audioPlayer.setCurrentSong(song);
  }
  
  public playAlbum(album: AlbumInterface): void{
    this.audioPlayer.setSongs(album?.songs);
    this.audioPlayer.setCurrentSong(album?.songs[0]);
  }

  public playPlaylist(playlist: PlaylistInterface): void{
    this.audioPlayer.setSongs(playlist?.songs);
    this.audioPlayer.setCurrentSong(playlist?.songs[0]);
  }

  public goToAlbum(id: string): void{
    this.router.navigate(['browse/album', id.toString()]);
  }

  public goToPlaylist(id: string): void{
    this.router.navigate(['browse/playlist', id.toString()]);
  }

  public goToSinger(id: string): void{
    this.router.navigate(['browse/singer', id.toString()]);
  }

  public changeSearch(event: string): void {
    this.searchFormGroup.patchValue({
      searchTerm: event,
    });
  }

  private updateSearch(): void {
    this.searchFormGroup.valueChanges
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.setDefaultFiltersQueryParams();
      });
  }

  private setDefaultFiltersQueryParams(): void {
    this.router.navigate([], {
      queryParams: {
        searchTerm: this.searchFormGroup.value.searchTerm,
      }
    });
  }


  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.user = currentUser;
      });
  }

  public getCategories(): void {
    this.categoryApiService.getAllCategories()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((categories): void => {
        this.categories = categories;
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

  public getPlaylistsuser(): void {
    this.playlistApiService.getUserPlaylists()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((playlists): void => {
        this.playlistsuser = playlists;
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
