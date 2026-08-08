/**
 * Access Control — assign permissions to roles and roles to users.
 */
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Shield, Save, Users } from 'lucide-react';
import { rbacService, userService, tenantService } from '@/services';
import { RbacMatrixDto, PermissionAction, RoleWithPermissionsDto } from '@/dtos';
import { UserManagementDto } from '@/dtos';
import { StoreDto } from '@/dtos/tenant.dto';
import {
  StoreRoleAssignmentsEditor,
  StoreRoleAssignment,
  assignmentsFromLegacy,
  summarizeAssignments,
} from '@/components/access-control/StoreRoleAssignmentsEditor';
import { useToast } from '@/context/ToastContext';
import { useAuth } from '@/context/AuthContext';
import { getApiErrorMessage } from '@/lib/api-error';
import { PageHeader } from '@/components/common/PageHeader';
import { RolesTab } from '@/components/access-control/RolesTab';
import { PermissionGate } from '@/components/common/PermissionGate';
import { usePermissions } from '@/hooks/usePermissions';
import { APP_MODULES, MODULE_LABELS } from '@/config/permissions';
import { useActiveStoreId } from '@/context/StoreContext';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { buildRoleFilterOptions } from '@/config/list-filters';
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
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';

const STANDARD_ACTIONS: PermissionAction[] = ['Read', 'Write', 'Update', 'Delete'];
const EXTRA_ACTIONS: PermissionAction[] = ['AccessAllStores'];

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
      className="h-4 w-4 rounded border-border bg-card text-primary focus:ring-ring"
    />
  );
}

export const AccessControlPage: React.FC = () => {
  const { showToast } = useToast();
  const { user: currentUser, refreshSession } = useAuth();
  const { hasPermission } = usePermissions();
  const canUpdateAccess = hasPermission(APP_MODULES.AccessControl, 'Update');
  const storeId = useActiveStoreId();

  const usersList = usePagedList<UserManagementDto>({
    fetchFn: useCallback((query) => userService.getUsersPaged({ ...query, storeId }), [storeId]),
    defaultSortBy: 'username',
    defaultSortDir: 'asc',
  });

  const [matrix, setMatrix] = useState<RbacMatrixDto | null>(null);
  const [selectedRoleId, setSelectedRoleId] = useState<string>('');
  const [rolePermissionIds, setRolePermissionIds] = useState<Set<number>>(new Set());
  const [stores, setStores] = useState<StoreDto[]>([]);
  const [assignmentUser, setAssignmentUser] = useState<UserManagementDto | null>(null);
  const [assignmentDraft, setAssignmentDraft] = useState<StoreRoleAssignment[]>([]);
  const [assignmentDefaultRoleId, setAssignmentDefaultRoleId] = useState('1');
  const [isSavingAssignments, setIsSavingAssignments] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isSavingRole, setIsSavingRole] = useState(false);

  const loadMatrix = useCallback(async () => {
    setIsLoading(true);
    try {
      const matrixData = await rbacService.getMatrix();
      setMatrix(matrixData);
      setSelectedRoleId((prev) => {
        if (prev) return prev;
        return matrixData.roles.length > 0 ? String(matrixData.roles[0].id) : '';
      });
      setRolePermissionIds((prev) => {
        if (prev.size > 0) return prev;
        return new Set(matrixData.roles[0]?.permissionIds ?? []);
      });
    } catch (err: unknown) {
      showToast('error', 'Load Failed', getApiErrorMessage(err, 'Could not load access control data.'));
    } finally {
      setIsLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    loadMatrix();
    tenantService.getStores().then(setStores).catch(() => setStores([]));
  }, [loadMatrix]);

  const openStoreAssignments = (user: UserManagementDto) => {
    setAssignmentUser(user);
    setAssignmentDefaultRoleId(String(user.roleId));
    setAssignmentDraft(
      user.storeAssignments?.length
        ? user.storeAssignments.map((a) => ({
            storeId: a.storeId,
            roleId: a.roleId,
            isDefault: a.isDefault,
          }))
        : assignmentsFromLegacy(
            user.storeIds?.length ? user.storeIds : user.storeId ? [user.storeId] : [],
            user.roleId,
            user.defaultStoreId ?? user.storeId
          )
    );
  };

  const saveStoreAssignments = async () => {
    if (!assignmentUser) return;
    setIsSavingAssignments(true);
    try {
      const defaultStoreId =
        assignmentDraft.find((a) => a.isDefault)?.storeId ?? assignmentDraft[0]?.storeId;
      await rbacService.assignUserStoreAssignments(assignmentUser.id, {
        defaultRoleId: parseInt(assignmentDefaultRoleId, 10) || assignmentUser.roleId,
        defaultStoreId,
        assignments: assignmentDraft.map((a) => ({
          storeId: a.storeId,
          roleId: a.roleId,
          isDefault: a.isDefault,
        })),
      });
      showToast('success', 'Saved', 'Store access and roles updated. User should re-login to refresh permissions.');
      setAssignmentUser(null);
      usersList.reload();
      if (currentUser?.id === assignmentUser.id) {
        await refreshSession();
      }
    } catch (err: unknown) {
      showToast('error', 'Save Failed', getApiErrorMessage(err, 'Could not update store assignments.'));
    } finally {
      setIsSavingAssignments(false);
    }
  };

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
    const modulePermissions = (permissionsByModule.get(module) ?? [])
      .filter((p) => STANDARD_ACTIONS.includes(p.action as PermissionAction));
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
      await loadMatrix();
      if (currentUser?.roleId === roleId) {
        await refreshSession();
      }
    } catch (err: unknown) {
      showToast('error', 'Save Failed', getApiErrorMessage(err, 'Could not update role permissions.'));
    } finally {
      setIsSavingRole(false);
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

  const userColumns: Column<UserManagementDto>[] = [
    {
      header: 'ID',
      sortKey: 'id',
      accessor: (row) => <span className="font-mono text-xs text-muted-foreground">#{row.id}</span>,
    },
    { header: 'Username', accessor: 'username', sortKey: 'username', className: 'font-medium text-foreground' },
    { header: 'Email', accessor: 'email', sortKey: 'email', className: 'text-muted-foreground' },
    {
      header: 'Default Role',
      accessor: (row) => getRoleName(row.roleId),
      className: 'text-xs text-muted-foreground',
    },
    {
      header: 'Store Roles',
      accessor: (row) => (
        <span className="text-xs text-muted-foreground">
          {row.storeAssignments?.length
            ? row.storeAssignments
                .map((a) => `${a.storeName ?? stores.find((s) => s.id === a.storeId)?.name ?? `#${a.storeId}`} (${a.roleName ?? getRoleName(a.roleId)})`)
                .join(', ')
            : summarizeAssignments(
                assignmentsFromLegacy(
                  row.storeIds?.length ? row.storeIds : row.storeId ? [row.storeId] : [],
                  row.roleId,
                  row.defaultStoreId ?? row.storeId
                ),
                stores,
                roles
              )}
        </span>
      ),
    },
    {
      header: 'Manage',
      accessor: (row) => (
        <PermissionGate module={APP_MODULES.AccessControl} action="Update">
          <Button size="sm" variant="secondary" onClick={() => openStoreAssignments(row)}>
            Store access
          </Button>
        </PermissionGate>
      ),
    },
  ];

  return (
    <div className="page-stack">
      <PageHeader
        title="Access Control"
        description="Fine-grained RBAC — assign permissions to roles and roles to users"
        icon={<Shield className="w-6 h-6 text-blue-400" />}
      />

      <Tabs defaultValue="roles">
        <TabsList>
          <TabsTrigger value="roles">Roles</TabsTrigger>
          <TabsTrigger value="permissions">Role Permissions</TabsTrigger>
          <TabsTrigger value="users">Store & User Access</TabsTrigger>
        </TabsList>

        <TabsContent value="roles">
          <RolesTab
            roles={roles}
            isLoading={isLoading}
            onChanged={loadMatrix}
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
                  Activity Log Read shows all users in the active store; without it, users see only their own entries.
                  Settings → Access All Stores lets a role switch between and manage every store (Admin has this by default).
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
                <p className="text-sm text-muted-foreground">Loading permissions...</p>
              ) : !selectedRole ? (
                <p className="text-sm text-muted-foreground">Select a role to edit permissions.</p>
              ) : (
                <div className="space-y-4">
                  <div className="flex items-center gap-2">
                    <Badge variant="secondary">{selectedRole.name}</Badge>
                    <span className="text-xs text-muted-foreground">
                      {rolePermissionIds.size} permission(s) selected
                    </span>
                  </div>
                  {!canUpdateAccess && (
                    <p className="text-xs text-amber-400/90">
                      View-only — you need AccessControl:Update to edit this matrix.
                    </p>
                  )}
                  <div className="overflow-x-auto -mx-1 px-1 sm:mx-0 sm:px-0 rounded-lg">
                    <table className="w-full min-w-[520px] text-sm border-collapse">
                      <thead>
                        <tr className="border-b border-border">
                          <th className="text-left py-3 px-3 sm:px-4 text-muted-foreground font-medium">Module</th>
                          {STANDARD_ACTIONS.map((action) => (
                            <th key={action} className="text-center py-3 px-2 sm:px-3 text-muted-foreground font-medium w-20">
                              {action}
                            </th>
                          ))}
                          <th className="text-center py-3 px-2 sm:px-3 text-muted-foreground font-medium w-24">All Stores</th>
                          <th className="text-center py-3 px-2 sm:px-3 text-muted-foreground font-medium w-24">All</th>
                        </tr>
                      </thead>
                      <tbody>
                        {modules.map((module) => {
                          const modulePerms = permissionsByModule.get(module) ?? [];
                          const standardPerms = modulePerms.filter((p) => STANDARD_ACTIONS.includes(p.action as PermissionAction));
                          const allChecked = standardPerms.every((p) => rolePermissionIds.has(p.id));
                          const someChecked = standardPerms.some((p) => rolePermissionIds.has(p.id));
                          const accessAllPerm = module === APP_MODULES.Settings
                            ? modulePerms.find((p) => p.action === 'AccessAllStores')
                            : undefined;

                          return (
                            <tr key={module} className="border-b border-border/60">
                              <td className="py-3 pr-4 font-medium text-foreground">
                                {MODULE_LABELS[module as keyof typeof MODULE_LABELS] ?? module}
                              </td>
                              {STANDARD_ACTIONS.map((action) => {
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
                                      <span className="text-muted-foreground">—</span>
                                    )}
                                  </td>
                                );
                              })}
                              <td className="text-center py-3 px-2">
                                {accessAllPerm ? (
                                  <MatrixCheckbox
                                    checked={rolePermissionIds.has(accessAllPerm.id)}
                                    onChange={(checked) => togglePermission(accessAllPerm.id, checked)}
                                    disabled={!canUpdateAccess}
                                  />
                                ) : (
                                  <span className="text-muted-foreground">—</span>
                                )}
                              </td>
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
                <Users className="w-5 h-5" /> Store Access & Roles
              </CardTitle>
              <CardDescription>
                Assign which stores each user can access and which role they hold at each store.
                Permissions update when they switch stores or re-login.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <PagedDataTable
                columns={userColumns}
                list={usersList}
                keyExtractor={(row) => row.id}
                searchPlaceholder="Search all columns…"
                emptyMessage="No users found."
                filters={
                  matrix && matrix.roles.length > 0 ? (
                    <ListFilterBar showClear={usersList.hasActiveFilters} onClear={usersList.clearFilters}>
                      <FilterSelect
                        label="Role"
                        options={buildRoleFilterOptions(matrix.roles)}
                        value={usersList.filters.roleId}
                        onChange={(value) => usersList.setFilter('roleId', value)}
                      />
                    </ListFilterBar>
                  ) : undefined
                }
              />
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>

      <Dialog open={!!assignmentUser} onOpenChange={(open) => !open && setAssignmentUser(null)}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>
              Store access for {assignmentUser?.username}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase tracking-wider text-foreground/90">
                Default role (fallback)
              </label>
              <Select value={assignmentDefaultRoleId} onValueChange={setAssignmentDefaultRoleId}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((role) => (
                    <SelectItem key={role.id} value={String(role.id)}>{role.name}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <StoreRoleAssignmentsEditor
              stores={stores}
              roles={roles}
              assignments={assignmentDraft}
              onChange={setAssignmentDraft}
              fallbackRoleId={parseInt(assignmentDefaultRoleId, 10) || 1}
              disabled={!canUpdateAccess}
            />
            <div className="flex justify-end gap-2">
              <Button variant="secondary" onClick={() => setAssignmentUser(null)}>Cancel</Button>
              <PermissionGate module={APP_MODULES.AccessControl} action="Update">
                <Button onClick={saveStoreAssignments} disabled={isSavingAssignments || assignmentDraft.length === 0}>
                  Save assignments
                </Button>
              </PermissionGate>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};
