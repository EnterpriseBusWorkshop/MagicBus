import { Injectable } from '@angular/core';
import { BehaviorSubject, Subscription} from "rxjs";
import { MessagesClient, MessageFilters, PagedMessagesOfDeadLetter} from "../portal-client";
import { filter, mergeMap} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class DeadLettersGridService {

  messagesSubject: BehaviorSubject<PagedMessagesOfDeadLetter> = new BehaviorSubject<PagedMessagesOfDeadLetter>(null);
  public messages$ = this.messagesSubject.asObservable();
  messageFiltersSubject: BehaviorSubject<MessageFilters> = new BehaviorSubject<MessageFilters>(null);
  public messageFilters$ = this.messageFiltersSubject.asObservable();

  filtersSubscription: Subscription;

  constructor(private messagesClient: MessagesClient) {

    this.messageFilters$.pipe(
      filter(f => f!= null),
      // tap(f => console.log('MESSAGE FILTERS CHANGED', f)),
      mergeMap(f => this.messagesClient.getDeadLetterMessages(f)
      )
    ).subscribe(m => this.messagesSubject.next(m));

  }

  public init () {
    this.applyFilters({
      dateFrom: null,
      dateTo: null,
      pageIndex: 0,
      pageSize: 20,
      sbQueue: 'messagestore',
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
