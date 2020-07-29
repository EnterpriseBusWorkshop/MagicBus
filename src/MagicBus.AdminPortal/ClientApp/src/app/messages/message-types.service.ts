import { Injectable } from '@angular/core';
import {MessagesClient, MessageTypeDto} from "../portal-client";
import {BehaviorSubject} from "rxjs";
import {filter} from "rxjs/operators";

/**
 * service loads and caches message types - only need to load once!
 */
@Injectable({
  providedIn: 'root'
})
export class MessageTypesService {

    private messageTypesSubject: BehaviorSubject<MessageTypeDto[]> = new BehaviorSubject<MessageTypeDto[]>(null);
    public messageTypes$ = this.messageTypesSubject.asObservable()
      .pipe(filter(t => t!= null));

  constructor(private messagesClient: MessagesClient) {
    this.messagesClient.getMessageTypes()
      .subscribe(m => this.messageTypesSubject.next(m));
  }
}
