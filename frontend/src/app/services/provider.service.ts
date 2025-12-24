import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Provider {
    id: number;
    businessName: string;
    firstName: string;
    lastName: string;
    address: string;
    description: string;
    averageRating: number;
    serviceCount: number;
}

export interface ProviderDetail extends Provider {
    services: ServiceItem[];
}

export interface ServiceItem {
    id: number;
    name: string;
    description: string;
    price: number;
    durationMinutes: number;
}

@Injectable({
    providedIn: 'root'
})
export class ProviderService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    // Tüm hizmet sağlayıcıları getir
    getAllProviders(): Observable<Provider[]> {
        return this.http.get<Provider[]>(`${this.apiUrl}/providers`);
    }

    // Tek sağlayıcının detayları
    getProviderById(id: number): Observable<ProviderDetail> {
        return this.http.get<ProviderDetail>(`${this.apiUrl}/providers/${id}`);
    }

    // Sağlayıcının hizmetleri
    getProviderServices(id: number): Observable<ServiceItem[]> {
        return this.http.get<ServiceItem[]>(`${this.apiUrl}/providers/${id}/services`);
    }

    // Müsait saatleri getir (Dinamik - dolu saatleri göstermez)
    getAvailableSlots(providerId: number, serviceId: number, date: Date): Observable<string[]> {
        const formattedDate = date.toISOString().split('T')[0]; // YYYY-MM-DD
        const params = new HttpParams()
            .set('providerId', providerId.toString())
            .set('serviceId', serviceId.toString())
            .set('date', formattedDate);

        return this.http.get<string[]>(`${this.apiUrl}/availability/slots`, { params });
    }
}
