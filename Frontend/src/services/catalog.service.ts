/** Catalog taxonomy CRUD: categories, subcategories, HSN master, tax regions. */
import { apiClient } from './api.client';
import { fetchAllItems, fetchPaged } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
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
  getCategoriesPaged(query: PagedQuery): Promise<PagedResult<CategoryDto>> {
    return fetchPaged<CategoryDto>('/category', query);
  },

  async getCategories(): Promise<CategoryDto[]> {
    return fetchAllItems<CategoryDto>('/category');
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

  getSubcategoriesPaged(query: PagedQuery): Promise<PagedResult<SubcategoryDto>> {
    return fetchPaged<SubcategoryDto>('/subcategory', query);
  },

  async getSubcategories(): Promise<SubcategoryDto[]> {
    return fetchAllItems<SubcategoryDto>('/subcategory');
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

  getHsnCodesPaged(query: PagedQuery): Promise<PagedResult<HsnMasterDto>> {
    return fetchPaged<HsnMasterDto>('/hsnmaster', query);
  },

  async getHsnCodes(): Promise<HsnMasterDto[]> {
    return fetchAllItems<HsnMasterDto>('/hsnmaster');
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

  getTaxRegionsPaged(query: PagedQuery): Promise<PagedResult<TaxRegionDto>> {
    return fetchPaged<TaxRegionDto>('/taxregion', query);
  },

  async getTaxRegions(): Promise<TaxRegionDto[]> {
    return fetchAllItems<TaxRegionDto>('/taxregion');
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
