import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ProfileUserResponse } from '../models/profile-user-response';
import { ProfileUserUpdateRequest } from '../models/profile-user-update-request';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private apiUrl = environment.apiUrl;

  private http = inject(HttpClient);

  getUserProfile(): Observable<ProfileUserResponse> {
    const url = `${this.apiUrl}/User/GetUser`;
    return this.http.get(url).pipe(
      map((response: any) => {
        if (response.success) {
          const data = response.data;
          const profile: ProfileUserResponse = {
            id: data.id || '',
            email: data.email || '',
            firstName: data.firstName || '',
            lastName: data.lastName || '',
            socialSecurityNumber: data.socialSecurityNumber || '',
            dateOfBirth: data.dateOfBirth || '',
            city: data.city || '',
            address: data.address || '',
            postalCode: data.postalCode || '',
            phoneNumber: data.phoneNumber || '',
            nationality: data.nationality || '',
          };
          return profile;
        } else {
          throw new Error(response.message || 'Failed to fetch user details');
        }
      }),
      catchError((error) => {
        console.error('Get user profile failed', error);
        const defaultProfile: ProfileUserResponse = {
          id: '',
          email: '',
          firstName: '',
          lastName: '',
          socialSecurityNumber: '',
          dateOfBirth: '',
          city: '',
          address: '',
          postalCode: '',
          phoneNumber: '',
          nationality: '',
        };
        return of(defaultProfile);
      }),
    );
  }

  updateUserProfile(profile: ProfileUserUpdateRequest): Observable<boolean> {
    const url = `${this.apiUrl}/User/Update`;
    return this.http.put(url, profile).pipe(
      map((response: any) => {
        if (response.success) {
          return true;
        } else {
          throw new Error(response.message || 'Failed to update user details');
        }
      }),
      catchError((error) => {
        console.error('Update user profile failed', error);
        return of(false);
      }),
    );
  }

  deleteUserProfile(): Observable<boolean> {
    const url = `${this.apiUrl}/User/Delete`;
    return this.http.delete(url).pipe(
      map((response: any) => {
        if (response.success) {
          return true;
        } else {
          throw new Error(response.message || 'Failed to delete user details');
        }
      }),
      catchError((error) => {
        console.error('Delete user profile failed', error);
        return of(false);
      }),
    );
  }
}
