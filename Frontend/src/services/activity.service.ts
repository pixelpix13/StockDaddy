/** Activity / audit log API (`/api/auditlog`). */
import { fetchPaged } from '@/lib/fetch-paged';
import { AuditLogDto } from '@/dtos/activity.dto';
import { PagedQuery, PagedResult } from '@/types/paging';

export const activityService = {
  getActivityPaged(query: PagedQuery): Promise<PagedResult<AuditLogDto>> {
    return fetchPaged<AuditLogDto>('/auditlog', query);
  },
};
