export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterRequest {
  tenantId: number;
  roleId: number;
  storeId?: number;
  username: string;
  email: string;
  password: string;
}

export interface UserDto {
  id: number;
  tenantId: number;
  roleId: number;
  storeId?: number;
  username: string;
  email: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
  deletedAt?: string;
}

export interface AuthResponse {
  token: string;
  expiration: string;
  user: UserDto;
}
