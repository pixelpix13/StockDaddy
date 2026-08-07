import React from 'react';
import { UserPlus } from 'lucide-react';
import { RegisterForm } from '../components/auth/RegisterForm';
import { BrandLogo } from '../components/common/BrandLogo';

export const RegisterPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center p-4 relative overflow-hidden">
      {/* Background glow */}
      <div className="absolute top-1/3 -right-20 w-96 h-96 bg-blue-600/15 rounded-full blur-3xl pointer-events-none" />
      <div className="absolute bottom-1/3 -left-20 w-96 h-96 bg-purple-600/15 rounded-full blur-3xl pointer-events-none" />

      <div className="w-full max-w-md relative z-10">
        <div className="text-center mb-8">
          <div className="inline-flex mb-4">
            <BrandLogo size="lg" />
          </div>
          <h1 className="text-3xl font-extrabold text-white tracking-tight">StockDaddy</h1>
          <p className="text-slate-400 text-sm mt-1">Create your user account</p>
        </div>

        <div className="glass-panel p-8 rounded-3xl border border-slate-800 shadow-2xl">
          <div className="flex items-center justify-between pb-4 mb-6 border-b border-slate-800">
            <div>
              <h2 className="text-lg font-bold text-slate-100">Create Account</h2>
              <p className="text-xs text-slate-400">Register and receive JWT session token</p>
            </div>
            <span className="p-2 rounded-lg bg-indigo-500/10 text-indigo-400 border border-indigo-500/20">
              <UserPlus className="w-5 h-5" />
            </span>
          </div>

          <RegisterForm />
        </div>

        <p className="text-center text-xs text-slate-500 mt-8">
          StockDaddy Platform &copy; 2026. Secure JWT Bearer Architecture.
        </p>
      </div>
    </div>
  );
};
