import { Injectable } from '@angular/core';
import {BehaviorSubject, Subscription} from "rxjs";
import {ArchivedMessage, MessageFilters, MessagesClient, PagedMessagesOfArchivedMessage} from "../portal-client";
import {filter, mergeMap, tap} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class MessagesGridService {

  messagesSubject: BehaviorSubject<PagedMessagesOfArchivedMessage> = new BehaviorSubject<PagedMessagesOfArchivedMessage>(null);
  public messages$ = this.messagesSubject.asObservable();
  messageFiltersSubject: BehaviorSubject<MessageFilters> = new BehaviorSubject<MessageFilters>(null);
  public messageFilters$ = this.messageFiltersSubject.asObservable();

  filtersSubscription: Subscription;

  constructor(private messagesClient: MessagesClient) {

    this.messageFilters$.pipe(
      filter(f => f!= null),
      // tap(f => console.log('MESSAGE FILTERS CHANGED', f)),
      mergeMap(f => this.messagesClient.getMessages(f)
      )
    ).subscribe(m => this.messagesSubject.next(m));

  }

  public init () {
    this.applyFilters({
      dateFrom: null,
      dateTo: null,
      pageIndex: 0,
      pageSize:20,
      typeName: '',
      correlationId: null,
      messageId: null
    } as MessageFilters)
  }

  public applyFilters(filters: MessageFilters) {
    this.messageFiltersSubject.next(filters);
  }

  public refresh() {
    this.messageFiltersSubject.next(this.messageFiltersSubject.value);
  }

  public setPage(pageIndex: number) {
    this.applyFilters({...this.messageFiltersSubject.value, pageIndex: pageIndex});
  }

}
