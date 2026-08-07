import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  Package,
  Boxes,
  ShoppingCart,
  Truck,
  Users,
  Building2,
  Settings,
  FolderTree,
  LogOut,
  UserCircle,
  Shield,
  History,
  Wallet,
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { BrandLogo } from '../common/BrandLogo';
import { NAV_ITEMS } from '@/config/permissions';

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
}

const NAV_ICONS: Record<string, React.ReactNode> = {
  '/': <LayoutDashboard className="w-5 h-5" />,
  '/catalog': <FolderTree className="w-5 h-5" />,
  '/products': <Package className="w-5 h-5" />,
  '/inventory': <Boxes className="w-5 h-5" />,
  '/sales': <ShoppingCart className="w-5 h-5" />,
  '/purchases': <Truck className="w-5 h-5" />,
  '/suppliers': <Building2 className="w-5 h-5" />,
  '/customers': <UserCircle className="w-5 h-5" />,
  '/activity': <History className="w-5 h-5" />,
  '/credit': <Wallet className="w-5 h-5" />,
  '/users': <Users className="w-5 h-5" />,
  '/access-control': <Shield className="w-5 h-5" />,
  '/settings': <Settings className="w-5 h-5" />,
};

export const Sidebar: React.FC<SidebarProps> = ({ isOpen, onClose }) => {
  const { user, logout, hasPermission } = useAuth();

  const visibleNavItems = NAV_ITEMS.filter((item) =>
    hasPermission(item.module, item.action ?? 'Read')
  );

  return (
    <>
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-slate-950/80 backdrop-blur-sm md:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={`fixed top-0 left-0 z-40 h-screen w-64 glass-panel border-r border-slate-800/80 flex flex-col justify-between transition-transform duration-300 ease-in-out md:translate-x-0 ${
          isOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <div>
          <div className="flex items-center gap-3 px-6 h-16 border-b border-slate-800/80">
            <BrandLogo size="md" showText />
          </div>

          <nav className="p-4 space-y-1.5 overflow-y-auto max-h-[calc(100vh-10rem)]">
            {visibleNavItems.map((item) => (
              <NavLink
                key={item.path}
                to={item.path}
                onClick={onClose}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-3.5 py-2.5 rounded-xl font-medium text-sm transition-all duration-200 ${
                    isActive
                      ? 'bg-blue-600/15 text-blue-400 border border-blue-500/30 shadow-md shadow-blue-500/10'
                      : 'text-slate-400 hover:text-slate-100 hover:bg-slate-800/60'
                  }`
                }
              >
                {NAV_ICONS[item.path]}
                <span>{item.label}</span>
              </NavLink>
            ))}
          </nav>
        </div>

        <div className="p-4 border-t border-slate-800/80">
          <div className="flex items-center justify-between p-2.5 rounded-xl bg-slate-900/60 border border-slate-800">
            <div className="flex items-center gap-3 overflow-hidden">
              <div className="w-8 h-8 rounded-full bg-gradient-to-tr from-blue-600 to-purple-600 flex items-center justify-center font-bold text-xs text-white uppercase shrink-0">
                {user?.username ? user.username.substring(0, 2) : 'SD'}
              </div>
              <div className="truncate">
                <p className="text-xs font-semibold text-slate-200 truncate">{user?.username}</p>
                <p className="text-[10px] text-slate-400 truncate">
                  {user?.roleName || user?.email}
                </p>
              </div>
            </div>
            <button
              onClick={logout}
              title="Logout"
              className="p-1.5 rounded-lg text-slate-400 hover:text-rose-400 hover:bg-rose-500/10 transition-colors"
            >
              <LogOut className="w-4 h-4" />
            </button>
          </div>
        </div>
      </aside>
    </>
  );
};
