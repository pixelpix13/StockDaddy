export interface ProductDto {
  id: number;
  tenantId: number;
  storeId?: number;
  subcategoryId?: number;
  name: string;
  description: string;
  unit: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
  deletedAt?: string;
}

export interface ProductVariantDto {
  id: number;
  productId: number;
  storeId: number;
  hsnCodeId: number;
  variantName: string;
  barcode: string;
  skuCode: string;
  costPrice: number;
  marginPercent: number;
  taxPercent: number;
  price: number;
  quantity: number;
  createdAt: string;
  updatedAt: string;
}

export interface ProductCatalogItem extends ProductDto {
  variant?: ProductVariantDto;
}

export interface CreateProductRequest {
  tenantId: number;
  storeId?: number;
  subcategoryId?: number;
  name: string;
  description?: string;
  unit?: string;
}

export interface CreateProductVariantRequest {
  productId: number;
  storeId: number;
  hsnCodeId: number;
  variantName: string;
  barcode?: string;
  skuCode: string;
  costPrice: number;
  marginPercent?: number;
  taxPercent?: number;
  price: number;
  quantity?: number;
}

export interface CategoryDto {
  id: number;
  storeId: number;
  tenantId: number;
  name: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCategoryRequest {
  tenantId: number;
  storeId: number;
  name: string;
}

export interface UpdateCategoryRequest {
  name: string;
}

export interface UpdateSubcategoryRequest {
  name: string;
  categoryId: number;
}

export interface UpdateProductRequest {
  storeId?: number;
  subcategoryId?: number;
  name: string;
  description?: string;
  unit?: string;
}

export interface UpdateProductVariantRequest {
  variantName: string;
  barcode?: string;
  skuCode: string;
  costPrice: number;
  marginPercent?: number;
  taxPercent: number;
  price: number;
  quantity: number;
  hsnCodeId: number;
}

export interface SubcategoryDto {
  id: number;
  storeId: number;
  tenantId: number;
  categoryId: number;
  name: string;
  createdAt: string;
  updatedAt: string;
}
