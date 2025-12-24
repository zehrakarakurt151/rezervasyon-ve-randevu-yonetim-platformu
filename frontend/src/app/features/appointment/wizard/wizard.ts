import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { ProviderService, Provider, ServiceItem } from '../../../services/provider.service';
import { AppointmentService } from '../../../services/appointment.service';

@Component({
  selector: 'app-appointment-wizard',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatStepperModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatRadioModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './wizard.html',
  styleUrl: './wizard.scss'
})
export class AppointmentWizardComponent implements OnInit {
  // Form Grupları
  providerFormGroup: FormGroup;
  serviceFormGroup: FormGroup;
  dateFormGroup: FormGroup;

  // API'den Gelen Veriler
  providers: Provider[] = [];
  services: ServiceItem[] = [];
  availableHours: string[] = []; // Dinamik olarak API'den gelecek

  // Yükleniyor durumları
  isLoadingProviders = false;
  isLoadingServices = false;
  isLoadingSlots = false;
  isSubmitting = false;

  // Seçilen değerler
  selectedProvider: Provider | null = null;
  selectedService: ServiceItem | null = null;

  // Minimum tarih (bugün)
  minDate = new Date();

  constructor(
    private _formBuilder: FormBuilder,
    private providerService: ProviderService,
    private appointmentService: AppointmentService,
    private snackBar: MatSnackBar,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.providerFormGroup = this._formBuilder.group({
      provider: ['', Validators.required],
    });

    this.serviceFormGroup = this._formBuilder.group({
      service: ['', Validators.required],
    });

    this.dateFormGroup = this._formBuilder.group({
      date: ['', Validators.required],
      time: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadProviders();

    // Tarih değişikliğini dinle
    this.dateFormGroup.get('date')?.valueChanges.subscribe((date: Date) => {
      if (date && this.providerFormGroup.value.provider && this.serviceFormGroup.value.service) {
        this.loadAvailableSlots(date);
      }
    });
  }

  loadProviders() {
    this.isLoadingProviders = true;
    this.providerService.getAllProviders().subscribe({
      next: (data: Provider[]) => {
        this.providers = data;
        this.isLoadingProviders = false;
        this.cdr.detectChanges();

        if (data.length === 0) {
          this.snackBar.open('Henüz kayıtlı hizmet sağlayıcı bulunmuyor.', 'Tamam', { duration: 5000 });
        }
      },
      error: (error: any) => {
        this.isLoadingProviders = false;
        console.error('Sağlayıcılar yüklenirken hata:', error);
        this.snackBar.open('Sağlayıcılar yüklenirken bir hata oluştu.', 'Tamam', { duration: 3000 });
      }
    });
  }

  onProviderChange(providerId: number) {
    this.selectedProvider = this.providers.find(p => p.id === providerId) || null;
    this.loadProviderServices(providerId);
  }

  loadProviderServices(providerId: number) {
    this.isLoadingServices = true;
    this.services = [];
    this.serviceFormGroup.reset();

    this.providerService.getProviderServices(providerId).subscribe({
      next: (data: ServiceItem[]) => {
        this.services = data;
        this.isLoadingServices = false;
        this.cdr.detectChanges();

        if (data.length === 0) {
          this.snackBar.open('Bu sağlayıcının henüz hizmeti bulunmuyor.', 'Tamam', { duration: 3000 });
        }
      },
      error: (error: any) => {
        this.isLoadingServices = false;
        console.error('Hizmetler yüklenirken hata:', error);
      }
    });
  }

  onServiceChange(serviceId: number) {
    this.selectedService = this.services.find(s => s.id === serviceId) || null;
    // Hizmet değiştiğinde seçili tarih varsa saatleri yenile
    if (this.dateFormGroup.value.date) {
      this.loadAvailableSlots(this.dateFormGroup.value.date);
    }
  }

  loadAvailableSlots(date: Date) {
    const providerId = this.providerFormGroup.value.provider;
    const serviceId = this.serviceFormGroup.value.service;

    if (!providerId || !serviceId) {
      return;
    }

    this.isLoadingSlots = true;
    this.availableHours = [];
    this.dateFormGroup.patchValue({ time: '' }); // Önceki saat seçimini temizle

    this.providerService.getAvailableSlots(providerId, serviceId, date).subscribe({
      next: (slots: string[]) => {
        this.availableHours = slots;
        this.isLoadingSlots = false;
        this.cdr.detectChanges();

        if (slots.length === 0) {
          this.snackBar.open('Bu tarihte müsait saat bulunmuyor.', 'Tamam', { duration: 3000 });
        }
      },
      error: (error: any) => {
        this.isLoadingSlots = false;
        // API hatası durumunda varsayılan saatleri göster
        this.availableHours = ['09:00', '10:00', '11:00', '13:00', '14:00', '15:00', '16:00', '17:00'];
        console.error('Müsait saatler yüklenirken hata:', error);
      }
    });
  }

  selectTime(time: string) {
    this.dateFormGroup.patchValue({ time: time });
  }

  finishAppointment() {
    if (this.providerFormGroup.valid && this.serviceFormGroup.valid && this.dateFormGroup.valid) {
      this.isSubmitting = true;

      const selectedDate = this.dateFormGroup.value.date as Date;
      const selectedTime = this.dateFormGroup.value.time as string;

      const appointmentData = {
        serviceProviderId: this.providerFormGroup.value.provider,
        serviceId: this.serviceFormGroup.value.service,
        date: selectedDate.toISOString(),
        time: selectedTime
      };

      this.appointmentService.createAppointment(appointmentData).subscribe({
        next: (response: any) => {
          this.isSubmitting = false;
          this.snackBar.open('Randevunuz başarıyla oluşturuldu!', 'Tamam', { duration: 5000 });
          this.router.navigate(['/customer-dashboard']);
        },
        error: (error: any) => {
          this.isSubmitting = false;
          const message = error.error || 'Randevu oluşturulamadı. Lütfen tekrar deneyin.';
          this.snackBar.open(message, 'Tamam', { duration: 5000 });
        }
      });
    } else {
      this.snackBar.open('Lütfen tüm alanları doldurunuz.', 'Tamam', { duration: 3000 });
    }
  }
}