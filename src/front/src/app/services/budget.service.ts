import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable } from 'rxjs';
import { BaseService } from './BaseService';
import { BudgetModel } from '../pages/budget/models/budget.model';

@Injectable({ providedIn: 'root' })
export class BudgetService extends BaseService {

  public currentUrl = new BehaviorSubject<any>(undefined);

  constructor(private http: HttpClient) {
    super();
  }

  getAll(): Observable<BudgetModel[]> {
    let response = this.http
      .get(this.UrlServiceV1 + 'v1/budget',  this.getAuthHeaderJson())
      .pipe(
        map(this.extractData),
        catchError(this.serviceError));

    return response;
  }

  create(category: BudgetModel): Observable<BudgetModel> {
    let response = this.http
      .post(this.UrlServiceV1 + 'v1/budget', category, this.getAuthHeaderJson())
      .pipe(
        map(this.extractData),
        catchError(this.serviceError));

    return response;
  }

  update(category: BudgetModel): Observable<BudgetModel> {
    let response = this.http
      .put(this.UrlServiceV1 + 'v1/budget', category, this.getAuthHeaderJson())
      .pipe(
        map(this.extractData),
        catchError(this.serviceError));

    return response;
  }  

  delete(categoryId: string): Observable<void> {
    let response = this.http
      .delete(this.UrlServiceV1 + `v1/budget/${categoryId}`, this.getAuthHeaderJson())
      .pipe(
        map(this.extractData),
        catchError(this.serviceError));

    return response;
  }  

}