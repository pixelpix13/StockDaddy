export interface CompanyDto {
  id: number;
  tenantId: number;
  name: string;
  contactName: string;
  phone: string;
  email: string;
  address: string;
  gstin: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
  deletedAt?: string | null;
}

export interface CreateCompanyRequest {
  tenantId: number;
  name: string;
  contactName?: string;
  phone: string;
  email?: string;
  address?: string;
  gstin?: string;
}

export interface UpdateCompanyRequest {
  name: string;
  contactName?: string;
  phone: string;
  email?: string;
  address?: string;
  gstin?: string;
}
