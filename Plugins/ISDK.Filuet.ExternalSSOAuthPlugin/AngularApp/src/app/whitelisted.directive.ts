import { Directive, HostBinding  } from "@angular/core";

@Directive({
    selector: "[WhitelistedCountries]"
})
export class WhitelistedDirective {
    @HostBinding('value') private _setValue;

    setValue(value: string) {
        this._setValue = value;
    }
}
