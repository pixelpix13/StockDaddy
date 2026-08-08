/**
 * Shared Axios instance for all API services.
 * - Attaches JWT from localStorage on every request.
 * - On 401, clears session and redirects to `/login`.
 * - On 403, dispatches a global event for toast display (permission denied).
 */
import axios from 'axios';
import { getStoredActiveStoreId, clearStoredActiveStoreId } from '@/lib/store';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Attach JWT Bearer Token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('stockdaddy_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    const activeStoreId = getStoredActiveStoreId();
    if (activeStoreId) {
      config.headers['X-Store-Id'] = String(activeStoreId);
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor: Handle 401 Unauthorized globally
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 403) {
      const message =
        typeof error.response.data?.message === 'string'
          ? error.response.data.message
          : 'You do not have permission for this action.';
      window.dispatchEvent(new CustomEvent('stockdaddy:forbidden', { detail: message }));
    }
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('stockdaddy_token');
      localStorage.removeItem('stockdaddy_user');
      clearStoredActiveStoreId();
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);
