import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SongInterface } from '../../interfaces/songs/song.interface';
import { HttpClient, HttpParams } from '@angular/common/http';
import { convertJsonToFormData } from '../../utils/form.utils';
import { SongFormInterface } from '../../interfaces/songs/song-form.interface';
import { EditSongInterface } from '../../interfaces/songs/edit-song.interface';
import { SearchInterface } from '../../interfaces/general/search.interface';

@Injectable({
  providedIn: 'root'
})
export class SongApiService {

  private readonly apiUrl: string = `${environment.apiAddress}Song`;

  constructor(private readonly httpClient: HttpClient) {
  }

  public getTopSongs(): Observable<SongInterface[]> {
    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/get-top-songs/`);
  }

  public getNewSongs(): Observable<SongInterface[]> {
    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/get-new-songs/`);
  }

  public getSearchedSongs(requestForm: SearchInterface): Observable<SongInterface[]> {
    return this.httpClient.post<SongInterface[]>(`${this.apiUrl}/search/`, requestForm);
  }

  public getRecommendedSongs(id: string): Observable<SongInterface[]> {
    let params = new HttpParams()
    .append('id', id);

    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/recommends-songs-to-user/`, { params });
  }

  public getSongById(id: string): Observable<SongInterface> {
    let params = new HttpParams()
    .append('id', id);

    return this.httpClient.get<SongInterface>(`${this.apiUrl}/get-song/`, { params });
  }

  public getTopSingerSongs(id: string): Observable<SongInterface[]> {
    let params = new HttpParams()
    .append('id', id);

    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/get-top-singer-songs/`, { params });
  }

  public getSongsBySinger(id: string): Observable<SongInterface[]> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/get-songs-by-singer/`, { params });
  }

  public getUserSongs(): Observable<SongInterface[]> {
    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/get-user-songs/`);
  }

  public getSongsByCategory(id: string): Observable<SongInterface[]> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/get-songs-by-category/`, { params });
  }

  public uploadSong(data: SongFormInterface): Observable<void> {
    const formData = convertJsonToFormData(data);

    return this.httpClient.post<void>(`${this.apiUrl}/upload-song`, formData);
  }

  public editSong(data: EditSongInterface): Observable<void> {    
    const formData = convertJsonToFormData(data);

    return this.httpClient.put<void>(`${this.apiUrl}/edit-song`, formData);
  }

  public deleteSong(id: string): Observable<boolean> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.delete<boolean>(`${this.apiUrl}/delete-song/`, { params });
  }

  public addSongToUser(songId: string): Observable<boolean> {
    let params = new HttpParams()
      .append('songId', songId);

    return this.httpClient.get<boolean>(`${this.apiUrl}/add-song-to-user/`, { params });
  }

  public removeSongFromUser(songId: string): Observable<boolean> {
    let params = new HttpParams()
      .append('songId', songId);

    return this.httpClient.delete<boolean>(`${this.apiUrl}/remove-song-form-user/`, { params });
  }

  public getUserRecentlySongs(): Observable<SongInterface[]> {
    return this.httpClient.get<SongInterface[]>(`${this.apiUrl}/get-user-recently-songs/`);
  }
}
