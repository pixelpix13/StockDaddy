import React, { useState, useEffect } from 'react';
import { Users, Plus, Mail, Trash2, Pencil } from 'lucide-react';
import { Card } from '../components/common/Card';
import { Table, Column } from '../components/common/Table';
import { Button } from '../components/common/Button';
import { Input } from '../components/common/Input';
import { Modal } from '../components/common/Modal';
import { Badge } from '../components/common/Badge';
import { userService } from '../services';
import { UserManagementDto, CreateUserManagementRequest, UpdateUserManagementRequest } from '../dtos';
import { useToast } from '../context/ToastContext';
import { useAuth } from '../context/AuthContext';
import { getApiErrorMessage } from '@/lib/api-error';

export const UsersPage: React.FC = () => {
  const [users, setUsers] = useState<UserManagementDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<UserManagementDto | null>(null);

  // Form
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [roleId, setRoleId] = useState('1');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { showToast } = useToast();
  const { user: currentUser } = useAuth();

  const fetchUsers = async () => {
    setIsLoading(true);
    try {
      const data = await userService.getUsers();
      setUsers(data);
    } catch (err) {
      showToast('error', 'Error', 'Failed to load user accounts.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

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
        storeId: currentUser?.storeId || 1,
        username,
        email,
        passwordHash: password, // Backend UserRepository hashes or processes request
      };

      await userService.createUser(payload);
      showToast('success', 'User Created', `Account for "${username}" created successfully.`);
      setIsModalOpen(false);
      setUsername('');
      setEmail('');
      setPassword('');
      fetchUsers();
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
      fetchUsers();
    } catch (err) {
      showToast('error', 'Action Failed', 'Could not delete user.');
    }
  };

  const openEditUser = (row: UserManagementDto) => {
    setEditingUser(row);
    setUsername(row.username);
    setEmail(row.email);
    setRoleId(String(row.roleId));
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
        storeId: currentUser?.storeId || 1,
        username,
        email,
        ...(password ? { passwordHash: password } : {}),
      };
      await userService.updateUser(editingUser.id, payload);
      showToast('success', 'User Updated', `Account for "${username}" saved.`);
      setIsEditModalOpen(false);
      setEditingUser(null);
      setPassword('');
      fetchUsers();
    } catch (err: unknown) {
      showToast('error', 'Update Failed', getApiErrorMessage(err, 'Could not update user.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const columns: Column<UserManagementDto>[] = [
    {
      header: 'Username',
      accessor: (row) => (
        <div className="font-bold text-slate-100 flex items-center gap-2">
          <div className="w-7 h-7 rounded-full bg-blue-600/20 text-blue-400 flex items-center justify-center font-mono text-xs border border-blue-500/30">
            {row.username ? row.username.substring(0, 2).toUpperCase() : 'U'}
          </div>
          <span>{row.username}</span>
        </div>
      ),
    },
    {
      header: 'Email',
      accessor: (row) => (
        <span className="text-xs text-slate-300 flex items-center gap-1.5">
          <Mail className="w-3.5 h-3.5 text-slate-400" />
          {row.email}
        </span>
      ),
    },
    {
      header: 'Tenant / Role',
      accessor: (row) => (
        <div className="flex items-center gap-2">
          <Badge variant="info">Tenant #{row.tenantId}</Badge>
          <Badge variant="neutral">Role #{row.roleId}</Badge>
        </div>
      ),
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <div className="flex items-center gap-1">
          <button
            onClick={() => openEditUser(row)}
            className="p-1.5 rounded-lg text-slate-400 hover:text-blue-400 hover:bg-blue-500/10 transition-colors"
            title="Edit User"
          >
            <Pencil className="w-4 h-4" />
          </button>
          <button
            onClick={() => handleDeleteUser(row.id)}
            className="p-1.5 rounded-lg text-slate-400 hover:text-rose-400 hover:bg-rose-500/10 transition-colors"
            title="Delete User"
          >
            <Trash2 className="w-4 h-4" />
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 glass-panel p-6 rounded-3xl border border-slate-800">
        <div>
          <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
            User Access Management <Users className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-slate-400 mt-1">
            Manage tenant staff, cashiers, managers, and system roles
          </p>
        </div>

        <Button
          variant="primary"
          onClick={() => setIsModalOpen(true)}
          icon={<Plus className="w-4 h-4" />}
        >
          Add Staff Account
        </Button>
      </div>

      {/* Table */}
      <Card title={`Active User Accounts (${users.length})`}>
        <Table
          columns={columns}
          data={users}
          keyExtractor={(row) => row.id}
          isLoading={isLoading}
          emptyMessage="No staff accounts found."
        />
      </Card>

      {/* Modal */}
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
            <label className="text-xs font-semibold uppercase tracking-wider text-slate-300">
              Role Level
            </label>
            <select
              value={roleId}
              onChange={(e) => setRoleId(e.target.value)}
              className="w-full bg-slate-900/80 border border-slate-800 focus:border-blue-500 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none"
            >
              <option value="1">Admin (Full Access)</option>
              <option value="2">Store Manager</option>
              <option value="3">Cashier / POS Staff</option>
            </select>
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
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
            <label className="text-xs font-semibold uppercase tracking-wider text-slate-300">
              Role Level
            </label>
            <select
              value={roleId}
              onChange={(e) => setRoleId(e.target.value)}
              className="w-full bg-slate-900/80 border border-slate-800 focus:border-blue-500 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none"
            >
              <option value="1">Admin (Full Access)</option>
              <option value="2">Store Manager</option>
              <option value="3">Cashier / POS Staff</option>
            </select>
          </div>
          <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
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
