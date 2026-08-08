import { useAuth } from '@/context/AuthContext';
import { useActiveStoreId } from '@/context/StoreContext';

/**
 * Returns the logged-in user's tenant and the active store from the store switcher.
 */
export function useTenantScope() {
  const { user } = useAuth();
  const activeStoreId = useActiveStoreId();
  return {
    tenantId: user?.tenantId ?? 1,
    storeId: activeStoreId,
    userId: user?.id ?? 1,
  };
}
