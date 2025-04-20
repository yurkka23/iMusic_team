import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from 'ngx-strongly-typed-forms';
import { Subject } from 'rxjs';
import { CategoryFormInterface } from 'src/app/core/interfaces/category/category-form.interface';
import { ManageCategoriesComponent } from '../manage-categories.component';
import { NotificationService, ValidationService } from 'src/app/core/services';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { CategoryFormDialogDataInterface } from 'src/app/core/interfaces/category/category-form-dialog-data.interface';
import { Validators } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { UpdateCategoryInterface } from 'src/app/core/interfaces/category/update-category.interface';

@Component({
  selector: 'mus-add-category',
  templateUrl: './add-category.component.html',
  styleUrls: ['./add-category.component.scss']
})
export class AddCategoryComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();
 
  public categoryForm: FormGroup<CategoryFormInterface>;
  public isPhotoUpdated: boolean = false;
  public titlePhoto: File;
  public titlePhotoUrl: string;

  constructor(private readonly formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public readonly dialogData: CategoryFormDialogDataInterface,
    private readonly dialogRef: MatDialogRef<ManageCategoriesComponent>,
    private readonly categoryApiService: CategoryApiService,
    private readonly notificationService: NotificationService,
    private readonly validationService: ValidationService) {
  }

  public ngOnInit(): void {
    this.setCategoryForm();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private setCategoryForm(): void {
    this.categoryForm = this.formBuilder.group<CategoryFormInterface>({
      name: [null, [Validators.required, Validators.maxLength(64), this.validationService.whitespaceValidator]],
      categoryImgUrl: [null, [Validators.maxLength(256), this.validationService.whitespaceValidator]],
    });
    
    if (this.dialogData.isCategoryUpdate) {
      this.categoryForm.patchValue(this.dialogData.category);
      this.titlePhotoUrl = this.dialogData.category.categoryImgUrl;
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

  public addNewCategory(): void {
    if (this.categoryForm.invalid || !this.titlePhoto) {
      return;
    }

    const data = this.categoryForm.value;

    data.newPhoto = this.titlePhoto;
    this.categoryApiService.createCategory(data)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.dialogRef.close({needUpdate: true});
        this.notificationService.showNotification("Thanks new category added.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

  public updateCategory(): void {
    if (this.categoryForm.invalid || !this.titlePhotoUrl || (this.categoryForm.pristine && !this.isPhotoUpdated)) {
      return;
    }

    const data: UpdateCategoryInterface = {
      ...this.categoryForm.value,
      id: this.dialogData.category.id,
      newPhoto: this.titlePhoto
    };

    this.categoryApiService.updateCategory(data)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.dialogRef.close({needUpdate: true});
        this.notificationService.showNotification("Thanks category was updated.");
      },
      (error: HttpErrorResponse): void => {
        const errorMessage = this.notificationService.getErrorMessage(error);

        this.notificationService.showNotification(errorMessage, 'snack-bar-error');
      });
  }

}
