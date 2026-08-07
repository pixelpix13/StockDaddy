/** RBAC matrix, roles, and user role assignment API. */
import { apiClient } from './api.client';
import {
  CreateRoleRequest,
  RbacMatrixDto,
  RoleWithPermissionsDto,
  UpdateRolePermissionsRequest,
  UpdateRoleRequest,
} from '../dtos';
import { UserDto } from '../dtos/auth.dto';

export const rbacService = {
  async getMatrix(): Promise<RbacMatrixDto> {
    const response = await apiClient.get<RbacMatrixDto>('/rbac/matrix');
    return response.data;
  },

  async getRoles(): Promise<RoleWithPermissionsDto[]> {
    const response = await apiClient.get<RoleWithPermissionsDto[]>('/rbac/roles');
    return response.data;
  },

  async createRole(request: CreateRoleRequest): Promise<RoleWithPermissionsDto> {
    const response = await apiClient.post<RoleWithPermissionsDto>('/rbac/roles', request);
    return response.data;
  },

  async updateRole(roleId: number, request: UpdateRoleRequest): Promise<RoleWithPermissionsDto> {
    const response = await apiClient.put<RoleWithPermissionsDto>(`/rbac/roles/${roleId}`, request);
    return response.data;
  },

  async deleteRole(roleId: number): Promise<void> {
    await apiClient.delete(`/rbac/roles/${roleId}`);
  },

  async updateRolePermissions(roleId: number, request: UpdateRolePermissionsRequest): Promise<RoleWithPermissionsDto> {
    const response = await apiClient.put<RoleWithPermissionsDto>(`/rbac/roles/${roleId}/permissions`, request);
    return response.data;
  },

  async assignUserRole(userId: number, request: { roleId: number }): Promise<UserDto> {
    const response = await apiClient.put<UserDto>(`/rbac/users/${userId}/role`, request);
    return response.data;
  },
};
