/**
 * Access Control — assign permissions to roles and roles to users.
 */
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Shield, Save, Users } from 'lucide-react';
import { rbacService, userService } from '@/services';
import { RbacMatrixDto, PermissionAction, RoleWithPermissionsDto } from '@/dtos';
import { UserManagementDto } from '@/dtos';
import { useToast } from '@/context/ToastContext';
import { useAuth } from '@/context/AuthContext';
import { getApiErrorMessage } from '@/lib/api-error';
import { PageHeader } from '@/components/common/PageHeader';
import { RolesTab } from '@/components/access-control/RolesTab';
import { PermissionGate } from '@/components/common/PermissionGate';
import { usePermissions } from '@/hooks/usePermissions';
import { APP_MODULES } from '@/config/permissions';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Badge } from '@/components/ui/badge';
import { Label } from '@/components/ui/label';

const ACTIONS: PermissionAction[] = ['Read', 'Write', 'Update', 'Delete'];

function MatrixCheckbox({
  checked,
  onChange,
  indeterminate,
  disabled,
}: {
  checked: boolean;
  onChange: (checked: boolean) => void;
  indeterminate?: boolean;
  disabled?: boolean;
}) {
  return (
    <input
      type="checkbox"
      checked={checked}
      disabled={disabled}
      ref={(el) => {
        if (el) el.indeterminate = !!indeterminate;
      }}
      onChange={(e) => onChange(e.target.checked)}
      className="h-4 w-4 rounded border-slate-600 bg-slate-900 text-blue-500 focus:ring-blue-500"
    />
  );
}

export const AccessControlPage: React.FC = () => {
  const { showToast } = useToast();
  const { user: currentUser, refreshSession } = useAuth();
  const { hasPermission } = usePermissions();
  const canUpdateAccess = hasPermission(APP_MODULES.AccessControl, 'Update');

  const [matrix, setMatrix] = useState<RbacMatrixDto | null>(null);
  const [users, setUsers] = useState<UserManagementDto[]>([]);
  const [selectedRoleId, setSelectedRoleId] = useState<string>('');
  const [rolePermissionIds, setRolePermissionIds] = useState<Set<number>>(new Set());
  const [userRoleDrafts, setUserRoleDrafts] = useState<Record<number, number>>({});
  const [isLoading, setIsLoading] = useState(true);
  const [isSavingRole, setIsSavingRole] = useState(false);
  const [savingUserId, setSavingUserId] = useState<number | null>(null);

  const loadData = useCallback(async () => {
    setIsLoading(true);
    try {
      const [matrixData, userList] = await Promise.all([
        rbacService.getMatrix(),
        userService.getUsers(),
      ]);
      setMatrix(matrixData);
      setUsers(userList);
      setSelectedRoleId((prev) => {
        if (prev) return prev;
        return matrixData.roles.length > 0 ? String(matrixData.roles[0].id) : '';
      });
      setRolePermissionIds((prev) => {
        if (prev.size > 0) return prev;
        return new Set(matrixData.roles[0]?.permissionIds ?? []);
      });
      setUserRoleDrafts(Object.fromEntries(userList.map((u) => [u.id, u.roleId])));
    } catch (err: unknown) {
      showToast('error', 'Load Failed', getApiErrorMessage(err, 'Could not load access control data.'));
    } finally {
      setIsLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const modules = useMemo(() => {
    if (!matrix) return [];
    return [...new Set(matrix.permissions.map((p) => p.module))].sort();
  }, [matrix]);

  const permissionsByModule = useMemo(() => {
    const map = new Map<string, RbacMatrixDto['permissions']>();
    matrix?.permissions.forEach((p) => {
      const list = map.get(p.module) ?? [];
      list.push(p);
      map.set(p.module, list);
    });
    return map;
  }, [matrix]);

  const handleRoleChange = (roleId: string) => {
    setSelectedRoleId(roleId);
    const role = matrix?.roles.find((r) => r.id === parseInt(roleId, 10));
    setRolePermissionIds(new Set(role?.permissionIds ?? []));
  };

  const togglePermission = (permissionId: number, checked: boolean) => {
    setRolePermissionIds((prev) => {
      const next = new Set(prev);
      if (checked) next.add(permissionId);
      else next.delete(permissionId);
      return next;
    });
  };

  const toggleModule = (module: string, checked: boolean) => {
    const modulePermissions = permissionsByModule.get(module) ?? [];
    setRolePermissionIds((prev) => {
      const next = new Set(prev);
      modulePermissions.forEach((p) => {
        if (checked) next.add(p.id);
        else next.delete(p.id);
      });
      return next;
    });
  };

  const saveRolePermissions = async () => {
    if (!selectedRoleId) return;
    setIsSavingRole(true);
    try {
      const roleId = parseInt(selectedRoleId, 10);
      await rbacService.updateRolePermissions(roleId, {
        permissionIds: Array.from(rolePermissionIds),
      });
      showToast('success', 'Saved', 'Role permissions updated. Users must re-login to refresh JWT claims.');
      await loadData();
      if (currentUser?.roleId === roleId) {
        await refreshSession();
      }
    } catch (err: unknown) {
      showToast('error', 'Save Failed', getApiErrorMessage(err, 'Could not update role permissions.'));
    } finally {
      setIsSavingRole(false);
    }
  };

  const saveUserRole = async (userId: number) => {
    const roleId = userRoleDrafts[userId];
    if (!roleId) return;
    setSavingUserId(userId);
    try {
      await rbacService.assignUserRole(userId, { roleId });
      showToast('success', 'Role Assigned', 'User role updated. They must re-login for new permissions.');
      await loadData();
      if (currentUser?.id === userId) {
        await refreshSession();
      }
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not assign role.'));
    } finally {
      setSavingUserId(null);
    }
  };

  const roles = matrix?.roles ?? [];

  const getRoleName = (roleId: number) =>
    roles.find((r) => r.id === roleId)?.name ?? `Role #${roleId}`;

  const handleNewRoleSelected = (roleId: number) => {
    setSelectedRoleId(String(roleId));
    setRolePermissionIds(new Set());
  };

  const selectedRole = roles.find((r) => r.id === parseInt(selectedRoleId, 10));

  return (
    <div className="space-y-6">
      <PageHeader
        title="Access Control"
        description="Fine-grained RBAC — assign permissions to roles and roles to users"
        icon={<Shield className="w-6 h-6 text-blue-400" />}
      />

      <Tabs defaultValue="roles">
        <TabsList>
          <TabsTrigger value="roles">Roles</TabsTrigger>
          <TabsTrigger value="permissions">Role Permissions</TabsTrigger>
          <TabsTrigger value="users">User Assignment</TabsTrigger>
        </TabsList>

        <TabsContent value="roles">
          <RolesTab
            roles={roles}
            isLoading={isLoading}
            onChanged={loadData}
            onSelectRole={handleNewRoleSelected}
          />
        </TabsContent>

        <TabsContent value="permissions">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between gap-4 flex-wrap">
              <div>
                <CardTitle>Permission Matrix</CardTitle>
                <CardDescription>
                  Select a role and toggle module permissions. Each maps to API endpoint access.
                </CardDescription>
              </div>
              <div className="flex items-center gap-2 flex-wrap">
                <Select value={selectedRoleId} onValueChange={handleRoleChange}>
                  <SelectTrigger className="w-[180px]">
                    <SelectValue placeholder="Select role" />
                  </SelectTrigger>
                  <SelectContent>
                    {roles.map((role: RoleWithPermissionsDto) => (
                      <SelectItem key={role.id} value={String(role.id)}>{role.name}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <PermissionGate module={APP_MODULES.AccessControl} action="Update">
                  <Button onClick={saveRolePermissions} disabled={isSavingRole || !selectedRoleId}>
                    <Save className="w-4 h-4" /> Save Role
                  </Button>
                </PermissionGate>
              </div>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <p className="text-sm text-slate-400">Loading permissions...</p>
              ) : !selectedRole ? (
                <p className="text-sm text-slate-400">Select a role to edit permissions.</p>
              ) : (
                <div className="space-y-4">
                  <div className="flex items-center gap-2">
                    <Badge variant="secondary">{selectedRole.name}</Badge>
                    <span className="text-xs text-slate-500">
                      {rolePermissionIds.size} permission(s) selected
                    </span>
                  </div>
                  {!canUpdateAccess && (
                    <p className="text-xs text-amber-400/90">
                      View-only — you need AccessControl:Update to edit this matrix.
                    </p>
                  )}
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm border-collapse">
                      <thead>
                        <tr className="border-b border-slate-800">
                          <th className="text-left py-2 pr-4 text-slate-400 font-medium">Module</th>
                          {ACTIONS.map((action) => (
                            <th key={action} className="text-center py-2 px-2 text-slate-400 font-medium w-20">
                              {action}
                            </th>
                          ))}
                          <th className="text-center py-2 px-2 text-slate-400 font-medium w-24">All</th>
                        </tr>
                      </thead>
                      <tbody>
                        {modules.map((module) => {
                          const modulePerms = permissionsByModule.get(module) ?? [];
                          const allChecked = modulePerms.every((p) => rolePermissionIds.has(p.id));
                          const someChecked = modulePerms.some((p) => rolePermissionIds.has(p.id));

                          return (
                            <tr key={module} className="border-b border-slate-800/60">
                              <td className="py-3 pr-4 font-medium text-slate-200">{module}</td>
                              {ACTIONS.map((action) => {
                                const perm = modulePerms.find((p) => p.action === action);
                                return (
                                  <td key={action} className="text-center py-3 px-2">
                                    {perm ? (
                                      <MatrixCheckbox
                                        checked={rolePermissionIds.has(perm.id)}
                                        onChange={(checked) => togglePermission(perm.id, checked)}
                                        disabled={!canUpdateAccess}
                                      />
                                    ) : (
                                      <span className="text-slate-600">—</span>
                                    )}
                                  </td>
                                );
                              })}
                              <td className="text-center py-3 px-2">
                                <MatrixCheckbox
                                  checked={allChecked}
                                  indeterminate={someChecked && !allChecked}
                                  onChange={(checked) => toggleModule(module, checked)}
                                  disabled={!canUpdateAccess}
                                />
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="users">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Users className="w-5 h-5" /> Assign Roles to Users
              </CardTitle>
              <CardDescription>
                Change a user&apos;s role. Permissions come from the role&apos;s permission matrix.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <p className="text-sm text-slate-400">Loading users...</p>
              ) : users.length === 0 ? (
                <p className="text-sm text-slate-400">No users found.</p>
              ) : (
                <div className="space-y-3">
                  {users.map((u) => (
                    <div
                      key={u.id}
                      className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 rounded-xl border border-slate-800 bg-slate-950/50 p-4"
                    >
                      <div>
                        <p className="font-semibold text-slate-100">{u.username}</p>
                        <p className="text-xs text-slate-500">{u.email}</p>
                        <p className="text-xs text-slate-500 mt-1">
                          Current: {getRoleName(u.roleId)}
                        </p>
                      </div>
                      <div className="flex items-center gap-2">
                        <Label className="sr-only">Role for {u.username}</Label>
                        <Select
                          value={String(userRoleDrafts[u.id] ?? u.roleId)}
                          onValueChange={(value) =>
                            setUserRoleDrafts((prev) => ({
                              ...prev,
                              [u.id]: parseInt(value, 10),
                            }))
                          }
                        >
                          <SelectTrigger className="w-[160px]">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            {roles.map((role) => (
                              <SelectItem key={role.id} value={String(role.id)}>
                                {role.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <PermissionGate module={APP_MODULES.AccessControl} action="Update">
                          <Button
                            size="sm"
                            onClick={() => saveUserRole(u.id)}
                            disabled={
                              savingUserId === u.id ||
                              userRoleDrafts[u.id] === u.roleId
                            }
                          >
                            Assign
                          </Button>
                        </PermissionGate>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
};
