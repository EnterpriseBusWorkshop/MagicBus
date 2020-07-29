import { Component, OnInit } from '@angular/core';
import { HealthCheckInfoService } from "../healthcheckinfo.service";
import { HealthCheckInfo, HealthCheckResponseStatus, HealthCheckRequest, HealthCheckResponse } from "../../portal-client";
import { Observable } from "rxjs";

@Component({
  selector: 'app-healthcheckinfo',
  templateUrl: './healthcheckinfo.component.html',
  styleUrls: ['healthcheckinfo.scss']
})
export class HealthCheckInfoComponent implements OnInit {

  messages$: Observable<HealthCheckInfo[]>;
  healthCheckResponseStatus = HealthCheckResponseStatus;
  bootstrapStatus = BootstrapStatus;
  faIconStatus = FaIconStatus;

  constructor(
    private healthCheckInfoService: HealthCheckInfoService,
  ) { }

  ngOnInit(): void {
    this.messages$ = this.healthCheckInfoService.messages$;
  }
}

export enum BootstrapStatus {
    Success = "success",
    Warning = "warning",
    Error = "danger",
}

export enum FaIconStatus {
  Success = "fa-check",
  Warning = "fa-exclamation",
  Error = "fa-exclamation-triangle",
}
