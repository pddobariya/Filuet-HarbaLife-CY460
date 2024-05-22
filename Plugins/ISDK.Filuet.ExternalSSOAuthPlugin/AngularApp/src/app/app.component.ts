import { Component, OnInit, Input, ElementRef, Renderer2 } from '@angular/core';
import { RestDataSource } from './model/rest.datasource';
import Country from './model/country.model';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    isEnableCountryRestrictions: boolean;
    whitelistedCountries: string;

    countryId: number;
    countries: Country[];
    selectedCountries: Country[];

    isShow: boolean = false;

    private errors = {};

    constructor(private renderer: Renderer2, private restDataSource: RestDataSource) {

        //this.whitelistedCountries = elementRef.nativeElement.getAttribute('whitelistedCountries');
    }

    changeStatus(event) {
        console.log(this.isEnableCountryRestrictions);
    }

    changeCountry(countryId: string) { // for debug
        console.log(countryId);
    }

    addCountry() {
        if (!this.selectedCountries.some(el => el.id == Number(this.countryId))) {
            let selectedCountry = this.countries.find(el => el.id == this.countryId);
            if (selectedCountry != null) {
                this.selectedCountries.push(selectedCountry);

                this.setWhitelistedCountries();
            }
        }
    }

    deleteLine(id: number) {

        let deletedIndex = this.selectedCountries.findIndex(el => el.id == id);

        this.selectedCountries.splice(deletedIndex, 1);
        this.setWhitelistedCountries();
    }

    enableRestrictions(event: Event) {
        let enableRestrictionsChbx = <HTMLInputElement>event.target;
        if (enableRestrictionsChbx.checked == true) {
            this.isShow = true;
            this.loadData();
        }
        else {
            this.isShow = false;
        }
    }

    loadData() {
        this.restDataSource.getCountries()
            .subscribe(countries => {
                this.countries = countries;
            }, error => {
                this.errors = error;
            });

        this.restDataSource.getSelectedCountries()
            .subscribe(countries => {
                this.selectedCountries = countries;
            }, error => {
                this.errors = error;
            })
    }

    setWhitelistedCountries() {
        this.whitelistedCountries = "";
        for (let i = 0; i < this.selectedCountries.length; i++) {
            this.whitelistedCountries += `${this.selectedCountries[i].iso_code},`;
        }
        const whitelistedElem = <HTMLInputElement>document.querySelector('#WhitelistedCountries');
        whitelistedElem.value = this.whitelistedCountries;
    }

    ngOnInit() {

        const isEnableRestrictionsElem = <HTMLInputElement>document.querySelector('#IsEnableCountryRestrictions');
        this.renderer.listen(isEnableRestrictionsElem, 'click', this.enableRestrictions.bind(this));

        const whitelistedElem = <HTMLInputElement>document.querySelector('#WhitelistedCountries');
        this.whitelistedCountries = whitelistedElem.value;

        this.isEnableCountryRestrictions = isEnableRestrictionsElem.checked == true;


        if (this.isEnableCountryRestrictions) {
            this.isShow = true;
            this.loadData();
        }

        //this.renderer.setProperty(isEnableElem, "value", "we,er");
        //this.renderer.setProperty(whitelistedElem, "value", "we,er");
        //this.restDataSource.loadData();
    }
}
