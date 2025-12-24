import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { Router, RouterModule } from '@angular/router';
import { AdminService, DashboardStats, User, AdminAppointment } from '../../../services/admin.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatTableModule,
    MatTabsModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    RouterModule
  ],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboardComponent implements OnInit {
  isSidebarOpen = true;

  // Stats
  stats: DashboardStats | null = null;
  isLoadingStats = false;

  // Users
  users: User[] = [];
  isLoadingUsers = false;
  userColumns = ['id', 'name', 'email', 'phone', 'role', 'createdDate'];

  // Appointments
  appointments: AdminAppointment[] = [];
  isLoadingAppointments = false;
  appointmentColumns = ['id', 'customer', 'provider', 'service', 'date', 'price', 'status'];

  constructor(
    private router: Router,
    private adminService: AdminService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loadStats();
    this.loadUsers();
    this.loadAppointments();
  }

  loadStats() {
    this.isLoadingStats = true;
    this.adminService.getStats().subscribe({
      next: (data: DashboardStats) => {
        this.stats = data;
        this.isLoadingStats = false;
      },
      error: (error: any) => {
        console.error('Stats yüklenirken hata:', error);
        this.isLoadingStats = false;
      }
    });
  }

  loadUsers() {
    this.isLoadingUsers = true;
    this.adminService.getAllUsers().subscribe({
      next: (data: User[]) => {
        this.users = data;
        this.isLoadingUsers = false;
      },
      error: (error: any) => {
        console.error('Kullanıcılar yüklenirken hata:', error);
        this.isLoadingUsers = false;
      }
    });
  }

  loadAppointments() {
    this.isLoadingAppointments = true;
    this.adminService.getAllAppointments().subscribe({
      next: (data: AdminAppointment[]) => {
        this.appointments = data;
        this.isLoadingAppointments = false;
      },
      error: (error: any) => {
        console.error('Randevular yüklenirken hata:', error);
        this.isLoadingAppointments = false;
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'accent';
      case 'Confirmed': return 'primary';
      case 'Completed': return 'primary';
      case 'CancelledByCustomer':
      case 'CancelledByProvider': return 'warn';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'Bekliyor';
      case 'Confirmed': return 'Onaylandı';
      case 'Completed': return 'Tamamlandı';
      case 'CancelledByCustomer': return 'Müşteri İptal';
      case 'CancelledByProvider': return 'Sağlayıcı İptal';
      default: return status;
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}