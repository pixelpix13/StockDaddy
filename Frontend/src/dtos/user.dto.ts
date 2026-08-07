export interface UserManagementDto {
  id: number;
  tenantId: number;
  roleId: number;
  storeId?: number;
  username: string;
  email: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
}

export interface UpdateUserManagementRequest {
  roleId: number;
  storeId?: number;
  username: string;
  email: string;
  passwordHash?: string;
}

export interface CreateUserManagementRequest {
  tenantId: number;
  roleId: number;
  storeId?: number;
  username: string;
  email: string;
  passwordHash: string;
}
