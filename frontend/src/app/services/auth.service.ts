import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    userId: number;
    role: string;
}

export interface CustomerRegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    password: string;
}

export interface ProviderRegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    password: string;
    businessName: string;
    address: string;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = environment.apiUrl;
    private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());

    isLoggedIn$ = this.isLoggedInSubject.asObservable();

    constructor(private http: HttpClient) { }

    private hasToken(): boolean {
        return !!localStorage.getItem('token');
    }

    login(credentials: LoginRequest): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, credentials).pipe(
            tap(response => {
                localStorage.setItem('token', response.token);
                localStorage.setItem('userId', response.userId.toString());
                localStorage.setItem('role', response.role);
                this.isLoggedInSubject.next(true);
            })
        );
    }

    registerCustomer(data: CustomerRegisterRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/auth/register/customer`, data);
    }

    registerProvider(data: ProviderRegisterRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/auth/register/provider`, data);
    }

    logout(): void {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('role');
        this.isLoggedInSubject.next(false);
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    getUserId(): number | null {
        const id = localStorage.getItem('userId');
        return id ? parseInt(id, 10) : null;
    }

    getRole(): string | null {
        return localStorage.getItem('role');
    }

    isAuthenticated(): boolean {
        return this.hasToken();
    }
}
