import type { UserStoreAssignmentDto } from './user.dto';

export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterRequest {
  tenantId: number;
  roleId: number;
  storeId?: number;
  storeIds?: number[];
  defaultStoreId?: number;
  username: string;
  email: string;
  password: string;
}

export interface UserDto {
  id: number;
  tenantId: number;
  roleId: number;
  roleName?: string;
  storeId?: number;
  defaultStoreId?: number;
  storeIds?: number[];
  assignedStores?: UserStoreOptionDto[];
  storeAssignments?: UserStoreAssignmentDto[];
  username: string;
  email: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
  deletedAt?: string;
  permissions?: string[];
}

export interface AuthResponse {
  token: string;
  expiration: string;
  user: UserDto;
}

export interface UserStoreOptionDto {
  id: number;
  name: string;
  location: string;
  roleId: number;
  roleName: string;
  isDefault: boolean;
  isActive: boolean;
}

export type { UserStoreAssignmentDto };
