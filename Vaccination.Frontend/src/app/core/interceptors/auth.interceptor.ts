import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const accessToken = authService.getToken();

  const auth = req.clone({
    headers: req.headers
      .set('Authorization', `Bearer ${accessToken}`)
      .set('Content-Type', 'application/json'),
  });

  return next(auth).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        authService.refreshToken$.next(true);
      }
      return throwError(error);
    })
  );
};
