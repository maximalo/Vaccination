import { HttpBackend, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { LoginRequest } from '../models/login-request';
import { LoginResponse } from '../models/login-response';
import { RegisterRequest } from '../models/register-request';
import { RegisterResponse } from '../models/register-response';
import { jwtDecode } from 'jwt-decode';
import { JwtPayload } from '../models/jwt-payload';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private http: HttpClient,
    private localStorageService: LocalStorageService,
    private handler: HttpBackend,
  ) {
    this.http = new HttpClient(handler);
    this.refreshToken$.subscribe(() => {
      this.refreshToken();
    });
  }

  private apiUrl = environment.apiUrl;
  public refreshToken$ = new BehaviorSubject<boolean>(false);
  isLoggedIn$ = new BehaviorSubject<boolean>(false);

  getToken() {
    return localStorage.getItem('access_token');
  }

  getRefreshToken() {
    return localStorage.getItem('refresh_token');
  }

  private getDecodedToken(): JwtPayload | null {
    const token = this.getToken();
    if (token) {
      return jwtDecode<JwtPayload>(token);
    }
    return null;
  }

  getUserId(): string | null {
    const decodedToken = this.getDecodedToken();
    return decodedToken ? decodedToken.userid : null;
  }

  getUserEmail(): string | null {
    const decodedToken = this.getDecodedToken();
    return decodedToken ? decodedToken.email : null;
  }

  login(loginRequest: LoginRequest): Observable<LoginResponse> {
    const url = `${this.apiUrl}/Auth/Login`;
    return this.http.post<LoginResponse>(url, loginRequest).pipe(
      tap((response) => {

console.log(response.token);
// Handle response


        this.localStorageService.setItem('access_token', response.token);
        this.localStorageService.setItem(
          'refresh_token',
          response.refreshToken,
        );
        this.isLoggedIn$.next(true);
      }),
    );
  }

  logout(): void {
    // Handle logout logic
    this.isLoggedIn$.next(false);
    this.localStorageService.removeItem('access_token');
    this.localStorageService.removeItem('refresh_token');
  }

  refreshToken() {
    const url = `${this.apiUrl}/Auth/RefreshToken`;
    console.log(this.getToken());
    this.http
      .post(url, {
        accessToken: this.getToken(),
        refreshToken: this.getRefreshToken(),
      })
      .subscribe((res: any) => {
        this.localStorageService.setItem('access_token', res.token);
        this.localStorageService.setItem('refresh_token', res.refreshToken);
      });
  }

  register(registerRequest: RegisterRequest): Observable<RegisterResponse> {
    const url = `${this.apiUrl}/Auth/Register`;
    return this.http.post<RegisterResponse>(url, registerRequest).pipe(
      map((response: any) => {
        return {
          isSucceed: response.isSucceed,
          message: response.message,
        } as RegisterResponse;
      }),
    );
  }

  setUserToAdmin(email: string): Observable<any> {
    const url = `${this.apiUrl}/User/SetToAdmin`;
    const body = { email };
    return this.http.post(url, body).pipe(
      map((response) => {
        // Handle response
        return response;
      }),
    );
  }

  deleteAccount(email: string): Observable<any> {
    const url = `${this.apiUrl}/User/DeleteAccount`;
    const body = { email };
    return this.http.post(url, body).pipe(
      map((response) => {
        // Handle response
        return response;
      }),
    );
  }
}
// Removed incorrect jwtDecode function implementation
