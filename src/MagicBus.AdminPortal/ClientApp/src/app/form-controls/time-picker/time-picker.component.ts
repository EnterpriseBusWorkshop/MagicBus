import {Component, OnDestroy, OnInit} from '@angular/core';
import {ControlValueAccessor, FormControl, NgControl} from "@angular/forms";
import {Subscription} from "rxjs";
import {filter, tap} from "rxjs/operators";


/**
 * this component wraps the ngx-timepicker - which has an issue where it fails to respect emitEvent:false from patchValue
 */
@Component({
  selector: 'app-time-picker',
  templateUrl: './time-picker.component.html',
  styleUrls: ['./time-picker.component.scss']
})
export class TimePickerComponent implements ControlValueAccessor, OnInit, OnDestroy {

  disabled: boolean;
  private onTouched = () => {};
  private onChanged = (c: any) => {};

  timePickerControl = new FormControl('');
  timePickerControlSub: Subscription;

  savedValue; // save the value emitted by timepicker so we can dedupe

  constructor(public control: NgControl) { this.control.valueAccessor = this; }

  ngOnInit(): void {
    this.timePickerControlSub = this.timePickerControl.valueChanges
      .pipe(
        filter(v => v  && v.getTime() != this.savedValue), // only emit when we have a different value
        tap(v => this.savedValue = v.getTime())
      ).subscribe(v => {
        this.onTouched();
        this.onChanged(v);
      });

  }

  registerOnChange(fn: any): void {
    this.onChanged = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }


  writeValue(obj: any): void {
    if (obj && obj instanceof Date) {
      this.savedValue = obj.getTime();
    }
    this.timePickerControl.patchValue(obj);
  }

  ngOnDestroy(): void {
    if (this.timePickerControlSub) {
      this.timePickerControlSub.unsubscribe();
    }
  }

}
