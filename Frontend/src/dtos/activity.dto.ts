export interface AuditLogDto {
  id: number;
  userId?: number | null;
  storeId?: number | null;
  username?: string | null;
  action: string;
  tableName: string;
  recordId: string;
  oldData: string;
  newData: string;
  timestamp: string;
  createdAt: string;
  updatedAt: string;
}
