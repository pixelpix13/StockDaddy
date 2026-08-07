import React from 'react';

interface PageHeaderProps {
  title: string;
  description: string;
  icon?: React.ReactNode;
}

/** Standard page title block used at the top of route screens. */
export function PageHeader({ title, description, icon }: PageHeaderProps) {
  return (
    <div className="glass-panel p-6 rounded-3xl border border-slate-800">
      <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
        {title} {icon}
      </h1>
      <p className="text-sm text-slate-400 mt-1">{description}</p>
    </div>
  );
}
