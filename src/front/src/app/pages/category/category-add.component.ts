import { Component, ElementRef, Inject, OnDestroy, OnInit, ViewChildren } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subject, takeUntil } from 'rxjs';
import { MaterialModule } from 'src/app/material.module';
import { CategoryService } from 'src/app/services/category.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { FormBaseComponent } from 'src/app/components/base-components/form-base.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FormControl, FormControlName, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryModel } from './models/category.model';
import { V } from '@angular/cdk/keycodes';

@Component({
  selector: 'app-category-add',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, MatButtonModule],
  templateUrl: './category-add.component.html',
})

export class CategoryAddComponent extends FormBaseComponent implements OnInit, OnDestroy {
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements!: ElementRef[];

  form: FormGroup = new FormGroup({});
  categoryModel!: CategoryModel;
  submitted = false;
  destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(public dialog: MatDialog,
    private categorySevice: CategoryService,
    private toastr: ToastrService,
    private dialogRef: MatDialogRef<CategoryAddComponent>,) {
    super();

    this.validationMessages = {
      description: {
        required: 'Informe a descrição da categoria.',
        minlength: 'A descrição precisa ter entre 4 e 100 caracteres.',
        maxlength: 'A descrição precisa ter entre 4 e 100 caracteres.',
      },
      type: {
        required: 'Informe o tipo da categoria.',
      },
    };

    super.configureMessagesValidation(this.validationMessages);
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      description: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(100)]),
      type: new FormControl('', [Validators.required]),
    });
  }

  ngAfterViewInit(): void {
    super.configureValidationFormBase(this.formInputElements, this.form);
  }

  checkDescription(event: FocusEvent) {
    const inputElement = event.target as HTMLInputElement;
    const value = inputElement.value;

  }

  submit() {
    this.categoryModel = this.form.value;
    this.categorySevice.create(this.categoryModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          //this.categoryModel = result;

          if (!result) {
            this.toastr.error('Erro ao salvar a categoria.');
            return;
          }

            let toast = this.toastr.success('Categoria criada com sucesso.');
            if (toast) {
              toast.onHidden.pipe(takeUntil(this.destroy$)).subscribe(() => {
                this.dialogRef.close({ inserted: true })
              });
            }

        },
        error: (fail) => {
          this.submitted = false;
          this.toastr.error(fail.error.errors);
        }
      });
  }

  cancel() {
    this.dialogRef.close({ inserted: false });
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }

}
