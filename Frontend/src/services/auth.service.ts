import { apiClient } from './api.client';
import { LoginRequest, RegisterRequest, AuthResponse, UserDto } from '../dtos';

export const authService = {
  async login(request: LoginRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/auth/login', request);
    if (response.data.token) {
      this.saveAuth(response.data);
    }
    return response.data;
  },

  async register(request: RegisterRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/auth/register', request);
    if (response.data.token) {
      this.saveAuth(response.data);
    }
    return response.data;
  },

  async getCurrentUser(): Promise<UserDto> {
    const response = await apiClient.get<UserDto>('/auth/me');
    return response.data;
  },

  saveAuth(authData: AuthResponse): void {
    localStorage.setItem('stockdaddy_token', authData.token);
    localStorage.setItem('stockdaddy_user', JSON.stringify(authData.user));
  },

  getToken(): string | null {
    return localStorage.getItem('stockdaddy_token');
  },

  getStoredUser(): UserDto | null {
    const user = localStorage.getItem('stockdaddy_user');
    return user ? JSON.parse(user) : null;
  },

  logout(): void {
    localStorage.removeItem('stockdaddy_token');
    localStorage.removeItem('stockdaddy_user');
  },
};
