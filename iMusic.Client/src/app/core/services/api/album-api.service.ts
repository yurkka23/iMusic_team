import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AlbumInterface } from '../../interfaces/album/album.interface';
import { AlbumFormInterface } from '../../interfaces/album/album-form.interface';
import { convertJsonToFormData } from '../../utils/form.utils';
import { EditAlbumInterface } from '../../interfaces/album/edit-album.interface';
import { SearchInterface } from '../../interfaces/general/search.interface';

@Injectable({
  providedIn: 'root'
})
export class AlbumApiService {
  private readonly apiUrl: string = `${environment.apiAddress}Album`;

  constructor(private readonly httpClient: HttpClient) {
  }

  public getTopAlbums(): Observable<AlbumInterface[]> {
    return this.httpClient.get<AlbumInterface[]>(`${this.apiUrl}/recommends-albums/`);
  }

  public getUserAlbums(): Observable<AlbumInterface[]> {
    return this.httpClient.get<AlbumInterface[]>(`${this.apiUrl}/get-user-abums/`);
  }

  public getAlbumsBySinger(id: string): Observable<AlbumInterface[]> {
    let params = new HttpParams()
      .append('singerId', id);

    return this.httpClient.get<AlbumInterface[]>(`${this.apiUrl}/get-albums-by-singer/`, { params });
  }

  public getSearchedAlbums(requestForm: SearchInterface): Observable<AlbumInterface[]> {
    return this.httpClient.post<AlbumInterface[]>(`${this.apiUrl}/search/`, requestForm);
  }

  public getAlbumById(id: string): Observable<AlbumInterface> {
    let params = new HttpParams()
      .append('albumId', id);

    return this.httpClient.get<AlbumInterface>(`${this.apiUrl}/get-album-by-id/`, { params });
  }

  public addAlbum(data: AlbumFormInterface): Observable<string> {    
    const formData = convertJsonToFormData(data);

    return this.httpClient.post<string>(`${this.apiUrl}/created-album`, formData);
  }

  public editAlbum(data: EditAlbumInterface): Observable<boolean> {    
    const formData = convertJsonToFormData(data);

    return this.httpClient.put<boolean>(`${this.apiUrl}/edit-album`, formData);
  }

  public deleteAlbum(id: string): Observable<boolean> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.delete<boolean>(`${this.apiUrl}/delete-album/`, { params });
  }

  public addAlbumToUser(albumId: string): Observable<boolean> {
    let params = new HttpParams()
      .append('albumId', albumId);

    return this.httpClient.get<boolean>(`${this.apiUrl}/add-album-to-user/`, { params });
  }

  public removeAlbumFromUser(albumId: string): Observable<boolean> {
    let params = new HttpParams()
      .append('albumId', albumId);

    return this.httpClient.delete<boolean>(`${this.apiUrl}/remove-album-form-user/`, { params });
  }

  public getUserRecentlyAlbums(): Observable<AlbumInterface[]> {
    return this.httpClient.get<AlbumInterface[]>(`${this.apiUrl}/get-user-recently-albums/`);
  }
}
