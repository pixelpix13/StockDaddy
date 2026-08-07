/** Product image URLs linked to a product (`/api/productimage`). */
import { apiClient } from './api.client';

export interface ProductImageDto {
  id: number;
  productId: number;
  imageUrl: string;
  isPrimary: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductImageRequest {
  productId: number;
  imageUrl: string;
  isPrimary?: boolean;
}

export const productImageService = {
  async createProductImage(request: CreateProductImageRequest): Promise<void> {
    await apiClient.post('/productimage', {
      ...request,
      isPrimary: request.isPrimary ?? true,
    });
  },
};
