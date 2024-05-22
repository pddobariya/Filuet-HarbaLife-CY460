import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import Country from './country.model';

@Injectable()
export class RestDataSource {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {

    }

    getCountries(): Observable<Country[]> {
        return this.http.get<Country[]>(this.baseUrl + "countries/all");
    }

    getSelectedCountries(): Observable<Country[]> {
        return this.http.get<Country[]>(this.baseUrl + "countries/whitelisted");
    }
}
