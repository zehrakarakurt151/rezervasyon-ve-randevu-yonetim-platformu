import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Service {
    id: number;
    name: string;
    description: string;
    price: number;
    durationMinutes: number;
    bufferTimeMinutes: number;
    serviceProviderId: number;
}

export interface CreateServiceRequest {
    name: string;
    description: string;
    price: number;
    durationMinutes: number;
    bufferTimeMinutes: number;
}

export interface ServiceProvider {
    id: number;
    firstName: string;
    lastName: string;
    businessName: string;
    address: string;
    description: string;
    averageRating: number;
}

@Injectable({
    providedIn: 'root'
})
export class ServiceService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    // Tüm hizmetleri getir
    getAllServices(): Observable<Service[]> {
        return this.http.get<Service[]>(`${this.apiUrl}/services`);
    }

    // Belirli bir sağlayıcının hizmetlerini getir
    getServicesByProvider(providerId: number): Observable<Service[]> {
        return this.http.get<Service[]>(`${this.apiUrl}/services/provider/${providerId}`);
    }

    // Hizmet sağlayıcı: Yeni hizmet ekle
    createService(data: CreateServiceRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/services`, data);
    }

    // Kendi hizmetlerimi getir (Provider için - yetkilendirilmiş)
    getMyServices(): Observable<Service[]> {
        return this.http.get<Service[]>(`${this.apiUrl}/services/my`);
    }


    // Hizmet sağlayıcı: Hizmet güncelle
    updateService(id: number, data: CreateServiceRequest): Observable<any> {
        return this.http.put(`${this.apiUrl}/services/${id}`, data);
    }

    // Hizmet sağlayıcı: Hizmet sil
    deleteService(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/services/${id}`);
    }
}
