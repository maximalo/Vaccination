import { inject } from '@angular/core';
import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { AuthService } from './features/auth/services/auth.service';
import { RegisterComponent } from './features/auth/register/register.component';

export const routes: Routes = [
  {
    path: 'home',
    loadComponent: () =>
      import('./features/home/component/home.component').then(
        (m) => m.HomeComponent,
      ),
  },
  {
    path: 'vaccination',
    loadComponent: () =>
      import('./features/vaccination/component/vaccination.component').then(
        (m) => m.VaccinationComponent,
      ),
    canActivate: [() => inject(AuthService).isLoggedIn$],
  },
  {
    path: 'profile',
    loadComponent: () =>
      import('./features/profile/component/profile.component').then(
        (m) => m.ProfileComponent,
      ),
    canActivate: [() => inject(AuthService).isLoggedIn$],
  },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
];
