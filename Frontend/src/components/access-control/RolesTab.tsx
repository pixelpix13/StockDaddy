/**
 * Roles list with create, rename, and delete (built-in roles cannot be deleted).
 */
import React, { useState } from 'react';
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

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between gap-4 flex-wrap">
          <div>
            <CardTitle className="flex items-center gap-2">
              <Shield className="w-5 h-5" /> Roles ({roles.length})
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
        <CardContent>
          {isLoading ? (
            <p className="text-sm text-slate-400">Loading roles...</p>
          ) : roles.length === 0 ? (
            <p className="text-sm text-slate-400">No roles yet. Create one to get started.</p>
          ) : (
            <div className="space-y-2">
              {roles.map((role) => {
                const isProtected = PROTECTED_ROLES.has(role.name.toLowerCase());
                return (
                  <div
                    key={role.id}
                    className="flex items-center justify-between rounded-xl border border-slate-800 bg-slate-950/50 p-4"
                  >
                    <div>
                      <div className="flex items-center gap-2">
                        <span className="font-semibold text-slate-100">{role.name}</span>
                        {isProtected && <Badge variant="secondary">Built-in</Badge>}
                      </div>
                      <p className="text-xs text-slate-500 mt-1">
                        {role.permissionIds.length} permission(s) · ID #{role.id}
                      </p>
                    </div>
                    <div className="flex items-center gap-1">
                      {canWrite && (
                        <Button variant="ghost" size="sm" onClick={() => openEdit(role)} title="Rename role">
                          <Pencil className="w-4 h-4" />
                        </Button>
                      )}
                      {canDelete && !isProtected && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleDelete(role)}
                          title="Delete role"
                          className="text-rose-400 hover:text-rose-300"
                        >
                          <Trash2 className="w-4 h-4" />
                        </Button>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          )}
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
