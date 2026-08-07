/** Route table and global providers (auth, toasts). All authenticated routes use MainLayout. */
import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ToastProvider } from './context/ToastContext';
import { ProtectedRoute } from './components/layout/ProtectedRoute';
import { PermissionRoute } from './components/layout/PermissionRoute';
import { MainLayout } from './components/layout/MainLayout';
import { APP_MODULES } from './config/permissions';

// Pages
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { CatalogPage } from './pages/CatalogPage';
import { DashboardPage } from './pages/DashboardPage';
import { ProductsPage } from './pages/ProductsPage';
import { InventoryPage } from './pages/InventoryPage';
import { SalesPage } from './pages/SalesPage';
import { PurchasesPage } from './pages/PurchasesPage';
import { SuppliersPage } from './pages/SuppliersPage';
import { UsersPage } from './pages/UsersPage';
import { AccessControlPage } from './pages/AccessControlPage';
import { CustomersPage } from './pages/CustomersPage';
import { SettingsPage } from './pages/SettingsPage';
import { FEATURES } from './config/features';
import { BillAdjustmentPage } from './features/bill-adjustment/BillAdjustmentPage';
import { ActivityPage } from './pages/ActivityPage';

export const App: React.FC = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ToastProvider>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            <Route element={<ProtectedRoute />}>
              <Route element={<MainLayout />}>
                <Route element={<PermissionRoute module={APP_MODULES.Dashboard} />}>
                  <Route path="/" element={<DashboardPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Catalog} />}>
                  <Route path="/catalog" element={<CatalogPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Product} />}>
                  <Route path="/products" element={<ProductsPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Inventory} />}>
                  <Route path="/inventory" element={<InventoryPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Sales} />}>
                  <Route path="/sales" element={<SalesPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Purchase} />}>
                  <Route path="/purchases" element={<PurchasesPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Supplier} />}>
                  <Route path="/suppliers" element={<SuppliersPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Customer} />}>
                  <Route path="/customers" element={<CustomersPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Activity} />}>
                  <Route path="/activity" element={<ActivityPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Users} />}>
                  <Route path="/users" element={<UsersPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.AccessControl} />}>
                  <Route path="/access-control" element={<AccessControlPage />} />
                </Route>
                <Route element={<PermissionRoute module={APP_MODULES.Settings} />}>
                  <Route path="/settings" element={<SettingsPage />} />
                </Route>
                {FEATURES.billAdjustment && (
                  <Route element={<PermissionRoute module={APP_MODULES.BillAdjustment} />}>
                    <Route
                      path={FEATURES.billAdjustmentSecretPath}
                      element={<BillAdjustmentPage />}
                    />
                  </Route>
                )}
              </Route>
            </Route>

            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </ToastProvider>
      </AuthProvider>
    </BrowserRouter>
  );
};

export default App;
