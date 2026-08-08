import React from 'react';
import { Package } from 'lucide-react';

export const Loader: React.FC<{ fullScreen?: boolean; label?: string }> = ({
  fullScreen = true,
  label = 'Loading StockDaddy...',
}) => {
  const content = (
    <div className="flex flex-col items-center justify-center gap-3 p-6 text-center">
      <div className="relative">
        <div className="w-12 h-12 rounded-full border-4 border-blue-500/20 border-t-blue-500 animate-spin" />
        <Package className="w-5 h-5 text-blue-400 absolute inset-0 m-auto" />
      </div>
      <p className="text-sm font-medium text-foreground/90 animate-pulse">{label}</p>
    </div>
  );

  if (fullScreen) {
    return (
      <div className="fixed inset-0 z-50 flex items-center justify-center bg-background/90 backdrop-blur-md">
        {content}
      </div>
    );
  }

  return content;
};
