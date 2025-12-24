import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
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
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  hidePassword = true;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;

      const credentials = {
        email: this.loginForm.get('email')?.value,
        password: this.loginForm.get('password')?.value
      };

      this.authService.login(credentials).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.snackBar.open('Giriş başarılı!', 'Tamam', { duration: 3000 });

          // Role göre yönlendirme
          switch (response.role) {
            case 'Customer':
              this.router.navigate(['/customer-dashboard']);
              break;
            case 'ServiceProvider':
              this.router.navigate(['/provider-dashboard']);
              break;
            case 'Admin':
              this.router.navigate(['/admin-dashboard']);
              break;
            default:
              this.router.navigate(['/customer-dashboard']);
          }
        },
        error: (error) => {
          this.isLoading = false;
          const message = error.error || 'E-posta veya şifre hatalı!';
          this.snackBar.open(message, 'Tamam', { duration: 5000 });
        }
      });
    }
  }
}