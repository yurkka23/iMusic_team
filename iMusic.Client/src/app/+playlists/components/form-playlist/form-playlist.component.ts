import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from 'ngx-strongly-typed-forms';
import { Subject } from 'rxjs';
import { StatusEnum } from 'src/app/core/enums';
import { PlaylistFormInterface } from 'src/app/core/interfaces/playlist/playlist-form.interface';
import { PlaylistsComponent } from '../playlists/playlists.component';
import { PlaylistApiService } from 'src/app/core/services/api/playlist-api.service';
import { NotificationService, ValidationService } from 'src/app/core/services';
import { Router } from '@angular/router';
import { PlaylistFormDialogDataInterface } from 'src/app/core/interfaces/playlist/playlist-form-dialog-data.interface';
import { Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { EditPlaylistInterface } from 'src/app/core/interfaces/playlist/edit-playlist.interface';

@Component({
  selector: 'mus-form-playlist',
  templateUrl: './form-playlist.component.html',
  styleUrls: ['./form-playlist.component.scss']
})
export class FormPlaylistComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();
 
  public status = StatusEnum;
  public playlistForm: FormGroup<PlaylistFormInterface>;
  public isPhotoUpdated: boolean = false;

  public titlePhoto: File;
  public titlePhotoUrl: string;


  constructor(private readonly formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public readonly dialogData: PlaylistFormDialogDataInterface,
    private readonly dialogRef: MatDialogRef<PlaylistsComponent>,
    private readonly playlistApiService: PlaylistApiService,
    private readonly notificationService: NotificationService,
    private readonly validationService: ValidationService,
    private readonly router: Router) {
  }

  public ngOnInit(): void {
    this.setPlaylistForm();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private setPlaylistForm(): void {
    this.playlistForm = this.formBuilder.group<PlaylistFormInterface>({
      id: [null],
      title: [null, [Validators.required, Validators.maxLength(64), this.validationService.whitespaceValidator]],
      status:[null, [Validators.required]],
      playlistImgUrl: [null, [Validators.maxLength(256)]]
    });
    
    if (this.dialogData.isPlayListUpdate) {
      this.playlistForm.patchValue(this.dialogData.playList);
      
      this.titlePhotoUrl = this.dialogData.playList.playlistImgUrl;
    }
  }

  public attachFile(event: Event): void {
    this.isPhotoUpdated = true;
    this.titlePhoto = (event.target as HTMLInputElement).files[0];
    const fileReader = new FileReader();

    fileReader.readAsDataURL(this.titlePhoto);
    fileReader.onload = (event: ProgressEvent<FileReader>): void => {
      this.titlePhotoUrl = (event.target.result as string);
      
    };
  }

  public removeTitlePhoto(): void {
    this.titlePhoto = null;
    this.titlePhotoUrl = null;
  }


  public addNewPlaylist(): void {
    if (this.playlistForm.invalid || !this.titlePhoto)  {
      return;
    }

    const data = this.playlistForm.value;
    data.playlistImg = this.titlePhoto;
    
    this.playlistApiService.addPlaylist(data)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val): void => {
        this.dialogRef.close({needUpdate: true});
        this.notificationService.showNotification("Thanks new playlist added.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public editPlaylist(): void {
    if (this.playlistForm.invalid || !this.titlePhotoUrl || (this.playlistForm.pristine && !this.isPhotoUpdated)) {
      return;
    }

    const data: EditPlaylistInterface = {
      id: this.dialogData.playList.id,
      title: this.playlistForm.value.title,
      status: this.playlistForm.value.status,
      playlistImg: this.titlePhoto ?? null,
    };

    this.playlistApiService.editPlaylist(data)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.dialogRef.close({needUpdate: true});
        this.notificationService.showNotification("Thanks playlist was edited.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

}
