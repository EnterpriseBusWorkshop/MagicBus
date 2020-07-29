import { Component, OnInit, TemplateRef, OnDestroy } from '@angular/core';
import {DeadLettersGridService} from "../deadletters-grid.service";
import {ArchivedMessage, MessageFilters, PagedMessagesOfDeadLetter, DeadLetter} from "../../portal-client";
import { Observable } from "rxjs";
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { DeadLetterDetailComponent } from '../../deadletter-detail/deadletter-detail.component';
import { DeadLettersFiltersComponent } from '../deadletters-filters/deadletters-filters.component';
import { stringify } from 'querystring';
import { take, map, takeWhile, filter, tap, debounceTime } from 'rxjs/operators';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-deadletters-grid',
  templateUrl: './deadletters-grid.component.html',
  styleUrls: ['./deadletters-grid.component.scss']
})
export class DeadLettersGridComponent implements OnInit, OnDestroy {

  componentExists = true;
  pagedMessages$: Observable<PagedMessagesOfDeadLetter>;
  bsModalRef: BsModalRef;

  pageFrom: number;
  pageTo: number;
  totalRecords: number;

  paginationControl: FormControl = new FormControl();

  constructor(
    private deadLettersGridService: DeadLettersGridService,
    private modalService: BsModalService
  ) { }

  ngOnInit(): void {
    this.pagedMessages$ = this.deadLettersGridService.messages$.pipe(
      takeWhile(t => this.componentExists),
      filter(pm => pm != null),
      tap(pm => {
        // console.log('GOT DATA', pm);
        this.pageFrom = (pm.pageIndex * pm.pageSize) +1;
        this.pageTo = this.pageFrom + pm.pageSize;
        this.totalRecords = pm.totalRecords;
        this.paginationControl.patchValue(pm.pageIndex+1, { emitEvent: false});
      })
    );

    this.paginationControl.valueChanges.pipe(
      debounceTime(1000)
    ) .subscribe(v => this.deadLettersGridService.setPage(v -1))
  }

  openMessageModal(message: DeadLetter) {
    const initialState = {
      messageJson: JSON.stringify(message, null, 2),
      message: message,
      canResubmit: this.messageCanResubmit(message.messageTypeName)
    };

    this.bsModalRef = this.modalService.show(DeadLetterDetailComponent, {
      initialState: initialState,
      'class': 'modal-lg'
    });
    this.bsModalRef.content.closeBtnName = 'Close';
  }

  messageCanResubmit(messageTypeName: string)
  {
    if(messageTypeName == "ExceptionMessage")
    {
      return false;
    }
    return true;
  }
  
  ngOnDestroy(): void {
    this.componentExists = false;
  }
}
