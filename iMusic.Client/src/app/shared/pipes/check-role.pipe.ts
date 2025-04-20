import {Pipe, PipeTransform} from '@angular/core';


@Pipe({
  name: 'checkRole'
})
export class CheckRolePipe implements PipeTransform {
  public transform(roles: string[], permissionRoles: string[]): boolean {
    return permissionRoles?.some((role: string): boolean => roles?.includes(role));
  }
}
