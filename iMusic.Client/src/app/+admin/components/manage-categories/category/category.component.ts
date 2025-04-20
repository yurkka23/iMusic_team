import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CategoryInterface } from 'src/app/core/interfaces/category/category.interface';
import { NotificationService } from 'src/app/core/services';
import { CategoryApiService } from 'src/app/core/services/api/category-api.service';
import { AddCategoryComponent } from '../add-category/add-category.component';
import { filter, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'mus-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.scss']
})
export class CategoryComponent implements OnInit {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();
  @Output() public updateEvent = new EventEmitter<void>();

  @Input() public category: CategoryInterface;
  
  constructor(
    private readonly dialog: MatDialog,
    private readonly categoryApiService: CategoryApiService,
    private readonly notificationService: NotificationService
  ) { }

  ngOnInit(): void {
  }

  public updateCategory(category: CategoryInterface): void {
    const dialogRef = this.dialog.open(AddCategoryComponent, {
      panelClass: 'article-dialog',
      maxWidth: '350px',
      data: {
        category,
        isCategoryUpdate: true
      }
    });

    dialogRef.afterClosed()
      .pipe(
        filter((data): boolean => !!data),
        takeUntil(this.unsubscribe$))
      .subscribe((data: { needUpdate: boolean }): void => {
        if (data.needUpdate) {
          this.updateEvent.emit();
        }
      });
  }

  public deleteCategory(id: string): void {
    this.categoryApiService.deleteCategory(id)
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((val): void => {
      if(val){
        this.updateEvent.emit();
        this.notificationService.showNotification("Thanks category was updated.");
      }else{
        this.notificationService.showNotification("You can't delete this category because this category used in albums, playlists or songs.", 'snack-bar-error');
      }
    },
    (error: HttpErrorResponse): void => {
      const errorMessage = this.notificationService.getErrorMessage(error);
      
      this.notificationService.showNotification(errorMessage, 'snack-bar-error');
    });
        
  }
}
