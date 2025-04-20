import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CategoryFormInterface } from '../../interfaces/category/category-form.interface';
import { CategoryInterface } from '../../interfaces/category/category.interface';
import { Observable } from 'rxjs';
import { convertJsonToFormData } from '../../utils/form.utils';
import { UpdateCategoryInterface } from '../../interfaces/category/update-category.interface';
import { CreateCategoryFromInterface } from '../../interfaces/category/create-catedory-form.interface';
import { EditCategoryFromInterface } from '../../interfaces/category/edit-category-form.interface';

@Injectable({
  providedIn: 'root'
})
export class CategoryApiService {

  private readonly apiUrl: string = `${environment.apiAddress}Category`;

  constructor(private readonly httpClient: HttpClient) {
  }

  public createCategory(data: CategoryFormInterface): Observable<CategoryInterface> {
    let request: CreateCategoryFromInterface = {
      name: data.name,
      categoryImg : data.newPhoto
    }
    const formData = convertJsonToFormData(request);

    return this.httpClient.post<CategoryInterface>(`${this.apiUrl}/create-category`, formData);
  }

  public updateCategory(data: UpdateCategoryInterface): Observable<CategoryInterface> {
    let request: EditCategoryFromInterface = {
      id: data.id,
      name: data.name,
      categoryImg : data.newPhoto
    }
    const formData = convertJsonToFormData(request);

    return this.httpClient.put<CategoryInterface>(`${this.apiUrl}/edit-category`, formData);
  }

  public deleteCategory(id: string): Observable<boolean> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.delete<boolean>(`${this.apiUrl}/delete-category/`, { params });
  }

  
  public getAllCategories(): Observable<CategoryInterface[]> {
    return this.httpClient.get<CategoryInterface[]>(`${this.apiUrl}/get-all`);
  }

  public getCategory(id: string): Observable<CategoryInterface> {
    let params = new HttpParams()
      .append('id', id);

    return this.httpClient.get<CategoryInterface>(`${this.apiUrl}/get-category`, { params });
  }
}
