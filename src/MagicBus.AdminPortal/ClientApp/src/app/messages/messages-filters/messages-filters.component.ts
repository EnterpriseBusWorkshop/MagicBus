import {Component, OnDestroy, OnInit} from '@angular/core';
import {MessagesGridService} from "../messages-grid.service";
import {FormBuilder, FormGroup} from "@angular/forms";
import {debounceTime, filter, take, takeWhile} from "rxjs/operators";
import {MessageFilters, MessageTypeDto} from "../../portal-client";
import {MessageTypesService} from "../message-types.service";
import {Observable} from "rxjs";

@Component({
  selector: 'app-messages-filters',
  templateUrl: './messages-filters.component.html',
  styleUrls: ['./messages-filters.component.scss']
})
export class MessagesFiltersComponent implements OnInit, OnDestroy {

  formGroup: FormGroup;
  componentExists = true;

  messageTypes$: Observable<MessageTypeDto[]>;

  constructor(
    private messagesGridService: MessagesGridService,
    private fb: FormBuilder,
    private messageTypesService: MessageTypesService
  ) { }

  ngOnInit(): void {

    this.messageTypes$ = this.messageTypesService.messageTypes$;

    // init with empty filters
    this.messagesGridService.init();

    this.buildForm();

    this.formGroup.valueChanges.pipe(
      takeWhile(t => this.componentExists),
      debounceTime(1000),
    ).subscribe(formVal => this.patchModel(formVal));

    this.messagesGridService.messageFilters$.pipe(
      filter(f => f!= null),
      takeWhile(t => this.componentExists),
    ).subscribe(filtersModel => this.patchForm(filtersModel));

  }


  buildForm() {
    this.formGroup = this.fb.group({
      fromDate: [''],
      fromTime: [''],
      toDate: [''],
      toTime: [''],
      typeName: [''],
      correlationId: [''],
      messageId: ['']
    })
  }

  // apply changes from form back to model in service
  patchModel(formVal: any) {
    //console.log('Form Changed!', formVal);
    let newFilters = { ...formVal } as MessageFilters;

    if (formVal.fromDate && formVal.fromTime) {
      newFilters.dateFrom = new Date(formVal.fromDate.getFullYear(), formVal.fromDate.getMonth(), formVal.fromDate.getDate(), formVal.fromTime?.getHours(), formVal.fromTime?.getMinutes(), 0,0);
      //console.log('set DateFrom', newFilters.dateFrom);
    } else if (formVal.fromDate) {
      newFilters.dateFrom = new Date(formVal.fromDate.getFullYear(), formVal.fromDate.getMonth(), formVal.fromDate.getDate());
    }

    if (formVal.toDate && formVal.toTime) {
      newFilters.dateTo = new Date(formVal.toDate.getFullYear(), formVal.toDate.getMonth(), formVal.toDate.getDate(), formVal.toTime?.getHours(), formVal.toTime?.getMinutes(), 0, 0);
    } else if (formVal.toDate) {
      newFilters.dateTo = new Date(formVal.toDate.getFullYear(), formVal.toDate.getMonth(), formVal.toDate.getDate());
    }

    this.messagesGridService.applyFilters(newFilters);
  }

  refresh() {
    this.messagesGridService.refresh();
  }

  // apply model changes to form
  patchForm(filtersModel: MessageFilters) {
    this.formGroup.patchValue({
      ...filtersModel,
      fromDate: filtersModel.dateFrom,
      fromTime: filtersModel.dateFrom,
      toDate: filtersModel.dateTo,
      toTime: filtersModel.dateTo,
    }, { emitEvent: false });
  }

  ngOnDestroy(): void {
    this.componentExists = false;
  }



  selectLast10Minutes() {
    this.setDates(
      new Date(new Date().getTime() - 10 * 60 * 1000),
      new Date()
    );
  }

  selectLast30Minutes() {
    this.setDates(
      new Date(new Date().getTime() - 30 * 60 * 1000),
      new Date()
    );
  }

  selectLastHour() {
    this.setDates(
      new Date(new Date().getTime() - 60 * 60 * 1000),
      new Date()
    );
  }

  selectLast4Hours() {
    this.setDates(
      new Date(new Date().getTime() - 4* 60 * 60 * 1000),
      new Date()
    );
  }

  selectToday() {
    const now = new Date();
    this.setDates(
      new Date(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
      new Date(new Date(now.getFullYear(), now.getMonth(), now.getDate()+1))
    );
  }

  selectYesterday() {
    const now = new Date();
    this.setDates(
      new Date(new Date(now.getFullYear(), now.getMonth(), now.getDate()-1)),
      new Date(new Date(now.getFullYear(), now.getMonth(), now.getDate()))
    );
  }

  selectThisWeek() {
    const now = new Date();
    this.setDates(
      new Date(new Date(now.getFullYear(), now.getMonth(), now.getDate()-6)),
      new Date(new Date(now.getFullYear(), now.getMonth(), now.getDate()+1))
    );
  }

  setDates(dateFrom, dateTo) {
    this.messagesGridService.messageFilters$
      .pipe(take(1))
      .subscribe(f => {
        f.dateTo = dateTo;
        f.dateFrom = dateFrom;
        this.messagesGridService.applyFilters(f);
      });
  }

  formReset() {
    this.messagesGridService.init();
  }
}
