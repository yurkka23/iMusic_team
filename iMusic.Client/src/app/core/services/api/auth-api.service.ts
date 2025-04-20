import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';

import {Observable} from 'rxjs';

import {environment} from 'src/environments/environment';

import {
  UserInterface,
  NewPasswordInterface,
  SignInInterface,
  SignUpInterface,
  TokenInformationInterface
} from '../../interfaces';

import {StorageService} from '../storage.service';


@Injectable({
  providedIn: 'root',
})
export class AuthApiService {
  private readonly apiUrl: string = `${environment.apiAddress}Auth`;

  constructor(private readonly httpClient: HttpClient,
              private readonly storageService: StorageService) {
  }

  public signIn(signInForm: SignInInterface): Observable<TokenInformationInterface> {
    return this.httpClient.post<TokenInformationInterface>(`${this.apiUrl}/login`, signInForm);
  }

  public refreshToken(): Observable<TokenInformationInterface> {
    const request = {
      accessToken: this.storageService.getToken(),
      refreshToken: this.storageService.getRefreshToken()
    };

    return this.httpClient.post<TokenInformationInterface>(`${this.apiUrl}/refresh-token`, request);
  }

  public resetPassword(request: NewPasswordInterface): Observable<void> {
    return this.httpClient.post<void>(`${this.apiUrl}/reset-password`, request);
  }

  public sendSignUpForm(signUpForm: SignUpInterface): Observable<void> {
    return this.httpClient.post<void>(`${this.apiUrl}/register`, signUpForm);
  }

  
}
