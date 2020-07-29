import {Component, OnDestroy, OnInit, TemplateRef} from '@angular/core';
import {MessagesGridService} from "../messages-grid.service";
import {ArchivedMessage, PagedMessagesOfArchivedMessage} from "../../portal-client";
import { Observable } from "rxjs";
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { MessageDetailComponent } from '../../message-detail/message-detail.component';
import {debounceTime, filter, takeWhile, tap} from "rxjs/operators";
import {FormControl} from "@angular/forms";

@Component({
  selector: 'app-messages-grid',
  templateUrl: './messages-grid.component.html',
  styleUrls: ['./messages-grid.component.scss']
})
export class MessagesGridComponent implements OnInit, OnDestroy {

  componentExists = true;
  pagedMessages$: Observable<PagedMessagesOfArchivedMessage>;
  bsModalRef: BsModalRef;

  pageFrom: number;
  pageTo: number;
  totalRecords: number;

  paginationControl: FormControl = new FormControl();

  constructor(
    private messagesGridService: MessagesGridService,
    private modalService: BsModalService
  ) { }



  ngOnInit(): void {
    this.pagedMessages$ = this.messagesGridService.messages$.pipe(
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
    ) .subscribe(v => this.messagesGridService.setPage(v -1))
  }

  openMessageModal(message: ArchivedMessage) {
    const initialState = {
      messageJson: JSON.stringify(message, null, 2),
      messageId: message.message.messageId,
      canResubmit: this.messageCanResubmit(message.messageTypeName)
    };
    this.bsModalRef = this.modalService.show(MessageDetailComponent, {
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
