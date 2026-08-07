import React, { useCallback, useEffect, useState } from 'react';
import { Boxes, Plus, RefreshCw } from 'lucide-react';
import { orchestrationService, catalogService } from '@/services';
import { VariantStockDto } from '@/dtos';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { STOCK_FILTER_OPTIONS, buildSubcategoryFilterOptions } from '@/config/list-filters';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { VariantSelect } from '@/components/catalog/VariantSelect';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';
import { getApiErrorMessage } from '@/lib/api-error';

export const InventoryPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const storeId = user?.storeId || 1;

  const list = usePagedList<VariantStockDto>({
    fetchFn: useCallback(
      (query) => orchestrationService.getVariantStockPaged(query, storeId),
      [storeId]
    ),
    defaultSortBy: 'productname',
    defaultSortDir: 'asc',
  });

  const [dropdownVariants, setDropdownVariants] = useState<VariantStockDto[]>([]);
  const [subcategories, setSubcategories] = useState<{ id: number; name: string }[]>([]);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedVariantId, setSelectedVariantId] = useState('');
  const [quantityChange, setQuantityChange] = useState('');
  const [reason, setReason] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    orchestrationService.getVariantStock(storeId).then((data) => {
      setDropdownVariants(data);
      if (data.length > 0 && !selectedVariantId) {
        setSelectedVariantId(String(data[0].id));
      }
    }).catch(() => {
      showToast('error', 'Load Failed', 'Could not load variants for adjustment.');
    });
    catalogService.getSubcategories().then((subs) => {
      setSubcategories(subs.map((s) => ({ id: s.id, name: s.name })));
    }).catch(() => {});
  }, [storeId]);

  const handleAdjust = async (e: React.FormEvent) => {
    e.preventDefault();
    const change = parseInt(quantityChange, 10);
    if (!selectedVariantId || !change || change === 0) {
      showToast('warning', 'Validation', 'Select a variant and enter a non-zero quantity change.');
      return;
    }

    setIsSubmitting(true);
    try {
      const result = await orchestrationService.adjustStock({
        productVariantId: parseInt(selectedVariantId, 10),
        quantityChange: change,
        reason,
      });
      showToast(
        'success',
        'Stock Updated',
        `${result.skuCode}: ${result.previousQuantity} → ${result.newQuantity} units`
      );
      setDialogOpen(false);
      setQuantityChange('');
      setReason('');
      list.reload();
      orchestrationService.getVariantStock(storeId).then(setDropdownVariants);
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not adjust stock.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const selected = dropdownVariants.find((v) => String(v.id) === selectedVariantId);

  const columns: Column<VariantStockDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-slate-500' },
    { header: 'Product', accessor: 'productName', sortKey: 'productname', className: 'font-medium text-slate-100' },
    { header: 'SKU', accessor: 'skuCode', sortKey: 'skucode', className: 'font-mono text-xs' },
    { header: 'Category', accessor: (row) => row.subcategoryName || '—', className: 'text-slate-400' },
    {
      header: 'Available',
      accessor: (row) => row.quantity,
      sortKey: 'quantity',
      className: 'font-bold',
    },
    {
      header: 'Price',
      accessor: (row) => `$${row.price.toFixed(2)}`,
      sortKey: 'price',
    },
    {
      header: 'Status',
      accessor: (row) => (
        <Badge variant={row.quantity <= 5 ? 'destructive' : row.quantity <= 20 ? 'warning' : 'success'}>
          {row.quantity === 0 ? 'Out of Stock' : row.quantity <= 5 ? 'Low' : 'In Stock'}
        </Badge>
      ),
    },
  ];

  return (
    <div className="page-stack">
      <div className="page-hero flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="page-hero-title">
            Stock & Inventory <Boxes className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-slate-400 mt-1">
            Variant-level stock synced with warehouse records
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => list.reload()} disabled={list.isLoading}>
            <RefreshCw className="w-4 h-4" /> Sync
          </Button>
          <PermissionGate module={APP_MODULES.Inventory} action="Update">
            <Button onClick={() => setDialogOpen(true)}>
              <Plus className="w-4 h-4" /> Adjust Stock
            </Button>
          </PermissionGate>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Variant Stock Levels ({list.totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search all columns…"
            emptyMessage="No variants found. Add products first."
            filters={
              <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                <FilterSelect
                  label="Stock"
                  options={STOCK_FILTER_OPTIONS}
                  value={list.filters.stockFilter}
                  onChange={(value) => list.setFilter('stockFilter', value)}
                />
                {subcategories.length > 0 ? (
                  <FilterSelect
                    label="Subcategory"
                    options={buildSubcategoryFilterOptions(subcategories)}
                    value={list.filters.subcategoryId}
                    onChange={(value) => list.setFilter('subcategoryId', value)}
                  />
                ) : null}
              </ListFilterBar>
            }
          />
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Adjust Stock Quantity</DialogTitle>
            <DialogDescription>
              Use positive numbers to add stock, negative to remove. Updates variant and warehouse record.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleAdjust} className="space-y-4">
            <VariantSelect
              variants={dropdownVariants}
              value={selectedVariantId}
              onValueChange={setSelectedVariantId}
            />
            {selected && (
              <p className="text-xs text-slate-400">
                Current stock: <strong className="text-slate-200">{selected.quantity} units</strong>
              </p>
            )}
            <div className="space-y-2">
              <Label>Quantity Change (+ add / − remove)</Label>
              <Input
                type="number"
                value={quantityChange}
                onChange={(e) => setQuantityChange(e.target.value)}
                placeholder="e.g. 50 or -10"
                required
              />
            </div>
            <div className="space-y-2">
              <Label>Reason (optional)</Label>
              <Input value={reason} onChange={(e) => setReason(e.target.value)} placeholder="Cycle count, damage, etc." />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>Save Adjustment</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
};
