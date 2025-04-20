import {Injectable} from '@angular/core';

import {BehaviorSubject} from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class SidebarService {
  public readonly isSidebarOpened$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public readonly isSidebarBufferOpened$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

}
