/**
 * Roles list with create, rename, and delete (built-in roles cannot be deleted).
 */
import React, { useMemo, useState } from 'react';
import { Plus, Pencil, Trash2, Shield } from 'lucide-react';
import { rbacService } from '@/services';
import { RoleWithPermissionsDto } from '@/dtos';
import { useToast } from '@/context/ToastContext';
import { usePermissions } from '@/hooks/usePermissions';
import { APP_MODULES } from '@/config/permissions';
import { getApiErrorMessage } from '@/lib/api-error';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { Table, Column } from '@/components/common/Table';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { ROLE_TYPE_OPTIONS } from '@/config/list-filters';

const PROTECTED_ROLES = new Set(['admin', 'manager', 'cashier']);

interface RolesTabProps {
  roles: RoleWithPermissionsDto[];
  isLoading: boolean;
  onChanged: () => void;
  onSelectRole?: (roleId: number) => void;
}

export function RolesTab({ roles, isLoading, onChanged, onSelectRole }: RolesTabProps) {
  const { showToast } = useToast();
  const { hasPermission } = usePermissions();
  const canWrite = hasPermission(APP_MODULES.AccessControl, 'Write');
  const canDelete = hasPermission(APP_MODULES.AccessControl, 'Delete');

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingRole, setEditingRole] = useState<RoleWithPermissionsDto | null>(null);
  const [roleName, setRoleName] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [roleTypeFilter, setRoleTypeFilter] = useState<string | undefined>();

  const filteredRoles = useMemo(() => {
    if (!roleTypeFilter) return roles;
    if (roleTypeFilter === 'builtin') {
      return roles.filter((r) => PROTECTED_ROLES.has(r.name.toLowerCase()));
    }
    return roles.filter((r) => !PROTECTED_ROLES.has(r.name.toLowerCase()));
  }, [roles, roleTypeFilter]);

  const openCreate = () => {
    setEditingRole(null);
    setRoleName('');
    setDialogOpen(true);
  };

  const openEdit = (role: RoleWithPermissionsDto) => {
    setEditingRole(role);
    setRoleName(role.name);
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!roleName.trim()) return;
    setIsSubmitting(true);
    try {
      if (editingRole) {
        await rbacService.updateRole(editingRole.id, { name: roleName.trim() });
        showToast('success', 'Updated', `Role "${roleName.trim()}" saved.`);
      } else {
        const created = await rbacService.createRole({ name: roleName.trim() });
        showToast('success', 'Created', `Role "${created.name}" created. Assign permissions in the Role Permissions tab.`);
        onSelectRole?.(created.id);
      }
      setDialogOpen(false);
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not save role.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (role: RoleWithPermissionsDto) => {
    if (PROTECTED_ROLES.has(role.name.toLowerCase())) {
      showToast('warning', 'Protected', `Built-in role "${role.name}" cannot be deleted.`);
      return;
    }
    if (!confirm(`Delete role "${role.name}"? This cannot be undone.`)) return;
    try {
      await rbacService.deleteRole(role.id);
      showToast('success', 'Deleted', `Role "${role.name}" removed.`);
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete role.'));
    }
  };

  const columns: Column<RoleWithPermissionsDto>[] = [
    {
      header: 'ID',
      accessor: (row) => `#${row.id}`,
      className: 'font-mono text-xs text-slate-500',
    },
    { header: 'Name', accessor: 'name', className: 'font-medium text-slate-100' },
    {
      header: 'Permissions',
      accessor: (row) => `${row.permissionIds.length} permission(s)`,
      className: 'text-slate-400',
    },
    {
      header: 'Type',
      accessor: (row) =>
        PROTECTED_ROLES.has(row.name.toLowerCase()) ? (
          <Badge variant="secondary">Built-in</Badge>
        ) : (
          <span className="text-slate-500">Custom</span>
        ),
    },
    {
      header: 'Actions',
      accessor: (row) => {
        const isProtected = PROTECTED_ROLES.has(row.name.toLowerCase());
        return (
          <div className="flex items-center gap-1">
            {canWrite && (
              <Button variant="ghost" size="sm" onClick={() => openEdit(row)} title="Rename role">
                <Pencil className="w-4 h-4" />
              </Button>
            )}
            {canDelete && !isProtected && (
              <Button
                variant="ghost"
                size="sm"
                onClick={() => handleDelete(row)}
                title="Delete role"
                className="text-rose-400 hover:text-rose-300"
              >
                <Trash2 className="w-4 h-4" />
              </Button>
            )}
          </div>
        );
      },
    },
  ];

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between gap-4 flex-wrap">
          <div>
            <CardTitle className="flex items-center gap-2">
              <Shield className="w-5 h-5" /> Roles ({filteredRoles.length})
            </CardTitle>
            <CardDescription>
              Create custom roles, then assign permissions in the Role Permissions tab.
            </CardDescription>
          </div>
          {canWrite && (
            <Button onClick={openCreate}>
              <Plus className="w-4 h-4" /> Create Role
            </Button>
          )}
        </CardHeader>
        <CardContent className="space-y-4">
          <ListFilterBar showClear={!!roleTypeFilter} onClear={() => setRoleTypeFilter(undefined)}>
            <FilterSelect
              label="Type"
              options={ROLE_TYPE_OPTIONS}
              value={roleTypeFilter}
              onChange={setRoleTypeFilter}
            />
          </ListFilterBar>
          <Table
            columns={columns}
            data={filteredRoles}
            keyExtractor={(row) => row.id}
            isLoading={isLoading}
            emptyMessage="No roles yet. Create one to get started."
          />
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingRole ? 'Rename Role' : 'Create Role'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="roleName">Role Name</Label>
              <Input
                id="roleName"
                value={roleName}
                onChange={(e) => setRoleName(e.target.value)}
                placeholder="e.g. Warehouse Staff"
                required
              />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {editingRole ? 'Save' : 'Create'}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </>
  );
}
