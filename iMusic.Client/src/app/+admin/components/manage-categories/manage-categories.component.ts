import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { AddCategoryComponent } from './add-category/add-category.component';
import { filter, takeUntil } from 'rxjs/operators';
import { CategoryInterface } from 'src/app/core/interfaces/category/category.interface';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { HttpErrorResponse } from '@angular/common/http';
import { NotificationService } from 'src/app/core/services';

@Component({
  selector: 'mus-manage-categories',
  templateUrl: './manage-categories.component.html',
  styleUrls: ['./manage-categories.component.scss']
})
export class ManageCategoriesComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public  categories: CategoryInterface[];

  constructor(
    private readonly dialog: MatDialog,
    private readonly categoryApiService: CategoryApiService,
    private readonly notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    this.getCategories();
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

  public addCategory(): void {
    const dialogRef = this.dialog.open(AddCategoryComponent, {
      panelClass: 'article-dialog',
      maxWidth: '350px',
      data: {
        isCatedoryUpdate: false
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.getCategories();
        }
      });
  }
}
