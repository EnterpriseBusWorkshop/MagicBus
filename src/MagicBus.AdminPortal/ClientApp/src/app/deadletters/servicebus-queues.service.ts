import { Injectable } from '@angular/core';
import {ServiceBusClient, MessageTypeDto} from "../portal-client";
import {BehaviorSubject} from "rxjs";
import {filter} from "rxjs/operators";

/**
 * service loads and caches message types - only need to load once!
 */
@Injectable({
  providedIn: 'root'
})
export class ServiceBusQueuesService {

    private serviceBusQueuesSubject: BehaviorSubject<string[]> = new BehaviorSubject<string[]>(null);
    public serviceBusQueues$ = this.serviceBusQueuesSubject.asObservable()
      .pipe(filter(t => t!= null));

  constructor(private serviceBusClient: ServiceBusClient) {
    this.serviceBusClient.getServiceBusQueues()
      .subscribe(m => this.serviceBusQueuesSubject.next(m));
  }
}
