import { PermissionAction } from '@/dtos/rbac.dto';

/**
 * Permission modules — must stay in sync with backend PermissionKeys.Modules.
 */
export const APP_MODULES = {
  Dashboard: 'Dashboard',
  Catalog: 'Catalog',
  Product: 'Product',
  Inventory: 'Inventory',
  Sales: 'Sales',
  Purchase: 'Purchase',
  Supplier: 'Supplier',
  Customer: 'Customer',
  Users: 'Users',
  AccessControl: 'AccessControl',
  Settings: 'Settings',
  BillAdjustment: 'BillAdjustment',
} as const;

export type AppModule = (typeof APP_MODULES)[keyof typeof APP_MODULES];

export interface NavItemConfig {
  label: string;
  path: string;
  module: AppModule;
  action?: PermissionAction;
}

/** Sidebar routes filtered by Read permission on each module. */
export const NAV_ITEMS: NavItemConfig[] = [
  { label: 'Dashboard', path: '/', module: APP_MODULES.Dashboard },
  { label: 'Catalog Setup', path: '/catalog', module: APP_MODULES.Catalog },
  { label: 'Products', path: '/products', module: APP_MODULES.Product },
  { label: 'Inventory', path: '/inventory', module: APP_MODULES.Inventory },
  { label: 'Sales & POS', path: '/sales', module: APP_MODULES.Sales },
  { label: 'Purchases', path: '/purchases', module: APP_MODULES.Purchase },
  { label: 'Suppliers', path: '/suppliers', module: APP_MODULES.Supplier },
  { label: 'Customers', path: '/customers', module: APP_MODULES.Customer },
  { label: 'User Management', path: '/users', module: APP_MODULES.Users },
  { label: 'Access Control', path: '/access-control', module: APP_MODULES.AccessControl },
  { label: 'Settings', path: '/settings', module: APP_MODULES.Settings },
];
