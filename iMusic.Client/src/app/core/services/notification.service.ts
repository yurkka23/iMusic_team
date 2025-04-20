import {Injectable} from '@angular/core';
import {MatSnackBar} from '@angular/material/snack-bar';


import {SnackbarComponent} from '../components/snackbar/snackbar.component';
import {HttpErrorResponse, HttpStatusCode} from '@angular/common/http';


@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private readonly snackBar: MatSnackBar) {
  }

  public getErrorMessage(error: HttpErrorResponse): string {
    let errorMessage: string;

    if (!error) {
      errorMessage = "Error Request";
    } else {
      switch (error.status) {
        case HttpStatusCode.BadRequest:
          errorMessage = Object.values(error.error?.errors)[0] as string;
          break;
        case HttpStatusCode.ServiceUnavailable:
        case HttpStatusCode.ImATeapot:
        case HttpStatusCode.UnprocessableEntity:
          errorMessage = error.error.title;
          break;
        default:
          errorMessage = "Error Request";
      }
    }

    return errorMessage;
  }

  public showNotification(text: string, panelClass: string = 'snack-bar-success'): void {
    this.snackBar.openFromComponent(SnackbarComponent, {
      duration: 10000,
      verticalPosition: 'bottom',
      horizontalPosition: 'center',
      panelClass: [panelClass],
      data: text
    });
  }
}
