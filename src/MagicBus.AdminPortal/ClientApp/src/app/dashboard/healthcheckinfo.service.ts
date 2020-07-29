import { Injectable } from '@angular/core';
import {HealthServiceClient, HealthCheckInfo, HealthCheckResponseStatus} from "../portal-client";
import {BehaviorSubject} from "rxjs";

/**
 * service loads and caches message types - only need to load once!
 */
@Injectable({
  providedIn: 'root'
})
export class HealthCheckInfoService {
    messagesSubject: BehaviorSubject<HealthCheckInfo[]> = new BehaviorSubject<HealthCheckInfo[]>([]);
    public messages$ = this.messagesSubject.asObservable();

  constructor(private healthServiceClient: HealthServiceClient) {
    this.healthServiceClient.getHealthCheckInfo()
      .subscribe(m => this.messagesSubject.next(m));
  }
}
