import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from 'ngx-strongly-typed-forms';
import { Subject } from 'rxjs';
import { YourSongsComponent } from '../your-songs.component';
import { SongApiService } from 'src/app/core/services/api/song-api.service';
import { NotificationService, ValidationService } from 'src/app/core/services';
import { Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { SongFormInterface } from 'src/app/core/interfaces/songs/song-form.interface';
import { SongFormDialogDataInterface } from 'src/app/core/interfaces/songs/song-form-dialog-data.interface';
import { StatusEnum } from 'src/app/core/enums';
import { CategoryInterface } from 'src/app/core/interfaces/category/category.interface';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { EditSongInterface } from 'src/app/core/interfaces/songs/edit-song.interface';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'mus-form-song',
  templateUrl: './form-song.component.html',
  styleUrls: ['./form-song.component.scss']
})
export class FormSongComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();
 
  public albumId: string;
  public categories: CategoryInterface[] = [];
  public status = StatusEnum;
  public songForm: FormGroup<SongFormInterface>;
  public isPhotoUpdated: boolean = false;
  public isSongUpdated: boolean = false;

  public titlePhoto: File;
  public titlePhotoUrl: string;

  public songFile: File;
  public songFileUrl: string;

  constructor(private readonly formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public readonly dialogData: SongFormDialogDataInterface,
    private readonly dialogRef: MatDialogRef<YourSongsComponent>,
    private readonly songApiService: SongApiService,
    private readonly route: ActivatedRoute,
    private readonly notificationService: NotificationService,
    private readonly categoryApiService: CategoryApiService,
    private readonly validationService: ValidationService) {
      this.getCategories()
  }

  public ngOnInit(): void {
    this.setSongForm();
  }

 
  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getCategories(): void {
    this.categoryApiService.getAllCategories()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((categories): void => {
        this.categories = categories;
      }, (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);
        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  private setSongForm(): void {
    this.songForm = this.formBuilder.group<SongFormInterface>({
      title: [null, [Validators.required, Validators.maxLength(64), this.validationService.whitespaceValidator]],
      text: [null ,[Validators.maxLength(1000)]],
      status:[null, [Validators.required]],
      categoryName: [null, [Validators.required, Validators.maxLength(64)]],
      songImgUrl: [null, [Validators.maxLength(256)]],
      songFileUrl: [null, [Validators.maxLength(256)]],
    });

    this.albumId = this.dialogData?.song?.albumId;
    
    if (this.dialogData.isSongUpdate) {
      this.songForm.patchValue(this.dialogData.song);
      
      this.songForm.patchValue({
        categoryName: this.dialogData.song.category.name
      })
      this.titlePhotoUrl = this.dialogData.song.songImgUrl;
      this.songFileUrl = this.dialogData.song.songUrl;
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

  public attachSongFile(event: Event): void {
    this.isSongUpdated = true;
    this.songFile = (event.target as HTMLInputElement).files[0];
  
    this.songFileUrl = "../../../../../assets/images/music-file.png";
  }

  public removeTitlePhoto(): void {
    this.titlePhoto = null;
    this.titlePhotoUrl = null;
  }

  public removeSongFile(): void {
    this.songFile = null;
    this.songFileUrl = null;
  }

  public addNewSong(): void {
    if (this.songForm.invalid || !this.titlePhoto || !this.songFile)  {
      return;
    }

    const data = this.songForm.value;
    data.songImg = this.titlePhoto;
    data.songFile = this.songFile;
    data.albumId = this.albumId;

    this.songApiService.uploadSong(data)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.dialogRef.close({needUpdate: true});
        this.notificationService.showNotification("Thanks new song added.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public editSong(): void {
    if (this.songForm.invalid || !this.titlePhotoUrl || !this.songFileUrl || (this.songForm.pristine && !this.isPhotoUpdated && !this.isSongUpdated)) {
      return;
    }

    const data: EditSongInterface = {
      ...this.songForm.value,
      id: this.dialogData.song.id,
      songFile: this.songFile,
      songImg: this.titlePhoto
    };

    this.songApiService.editSong(data)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.dialogRef.close({needUpdate: true});
        this.notificationService.showNotification("Thanks song was edited.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

}
