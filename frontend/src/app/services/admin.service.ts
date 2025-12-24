import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DashboardStats {
    totalCustomers: number;
    totalProviders: number;
    totalAppointments: number;
    pendingAppointments: number;
    completedAppointments: number;
    totalRevenue: number;
}

export interface User {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    role: string;
    createdDate: string;
}

export interface AdminAppointment {
    id: number;
    customerName: string;
    providerName: string;
    serviceName: string;
    startTime: string;
    endTime: string;
    totalPrice: number;
    status: string;
    createdDate: string;
}

@Injectable({
    providedIn: 'root'
})
export class AdminService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getStats(): Observable<DashboardStats> {
        return this.http.get<DashboardStats>(`${this.apiUrl}/admin/stats`);
    }

    getAllUsers(): Observable<User[]> {
        return this.http.get<User[]>(`${this.apiUrl}/admin/users`);
    }

    getAllAppointments(): Observable<AdminAppointment[]> {
        return this.http.get<AdminAppointment[]>(`${this.apiUrl}/admin/appointments`);
    }
}
