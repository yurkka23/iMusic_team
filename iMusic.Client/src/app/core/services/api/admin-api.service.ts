import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { StorageService } from '../storage.service';
import { Observable } from 'rxjs';
import { UserInterface } from '../../interfaces';
import { SongInterface } from '../../interfaces/songs/song.interface';
import { AlbumInterface } from '../../interfaces/album/album.interface';
import { FavoriteListInterface } from '../../interfaces/favoritelist/FavoriteList.interface';
import { PlaylistInterface } from '../../interfaces/playlist/playlist.interface';

@Injectable({
  providedIn: 'root'
})
export class AdminApiService {

  private readonly apiUrl: string = `${environment.apiAddress}Admin`;

  constructor(private readonly httpClient: HttpClient,
              private readonly storageService: StorageService) {
  }

  public getBecomeusersRequests(): Observable<UserInterface[]> {
    return this.httpClient.get<UserInterface[]>(`${this.apiUrl}/get-become-singer-requests`);
  }

  public getUsers(): Observable<UserInterface[]> {
    return this.httpClient.get<UserInterface[]>(`${this.apiUrl}/get-users`);
  }

  public getBannedUsers(): Observable<UserInterface[]> {
    return this.httpClient.get<UserInterface[]>(`${this.apiUrl}/get-banned-users`);
  }

  public getSingers(): Observable<UserInterface[]> {
    return this.httpClient.get<UserInterface[]>(`${this.apiUrl}/get-singers`);
  }

  public approveSinger(id: string): Observable<void> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<void>(`${this.apiUrl}/approve-singer`, { params });
  }

  public rejectSinger(id: string): Observable<void> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<void>(`${this.apiUrl}/reject-singer`, { params });
  }

  public removeSingerRole(id: string): Observable<void> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<void>(`${this.apiUrl}/remove-singer-role`, { params });
  }

  public ban(id: string): Observable<void> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<void>(`${this.apiUrl}/ban`, { params });
  }

  public unban(id: string): Observable<void> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<void>(`${this.apiUrl}/unban`, { params });
  }

  public getUserInfo(id: string): Observable<UserInterface> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<UserInterface>(`${this.apiUrl}/get-user-info`, { params });
  }

  public getUserAddedSongs(id: string): Observable<SongInterface[]> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/get-user-added-songs`, { params });
  }

  public getUserAddedAlbums(id: string): Observable<AlbumInterface[]> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<AlbumInterface[]>(`${this.apiUrl}/get-user-added-albums`, { params });
  }

  public getUserFavoriteList(id: string): Observable<FavoriteListInterface> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<FavoriteListInterface>(`${this.apiUrl}/get-user-favoritelist`, { params });
  }

  public getUserPlaylists(id: string): Observable<PlaylistInterface[]> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<PlaylistInterface[]>(`${this.apiUrl}/get-user-playlists`, { params });
  }
 
}
