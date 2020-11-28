import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Injectable()
export class SnackbarService{

  constructor(private _snackBar: MatSnackBar) {}

  public open(message, action = 'success', durationInSeconds = 5) {
    // this._snackBar.openFromComponent(SnackbarComponent, {
    //   duration: this.durationInSeconds * 1000,
    // });
    let duration: number = durationInSeconds * 1000;
    this._snackBar.open(message, action, { duration });
  }
}
