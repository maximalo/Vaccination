import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../services/auth.service';
import { LoginRequest } from '../models/login-request';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    MatCardModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  registerForm: FormGroup;
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {
    this.registerForm = this.fb.group(
      {
        email: ['', [Validators.required, Validators.email]],
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', Validators.required],
      },
      { validator: this.passwordMatchValidator },
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null
      : { mismatch: true };
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe(
        (response) => {
          if (response.isSucceed) {
            console.log('Registration successful', response);
            // Créer une instance de LoginRequest
            const loginRequest: LoginRequest = {
              email: this.registerForm.value.email,
              password: this.registerForm.value.password,
            };
            // Appeler la méthode de connexion après une inscription réussie
            this.authService.login(loginRequest).subscribe(
              (loginResponse) => {
                console.log('Login successful', loginResponse);
                this.router.navigate(['/vaccination']);
                // Rediriger l'utilisateur ou effectuer d'autres actions après la connexion
              },
              (loginError) => {
                console.error('Login failed', loginError);
              },
            );
          }
        },
        (error) => {
          console.error('Registration failed', error);
        },
      );
    }
  }
}
