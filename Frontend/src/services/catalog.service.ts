import { apiClient } from './api.client';
import {
  CategoryDto,
  CreateCategoryRequest,
  SubcategoryDto,
  UpdateCategoryRequest,
  UpdateSubcategoryRequest,
  HsnMasterDto,
  CreateHsnMasterRequest,
  UpdateHsnMasterRequest,
  TaxRegionDto,
  CreateTaxRegionRequest,
  UpdateTaxRegionRequest,
} from '../dtos';

export interface CreateSubcategoryRequest {
  tenantId: number;
  storeId: number;
  categoryId: number;
  name: string;
}

export const catalogService = {
  async getCategories(): Promise<CategoryDto[]> {
    const response = await apiClient.get<CategoryDto[]>('/category');
    return response.data;
  },

  async getCategoryById(id: number): Promise<CategoryDto> {
    const response = await apiClient.get<CategoryDto>(`/category/${id}`);
    return response.data;
  },

  async createCategory(request: CreateCategoryRequest): Promise<void> {
    await apiClient.post('/category', request);
  },

  async updateCategory(id: number, request: UpdateCategoryRequest): Promise<void> {
    await apiClient.put(`/category/${id}`, request);
  },

  async deleteCategory(id: number): Promise<void> {
    await apiClient.delete(`/category/${id}`);
  },

  async getSubcategories(): Promise<SubcategoryDto[]> {
    const response = await apiClient.get<SubcategoryDto[]>('/subcategory');
    return response.data;
  },

  async getSubcategoryById(id: number): Promise<SubcategoryDto> {
    const response = await apiClient.get<SubcategoryDto>(`/subcategory/${id}`);
    return response.data;
  },

  async createSubcategory(request: CreateSubcategoryRequest): Promise<void> {
    await apiClient.post('/subcategory', request);
  },

  async updateSubcategory(id: number, request: UpdateSubcategoryRequest): Promise<void> {
    await apiClient.put(`/subcategory/${id}`, request);
  },

  async deleteSubcategory(id: number): Promise<void> {
    await apiClient.delete(`/subcategory/${id}`);
  },

  async getHsnCodes(): Promise<HsnMasterDto[]> {
    const response = await apiClient.get<HsnMasterDto[]>('/hsnmaster');
    return response.data;
  },

  async createHsnCode(request: CreateHsnMasterRequest): Promise<void> {
    await apiClient.post('/hsnmaster', request);
  },

  async updateHsnCode(id: number, request: UpdateHsnMasterRequest): Promise<void> {
    await apiClient.put(`/hsnmaster/${id}`, request);
  },

  async deleteHsnCode(id: number): Promise<void> {
    await apiClient.delete(`/hsnmaster/${id}`);
  },

  async getTaxRegions(): Promise<TaxRegionDto[]> {
    const response = await apiClient.get<TaxRegionDto[]>('/taxregion');
    return response.data;
  },

  async createTaxRegion(request: CreateTaxRegionRequest): Promise<void> {
    await apiClient.post('/taxregion', request);
  },

  async updateTaxRegion(id: number, request: UpdateTaxRegionRequest): Promise<void> {
    await apiClient.put(`/taxregion/${id}`, request);
  },

  async deleteTaxRegion(id: number): Promise<void> {
    await apiClient.delete(`/taxregion/${id}`);
  },
};
