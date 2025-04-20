import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl } from 'ngx-strongly-typed-forms';
import { Subject } from 'rxjs';
import { debounceTime, map, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'mus-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {
  @Output() public search = new EventEmitter<string>();
  @Input() public searchTerm: string;
  @Input() public clearEvent$: Subject<void>;

  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public searchFormControl: FormControl<string>;

  constructor(private readonly formBuilder: FormBuilder) {
  }

  public ngOnInit(): void {
    this.setControl();
    this.checkUpdateValues();
    this.clearEventHandler();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private setControl(): void {
    this.searchFormControl = this.formBuilder.control<string>(this.searchTerm || '');
  }

  private checkUpdateValues(): void {
    this.searchFormControl.valueChanges
      .pipe(
        debounceTime(700),
        map((value): string => value?.trim()),
        takeUntil(this.unsubscribe$)
      )
      .subscribe((value): void => {
        if (value?.length > 1) {
          this.search.emit(value);
        } else {
          this.search.emit(null);
        }
      });
  }

  private clearEventHandler(): void {
    this.clearEvent$
      ?.pipe(takeUntil(this.unsubscribe$))
      .subscribe((): void => {
        this.searchFormControl.reset();
      });
  }

}
