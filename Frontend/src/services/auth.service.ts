/** Login, register, session storage, and current-user lookup. */
import { apiClient } from './api.client';
import { LoginRequest, RegisterRequest, AuthResponse, UserDto, UserStoreOptionDto } from '../dtos';
import { clearStoredActiveStoreId } from '@/lib/store';

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

  async refreshSession(): Promise<AuthResponse> {
    const response = await apiClient.get<AuthResponse>('/auth/me');
    if (response.data.token) {
      this.saveAuth(response.data);
    }
    return response.data;
  },

  async getMyStores(): Promise<UserStoreOptionDto[]> {
    const response = await apiClient.get<UserStoreOptionDto[]>('/auth/me/stores');
    return response.data;
  },

  async switchStore(storeId: number): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/auth/switch-store', { storeId });
    if (response.data.token) {
      this.saveAuth(response.data);
    }
    return response.data;
  },

  /** @deprecated Use refreshSession — /auth/me now returns a fresh JWT with current permissions. */
  async getCurrentUser(): Promise<UserDto> {
    const session = await this.refreshSession();
    return session.user;
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
    clearStoredActiveStoreId();
  },
};
