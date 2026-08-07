import React, { useCallback, useEffect, useState } from 'react';
import {
  DollarSign,
  Package,
  AlertTriangle,
  ShoppingCart,
  LayoutDashboard,
  Plus,
  RefreshCw,
  ArrowUpRight,
} from 'lucide-react';
import { StatCard } from '../components/common/StatCard';
import { Card } from '../components/common/Card';
import { Table, Column } from '../components/common/Table';
import { Badge } from '../components/common/Badge';
import { Button } from '../components/common/Button';
import { orchestrationService, saleService, inventoryService } from '../services';
import { VariantStockDto, SaleDto, ProductRestockAlertDto } from '../dtos';
import { useToast } from '../context/ToastContext';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';
import { usePagedList } from '@/hooks/usePagedList';
import { Pagination } from '@/components/common/Pagination';
import { ListToolbar } from '@/components/common/ListToolbar';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { ALERT_STATUS_OPTIONS, PAYMENT_METHOD_FILTER_OPTIONS } from '@/config/list-filters';

export const DashboardPage: React.FC = () => {
  const salesList = usePagedList<SaleDto>({
    fetchFn: useCallback((query) => saleService.getSalesPaged(query), []),
    defaultSortBy: 'id',
    defaultSortDir: 'desc',
    defaultPageSize: 5,
  });

  const alertsList = usePagedList<ProductRestockAlertDto>({
    fetchFn: useCallback((query) => inventoryService.getRestockAlertsPaged(query), []),
    defaultSortBy: 'id',
    defaultSortDir: 'desc',
    defaultPageSize: 5,
  });

  const [variants, setVariants] = useState<VariantStockDto[]>([]);
  const [statsLoading, setStatsLoading] = useState(true);
  const [totalRevenue, setTotalRevenue] = useState(0);

  const { showToast } = useToast();

  const loadStats = async () => {
    setStatsLoading(true);
    try {
      const [variantsData, salesResult] = await Promise.allSettled([
        orchestrationService.getVariantStock(),
        saleService.getSalesPaged({ page: 1, pageSize: 100, sortBy: 'id', sortDir: 'desc' }),
      ]);

      if (variantsData.status === 'fulfilled') setVariants(variantsData.value);
      if (salesResult.status === 'fulfilled') {
        setTotalRevenue(
          salesResult.value.items.reduce((acc, curr) => acc + (curr.totalAmount || 0), 0)
        );
      }
    } catch {
      showToast('info', 'Live Data Sync', 'Dashboard initialized with default data feed.');
    } finally {
      setStatsLoading(false);
    }
  };

  useEffect(() => {
    loadStats();
  }, []);

  const refreshAll = () => {
    loadStats();
    salesList.reload();
    alertsList.reload();
  };

  const totalProductsCount = variants.length;
  const totalSalesCount = salesList.totalCount;
  const lowStockCount = variants.filter((v) => v.quantity <= 5).length;
  const isLoading = statsLoading || salesList.isLoading;

  const salesColumns: Column<SaleDto>[] = [
    {
      header: 'ID',
      accessor: (row) => (
        <span className="font-mono text-xs font-bold text-blue-400">#{row.id}</span>
      ),
      sortKey: 'id',
    },
    {
      header: 'Store',
      accessor: (row) => <span className="text-slate-300">Store #{row.storeId ?? '—'}</span>,
    },
    {
      header: 'Amount',
      accessor: (row) => (
        <span className="font-semibold text-slate-100">${(row.totalAmount || 0).toFixed(2)}</span>
      ),
    },
    {
      header: 'Payment',
      accessor: (row) => <Badge variant="success">{row.paymentMethod || 'Cash'}</Badge>,
    },
    {
      header: 'Date',
      accessor: (row) => (
        <span className="text-xs text-slate-400">
          {new Date(row.createdAt || Date.now()).toLocaleDateString()}
        </span>
      ),
      sortKey: 'createdat',
    },
  ];

  const alertsColumns: Column<ProductRestockAlertDto>[] = [
    {
      header: 'ID',
      accessor: (row) => `#${row.id}`,
      sortKey: 'id',
      className: 'font-mono text-xs text-slate-500',
    },
    {
      header: 'Variant ID',
      accessor: (row) => `#${row.variantId}`,
      className: 'text-xs font-semibold text-slate-200',
    },
    {
      header: 'Product ID',
      accessor: (row) => `#${row.productId}`,
      className: 'text-[10px] text-slate-400',
    },
    {
      header: 'Status',
      accessor: (row) => row.status,
      className: 'text-[10px] text-slate-400',
    },
    {
      header: 'Severity',
      accessor: () => <Badge variant="error">Critical</Badge>,
    },
  ];

  return (
    <div className="page-stack">
      <div className="page-hero flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="page-hero-title">
            Executive Dashboard <LayoutDashboard className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-slate-400 mt-1">
            Real-time multi-tenant inventory analytics & store sales metrics
          </p>
        </div>

        <div className="flex items-center gap-3">
          <Button
            variant="outline"
            size="sm"
            onClick={refreshAll}
            isLoading={isLoading}
            icon={<RefreshCw className="w-4 h-4" />}
          >
            Refresh
          </Button>
          <PermissionGate module={APP_MODULES.Sales} action="Write">
            <Button
              variant="primary"
              size="sm"
              onClick={() => (window.location.href = '/sales')}
              icon={<Plus className="w-4 h-4" />}
            >
              New Sale POS
            </Button>
          </PermissionGate>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-5">
        <StatCard
          title="Total Sales Revenue"
          value={`$${totalRevenue.toLocaleString(undefined, { minimumFractionDigits: 2 })}`}
          icon={<DollarSign className="w-5 h-5" />}
          trend={{ value: '14.2%', isPositive: true }}
          color="blue"
        />
        <StatCard
          title="Active Products"
          value={totalProductsCount}
          icon={<Package className="w-5 h-5" />}
          trend={{ value: '8.5%', isPositive: true }}
          color="indigo"
        />
        <StatCard
          title="Completed Transactions"
          value={totalSalesCount}
          icon={<ShoppingCart className="w-5 h-5" />}
          trend={{ value: '5.1%', isPositive: true }}
          color="emerald"
        />
        <StatCard
          title="Stock Alerts"
          value={lowStockCount}
          icon={<AlertTriangle className="w-5 h-5" />}
          trend={{ value: lowStockCount > 0 ? 'Requires Action' : 'All Good', isPositive: lowStockCount === 0 }}
          color={lowStockCount > 0 ? 'rose' : 'emerald'}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
        <div className="lg:col-span-2">
          <Card
            title="Recent Sales Transactions"
            subtitle="Latest orders processed through POS and web store"
            action={
              <PermissionGate module={APP_MODULES.Sales} action="Read">
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => (window.location.href = '/sales')}
                  icon={<ArrowUpRight className="w-4 h-4" />}
                >
                  View All
                </Button>
              </PermissionGate>
            }
          >
            <div className="space-y-4 mb-4">
              <ListToolbar
                searchInput={salesList.searchInput}
                onSearchChange={salesList.handleSearchChange}
                onSearchCommit={salesList.handleSearchCommit}
                searchPlaceholder="Search all columns…"
                filters={
                  <ListFilterBar showClear={salesList.hasActiveFilters} onClear={salesList.clearFilters}>
                    <FilterSelect
                      label="Payment"
                      options={PAYMENT_METHOD_FILTER_OPTIONS}
                      value={salesList.filters.paymentMethod}
                      onChange={(value) => salesList.setFilter('paymentMethod', value)}
                    />
                  </ListFilterBar>
                }
              />
            </div>
            <Table
              columns={salesColumns}
              data={salesList.items}
              keyExtractor={(row) => row.id}
              emptyMessage="No sales recorded yet. Click 'New Sale POS' to record your first order!"
              isLoading={salesList.isLoading}
              sort={salesList.sort}
              onSortChange={salesList.toggleSort}
              footer={
                <Pagination
                  page={salesList.page}
                  pageSize={salesList.pageSize}
                  totalCount={salesList.totalCount}
                  onPageChange={salesList.setPage}
                  pageSizeOptions={[5, 10, 20]}
                />
              }
            />
          </Card>
        </div>

        <div className="space-y-4 sm:space-y-6">
          <Card
            title="Inventory Restock Alerts"
            subtitle="Products reaching minimum stock threshold"
          >
            <div className="space-y-4 mb-4">
              <ListToolbar
                searchInput={alertsList.searchInput}
                onSearchChange={alertsList.handleSearchChange}
                onSearchCommit={alertsList.handleSearchCommit}
                searchPlaceholder="Search all columns…"
                filters={
                  <ListFilterBar showClear={alertsList.hasActiveFilters} onClear={alertsList.clearFilters}>
                    <FilterSelect
                      label="Status"
                      options={ALERT_STATUS_OPTIONS}
                      value={alertsList.filters.status}
                      onChange={(value) => alertsList.setFilter('status', value)}
                    />
                  </ListFilterBar>
                }
              />
            </div>
            <Table
              columns={alertsColumns}
              data={alertsList.items}
              keyExtractor={(row) => row.id}
              isLoading={alertsList.isLoading}
              emptyMessage="No active restock alerts."
              sort={alertsList.sort}
              onSortChange={alertsList.toggleSort}
              footer={
                alertsList.totalCount > 0 ? (
                  <Pagination
                    page={alertsList.page}
                    pageSize={alertsList.pageSize}
                    totalCount={alertsList.totalCount}
                    onPageChange={alertsList.setPage}
                    pageSizeOptions={[5, 10, 20]}
                  />
                ) : undefined
              }
            />
          </Card>

          <Card title="Quick Architecture Note">
            <div className="p-3 rounded-xl bg-blue-500/10 border border-blue-500/20 text-xs text-blue-300 space-y-2">
              <p>
                <strong>JWT Session Active:</strong> Authentication tokens are stored securely and refreshed via the <code>AuthContext</code> provider.
              </p>
              <p className="text-[11px] text-slate-400">
                Frontend is connected to the .NET API via the Vite dev proxy at <code>/api</code>.
              </p>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
};
