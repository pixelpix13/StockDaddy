import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from 'react';
import { useAuth } from './AuthContext';
import { authService } from '@/services';
import {
  clearStoredActiveStoreId,
  getStoredActiveStoreId,
  setStoredActiveStoreId,
} from '@/lib/store';

export interface StoreOption {
  id: number;
  name: string;
  location: string;
  roleId?: number;
  roleName?: string;
  isDefault: boolean;
  isActive: boolean;
}

interface StoreContextType {
  stores: StoreOption[];
  activeStoreId: number | null;
  activeStore: StoreOption | null;
  isLoading: boolean;
  setActiveStore: (storeId: number) => Promise<void>;
  reloadStores: () => Promise<void>;
}

const StoreContext = createContext<StoreContextType | undefined>(undefined);

export const StoreProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user, isAuthenticated, refreshSession } = useAuth();
  const [stores, setStores] = useState<StoreOption[]>([]);
  const [activeStoreId, setActiveStoreId] = useState<number | null>(() => getStoredActiveStoreId());
  const [isLoading, setIsLoading] = useState(false);

  const syncActiveFromUser = useCallback((nextStores: StoreOption[]) => {
    const stored = getStoredActiveStoreId();
    const fromStored = stored && nextStores.some((s) => s.id === stored) ? stored : null;
    const fromUser = user?.storeId && nextStores.some((s) => s.id === user.storeId) ? user.storeId : null;
    const fallback =
      nextStores.find((s) => s.isDefault)?.id ??
      nextStores.find((s) => s.isActive)?.id ??
      nextStores[0]?.id ??
      null;

    const resolved = fromStored ?? fromUser ?? fallback;
    if (resolved) {
      setActiveStoreId(resolved);
      setStoredActiveStoreId(resolved);
    }
  }, [user?.storeId]);

  const reloadStores = useCallback(async () => {
    if (!isAuthenticated) {
      setStores([]);
      setActiveStoreId(null);
      clearStoredActiveStoreId();
      return;
    }

    setIsLoading(true);
    try {
      const list = await authService.getMyStores();
      setStores(list);
      syncActiveFromUser(list);
    } catch {
      if (user?.assignedStores?.length) {
        const fallback = user.assignedStores.map((s) => ({
          id: s.id,
          name: s.name,
          location: s.location,
          roleId: s.roleId,
          roleName: s.roleName,
          isDefault: s.isDefault,
          isActive: s.isActive,
        }));
        setStores(fallback);
        syncActiveFromUser(fallback);
      }
    } finally {
      setIsLoading(false);
    }
  }, [isAuthenticated, syncActiveFromUser, user?.assignedStores]);

  useEffect(() => {
    reloadStores();
  }, [reloadStores, user?.id]);

  const setActiveStore = useCallback(
    async (storeId: number) => {
      setActiveStoreId(storeId);
      setStoredActiveStoreId(storeId);
      try {
        await authService.switchStore(storeId);
        await refreshSession();
      } catch {
        // Header still sends X-Store-Id; JWT may lag until next refresh.
      }
      await reloadStores();
      window.dispatchEvent(new CustomEvent('stockdaddy:store-changed', { detail: storeId }));
    },
    [refreshSession, reloadStores]
  );

  const activeStore = useMemo(
    () => stores.find((s) => s.id === activeStoreId) ?? null,
    [stores, activeStoreId]
  );

  const value = useMemo(
    () => ({ stores, activeStoreId, activeStore, isLoading, setActiveStore, reloadStores }),
    [stores, activeStoreId, activeStore, isLoading, setActiveStore, reloadStores]
  );

  return <StoreContext.Provider value={value}>{children}</StoreContext.Provider>;
};

export function useStoreContext(): StoreContextType {
  const context = useContext(StoreContext);
  if (!context) {
    throw new Error('useStoreContext must be used within a StoreProvider');
  }
  return context;
}

export function useActiveStoreId(): number {
  const { activeStoreId } = useStoreContext();
  return activeStoreId ?? 1;
}
