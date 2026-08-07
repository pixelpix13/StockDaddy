import React from 'react';

interface PageHeaderProps {
  title: string;
  description: string;
  icon?: React.ReactNode;
  action?: React.ReactNode;
  className?: string;
}

/** Standard page title block used at the top of route screens. */
export function PageHeader({ title, description, icon, action, className = '' }: PageHeaderProps) {
  return (
    <div className={`page-hero ${className}`}>
      <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
        <div className="min-w-0">
          <h1 className="page-hero-title">
            {title} {icon}
          </h1>
          <p className="page-hero-description">{description}</p>
        </div>
        {action ? <div className="shrink-0">{action}</div> : null}
      </div>
    </div>
  );
}
