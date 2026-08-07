export interface TenantDto {
  id: number;
  name: string;
  code: string;
  createdAt: string;
}

export interface StoreDto {
  id: number;
  tenantId: number;
  name: string;
  location: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateStoreRequest {
  tenantId: number;
  name: string;
  location: string;
}

export interface UpdateStoreRequest {
  name: string;
  location: string;
}

export interface RoleDto {
  id: number;
  name: string;
  description?: string;
}
