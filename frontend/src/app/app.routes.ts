import { Routes } from '@angular/router';
import { UserListComponent } from './features/admin/user-list/user-list';
import { ProviderListComponent } from './features/admin/provider-list/provider-list';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { AdminDashboardComponent } from './features/dashboard/admin-dashboard/admin-dashboard';
import { CustomerDashboardComponent } from './features/dashboard/customer-dashboard/customer-dashboard';
// Yeni eklediğimiz Provider (Hizmet Sağlayıcı) paneli
import { ProviderDashboardComponent } from './features/dashboard/provider-dashboard/provider-dashboard';
import { ProfileComponent } from './features/profile/profile';
import { AppointmentWizardComponent } from './features/appointment/wizard/wizard';


export const routes: Routes = [
    // Uygulama ilk açıldığında login'e git
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    
    // Auth Sayfaları
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },

    { path: 'admin/users', component: UserListComponent },
    { path: 'admin/providers', component: ProviderListComponent },
    
    { path: 'profile', component: ProfileComponent },
    // Dashboard (Panel) Sayfaları

    { path: 'appointment-new', component: AppointmentWizardComponent },

    { path: 'admin-dashboard', component: AdminDashboardComponent },       // Yönetici
    { path: 'customer-dashboard', component: CustomerDashboardComponent }, // Müşteri
    { path: 'provider-dashboard', component: ProviderDashboardComponent }, // Hizmet Sağlayıcı (YENİ)
    
    // Hatalı bir adres yazılırsa login'e at
    { path: '**', redirectTo: 'login' } 
];