import React from 'react';
import { Menu, Bell, ShieldCheck, Building, Store } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { useStoreContext } from '../../context/StoreContext';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../ui/select';

interface HeaderProps {
  onToggleSidebar: () => void;
}

export const Header: React.FC<HeaderProps> = ({ onToggleSidebar }) => {
  const { user } = useAuth();
  const { stores, activeStoreId, setActiveStore, isLoading } = useStoreContext();

  return (
    <header className="h-14 sm:h-16 glass-panel border-b border-border sticky top-0 z-30 px-4 sm:px-6 flex items-center justify-between gap-3">
      <div className="flex items-center gap-4 min-w-0">
        <button
          onClick={onToggleSidebar}
          className="p-2 rounded-lg text-muted-foreground hover:text-foreground hover:bg-muted md:hidden transition-colors"
        >
          <Menu className="w-5 h-5" />
        </button>
        <div className="hidden sm:flex items-center gap-2 text-xs font-semibold text-muted-foreground shrink-0">
          <span className="flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-card border border-border text-primary">
            <Building className="w-3.5 h-3.5" />
            Tenant #{user?.tenantId || 1}
          </span>
        </div>
        {stores.length > 0 ? (
          <div className="min-w-[180px] max-w-[260px]">
            <Select
              value={activeStoreId ? String(activeStoreId) : undefined}
              onValueChange={(value) => void setActiveStore(parseInt(value, 10))}
              disabled={isLoading || stores.length <= 1}
            >
              <SelectTrigger className="h-9 text-xs">
                <div className="flex items-center gap-2 truncate">
                  <Store className="w-3.5 h-3.5 shrink-0 text-emerald-500" />
                  <SelectValue placeholder="Select store" />
                </div>
              </SelectTrigger>
              <SelectContent>
                {stores.map((store) => (
                  <SelectItem key={store.id} value={String(store.id)}>
                    <span className="flex flex-col items-start">
                      <span>{store.name}{store.location ? ` · ${store.location}` : ''}</span>
                      {store.roleName ? (
                        <span className="text-[10px] text-muted-foreground">{store.roleName}</span>
                      ) : null}
                    </span>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        ) : user?.storeId ? (
          <span className="flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-card border border-border text-emerald-500 text-xs">
            <Store className="w-3.5 h-3.5" />
            Store #{user.storeId}
          </span>
        ) : null}
      </div>

      <div className="flex items-center gap-2 sm:gap-3 shrink-0">
        <div className="hidden md:flex items-center gap-2 px-2 sm:px-3 py-1 rounded-full bg-emerald-500/10 border border-emerald-500/20 text-emerald-600 dark:text-emerald-400 text-xs font-semibold">
          <ShieldCheck className="w-3.5 h-3.5" />
          <span>JWT Authorized</span>
        </div>

        <button className="p-2 rounded-lg text-muted-foreground hover:text-foreground hover:bg-muted transition-colors relative">
          <Bell className="w-5 h-5" />
          <span className="absolute top-1.5 right-1.5 w-2 h-2 rounded-full bg-blue-500 animate-ping" />
          <span className="absolute top-1.5 right-1.5 w-2 h-2 rounded-full bg-blue-500" />
        </button>
      </div>
    </header>
  );
};
