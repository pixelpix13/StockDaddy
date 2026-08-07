import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { User, Lock, ArrowRight } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { getApiErrorMessage } from '@/lib/api-error';

export const LoginForm: React.FC = () => {
  const [usernameOrEmail, setUsernameOrEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const { login } = useAuth();
  const { showToast } = useToast();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!usernameOrEmail || !password) {
      showToast('warning', 'Validation Error', 'Please fill in both fields.');
      return;
    }

    setIsLoading(true);
    try {
      await login({ usernameOrEmail, password });
      showToast('success', 'Welcome back!', 'Logged in successfully via JWT token.');
      navigate('/');
    } catch (err: unknown) {
      showToast('error', 'Login Failed', getApiErrorMessage(err, 'Invalid credentials.'));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        label="Username or Email"
        type="text"
        placeholder="e.g. admin or admin@stockdaddy.com"
        value={usernameOrEmail}
        onChange={(e) => setUsernameOrEmail(e.target.value)}
        icon={<User className="w-4 h-4" />}
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

      <Button
        type="submit"
        variant="primary"
        size="lg"
        className="w-full mt-2"
        isLoading={isLoading}
        icon={<ArrowRight className="w-4 h-4" />}
      >
        Sign In to StockDaddy
      </Button>

      <p className="text-center text-xs text-slate-400 mt-4">
        Don't have an account?{' '}
        <Link to="/register" className="text-blue-400 hover:underline font-semibold">
          Register new tenant account
        </Link>
      </p>
    </form>
  );
};
