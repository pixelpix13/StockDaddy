export interface HsnMasterDto {
  id: number;
  hsnCode: string;
  description: string;
  cgstPercent: number;
  sgstPercent: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateHsnMasterRequest {
  hsnCode: string;
  description: string;
  cgstPercent: number;
  sgstPercent: number;
}

export interface UpdateHsnMasterRequest {
  description: string;
  cgstPercent: number;
  sgstPercent: number;
}

export interface TaxRegionDto {
  id: number;
  tenantId: number;
  storeId?: number;
  regionName: string;
  taxPercent: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaxRegionRequest {
  tenantId: number;
  storeId?: number;
  regionName: string;
  taxPercent: number;
}

export interface UpdateTaxRegionRequest {
  regionName: string;
  taxPercent: number;
}
