import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {base64ToFile, ImageCroppedEvent} from 'ngx-image-cropper';
import { AcountComponent } from '../acount/acount.component';

@Component({
  selector: 'mus-image-cropper',
  templateUrl: './image-cropper.component.html',
  styleUrls: ['./image-cropper.component.scss']
})
export class ImageCropperComponent {

  private avatarFile: Blob;

  constructor(@Inject(MAT_DIALOG_DATA) public readonly dialogData: Event,
              private readonly dialogRef: MatDialogRef<AcountComponent>) {
  }

  public imageCropped(event: ImageCroppedEvent): void {
    this.avatarFile = base64ToFile(event.base64);
  }

  public save(): void {
    this.dialogRef.close(this.avatarFile);
  }

}
