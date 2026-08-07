export interface CustomerDto {
  id: number;
  tenantId: number;
  name: string;
  phone: string;
  email: string;
  address: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
}

export interface CreateCustomerRequest {
  tenantId: number;
  name: string;
  phone: string;
  email: string;
  address: string;
}

export interface UpdateCustomerRequest {
  name: string;
  phone: string;
  email: string;
  address: string;
}
