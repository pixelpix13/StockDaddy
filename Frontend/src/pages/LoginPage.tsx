import React from 'react';
import { ShieldCheck } from 'lucide-react';
import { LoginForm } from '../components/auth/LoginForm';
import { BrandLogo } from '../components/common/BrandLogo';

export const LoginPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center p-4 relative overflow-hidden">
      {/* Dynamic Ambient Glow Highlights */}
      <div className="absolute top-1/4 -left-20 w-96 h-96 bg-blue-600/15 rounded-full blur-3xl pointer-events-none" />
      <div className="absolute bottom-1/4 -right-20 w-96 h-96 bg-indigo-600/15 rounded-full blur-3xl pointer-events-none" />

      {/* Main Container */}
      <div className="w-full max-w-md relative z-10">
        {/* Brand Logo Header */}
        <div className="text-center mb-8">
          <div className="inline-flex mb-4">
            <BrandLogo size="lg" />
          </div>
          <h1 className="text-3xl font-extrabold text-white tracking-tight">StockDaddy</h1>
          <p className="text-slate-400 text-sm mt-1">Multi-tenant Inventory & Point of Sale System</p>
        </div>

        {/* Card Form */}
        <div className="glass-panel p-8 rounded-3xl border border-slate-800 shadow-2xl">
          <div className="flex items-center justify-between pb-4 mb-6 border-b border-slate-800">
            <div>
              <h2 className="text-lg font-bold text-slate-100">Authentication</h2>
              <p className="text-xs text-slate-400">Sign in with your JWT credentials</p>
            </div>
            <span className="p-2 rounded-lg bg-blue-500/10 text-blue-400 border border-blue-500/20">
              <ShieldCheck className="w-5 h-5" />
            </span>
          </div>

          <LoginForm />
        </div>

        {/* Footer info */}
        <p className="text-center text-xs text-slate-500 mt-8">
          StockDaddy Platform &copy; 2026. Secure JWT Bearer Architecture.
        </p>
      </div>
    </div>
  );
};
