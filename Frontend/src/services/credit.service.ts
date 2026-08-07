/** Credit ledger — receivables from customers and payables to suppliers. */
import { apiClient } from './api.client';
import { fetchPaged } from '@/lib/fetch-paged';
import { PagedQuery, PagedResult } from '@/types/paging';
import {
  CreditLedgerDto,
  RecordCreditPaymentRequest,
  UpdateCreditLedgerRequest,
} from '@/dtos/credit.dto';

export const creditService = {
  getCreditPaged(query: PagedQuery): Promise<PagedResult<CreditLedgerDto>> {
    return fetchPaged<CreditLedgerDto>('/creditledger', query);
  },

  async getById(id: number): Promise<CreditLedgerDto> {
    const response = await apiClient.get<CreditLedgerDto>(`/creditledger/${id}`);
    return response.data;
  },

  async recordPayment(id: number, request: RecordCreditPaymentRequest): Promise<CreditLedgerDto> {
    const response = await apiClient.post<CreditLedgerDto>(`/creditledger/${id}/payments`, request);
    return response.data;
  },

  async update(id: number, request: UpdateCreditLedgerRequest): Promise<CreditLedgerDto> {
    const response = await apiClient.put<CreditLedgerDto>(`/creditledger/${id}`, request);
    return response.data;
  },
};
