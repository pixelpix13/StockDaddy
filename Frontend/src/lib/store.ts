export const ACTIVE_STORE_STORAGE_KEY = 'stockdaddy-active-store';

export function getStoredActiveStoreId(): number | null {
  if (typeof window === 'undefined') return null;
  const stored = localStorage.getItem(ACTIVE_STORE_STORAGE_KEY);
  if (!stored) return null;
  const id = parseInt(stored, 10);
  return Number.isFinite(id) ? id : null;
}

export function setStoredActiveStoreId(storeId: number): void {
  localStorage.setItem(ACTIVE_STORE_STORAGE_KEY, String(storeId));
}

export function clearStoredActiveStoreId(): void {
  localStorage.removeItem(ACTIVE_STORE_STORAGE_KEY);
}
