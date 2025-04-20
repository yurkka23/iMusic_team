import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';

import {Observable} from 'rxjs';

import {StorageService} from '../services';


@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private readonly storageService: StorageService,
              private readonly router: Router) {
  }

  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const acceptableRoles: string[] = route.data.roles;

    const currentUserRoles = this.storageService.getCurrentUserRoles();

    if (currentUserRoles) {
      return this.checkAccess(acceptableRoles, currentUserRoles);
    } else {
      this.router.navigate(['auth/sign-in']);

      return false;
    }
  }

  private checkAccess(acceptableRoles: string[], userRoles: string[]): boolean {
    const isPageAvailable = acceptableRoles.some((role): boolean => userRoles.includes(role));

    if (!isPageAvailable) {
      this.router.navigate(['403']);
    }

    return isPageAvailable;
  }
}
