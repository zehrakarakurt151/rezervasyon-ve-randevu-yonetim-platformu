import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { AppointmentService, Appointment } from '../../../services/appointment.service';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    RouterModule
  ],
  templateUrl: './customer-dashboard.html',
  styleUrl: './customer-dashboard.scss'
})
export class CustomerDashboardComponent implements OnInit {
  isSidebarOpen = true;
  isLoading = false;
  appointments: Appointment[] = [];

  constructor(
    private router: Router,
    private authService: AuthService,
    private appointmentService: AppointmentService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) { }

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.cdr.detectChanges();
    this.appointmentService.getMyAppointments().subscribe({
      next: (data: Appointment[]) => {
        this.ngZone.run(() => {
          this.appointments = data;
          this.isLoading = false;
          this.cdr.detectChanges();
        });
      },
      error: (error: any) => {
        this.ngZone.run(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        });
        console.error('Randevular yüklenirken hata:', error);
        this.snackBar.open('Randevular yüklenirken bir hata oluştu.', 'Tamam', { duration: 3000 });
      }
    });
  }

  cancelAppointment(id: number) {
    if (confirm('Bu randevuyu iptal etmek istediğinize emin misiniz?')) {
      this.appointmentService.cancelAppointment(id, 'Müşteri tarafından iptal edildi').subscribe({
        next: () => {
          this.snackBar.open('Randevu iptal edildi.', 'Tamam', { duration: 3000 });
          this.loadAppointments();
        },
        error: (error: any) => {
          const message = error.error || 'İptal işlemi başarısız oldu.';
          this.snackBar.open(message, 'Tamam', { duration: 5000 });
        }
      });
    }
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
      case 'CancelledByCustomer': return 'İptal (Siz)';
      case 'CancelledByProvider': return 'İptal (Sağlayıcı)';
      case 'NoShow': return 'Gelmedi';
      default: return status;
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric' });
  }

  formatTime(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}