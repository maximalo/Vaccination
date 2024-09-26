export interface LoginResponse {
  expiration: string;
  isSucceed: boolean;
  message: string;
  refreshToken: string;
  token: string;
}