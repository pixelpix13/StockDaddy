export interface PermissionSummaryDto {
  id: number;
  module: string;
  action: string;
  key: string;
}

export interface RoleWithPermissionsDto {
  id: number;
  name: string;
  permissionIds: number[];
}

export interface RbacMatrixDto {
  permissions: PermissionSummaryDto[];
  roles: RoleWithPermissionsDto[];
}

export interface UpdateRolePermissionsRequest {
  permissionIds: number[];
}

export interface AssignUserRoleRequest {
  roleId: number;
}

export interface CreateRoleRequest {
  name: string;
}

export interface UpdateRoleRequest {
  name: string;
}

export type PermissionAction = 'Read' | 'Write' | 'Update' | 'Delete' | 'AccessAllStores';

export function permissionKey(module: string, action: PermissionAction): string {
  return `${module}:${action}`;
}
