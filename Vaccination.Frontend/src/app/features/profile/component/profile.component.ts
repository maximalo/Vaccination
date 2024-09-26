import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { AuthService } from '../../auth/services/auth.service';
import { ProfileUserUpdateRequest } from '../models/profile-user-update-request';
import { ProfileService } from '../services/profile.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatDialogModule,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit {
  profileForm!: FormGroup;

  private fb = inject(FormBuilder);
  private dialog = inject(MatDialog);
  private profileService = inject(ProfileService);
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit() {
    this.profileForm = this.fb.group({
      email: [{ value: '', disabled: true }],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      socialSecurityNumber: [''],
      dateOfBirth: [''],
      city: [''],
      nationality: [''],
      address: [''],
      postalCode: [''],
      phoneNumber: [''],
    });

    this.loadProfileData();
  }

  loadProfileData() {
    this.profileService.getUserProfile().subscribe((profile) => {
      this.profileForm.patchValue(profile);
    });
  }

  saveProfile() {
    if (this.profileForm.valid) {
      // Enable field temporarily to update the profile and disable it again
      this.profileForm.get('email')?.enable();
      const profile: ProfileUserUpdateRequest = {
        email: this.profileForm.value.email,
        firstName: this.profileForm.value.firstName,
        lastName: this.profileForm.value.lastName,
        socialSecurityNumber: this.profileForm.value.socialSecurityNumber,
        dateOfBirth: this.profileForm.value.dateOfBirth,
        city: this.profileForm.value.city,
        address: this.profileForm.value.address,
        postalCode: this.profileForm.value.postalCode,
        phoneNumber: this.profileForm.value.phoneNumber,
        nationality: this.profileForm.value.nationality,
      };
      this.profileForm.get('email')?.disable();
      this.profileService.updateUserProfile(profile).subscribe(() => {
        console.log('Profil mis à jour');
      });
    }
  }

  confirmDeleteAccount() {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Supprimer le compte',
        message:
          'Êtes-vous sûr de vouloir supprimer votre compte ? Cette action est irréversible.',
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.deleteAccount();
      }
    });
  }

  deleteAccount() {
    this.profileService.deleteUserProfile().subscribe(() => {
      this.authService.logout();
      this.router.navigate(['/login']);
    });
  }
}
