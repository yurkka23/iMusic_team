import {Injectable} from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly token = 'token';
  private readonly tokenExpiration = 'tokenExpiration';
  private readonly refreshToken = 'refreshToken';
  private readonly currentUserRoles = 'currentUserRoles';
  private readonly initialUserRoles = 'initialUserRoles';

  public clear(): void {
    localStorage.clear();
  }

  public setToken(token: string): void {
    localStorage.setItem(this.token, token);
  }

  public setRefreshToken(refreshToken: string): void {
    localStorage.setItem(this.refreshToken, refreshToken);
  }

  public getToken(): string {
    return localStorage.getItem(this.token);
  }

  public getRefreshToken(): string {
    return localStorage.getItem(this.refreshToken);
  }

  public removeToken(): void {
    return localStorage.removeItem(this.token);
  }

  public removeRefreshToken(): void {
    return localStorage.removeItem(this.refreshToken);
  }

  public setTokenExpiration(tokenExpiration: string): void {
    localStorage.setItem(this.tokenExpiration, tokenExpiration);
  }

  public getTokenExpiration(): string {
    return localStorage.getItem(this.tokenExpiration);
  }

  public save<T>(key: string, data: T): void {
    localStorage.setItem(key, JSON.stringify(data));
  }

  public load<T>(key: string): T {
    return JSON.parse(localStorage.getItem(key)) as T;
  }

  public setCurrentUserRoles(roles: string[]): void {
    localStorage.setItem(this.currentUserRoles, roles.join(','));
  }

  public setInitialRolesRoles(roles: string[]): void {
    localStorage.setItem(this.initialUserRoles, JSON.stringify(roles));
  }

  public getCurrentUserRoles(): string[] {
    return localStorage.getItem(this.currentUserRoles)?.split(',');
  }

  public getInitialRoles(): string[] {
    return JSON.parse(localStorage.getItem(this.initialUserRoles));
  }

  public checkInitialRoles(): boolean {
    return localStorage.getItem(this.initialUserRoles) !== null;
  }

}
