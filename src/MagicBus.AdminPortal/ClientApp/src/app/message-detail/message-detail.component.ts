import { Component, OnInit, Input } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import {MessagesClient} from "../portal-client";

@Component({
  selector: 'app-message-detail',
  templateUrl: './message-detail.component.html'
})
export class MessageDetailComponent implements OnInit {
  @Input() messageJson: string;
  @Input() messageId: string;
  @Input() canResubmit: boolean;

  constructor(
    public bsModalRef: BsModalRef,
    public messagesClient: MessagesClient
  ) { }

  ngOnInit() {
  }

  resubmitMessage() {
    this.messagesClient.resubmitMessage(this.messageId)
      .subscribe(res =>{
        if (res) { window.alert('resubmitted'); }
      } )
  }
}
