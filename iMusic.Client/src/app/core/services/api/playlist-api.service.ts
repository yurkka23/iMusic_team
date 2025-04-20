import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PlaylistInterface } from '../../interfaces/playlist/playlist.interface';
import { SearchInterface } from '../../interfaces/general/search.interface';
import { PlaylistFormInterface } from '../../interfaces/playlist/playlist-form.interface';
import { convertJsonToFormData } from '../../utils/form.utils';
import { EditPlaylistInterface } from '../../interfaces/playlist/edit-playlist.interface';

@Injectable({
  providedIn: 'root'
})
export class PlaylistApiService {
  private readonly apiUrl: string = `${environment.apiAddress}Playlist`;
  public readonly needToupdate$: Subject<boolean> = new Subject<boolean>();  

  constructor(private readonly httpClient: HttpClient) {
  }

  public getTopPlaylists(): Observable<PlaylistInterface[]> {
    return this.httpClient.get<PlaylistInterface[]>(`${this.apiUrl}/get-top-playlist/`);
  }

  public getUserPlaylists(): Observable<PlaylistInterface[]> {
    return this.httpClient.get<PlaylistInterface[]>(`${this.apiUrl}/get-playlists-by-user/`);
  }

  public getPlaylist(id: string): Observable<PlaylistInterface> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<PlaylistInterface>(`${this.apiUrl}/get-playlist`, { params });
  }
  
  public getSearchedPlaylists(requestForm: SearchInterface): Observable<PlaylistInterface[]> {
    return this.httpClient.post<PlaylistInterface[]>(`${this.apiUrl}/search/`, requestForm);
  }

  public addPlaylist(data: PlaylistFormInterface): Observable<boolean> {    
    const formData = convertJsonToFormData(data);

    return this.httpClient.post<boolean>(`${this.apiUrl}/created-playlist`, formData);
  }

  public editPlaylist(data: EditPlaylistInterface): Observable<boolean> {    
    const formData = convertJsonToFormData(data);

    return this.httpClient.put<boolean>(`${this.apiUrl}/edit-playlist`, formData);
  }

  public deletePlaylist(id: string): Observable<boolean> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.delete<boolean>(`${this.apiUrl}/delete-playlist/`, { params });
  }

  public addSongToPlaylist(songId: string, playlistId: string): Observable<boolean> {
    let params = new HttpParams()
    .append('playlistId', playlistId)
    .append('songId', songId);

    return this.httpClient.get<boolean>(`${this.apiUrl}/add-song`, {params});
  }

  public removeSongFromPlaylist(songId: string, playlistId: string): Observable<boolean> {
    let params = new HttpParams()
    .append('playlistId', playlistId)
    .append('songId', songId);

    return this.httpClient.get<boolean>(`${this.apiUrl}/remove-song`, {params});
  }

}
