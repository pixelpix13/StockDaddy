import React from 'react';
import { usePermissions } from '@/hooks/usePermissions';
import { PermissionAction } from '@/dtos/rbac.dto';
import type { AppModule } from '@/config/permissions';

interface PermissionGateProps {
  /** Permission module, e.g. "Product", "Catalog". */
  module: AppModule | string;
  /** Required action; defaults to Read. */
  action?: PermissionAction;
  /** Rendered when permission is denied (default: nothing). */
  fallback?: React.ReactNode;
  children: React.ReactNode;
}

/**
 * Hides UI when the current user lacks the required permission.
 * Pair with backend PermissionAuthorizationFilter — API stays blocked even if UI is bypassed.
 */
export function PermissionGate({
  module,
  action = 'Read',
  fallback = null,
  children,
}: PermissionGateProps) {
  const { hasPermission } = usePermissions();

  if (!hasPermission(module, action)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}
