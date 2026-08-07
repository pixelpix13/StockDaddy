/** Route table and global providers (auth, toasts). All authenticated routes use MainLayout. */
import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ToastProvider } from './context/ToastContext';
import { ProtectedRoute } from './components/layout/ProtectedRoute';
import { MainLayout } from './components/layout/MainLayout';

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
import { CustomersPage } from './pages/CustomersPage';
import { SettingsPage } from './pages/SettingsPage';
import { FEATURES } from './config/features';
import { BillAdjustmentPage } from './features/bill-adjustment/BillAdjustmentPage';

export const App: React.FC = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ToastProvider>
          <Routes>
            {/* Public Auth Routes */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            {/* Protected Application Routes */}
            <Route element={<ProtectedRoute />}>
              <Route element={<MainLayout />}>
                <Route path="/" element={<DashboardPage />} />
                <Route path="/catalog" element={<CatalogPage />} />
                <Route path="/products" element={<ProductsPage />} />
                <Route path="/inventory" element={<InventoryPage />} />
                <Route path="/sales" element={<SalesPage />} />
                <Route path="/purchases" element={<PurchasesPage />} />
                <Route path="/suppliers" element={<SuppliersPage />} />
                <Route path="/customers" element={<CustomersPage />} />
                <Route path="/users" element={<UsersPage />} />
                {FEATURES.billAdjustment && (
                  <Route
                    path={FEATURES.billAdjustmentSecretPath}
                    element={<BillAdjustmentPage />}
                  />
                )}
                <Route path="/settings" element={<SettingsPage />} />
              </Route>
            </Route>

            {/* Fallback */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </ToastProvider>
      </AuthProvider>
    </BrowserRouter>
  );
};

export default App;
