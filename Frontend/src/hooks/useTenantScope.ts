import { useAuth } from '@/context/AuthContext';

/**
 * Returns the logged-in user's tenant and store IDs.
 * Falls back to `1` for local dev when claims are missing (matches seeded data).
 */
export function useTenantScope() {
  const { user } = useAuth();
  return {
    tenantId: user?.tenantId ?? 1,
    storeId: user?.storeId ?? 1,
    userId: user?.id ?? 1,
  };
}
