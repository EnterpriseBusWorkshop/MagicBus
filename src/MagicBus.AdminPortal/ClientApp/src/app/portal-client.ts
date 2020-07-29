﻿/* tslint:disable */
/* eslint-disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.2.2.0 (NJsonSchema v10.1.4.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');

export interface IHealthServiceClient {
    getHealthCheckInfo(): Observable<HealthCheckInfo[]>;
}

@Injectable({
    providedIn: 'root'
})
export class HealthServiceClient implements IHealthServiceClient {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
    }

    getHealthCheckInfo(): Observable<HealthCheckInfo[]> {
        let url_ = this.baseUrl + "/api/HealthService";
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",			
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGetHealthCheckInfo(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGetHealthCheckInfo(<any>response_);
                } catch (e) {
                    return <Observable<HealthCheckInfo[]>><any>_observableThrow(e);
                }
            } else
                return <Observable<HealthCheckInfo[]>><any>_observableThrow(response_);
        }));
    }

    protected processGetHealthCheckInfo(response: HttpResponseBase): Observable<HealthCheckInfo[]> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <HealthCheckInfo[]>JSON.parse(_responseText, this.jsonParseReviver);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<HealthCheckInfo[]>(<any>null);
    }
}

export interface IMessagesClient {
    getMessageTypes(): Observable<MessageTypeDto[]>;
    getMessages(messageFilters: MessageFilters): Observable<PagedMessagesOfArchivedMessage>;
    resubmitMessage(messageId: string | null | undefined): Observable<boolean>;
    getDeadLetterMessages(messageFilters: MessageFilters): Observable<PagedMessagesOfDeadLetter>;
}

@Injectable({
    providedIn: 'root'
})
export class MessagesClient implements IMessagesClient {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
    }

    getMessageTypes(): Observable<MessageTypeDto[]> {
        let url_ = this.baseUrl + "/api/Messages/message-types";
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",			
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGetMessageTypes(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGetMessageTypes(<any>response_);
                } catch (e) {
                    return <Observable<MessageTypeDto[]>><any>_observableThrow(e);
                }
            } else
                return <Observable<MessageTypeDto[]>><any>_observableThrow(response_);
        }));
    }

    protected processGetMessageTypes(response: HttpResponseBase): Observable<MessageTypeDto[]> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <MessageTypeDto[]>JSON.parse(_responseText, this.jsonParseReviver);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<MessageTypeDto[]>(<any>null);
    }

    getMessages(messageFilters: MessageFilters): Observable<PagedMessagesOfArchivedMessage> {
        let url_ = this.baseUrl + "/api/Messages";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(messageFilters);

        let options_ : any = {
            body: content_,
            observe: "response",
            responseType: "blob",			
            headers: new HttpHeaders({
                "Content-Type": "application/json", 
                "Accept": "application/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGetMessages(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGetMessages(<any>response_);
                } catch (e) {
                    return <Observable<PagedMessagesOfArchivedMessage>><any>_observableThrow(e);
                }
            } else
                return <Observable<PagedMessagesOfArchivedMessage>><any>_observableThrow(response_);
        }));
    }

    protected processGetMessages(response: HttpResponseBase): Observable<PagedMessagesOfArchivedMessage> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <PagedMessagesOfArchivedMessage>JSON.parse(_responseText, this.jsonParseReviver);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<PagedMessagesOfArchivedMessage>(<any>null);
    }

    resubmitMessage(messageId: string | null | undefined): Observable<boolean> {
        let url_ = this.baseUrl + "/api/Messages/resubmitMessage?";
        if (messageId !== undefined)
            url_ += "messageId=" + encodeURIComponent("" + messageId) + "&"; 
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",			
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processResubmitMessage(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processResubmitMessage(<any>response_);
                } catch (e) {
                    return <Observable<boolean>><any>_observableThrow(e);
                }
            } else
                return <Observable<boolean>><any>_observableThrow(response_);
        }));
    }

    protected processResubmitMessage(response: HttpResponseBase): Observable<boolean> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <boolean>JSON.parse(_responseText, this.jsonParseReviver);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<boolean>(<any>null);
    }

    getDeadLetterMessages(messageFilters: MessageFilters): Observable<PagedMessagesOfDeadLetter> {
        let url_ = this.baseUrl + "/api/Messages/deadletters";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(messageFilters);

        let options_ : any = {
            body: content_,
            observe: "response",
            responseType: "blob",			
            headers: new HttpHeaders({
                "Content-Type": "application/json", 
                "Accept": "application/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGetDeadLetterMessages(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGetDeadLetterMessages(<any>response_);
                } catch (e) {
                    return <Observable<PagedMessagesOfDeadLetter>><any>_observableThrow(e);
                }
            } else
                return <Observable<PagedMessagesOfDeadLetter>><any>_observableThrow(response_);
        }));
    }

    protected processGetDeadLetterMessages(response: HttpResponseBase): Observable<PagedMessagesOfDeadLetter> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <PagedMessagesOfDeadLetter>JSON.parse(_responseText, this.jsonParseReviver);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<PagedMessagesOfDeadLetter>(<any>null);
    }
}

export interface IServiceBusClient {
    getServiceBusQueues(): Observable<string[]>;
    resubmitDeadLetter(messageId: string | null | undefined, sequenceNumber: number | undefined, sbQueue: string | null | undefined): Observable<boolean>;
}

@Injectable({
    providedIn: 'root'
})
export class ServiceBusClient implements IServiceBusClient {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
    }

    getServiceBusQueues(): Observable<string[]> {
        let url_ = this.baseUrl + "/api/ServiceBus/servicebus-queues";
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",			
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGetServiceBusQueues(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGetServiceBusQueues(<any>response_);
                } catch (e) {
                    return <Observable<string[]>><any>_observableThrow(e);
                }
            } else
                return <Observable<string[]>><any>_observableThrow(response_);
        }));
    }

    protected processGetServiceBusQueues(response: HttpResponseBase): Observable<string[]> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <string[]>JSON.parse(_responseText, this.jsonParseReviver);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<string[]>(<any>null);
    }

    resubmitDeadLetter(messageId: string | null | undefined, sequenceNumber: number | undefined, sbQueue: string | null | undefined): Observable<boolean> {
        let url_ = this.baseUrl + "/api/ServiceBus/resubmitDeadLetter?";
        if (messageId !== undefined)
            url_ += "messageId=" + encodeURIComponent("" + messageId) + "&"; 
        if (sequenceNumber === null)
            throw new Error("The parameter 'sequenceNumber' cannot be null.");
        else if (sequenceNumber !== undefined)
            url_ += "sequenceNumber=" + encodeURIComponent("" + sequenceNumber) + "&"; 
        if (sbQueue !== undefined)
            url_ += "sbQueue=" + encodeURIComponent("" + sbQueue) + "&"; 
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",			
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processResubmitDeadLetter(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processResubmitDeadLetter(<any>response_);
                } catch (e) {
                    return <Observable<boolean>><any>_observableThrow(e);
                }
            } else
                return <Observable<boolean>><any>_observableThrow(response_);
        }));
    }

    protected processResubmitDeadLetter(response: HttpResponseBase): Observable<boolean> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <boolean>JSON.parse(_responseText, this.jsonParseReviver);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<boolean>(<any>null);
    }
}

export interface IWeatherForecastClient {
    get(): Observable<WeatherForecast[]>;
}

@Injectable({
    providedIn: 'root'
})
export class WeatherForecastClient implements IWeatherForecastClient {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
    }

    get(): Observable<WeatherForecast[]> {
        let url_ = this.baseUrl + "/WeatherForecast";
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",			
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGet(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGet(<any>response_);
                } catch (e) {
                    return <Observable<WeatherForecast[]>><any>_observableThrow(e);
                }
            } else
                return <Observable<WeatherForecast[]>><any>_observableThrow(response_);
        }));
    }

    protected processGet(response: HttpResponseBase): Observable<WeatherForecast[]> {
        const status = response.status;
        const responseBlob = 
            response instanceof HttpResponse ? response.body : 
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <WeatherForecast[]>JSON.parse(_responseText, this.jsonParseReviver);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<WeatherForecast[]>(<any>null);
    }
}

export interface HealthCheckInfo {
    healthCheckRequest?: HealthCheckRequest | null;
    status?: HealthCheckResponseStatus;
    healthCheckResponses?: HealthCheckResponse[] | null;
}

export interface MessageBase {
    correlationId?: string | null;
    messageDate?: Date;
    messageId?: string | null;
    messageType?: string | null;
}

export interface HealthCheckRequest extends MessageBase {
    expectedServices?: string[] | null;
}

export enum HealthCheckResponseStatus {
    Success = 1,
    Warning = 2,
    Error = 3,
}

export interface HealthCheckResponse extends MessageBase {
    appName?: string | null;
    responseStatus?: HealthCheckResponseStatus;
    description?: string | null;
    testResults?: HealthCheckTestResult[] | null;
}

export interface HealthCheckTestResult {
    name?: string | null;
    responseStatus?: HealthCheckResponseStatus;
    message?: string | null;
}

export interface MessageTypeDto {
    name?: string | null;
    fullName?: string | null;
}

export interface PagedMessagesOfArchivedMessage {
    pageIndex?: number;
    pageSize?: number;
    totalRecords?: number;
    messages?: ArchivedMessage[] | null;
}

export interface ArchivedMessage {
    id?: string | null;
    message?: MessageBase | null;
    messageDate?: Date;
    messageTypeName?: string | null;
}

export interface MessageFilters {
    dateFrom?: Date | null;
    dateTo?: Date | null;
    sbQueue?: string | null;
    typeName?: string | null;
    messageType?: string | null;
    messageId?: string | null;
    correlationId?: string | null;
    pageIndex?: number;
    pageSize?: number;
}

export interface PagedMessagesOfDeadLetter {
    pageIndex?: number;
    pageSize?: number;
    totalRecords?: number;
    messages?: DeadLetter[] | null;
}

export interface DeadLetter {
    id?: string | null;
    message?: MessageBase | null;
    messageDate?: Date;
    messageTypeName?: string | null;
    deadLetterReason?: string | null;
    deadLetterErrorDescription?: string | null;
    sequenceNumber?: number;
    subscriptionName?: string | null;
}

export interface WeatherForecast {
    date?: Date;
    temperatureC?: number;
    temperatureF?: number;
    summary?: string | null;
}

export class SwaggerException extends Error {
    message: string;
    status: number; 
    response: string; 
    headers: { [key: string]: any; };
    result: any; 

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isSwaggerException = true;

    static isSwaggerException(obj: any): obj is SwaggerException {
        return obj.isSwaggerException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): Observable<any> {
    if (result !== null && result !== undefined)
        return _observableThrow(result);
    else
        return _observableThrow(new SwaggerException(message, status, response, headers, null));
}

function blobToText(blob: any): Observable<string> {
    return new Observable<string>((observer: any) => {
        if (!blob) {
            observer.next("");
            observer.complete();
        } else {
            let reader = new FileReader(); 
            reader.onload = event => { 
                observer.next((<any>event.target).result);
                observer.complete();
            };
            reader.readAsText(blob); 
        }
    });
}