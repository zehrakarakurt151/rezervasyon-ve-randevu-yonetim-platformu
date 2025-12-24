import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Appointment {
    id: number;
    startTime: string;
    endTime: string;
    serviceName: string;
    price: number;
    status: string;
    providerName: string;
    customerName: string;
    customerPhone: string;
}

export interface CreateAppointmentRequest {
    serviceProviderId: number;
    serviceId: number;
    date: string;    // ISO date string
    time: string;    // "HH:mm" format (e.g., "09:00")
}

@Injectable({
    providedIn: 'root'
})
export class AppointmentService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    // Müşteri: Randevu oluştur
    createAppointment(data: CreateAppointmentRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/appointments`, data);
    }

    // Müşteri: Kendi randevularını getir
    getMyAppointments(): Observable<Appointment[]> {
        return this.http.get<Appointment[]>(`${this.apiUrl}/appointments/my-appointments`);
    }

    // Hizmet Sağlayıcı: Gelen randevularını getir
    getIncomingAppointments(): Observable<Appointment[]> {
        return this.http.get<Appointment[]>(`${this.apiUrl}/appointments/incoming-appointments`);
    }

    // Randevu iptal et
    cancelAppointment(id: number, reason: string): Observable<any> {
        return this.http.put(`${this.apiUrl}/appointments/${id}/cancel`, { reason });
    }

    // Hizmet Sağlayıcı: Randevuyu tamamla
    completeAppointment(id: number): Observable<any> {
        return this.http.put(`${this.apiUrl}/appointments/${id}/complete`, {});
    }
}
