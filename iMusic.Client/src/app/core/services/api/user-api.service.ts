import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';

import {Observable} from 'rxjs';

import {environment} from 'src/environments/environment';

import {
  UserInterface,
  NewPasswordInterface,
  SignInInterface,
  SignUpInterface,
  TokenInformationInterface,
  PersonalInfoInterface,
  ChangePasswordInterface
} from '../../interfaces';

import {StorageService} from '../storage.service';
import { SearchInterface } from '../../interfaces/general/search.interface';


@Injectable({
  providedIn: 'root',
})
export class UserApiService {
  private readonly apiUrl: string = `${environment.apiAddress}User`;
  private readonly apiUrlAuth: string = `${environment.apiAddress}Auth`;

  constructor(private readonly httpClient: HttpClient,
              private readonly storageService: StorageService) {
  }

  public getCurrentUser(): Observable<UserInterface> {
    return this.httpClient.get<UserInterface>(`${this.apiUrl}/me`);
  }

  public getSearchedSingers(requestForm: SearchInterface): Observable<UserInterface[]> {
    return this.httpClient.post<UserInterface[]>(`${this.apiUrl}/search-singer/`, requestForm);
  }

  public getUserSingers(): Observable<UserInterface[]> {
    return this.httpClient.get<UserInterface[]>(`${this.apiUrl}/get-user-singers/`);
  }
  
  public getUser(id: string): Observable<UserInterface> {
    let params = new HttpParams()
       .append('id', id);

    return this.httpClient.get<UserInterface>(`${this.apiUrl}/get-user`, {params});
  }

  public updateUserProfileImage(userProfileImage: FormData) : Observable<void> {
    return this.httpClient.put<void>(`${this.apiUrl}/update-user-profile-image`, userProfileImage);
  }

  public updatePersonalInfo(data: PersonalInfoInterface): Observable<void> {
    return this.httpClient.put<void>(`${this.apiUrl}/update`, {...data});
  }

  public changePassword(data: ChangePasswordInterface): Observable<void> {
    return this.httpClient.post<void>(`${this.apiUrlAuth}/reset-password`, data);
  }

  public becomeSinger(id: string): Observable<void> {
     let params = new HttpParams()
       .append('id', id);

    return this.httpClient.get<void>(`${this.apiUrl}/become-singer/`, {params});
  }

  public deleteAcount(): Observable<void> {
   return this.httpClient.delete<void>(`${this.apiUrl}/delete-acount/`);
 }
}
