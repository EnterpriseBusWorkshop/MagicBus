import { Component, OnInit, Input } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ServiceBusClient, DeadLetter } from "../portal-client";

@Component({
  selector: 'app-deadletter-detail',
  templateUrl: './deadletter-detail.component.html'
})
export class DeadLetterDetailComponent implements OnInit {
  @Input() messageJson: string;
  @Input() message: DeadLetter;
  @Input() canResubmit: boolean;

  constructor(public bsModalRef: BsModalRef, private serviceBusClient: ServiceBusClient) { }

  ngOnInit() {
  }

  resubmitMessage()
  {
    this.serviceBusClient.resubmitDeadLetter(
      this.message.id,
      this.message.sequenceNumber,
      this.message.subscriptionName
      ).subscribe(res =>{
        if (res) { window.alert('resubmitted'); }
      } );

    this.bsModalRef.hide();
  }
}
