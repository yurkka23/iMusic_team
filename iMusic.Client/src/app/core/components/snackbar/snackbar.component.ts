import {Component, Inject} from '@angular/core';
import {MAT_SNACK_BAR_DATA, MatSnackBarRef} from '@angular/material/snack-bar';


@Component({
  templateUrl: './snackbar.component.html',
  styleUrls: ['./snackbar.component.scss']
})
export class SnackbarComponent {
  constructor(@Inject(MAT_SNACK_BAR_DATA) public message: string,
    private readonly snackBarRef: MatSnackBarRef<SnackbarComponent>) {
  }

  public close(): void {
    this.snackBarRef.dismiss();
  }
}
