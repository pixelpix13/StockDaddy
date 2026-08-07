/**
 * Global auth state: JWT token + current user profile with RBAC permissions.
 * On mount, re-validates stored token via `GET /api/auth/me`.
 */
import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { UserDto, LoginRequest, RegisterRequest } from '../dtos';
import { PermissionAction, permissionKey } from '../dtos/rbac.dto';
import { authService } from '../services';

interface AuthContextType {
  user: UserDto | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => void;
  refreshSession: () => Promise<void>;
  hasPermission: (module: string, action?: PermissionAction) => boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<UserDto | null>(() => authService.getStoredUser());
  const [token, setToken] = useState<string | null>(() => authService.getToken());
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    const initAuth = async () => {
      const storedToken = authService.getToken();
      if (storedToken) {
        try {
          const currentUser = await authService.getCurrentUser();
          setUser(currentUser);
          setToken(storedToken);
        } catch (error) {
          console.error('Failed to validate session:', error);
          authService.logout();
          setUser(null);
          setToken(null);
        }
      }
      setIsLoading(false);
    };

    initAuth();
  }, []);

  const login = async (request: LoginRequest) => {
    const response = await authService.login(request);
    setToken(response.token);
    setUser(response.user);
  };

  const register = async (request: RegisterRequest) => {
    const response = await authService.register(request);
    setToken(response.token);
    setUser(response.user);
  };

  const logout = () => {
    authService.logout();
    setUser(null);
    setToken(null);
  };

  const refreshSession = useCallback(async () => {
    const currentUser = await authService.getCurrentUser();
    setUser(currentUser);
  }, []);

  const hasPermission = useCallback(
    (module: string, action: PermissionAction = 'Read') => {
      const key = permissionKey(module, action).toLowerCase();
      return (user?.permissions ?? []).some((p) => p.toLowerCase() === key);
    },
    [user?.permissions]
  );

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token,
        isLoading,
        login,
        register,
        logout,
        refreshSession,
        hasPermission,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
