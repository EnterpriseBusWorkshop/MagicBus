import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {HomeComponent} from "./home/home.component";
import {NavMenuComponent} from "./nav-menu/nav-menu.component";
import {MessagesComponent} from "./messages/messages/messages.component";
import {MessageDetailComponent} from "./message-detail/message-detail.component";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import { MessagesGridComponent } from './messages/messages-grid/messages-grid.component';
import { MessagesFiltersComponent } from './messages/messages-filters/messages-filters.component';
import {CommonModule} from "@angular/common";

import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TimePickerComponent } from './form-controls/time-picker/time-picker.component';
import {BsDropdownModule} from "ngx-bootstrap/dropdown";
import { TimepickerModule } from "ngx-bootstrap/timepicker";
import { ModalModule } from 'ngx-bootstrap/modal';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { DeadLettersComponent } from './deadletters/deadletters/deadletters.component';
import { DeadLetterDetailComponent } from './deadletter-detail/deadletter-detail.component'
import { DeadLettersGridComponent } from './deadletters/deadletters-grid/deadletters-grid.component';
import { DeadLettersFiltersComponent } from './deadletters/deadletters-filters/deadletters-filters.component';
import { HealthCheckInfoComponent } from './dashboard/healthcheckinfo/healthcheckinfo.component';
import { DashboardComponent } from './dashboard/dashboard.component';
//import { SummaryComponent } from './mapping-config/summary/summary.component';
//import { TranslationComponent } from './mapping-config/translation/translation.component';
//import { ValueMapperComponent } from './mapping-config/value-mapper/value-mapper.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    MessagesComponent,
    MessageDetailComponent,
    MessagesGridComponent,
    MessagesFiltersComponent,
    TimePickerComponent,
    MessagesFiltersComponent,
    DeadLettersComponent,
    DeadLettersGridComponent,
    DeadLetterDetailComponent,
    DeadLettersFiltersComponent,
    HealthCheckInfoComponent,
    DashboardComponent,
  //  SummaryComponent,
  //  TranslationComponent,
  //  ValueMapperComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    BsDatepickerModule.forRoot(),
    TimepickerModule.forRoot(),
    BsDropdownModule.forRoot(),
    ModalModule.forRoot(),
    PaginationModule.forRoot(),
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
