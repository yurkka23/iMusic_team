import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { FavoriteListInterface } from 'src/app/core/interfaces/favoritelist/FavoriteList.interface';

@Injectable({
  providedIn: 'root'
})
export class FavoritelistApiService {
  private readonly apiUrl: string = `${environment.apiAddress}FavoriteList`;

  constructor(private readonly httpClient: HttpClient) {
  }

  public addSong(id: string ): Observable<boolean> {
    let params = new HttpParams()
    .append('songId', id);

    return this.httpClient.get<boolean>(`${this.apiUrl}/add-song`, {params});
  }

  public addAlbum(id: string ): Observable<boolean> {
    let params = new HttpParams()
    .append('albumId', id);

    return this.httpClient.get<boolean>(`${this.apiUrl}/add-album`, {params});
  }

  public addPlaylist(id: string ): Observable<boolean> {
    let params = new HttpParams()
    .append('playlistId', id);

    return this.httpClient.get<boolean>(`${this.apiUrl}/add-playlist`, {params});
  }

 
  public removeSong(id: string ): Observable<boolean> {
    let params = new HttpParams()
    .append('songId', id);

    return this.httpClient.get<boolean>(`${this.apiUrl}/remove-song`, {params});
  }

  public removeAlbum(id: string ): Observable<boolean> {
    let params = new HttpParams()
    .append('albumId', id);

    return this.httpClient.get<boolean>(`${this.apiUrl}/remove-album`, {params});
  }

  public removePlaylist(id: string ): Observable<boolean> {
    let params = new HttpParams()
    .append('playlistId', id);

    return this.httpClient.get<boolean>(`${this.apiUrl}/remove-playlist`, {params});
  }


  public getFavoritelist(): Observable<FavoriteListInterface> {
    return this.httpClient.get<FavoriteListInterface>(`${this.apiUrl}/get-favorite-list`);
  }
}
