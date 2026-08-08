export interface UserStoreAssignmentDto {
  storeId: number;
  roleId: number;
  isDefault: boolean;
  storeName?: string;
  roleName?: string;
}

export interface UserManagementDto {
  id: number;
  tenantId: number;
  roleId: number;
  storeId?: number;
  storeIds?: number[];
  defaultStoreId?: number;
  storeAssignments?: UserStoreAssignmentDto[];
  username: string;
  email: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
}

export interface UpdateUserManagementRequest {
  roleId: number;
  storeId?: number;
  storeIds?: number[];
  defaultStoreId?: number;
  storeAssignments?: UserStoreAssignmentDto[];
  username: string;
  email: string;
  passwordHash?: string;
}

export interface CreateUserManagementRequest {
  tenantId: number;
  roleId: number;
  storeId?: number;
  storeIds?: number[];
  defaultStoreId?: number;
  storeAssignments?: UserStoreAssignmentDto[];
  username: string;
  email: string;
  passwordHash: string;
}

export interface AssignUserStoreAssignmentsRequest {
  defaultRoleId?: number;
  defaultStoreId?: number;
  assignments: UserStoreAssignmentDto[];
}
