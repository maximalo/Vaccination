import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { LoginRequest } from '../models/login-request';
import { LoginResponse } from '../models/login-response';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  email: string = 'abc@cde.com';
  password: string = 'SuperPassw0rd!';

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  onSubmit(): void {
    const credentials: LoginRequest = {
      email: this.email,
      password: this.password,
    };

    this.authService.login(credentials).subscribe(
      (response: LoginResponse) => {
        if (response.isSucceed) {
          console.log('Login successful', response);
          this.router.navigate(['/vaccination']);
        } else {
          console.error('Login failed', response.message);
        }
      },
      (error) => {
        console.error('Login error', error);
      },
    );
  }
}
