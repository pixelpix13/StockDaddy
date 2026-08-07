import { useCallback, useMemo } from 'react';
import { useAuth } from '@/context/AuthContext';
import { PermissionAction, permissionKey } from '@/dtos/rbac.dto';

/** Check effective permissions from the logged-in user's JWT-backed profile. */
export function usePermissions() {
  const { user } = useAuth();

  const permissionSet = useMemo(
    () => new Set((user?.permissions ?? []).map((p) => p.toLowerCase())),
    [user?.permissions]
  );

  const hasPermission = useCallback(
    (module: string, action: PermissionAction) =>
      permissionSet.has(permissionKey(module, action).toLowerCase()),
    [permissionSet]
  );

  const hasAnyPermission = useCallback(
    (...keys: string[]) => keys.some((key) => permissionSet.has(key.toLowerCase())),
    [permissionSet]
  );

  return {
    permissions: user?.permissions ?? [],
    roleName: user?.roleName ?? '',
    hasPermission,
    hasAnyPermission,
  };
}
