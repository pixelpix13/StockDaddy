/**
 * Product and variant CRUD against `/api/product` and `/api/productvariant`.
 * For catalog taxonomy (categories, HSN, tax), use `catalogService` instead.
 * For product+variant+stock in one call, use `orchestrationService`.
 */
import { apiClient } from './api.client';
import {
  ProductDto,
  ProductVariantDto,
  CreateProductRequest,
  CreateProductVariantRequest,
  UpdateProductRequest,
  UpdateProductVariantRequest,
} from '../dtos';

export const productService = {
  async getProducts(): Promise<ProductDto[]> {
    const response = await apiClient.get<ProductDto[]>('/product');
    return response.data;
  },

  async getProductById(id: number): Promise<ProductDto> {
    const response = await apiClient.get<ProductDto>(`/product/${id}`);
    return response.data;
  },

  async createProduct(request: CreateProductRequest): Promise<number> {
    const response = await apiClient.post<{ id: number }>('/product', request);
    return response.data.id;
  },

  async updateProduct(id: number, request: UpdateProductRequest): Promise<void> {
    await apiClient.put(`/product/${id}`, request);
  },

  async deleteProduct(id: number): Promise<void> {
    await apiClient.delete(`/product/${id}`);
  },

  async getProductVariants(): Promise<ProductVariantDto[]> {
    const response = await apiClient.get<ProductVariantDto[]>('/productvariant');
    return response.data;
  },

  async getProductVariantById(id: number): Promise<ProductVariantDto> {
    const response = await apiClient.get<ProductVariantDto>(`/productvariant/${id}`);
    return response.data;
  },

  async createProductVariant(request: CreateProductVariantRequest): Promise<number> {
    const response = await apiClient.post<{ id: number }>('/productvariant', request);
    return response.data.id;
  },

  async updateProductVariant(id: number, request: UpdateProductVariantRequest): Promise<void> {
    await apiClient.put(`/productvariant/${id}`, request);
  },

  async deleteProductVariant(id: number): Promise<void> {
    await apiClient.delete(`/productvariant/${id}`);
  },
};
