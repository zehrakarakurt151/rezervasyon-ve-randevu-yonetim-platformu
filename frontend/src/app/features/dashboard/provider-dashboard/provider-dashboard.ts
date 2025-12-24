import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { AppointmentService, Appointment } from '../../../services/appointment.service';
import { ServiceService, Service, CreateServiceRequest } from '../../../services/service.service';

@Component({
  selector: 'app-provider-dashboard',
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
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule,
    MatTooltipModule,
    MatTabsModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './provider-dashboard.html',
  styleUrl: './provider-dashboard.scss'
})
export class ProviderDashboardComponent implements OnInit {
  isSidebarOpen = true;
  isLoading = false;
  isServicesLoading = false;
  showServiceForm = false;
  editingService: Service | null = null;

  // Tablo sütunları
  displayedColumns: string[] = ['time', 'client', 'service', 'status', 'action'];
  serviceColumns: string[] = ['name', 'description', 'price', 'duration', 'actions'];

  appointments: Appointment[] = [];
  services: Service[] = [];

  // Seçilen görünüm
  currentView: 'appointments' | 'services' = 'appointments';

  // Hizmet formu
  serviceForm: FormGroup;

  get pendingCount(): number {
    return this.appointments.filter(a => a.status === 'Pending').length;
  }

  get completedCount(): number {
    return this.appointments.filter(a => a.status === 'Completed').length;
  }

  constructor(
    private router: Router,
    private authService: AuthService,
    private appointmentService: AppointmentService,
    private serviceService: ServiceService,
    private snackBar: MatSnackBar,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) {
    this.serviceForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required]],
      price: [0, [Validators.required, Validators.min(0)]],
      durationMinutes: [30, [Validators.required, Validators.min(15)]],
      bufferTimeMinutes: [0, [Validators.min(0)]]
    });
  }

  ngOnInit() {
    this.loadAppointments();
    this.loadServices();
  }

  switchView(view: 'appointments' | 'services') {
    this.currentView = view;
    if (view === 'services') {
      this.loadServices();
    }
  }

  loadAppointments() {
    this.isLoading = true;
    this.cdr.detectChanges();
    this.appointmentService.getIncomingAppointments().subscribe({
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

  loadServices() {
    this.isServicesLoading = true;
    this.cdr.detectChanges();
    this.serviceService.getMyServices().subscribe({
      next: (data: Service[]) => {
        this.ngZone.run(() => {
          this.services = data;
          this.isServicesLoading = false;
          this.cdr.detectChanges();
        });
      },
      error: (error: any) => {
        this.ngZone.run(() => {
          this.isServicesLoading = false;
          this.cdr.detectChanges();
        });
        console.error('Hizmetler yüklenirken hata:', error);
        this.snackBar.open('Hizmetler yüklenirken bir hata oluştu.', 'Tamam', { duration: 3000 });
      }
    });
  }

  openServiceForm(service?: Service) {
    this.showServiceForm = true;
    if (service) {
      this.editingService = service;
      this.serviceForm.patchValue({
        name: service.name,
        description: service.description,
        price: service.price,
        durationMinutes: service.durationMinutes,
        bufferTimeMinutes: service.bufferTimeMinutes
      });
    } else {
      this.editingService = null;
      this.serviceForm.reset({
        name: '',
        description: '',
        price: 0,
        durationMinutes: 30,
        bufferTimeMinutes: 0
      });
    }
  }

  closeServiceForm() {
    this.showServiceForm = false;
    this.editingService = null;
    this.serviceForm.reset();
  }

  saveService() {
    if (this.serviceForm.valid) {
      const serviceData: CreateServiceRequest = {
        name: this.serviceForm.value.name,
        description: this.serviceForm.value.description,
        price: this.serviceForm.value.price,
        durationMinutes: this.serviceForm.value.durationMinutes,
        bufferTimeMinutes: this.serviceForm.value.bufferTimeMinutes || 0
      };

      if (this.editingService) {
        // Güncelleme
        this.serviceService.updateService(this.editingService.id, serviceData).subscribe({
          next: () => {
            this.snackBar.open('Hizmet güncellendi!', 'Tamam', { duration: 3000 });
            this.closeServiceForm();
            this.loadServices();
          },
          error: (error: any) => {
            const message = error.error || 'Hizmet güncellenirken bir hata oluştu.';
            this.snackBar.open(message, 'Tamam', { duration: 5000 });
          }
        });
      } else {
        // Yeni hizmet oluştur
        this.serviceService.createService(serviceData).subscribe({
          next: () => {
            this.snackBar.open('Hizmet başarıyla eklendi!', 'Tamam', { duration: 3000 });
            this.closeServiceForm();
            this.loadServices();
          },
          error: (error: any) => {
            const message = error.error || 'Hizmet eklenirken bir hata oluştu.';
            this.snackBar.open(message, 'Tamam', { duration: 5000 });
          }
        });
      }
    }
  }

  deleteService(service: Service) {
    if (confirm(`"${service.name}" hizmetini silmek istediğinize emin misiniz?`)) {
      this.serviceService.deleteService(service.id).subscribe({
        next: () => {
          this.snackBar.open('Hizmet silindi!', 'Tamam', { duration: 3000 });
          this.loadServices();
        },
        error: (error: any) => {
          const message = error.error || 'Hizmet silinirken bir hata oluştu.';
          this.snackBar.open(message, 'Tamam', { duration: 5000 });
        }
      });
    }
  }

  completeAppointment(id: number) {
    this.appointmentService.completeAppointment(id).subscribe({
      next: () => {
        this.snackBar.open('Randevu tamamlandı olarak işaretlendi.', 'Tamam', { duration: 3000 });
        this.loadAppointments();
      },
      error: (error: any) => {
        const message = error.error || 'İşlem başarısız oldu.';
        this.snackBar.open(message, 'Tamam', { duration: 5000 });
      }
    });
  }

  cancelAppointment(id: number) {
    if (confirm('Bu randevuyu iptal etmek istediğinize emin misiniz?')) {
      this.appointmentService.cancelAppointment(id, 'Hizmet sağlayıcı tarafından iptal edildi').subscribe({
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
      case 'CancelledByCustomer': return 'İptal (Müşteri)';
      case 'CancelledByProvider': return 'İptal (Siz)';
      case 'NoShow': return 'Gelmedi';
      default: return status;
    }
  }

  formatTime(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric' });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}