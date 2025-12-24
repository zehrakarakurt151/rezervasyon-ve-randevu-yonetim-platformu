import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatRadioModule } from '@angular/material/radio';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { PhoneMaskDirective } from '../../../shared/directives/phone-mask.directive';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatRadioModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    PhoneMaskDirective
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  hidePassword = true;
  isLoading = false;
  userType: 'customer' | 'provider' = 'customer';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      // Provider için ek alanlar
      businessName: [''],
      address: ['']
    });
  }

  onUserTypeChange() {
    if (this.userType === 'provider') {
      this.registerForm.get('businessName')?.setValidators([Validators.required]);
      this.registerForm.get('address')?.setValidators([Validators.required]);
    } else {
      this.registerForm.get('businessName')?.clearValidators();
      this.registerForm.get('address')?.clearValidators();
    }
    this.registerForm.get('businessName')?.updateValueAndValidity();
    this.registerForm.get('address')?.updateValueAndValidity();
  }

  onSubmit() {
    if (this.registerForm.valid) {
      // Şifre eşleşme kontrolü
      if (this.registerForm.value.password !== this.registerForm.value.confirmPassword) {
        this.snackBar.open('Şifreler eşleşmiyor!', 'Tamam', { duration: 3000 });
        return;
      }

      this.isLoading = true;

      if (this.userType === 'customer') {
        const customerData = {
          firstName: this.registerForm.value.firstName,
          lastName: this.registerForm.value.lastName,
          email: this.registerForm.value.email,
          phoneNumber: this.registerForm.value.phoneNumber,
          password: this.registerForm.value.password
        };

        this.authService.registerCustomer(customerData).subscribe({
          next: () => {
            this.isLoading = false;
            this.snackBar.open('Kayıt başarılı! Giriş yapabilirsiniz.', 'Tamam', { duration: 3000 });
            this.router.navigate(['/login']);
          },
          error: (error) => {
            this.isLoading = false;
            const message = error.error || 'Kayıt sırasında bir hata oluştu!';
            this.snackBar.open(message, 'Tamam', { duration: 5000 });
          }
        });
      } else {
        const providerData = {
          firstName: this.registerForm.value.firstName,
          lastName: this.registerForm.value.lastName,
          email: this.registerForm.value.email,
          phoneNumber: this.registerForm.value.phoneNumber,
          password: this.registerForm.value.password,
          businessName: this.registerForm.value.businessName,
          address: this.registerForm.value.address
        };

        this.authService.registerProvider(providerData).subscribe({
          next: () => {
            this.isLoading = false;
            this.snackBar.open('Hizmet sağlayıcı kaydı başarılı!', 'Tamam', { duration: 3000 });
            this.router.navigate(['/login']);
          },
          error: (error) => {
            this.isLoading = false;
            const message = error.error || 'Kayıt sırasında bir hata oluştu!';
            this.snackBar.open(message, 'Tamam', { duration: 5000 });
          }
        });
      }
    }
  }
}