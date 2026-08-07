/** User management CRUD for admin screens. */
import { apiClient } from './api.client';
import { UserManagementDto, CreateUserManagementRequest, UpdateUserManagementRequest } from '../dtos';

export const userService = {
  async getUsers(): Promise<UserManagementDto[]> {
    const response = await apiClient.get<UserManagementDto[]>('/user');
    return response.data;
  },

  async getUserById(id: number): Promise<UserManagementDto> {
    const response = await apiClient.get<UserManagementDto>(`/user/${id}`);
    return response.data;
  },

  async createUser(request: CreateUserManagementRequest): Promise<void> {
    await apiClient.post('/user', request);
  },

  async updateUser(id: number, request: UpdateUserManagementRequest): Promise<void> {
    await apiClient.put(`/user/${id}`, request);
  },

  async deleteUser(id: number): Promise<void> {
    await apiClient.delete(`/user/${id}`);
  },
};
