import React, { useCallback, useEffect, useState } from 'react';
import { Users, Plus, Mail, Trash2, Pencil } from 'lucide-react';
import { Card } from '../components/common/Card';
import { Table, Column } from '../components/common/Table';
import { Button } from '../components/common/Button';
import { Input } from '../components/common/Input';
import { Modal } from '../components/common/Modal';
import { Badge } from '../components/common/Badge';
import { userService, tenantService } from '../services';
import { UserManagementDto, CreateUserManagementRequest, UpdateUserManagementRequest } from '../dtos';
import { RoleDto, StoreDto } from '../dtos/tenant.dto';
import {
  StoreRoleAssignmentsEditor,
  StoreRoleAssignment,
  assignmentsFromLegacy,
  summarizeAssignments,
} from '@/components/access-control/StoreRoleAssignmentsEditor';
import { useToast } from '../context/ToastContext';
import { useAuth } from '../context/AuthContext';
import { getApiErrorMessage } from '@/lib/api-error';
import { PermissionGate } from '@/components/common/PermissionGate';
import { usePermissions } from '@/hooks/usePermissions';
import { APP_MODULES } from '@/config/permissions';
import { usePagedList } from '@/hooks/usePagedList';
import { ListToolbar } from '@/components/common/ListToolbar';
import { Pagination } from '@/components/common/Pagination';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { buildRoleFilterOptions } from '@/config/list-filters';
import { useActiveStoreId } from '@/context/StoreContext';

export const UsersPage: React.FC = () => {
  const storeId = useActiveStoreId();
  const list = usePagedList<UserManagementDto>({
    fetchFn: useCallback((query) => userService.getUsersPaged({ ...query, storeId }), [storeId]),
    defaultSortBy: 'username',
    defaultSortDir: 'asc',
  });

  useEffect(() => {
    const reload = () => list.reload();
    window.addEventListener('stockdaddy:store-changed', reload);
    return () => window.removeEventListener('stockdaddy:store-changed', reload);
  }, [list]);

  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [stores, setStores] = useState<StoreDto[]>([]);
  const [storeAssignments, setStoreAssignments] = useState<StoreRoleAssignment[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<UserManagementDto | null>(null);

  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [roleId, setRoleId] = useState('1');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { showToast } = useToast();
  const { user: currentUser } = useAuth();
  const { hasPermission } = usePermissions();
  const canUpdateUsers = hasPermission(APP_MODULES.Users, 'Update');
  const canDeleteUsers = hasPermission(APP_MODULES.Users, 'Delete');
  const defaultStoreId = storeAssignments.find((a) => a.isDefault)?.storeId ?? storeAssignments[0]?.storeId ?? null;

  useEffect(() => {
    tenantService.getRoles().then(setRoles).catch(() => {
      showToast('error', 'Error', 'Failed to load roles.');
    });
    tenantService.getStores().then(setStores).catch(() => setStores([]));
  }, [showToast]);

  const handleCreateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!username || !email || !password) {
      showToast('warning', 'Validation Error', 'Please complete all required fields.');
      return;
    }

    setIsSubmitting(true);
    try {
      const payload: CreateUserManagementRequest = {
        tenantId: currentUser?.tenantId || 1,
        roleId: parseInt(roleId, 10) || 1,
        storeAssignments,
        storeIds: storeAssignments.map((a) => a.storeId),
        defaultStoreId: defaultStoreId ?? storeAssignments[0]?.storeId,
        storeId: defaultStoreId ?? storeAssignments[0]?.storeId,
        username,
        email,
        passwordHash: password,
      };

      await userService.createUser(payload);
      showToast('success', 'User Created', `Account for "${username}" created successfully.`);
      setIsModalOpen(false);
      setUsername('');
      setEmail('');
      setPassword('');
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Creation Failed', getApiErrorMessage(err, 'Could not create user.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteUser = async (id: number) => {
    if (!confirm('Are you sure you want to remove this user access?')) return;
    try {
      await userService.deleteUser(id);
      showToast('success', 'User Removed', 'User soft-deleted.');
      list.reload();
    } catch (err) {
      showToast('error', 'Action Failed', 'Could not delete user.');
    }
  };

  const openEditUser = (row: UserManagementDto) => {
    setEditingUser(row);
    setUsername(row.username);
    setEmail(row.email);
    setRoleId(String(row.roleId));
    const assignments = row.storeAssignments?.length
      ? row.storeAssignments.map((a) => ({
          storeId: a.storeId,
          roleId: a.roleId,
          isDefault: a.isDefault,
        }))
      : assignmentsFromLegacy(
          row.storeIds?.length ? row.storeIds : row.storeId ? [row.storeId] : [],
          row.roleId,
          row.defaultStoreId ?? row.storeId
        );
    setStoreAssignments(assignments);
    setPassword('');
    setIsEditModalOpen(true);
  };

  const handleUpdateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingUser || !username || !email) return;
    setIsSubmitting(true);
    try {
      const payload: UpdateUserManagementRequest = {
        roleId: parseInt(roleId, 10) || 1,
        storeAssignments,
        storeIds: storeAssignments.map((a) => a.storeId),
        defaultStoreId: defaultStoreId ?? storeAssignments[0]?.storeId,
        storeId: defaultStoreId ?? storeAssignments[0]?.storeId,
        username,
        email,
        ...(password ? { passwordHash: password } : {}),
      };
      await userService.updateUser(editingUser.id, payload);
      showToast('success', 'User Updated', `Account for "${username}" saved.`);
      setIsEditModalOpen(false);
      setEditingUser(null);
      setPassword('');
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Update Failed', getApiErrorMessage(err, 'Could not update user.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const getRoleName = (id: number) =>
    roles.find((r) => r.id === id)?.name ?? `Role #${id}`;

  const columns: Column<UserManagementDto>[] = [
    {
      header: 'ID',
      sortKey: 'id',
      accessor: (row) => (
        <span className="font-mono text-xs text-muted-foreground">#{row.id}</span>
      ),
    },
    {
      header: 'Username',
      sortKey: 'username',
      accessor: (row) => (
        <div className="font-bold text-foreground flex items-center gap-2">
          <div className="w-7 h-7 rounded-full bg-blue-600/20 text-blue-400 flex items-center justify-center font-mono text-xs border border-blue-500/30">
            {row.username ? row.username.substring(0, 2).toUpperCase() : 'U'}
          </div>
          <span>{row.username}</span>
        </div>
      ),
    },
    {
      header: 'Email',
      sortKey: 'email',
      accessor: (row) => (
        <span className="text-xs text-foreground/90 flex items-center gap-1.5">
          <Mail className="w-3.5 h-3.5 text-muted-foreground" />
          {row.email}
        </span>
      ),
    },
    {
      header: 'Store Roles',
      accessor: (row) => {
        const summary = row.storeAssignments?.length
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
            );
        return <span className="text-xs text-muted-foreground">{summary}</span>;
      },
    },
    {
      header: 'Default Role',
      accessor: (row) => (
        <div className="flex items-center gap-2">
          <Badge variant="info">Tenant #{row.tenantId}</Badge>
          <Badge variant="neutral">{getRoleName(row.roleId)}</Badge>
        </div>
      ),
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <div className="flex items-center gap-1">
          {canUpdateUsers && (
            <button
              onClick={() => openEditUser(row)}
              className="p-1.5 rounded-lg text-muted-foreground hover:text-blue-400 hover:bg-blue-500/10 transition-colors"
              title="Edit User"
            >
              <Pencil className="w-4 h-4" />
            </button>
          )}
          {canDeleteUsers && (
            <button
              onClick={() => handleDeleteUser(row.id)}
              className="p-1.5 rounded-lg text-muted-foreground hover:text-rose-400 hover:bg-rose-500/10 transition-colors"
              title="Delete User"
            >
              <Trash2 className="w-4 h-4" />
            </button>
          )}
        </div>
      ),
    },
  ];

  return (
    <div className="page-stack">
      <div className="page-hero flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="page-hero-title">
            User Access Management <Users className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-muted-foreground mt-1">
            Manage tenant staff, cashiers, managers, and system roles
          </p>
        </div>

        <PermissionGate module={APP_MODULES.Users} action="Write">
          <Button
            variant="primary"
            onClick={() => {
              setUsername('');
              setEmail('');
              setPassword('');
              setRoleId('1');
              setStoreAssignments(
                assignmentsFromLegacy(
                  currentUser?.storeIds?.length ? currentUser.storeIds : currentUser?.storeId ? [currentUser.storeId] : [],
                  currentUser?.roleId ?? 1,
                  currentUser?.defaultStoreId ?? currentUser?.storeId
                )
              );
              setIsModalOpen(true);
            }}
            icon={<Plus className="w-4 h-4" />}
          >
            Add Staff Account
          </Button>
        </PermissionGate>
      </div>

      <Card title={`Active User Accounts (${list.totalCount})`}>
        <div className="space-y-4 mb-4">
          <ListToolbar
            searchInput={list.searchInput}
            onSearchChange={list.handleSearchChange}
            onSearchCommit={list.handleSearchCommit}
            searchPlaceholder="Search all columns…"
            filters={
              roles.length > 0 ? (
                <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                  <FilterSelect
                    label="Role"
                    options={buildRoleFilterOptions(roles)}
                    value={list.filters.roleId}
                    onChange={(value) => list.setFilter('roleId', value)}
                  />
                </ListFilterBar>
              ) : undefined
            }
          />
        </div>
        <Table
          columns={columns}
          data={list.items}
          keyExtractor={(row) => row.id}
          isLoading={list.isLoading}
          sort={list.sort}
          onSortChange={list.toggleSort}
          emptyMessage="No staff accounts found."
          footer={
            <Pagination
              page={list.page}
              pageSize={list.pageSize}
              totalCount={list.totalCount}
              onPageChange={list.setPage}
              onPageSizeChange={list.setPageSize}
            />
          }
        />
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="Add Staff User Account"
      >
        <form onSubmit={handleCreateUser} className="space-y-4">
          <Input
            label="Username"
            placeholder="sam_manager"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />

          <Input
            label="Email Address"
            type="email"
            placeholder="sam@stockdaddy.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />

          <Input
            label="Initial Password"
            type="password"
            placeholder="••••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-semibold uppercase tracking-wider text-foreground/90">
              Default Role
            </label>
            <select
              value={roleId}
              onChange={(e) => setRoleId(e.target.value)}
              className="w-full bg-card/80 border border-border focus:border-blue-500 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none"
            >
              {roles.map((role) => (
                <option key={role.id} value={role.id}>{role.name}</option>
              ))}
            </select>
          </div>

          {stores.length > 0 ? (
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase tracking-wider text-foreground/90">
                Store Access & Roles
              </label>
              <p className="text-xs text-muted-foreground">
                Pick stores and assign a role per store. Default role above applies when adding a store.
              </p>
              <StoreRoleAssignmentsEditor
                stores={stores}
                roles={roles}
                assignments={storeAssignments}
                onChange={setStoreAssignments}
                fallbackRoleId={parseInt(roleId, 10) || 1}
              />
            </div>
          ) : null}

          <div className="flex justify-end gap-3 pt-4 border-t border-border">
            <Button type="button" variant="secondary" onClick={() => setIsModalOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" variant="primary" isLoading={isSubmitting}>
              Create User Account
            </Button>
          </div>
        </form>
      </Modal>

      <Modal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        title="Edit Staff User"
      >
        <form onSubmit={handleUpdateUser} className="space-y-4">
          <Input
            label="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
          <Input
            label="Email Address"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
          <Input
            label="New Password (leave blank to keep current)"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-semibold uppercase tracking-wider text-foreground/90">
              Default Role
            </label>
            <select
              value={roleId}
              onChange={(e) => setRoleId(e.target.value)}
              className="w-full bg-card/80 border border-border focus:border-blue-500 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none"
            >
              {roles.map((role) => (
                <option key={role.id} value={role.id}>{role.name}</option>
              ))}
            </select>
          </div>

          {stores.length > 0 ? (
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase tracking-wider text-foreground/90">
                Store Access & Roles
              </label>
              <StoreRoleAssignmentsEditor
                stores={stores}
                roles={roles}
                assignments={storeAssignments}
                onChange={setStoreAssignments}
                fallbackRoleId={parseInt(roleId, 10) || 1}
              />
            </div>
          ) : null}

          <div className="flex justify-end gap-3 pt-4 border-t border-border">
            <Button type="button" variant="secondary" onClick={() => setIsEditModalOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" variant="primary" isLoading={isSubmitting}>
              Save Changes
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
