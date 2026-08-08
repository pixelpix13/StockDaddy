import { Warehouse } from 'lucide-react';

interface BrandLogoProps {
  size?: 'sm' | 'md' | 'lg';
  showText?: boolean;
}

const sizeMap = {
  sm: 'w-5 h-5',
  md: 'w-6 h-6',
  lg: 'w-8 h-8',
};

const boxSizeMap = {
  sm: 'p-1.5 rounded-lg',
  md: 'p-2 rounded-xl',
  lg: 'p-3 rounded-2xl',
};

export function BrandLogo({ size = 'md', showText = false }: BrandLogoProps) {
  return (
    <div className="flex items-center gap-3">
      <div
        className={`${boxSizeMap[size]} bg-gradient-to-br from-blue-600 to-indigo-600 shadow-lg shadow-blue-500/25`}
      >
        <Warehouse className={`${sizeMap[size]} text-white`} />
      </div>
      {showText && (
        <div>
          <h1 className="text-lg font-bold bg-gradient-to-r from-foreground via-foreground/80 to-blue-500 bg-clip-text text-transparent">
            StockDaddy
          </h1>
          <span className="text-[10px] uppercase tracking-wider text-blue-400 font-semibold block">
            Inventory OS
          </span>
        </div>
      )}
    </div>
  );
}
