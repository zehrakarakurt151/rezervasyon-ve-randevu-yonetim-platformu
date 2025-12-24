import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs'; // Sekmeler için
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PhoneMaskDirective } from '../../shared/directives/phone-mask.directive';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    ReactiveFormsModule,
    PhoneMaskDirective
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent {
  infoForm: FormGroup;
  passwordForm: FormGroup;
  hidePassword = true;

  constructor(private fb: FormBuilder) {
    // Profil Bilgileri Formu
    this.infoForm = this.fb.group({
      firstName: ['Ahmet', Validators.required],
      lastName: ['Yılmaz', Validators.required],
      email: ['ahmet@mail.com', [Validators.required, Validators.email]],
      phone: ['555-123-4567']
    });

    // Şifre Değiştirme Formu
    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    });
  }

  updateInfo() {
    if (this.infoForm.valid) {
      console.log('Bilgiler güncellendi:', this.infoForm.value);
      alert('Profil bilgileriniz başarıyla güncellendi!');
    }
  }

  changePassword() {
    if (this.passwordForm.valid) {
      console.log('Şifre değişti:', this.passwordForm.value);
      alert('Şifreniz başarıyla değiştirildi!');
    }
  }
}