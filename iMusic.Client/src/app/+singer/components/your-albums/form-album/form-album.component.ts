import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from 'ngx-strongly-typed-forms';
import { Subject } from 'rxjs';
import { StatusEnum } from 'src/app/core/enums';
import { AlbumFormInterface } from 'src/app/core/interfaces/album/album-form.interface';
import { CategoryInterface } from 'src/app/core/interfaces/category/category.interface';
import { YourAlbumsComponent } from '../your-albums.component';
import { NotificationService, ValidationService } from 'src/app/core/services';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { Validators } from '@angular/forms';
import { AlbumApiService } from 'src/app/core/services/api/album-api.service';
import { AlbumFormDialogDataInterface } from 'src/app/core/interfaces/album/album-form-dialog-data.interface';
import { Router } from '@angular/router';
import { EditAlbumInterface } from 'src/app/core/interfaces/album/edit-album.interface';

@Component({
  selector: 'mus-form-album',
  templateUrl: './form-album.component.html',
  styleUrls: ['./form-album.component.scss']
})
export class FormAlbumComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();
 
  public categories: CategoryInterface[] = [];
  public status = StatusEnum;
  public albumForm: FormGroup<AlbumFormInterface>;
  public isPhotoUpdated: boolean = false;

  public titlePhoto: File;
  public titlePhotoUrl: string;


  constructor(private readonly formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public readonly dialogData: AlbumFormDialogDataInterface,
    private readonly dialogRef: MatDialogRef<YourAlbumsComponent>,
    private readonly albumApiService: AlbumApiService,
    private readonly notificationService: NotificationService,
    private readonly categoryApiService: CategoryApiService,
    private readonly validationService: ValidationService,
    private readonly router: Router) {
      this.getCategories()
  }

  public ngOnInit(): void {
    this.setAlbumForm();
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

  private setAlbumForm(): void {
    this.albumForm = this.formBuilder.group<AlbumFormInterface>({
      id : [null],
      title: [null, [Validators.required, Validators.maxLength(64), this.validationService.whitespaceValidator]],
      status:[null, [Validators.required]],
      categoryId: [null, [Validators.required, Validators.maxLength(64)]],
      albumImgUrl: [null, [Validators.maxLength(256)]],
    });
    
    if (this.dialogData.isAlbumUpdate) {
      this.albumForm.patchValue(this.dialogData.album);
      
      this.titlePhotoUrl = this.dialogData.album.albumImgUrl;
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


  public addNewAlbum(): void {
    if (this.albumForm.invalid || !this.titlePhoto)  {
      return;
    }

    const data = this.albumForm.value;
    data.albumImg = this.titlePhoto;
    
    this.albumApiService.addAlbum(data)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((id): void => {
        this.dialogRef.close({needUpdate: true});
        this.router.navigate(['singer/album', id.toString()]);
        this.notificationService.showNotification("Thanks new album added.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public editAlbum(): void {
    if (this.albumForm.invalid || !this.titlePhotoUrl || (this.albumForm.pristine && !this.isPhotoUpdated)) {
      return;
    }

    const data: EditAlbumInterface = {
      id: this.dialogData.album.id,
      title: this.albumForm.value.title,
      status: this.albumForm.value.status,
      categoryId: this.albumForm.value.categoryId,
      albumImg: this.titlePhoto ?? null,
    };

    this.albumApiService.editAlbum(data)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.dialogRef.close({needUpdate: true});
        this.notificationService.showNotification("Thanks album was edited.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

}
