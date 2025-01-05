import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subject, takeUntil } from 'rxjs';
import { MaterialModule } from 'src/app/material.module';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent, ConfirmDialogModel } from 'src/app/components/confirm-dialog/confirm-dialog.component';
import { BudgetModel } from '../models/budget.model';
import { BudgetService } from 'src/app/services/budget.service';
import { CategoryTypeEnum } from '../../category/enums/category-type.enum';
import { BudgetByCategoryAddComponent } from './budget-by-category-add.component';



@Component({
  selector: 'app-budget-list',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './budget-by-category-list.component.html',
  styleUrls: ['./budget-by-category-list.conponent.scss'],
})


export class BudgetByCategoryListComponent implements OnInit, OnDestroy {
  budgetModel: BudgetModel[] = [];
  displayedColumns: string[] = ['type', 'amount', 'Menu'];
  destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(private budgetSevice: BudgetService,
    private toastr: ToastrService,
    public dialog: MatDialog) { }


  ngOnInit(): void {
    this.getBudgeties();
  }

  getBudgeties() {
    this.budgetSevice.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.budgetModel = response;
        },
        error: (fail) => {
          this.toastr.error(fail.error.errors);
        }
      });
  }

  getDescription(type: CategoryTypeEnum): string {
    // return CategoryTypeDescriptions[type] || 'Unknown';
    return '';
  }

  addDialog() {
    const dialogRef = this.dialog.open(BudgetByCategoryAddComponent, {
      width: '500px',
      height: '400px',
      data: this.budgetModel
    });

    dialogRef.afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe(res => {
        if (res.inserted) {
          this.getBudgeties();
        }
      })
  }

  updateDialog(row:any) {

    // let category: CategoryModel = {
    //   categoryId: row.categoryId,
    //   description: row.description,
    //   userId: row.userId,
    //   type: row.type
    // };

    // const dialogRef = this.dialog.open(CategoryUpdateComponent, {
    //   width: '500px',
    //   height: '300px',
    //   data: category
    // });

    // dialogRef.afterClosed()
    //   .pipe(takeUntil(this.destroy$))
    //   .subscribe(res => {
    //     if (res.inserted) {
    //       this.getCategories();
    //     }
    //   })
  }


  deleteCategory(id: string) {

    // const dialogData = new ConfirmDialogModel('Atenção', 'Confirma exclusão ?');

    // const dialogRef = this.dialog.open(ConfirmDialogComponent, {
    //   maxWidth: "400px",
    //   data: dialogData
    // });

    // dialogRef.afterClosed()
    //   .pipe(takeUntil(this.destroy$))
    //   .subscribe(dialogResult => {
    //     if (!dialogResult) return;

    //     this.budgetSevice.delete(id)
    //       .pipe(takeUntil(this.destroy$))
    //       .subscribe({
    //         next: () => {
    //           this.toastr.success('Categoria excluída com sucesso.');
    //           this.getCategories();
    //         },
    //         error: (fail) => {
    //           this.toastr.error(fail.error.errors);
    //         }
    //       });
    //   });
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }

}
