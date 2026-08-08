import React from 'react';
import { Outlet } from 'react-router-dom';
import { ShieldOff } from 'lucide-react';
import { usePermissions } from '@/hooks/usePermissions';
import { PermissionAction } from '@/dtos/rbac.dto';

interface PermissionRouteProps {
  module: string;
  action?: PermissionAction;
}

/** Route guard requiring a specific module permission (defaults to Read). */
export const PermissionRoute: React.FC<PermissionRouteProps> = ({ module, action = 'Read' }) => {
  const { hasPermission } = usePermissions();

  if (!hasPermission(module, action)) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[40vh] text-center space-y-3">
        <ShieldOff className="w-12 h-12 text-rose-400" />
        <h2 className="text-xl font-bold text-foreground">Access Denied</h2>
        <p className="text-sm text-muted-foreground max-w-md">
          Your role does not include <span className="text-foreground">{module}:{action}</span>.
          Ask an admin to update permissions in Access Control.
        </p>
      </div>
    );
  }

  return <Outlet />;
};
