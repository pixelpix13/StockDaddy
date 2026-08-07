import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { User, Mail, Lock, Building, ArrowRight } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { getApiErrorMessage } from '@/lib/api-error';

export const RegisterForm: React.FC = () => {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [tenantId, setTenantId] = useState('1');
  const [roleId, setRoleId] = useState('3');
  const [isLoading, setIsLoading] = useState(false);

  const { register } = useAuth();
  const { showToast } = useToast();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!username || !email || !password) {
      showToast('warning', 'Validation Error', 'Please complete all required fields.');
      return;
    }

    setIsLoading(true);
    try {
      await register({
        tenantId: parseInt(tenantId, 10) || 1,
        roleId: parseInt(roleId, 10) || 3,
        storeId: 1,
        username,
        email,
        password,
      });
      showToast('success', 'Account Registered!', 'Logged in with your new JWT session.');
      navigate('/');
    } catch (err: unknown) {
      showToast(
        'error',
        'Registration Failed',
        getApiErrorMessage(err, 'Registration failed. Username/Email might be taken.')
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        label="Username"
        type="text"
        placeholder="johndoe"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        icon={<User className="w-4 h-4" />}
        required
      />

      <Input
        label="Email Address"
        type="email"
        placeholder="john@example.com"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        icon={<Mail className="w-4 h-4" />}
        required
      />

      <Input
        label="Password"
        type="password"
        placeholder="••••••••"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        icon={<Lock className="w-4 h-4" />}
        required
      />

      <div className="grid grid-cols-2 gap-3">
        <Input
          label="Tenant ID"
          type="number"
          value={tenantId}
          onChange={(e) => setTenantId(e.target.value)}
          icon={<Building className="w-4 h-4" />}
        />

        <div className="flex flex-col gap-1.5">
          <label className="text-xs font-semibold uppercase tracking-wider text-slate-300">
            Role
          </label>
          <select
            value={roleId}
            onChange={(e) => setRoleId(e.target.value)}
            className="w-full bg-slate-900/80 border border-slate-800 focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 rounded-lg px-3 py-2 text-sm text-slate-100 focus:outline-none"
          >
            <option value="1">Admin (Role #1)</option>
            <option value="2">Manager (Role #2)</option>
            <option value="3">Cashier (Role #3)</option>
          </select>
        </div>
      </div>

      <Button
        type="submit"
        variant="primary"
        size="lg"
        className="w-full mt-2"
        isLoading={isLoading}
        icon={<ArrowRight className="w-4 h-4" />}
      >
        Create Account & Sign In
      </Button>

      <p className="text-center text-xs text-slate-400 mt-4">
        Already have an account?{' '}
        <Link to="/login" className="text-blue-400 hover:underline font-semibold">
          Back to Login
        </Link>
      </p>
    </form>
  );
};
