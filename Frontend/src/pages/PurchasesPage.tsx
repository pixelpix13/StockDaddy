import React, { useCallback, useEffect, useState } from 'react';
import { Truck, Plus, PackageCheck } from 'lucide-react';
import { orchestrationService, purchaseService } from '@/services';
import {
  PurchaseOrderDto,
  SupplierDto,
  VariantStockDto,
  PurchaseOrderStatus,
} from '@/dtos';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { PURCHASE_ORDER_STATUS_OPTIONS, buildSupplierFilterOptions } from '@/config/list-filters';
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Badge } from '@/components/ui/badge';
import { VariantSelect } from '@/components/catalog/VariantSelect';
import { getApiErrorMessage } from '@/lib/api-error';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';
import { Button } from '@/components/ui/button';

interface PoLineDraft {
  productVariantId: number;
  quantity: number;
  unitCost: number;
  label: string;
}

export const PurchasesPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;
  const storeId = user?.storeId || 1;

  const list = usePagedList<PurchaseOrderDto>({
    fetchFn: useCallback((query) => purchaseService.getPurchaseOrdersPaged(query), []),
    defaultSortBy: 'id',
    defaultSortDir: 'desc',
  });

  const [suppliers, setSuppliers] = useState<SupplierDto[]>([]);
  const [variants, setVariants] = useState<VariantStockDto[]>([]);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [editingOrder, setEditingOrder] = useState<PurchaseOrderDto | null>(null);
  const [editStatus, setEditStatus] = useState<PurchaseOrderStatus>('Pending');
  const [editNotes, setEditNotes] = useState('');
  const [editExpectedDelivery, setEditExpectedDelivery] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [supplierId, setSupplierId] = useState('1');
  const [status, setStatus] = useState<PurchaseOrderStatus>('Pending');
  const [notes, setNotes] = useState('');
  const [selectedVariantId, setSelectedVariantId] = useState('');
  const [lineQty, setLineQty] = useState('1');
  const [lineCost, setLineCost] = useState('');
  const [lines, setLines] = useState<PoLineDraft[]>([]);

  useEffect(() => {
    Promise.all([
      purchaseService.getSuppliers(),
      orchestrationService.getVariantStock(storeId),
    ])
      .then(([suppliersRes, stock]) => {
        setSuppliers(suppliersRes);
        setVariants(stock);
        if (suppliersRes.length > 0) setSupplierId(String(suppliersRes[0].id));
        if (stock.length > 0 && !selectedVariantId) setSelectedVariantId(String(stock[0].id));
      })
      .catch(() => {
        showToast('error', 'Load Failed', 'Could not load purchase dropdown data.');
      });
  }, [storeId]);

  const addLine = () => {
    const variant = variants.find((v) => String(v.id) === selectedVariantId);
    const qty = parseInt(lineQty, 10) || 1;
    const cost = parseFloat(lineCost) || variant?.costPrice || 0;
    if (!variant) return;
    setLines((prev) => [
      ...prev,
      {
        productVariantId: variant.id,
        quantity: qty,
        unitCost: cost,
        label: `${variant.productName} (${variant.skuCode})`,
      },
    ]);
    setLineQty('1');
    setLineCost('');
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (lines.length === 0) {
      showToast('warning', 'No Items', 'Add at least one line item.');
      return;
    }
    setIsSubmitting(true);
    try {
      const now = new Date();
      const expected = new Date(now);
      expected.setDate(expected.getDate() + 7);
      await orchestrationService.createPurchaseOrderWithItems({
        tenantId,
        supplierId: parseInt(supplierId, 10),
        storeId,
        orderDate: now.toISOString(),
        expectedDelivery: expected.toISOString(),
        status,
        notes: notes || 'Created from StockDaddy UI',
        items: lines.map((l) => ({
          productVariantId: l.productVariantId,
          quantity: l.quantity,
          unitCost: l.unitCost,
        })),
      });
      showToast('success', 'PO Created', 'Purchase order submitted.');
      setDialogOpen(false);
      setLines([]);
      setNotes('');
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not create PO.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleReceive = async (orderId: number) => {
    try {
      await orchestrationService.receivePurchaseOrder(orderId);
      showToast('success', 'Received', `PO #${orderId} received and stock updated.`);
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not receive order.'));
    }
  };

  const openEditOrder = (order: PurchaseOrderDto) => {
    setEditingOrder(order);
    setEditStatus(order.status);
    setEditNotes(order.notes || '');
    setEditExpectedDelivery(order.expectedDelivery?.slice(0, 10) || '');
    setEditDialogOpen(true);
  };

  const handleUpdateOrder = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingOrder) return;
    setIsSubmitting(true);
    try {
      await purchaseService.updatePurchaseOrder(editingOrder.id, {
        status: editStatus,
        notes: editNotes,
        expectedDelivery: editExpectedDelivery
          ? new Date(editExpectedDelivery).toISOString()
          : editingOrder.expectedDelivery,
      });
      showToast('success', 'Updated', `PO #${editingOrder.id} updated.`);
      setEditDialogOpen(false);
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not update order.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteOrder = async (order: PurchaseOrderDto) => {
    if (!confirm(`Delete purchase order #${order.id}?`)) return;
    try {
      await purchaseService.deletePurchaseOrder(order.id);
      showToast('success', 'Deleted', 'Purchase order removed.');
      list.reload();
    } catch {
      showToast('error', 'Failed', 'Could not delete order.');
    }
  };

  const getSupplierName = (supplierId: number) =>
    suppliers.find((s) => s.id === supplierId)?.name ?? `Supplier #${supplierId}`;

  const columns: Column<PurchaseOrderDto>[] = [
    {
      header: 'ID',
      accessor: (row) => <span className="font-mono text-sm text-blue-400">#{row.id}</span>,
      sortKey: 'id',
    },
    {
      header: 'Supplier',
      accessor: (row) => getSupplierName(row.supplierId),
      className: 'text-slate-300',
    },
    {
      header: 'Order Date',
      accessor: (row) => new Date(row.orderDate).toLocaleDateString(),
      sortKey: 'createdat',
      className: 'text-slate-400',
    },
    {
      header: 'Status',
      accessor: (row) => (
        <Badge
          variant={
            row.status === 'Delivered'
              ? 'success'
              : row.status === 'Cancelled'
              ? 'destructive'
              : 'secondary'
          }
        >
          {row.status}
        </Badge>
      ),
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <div className="flex items-center gap-2">
          {row.status !== 'Delivered' && (
            <PermissionGate module={APP_MODULES.Purchase} action="Update">
              <Button size="sm" variant="outline" onClick={() => handleReceive(row.id)}>
                <PackageCheck className="w-4 h-4" /> Receive
              </Button>
            </PermissionGate>
          )}
          <CrudRowActions
            module={APP_MODULES.Purchase}
            onEdit={() => openEditOrder(row)}
            onDelete={() => handleDeleteOrder(row)}
          />
        </div>
      ),
    },
  ];

  return (
    <div className="page-stack">
      <div className="page-hero flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="page-hero-title">
            Purchase Orders <Truck className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-slate-400 mt-1">
            Create POs with line items and receive goods into variant stock
          </p>
        </div>
        <PermissionGate module={APP_MODULES.Purchase} action="Write">
          <Button onClick={() => setDialogOpen(true)}>
            <Plus className="w-4 h-4" /> Create PO
          </Button>
        </PermissionGate>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Orders ({list.totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search all columns…"
            emptyMessage="No purchase orders yet."
            filters={
              <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                <FilterSelect
                  label="Status"
                  options={PURCHASE_ORDER_STATUS_OPTIONS}
                  value={list.filters.status}
                  onChange={(value) => list.setFilter('status', value)}
                />
                {suppliers.length > 0 ? (
                  <FilterSelect
                    label="Supplier"
                    options={buildSupplierFilterOptions(suppliers)}
                    value={list.filters.supplierId}
                    onChange={(value) => list.setFilter('supplierId', value)}
                  />
                ) : null}
              </ListFilterBar>
            }
          />
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Create Purchase Order</DialogTitle>
            <DialogDescription>Add line items and submit to your supplier.</DialogDescription>
          </DialogHeader>
          <form onSubmit={handleCreate} className="space-y-4">
            <div className="space-y-2">
              <Label>Supplier</Label>
              <Select value={supplierId} onValueChange={setSupplierId}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                  {suppliers.map((s) => (
                    <SelectItem key={s.id} value={String(s.id)}>{s.name}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Status</Label>
              <Select value={status} onValueChange={(v) => setStatus(v as PurchaseOrderStatus)}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                  <SelectItem value="Pending">Pending</SelectItem>
                  <SelectItem value="Paid">Paid</SelectItem>
                  <SelectItem value="Unpaid">Unpaid</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="rounded-xl border border-slate-800 p-4 space-y-3">
              <p className="text-xs font-semibold uppercase text-slate-400">Line Items</p>
              <VariantSelect variants={variants} value={selectedVariantId} onValueChange={setSelectedVariantId} />
              <div className="grid grid-cols-2 gap-3">
                <div className="space-y-2">
                  <Label>Qty</Label>
                  <Input type="number" min="1" value={lineQty} onChange={(e) => setLineQty(e.target.value)} />
                </div>
                <div className="space-y-2">
                  <Label>Unit Cost ($)</Label>
                  <Input type="number" step="0.01" value={lineCost} onChange={(e) => setLineCost(e.target.value)} placeholder="Auto from variant" />
                </div>
              </div>
              <Button type="button" variant="secondary" className="w-full" onClick={addLine}>
                <Plus className="w-4 h-4" /> Add Line
              </Button>
              {lines.map((line, idx) => (
                <p key={idx} className="text-xs text-slate-300">
                  {line.label} · {line.quantity} × ${line.unitCost.toFixed(2)}
                </p>
              ))}
            </div>

            <div className="space-y-2">
              <Label>Notes</Label>
              <Input value={notes} onChange={(e) => setNotes(e.target.value)} />
            </div>

            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>Submit PO</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit PO #{editingOrder?.id}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleUpdateOrder} className="space-y-4">
            <div className="space-y-2">
              <Label>Status</Label>
              <Select value={editStatus} onValueChange={(v) => setEditStatus(v as PurchaseOrderStatus)}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                  <SelectItem value="Pending">Pending</SelectItem>
                  <SelectItem value="Paid">Paid</SelectItem>
                  <SelectItem value="Unpaid">Unpaid</SelectItem>
                  <SelectItem value="Delivered">Delivered</SelectItem>
                  <SelectItem value="Cancelled">Cancelled</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Expected Delivery</Label>
              <Input type="date" value={editExpectedDelivery} onChange={(e) => setEditExpectedDelivery(e.target.value)} />
            </div>
            <div className="space-y-2">
              <Label>Notes</Label>
              <Input value={editNotes} onChange={(e) => setEditNotes(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setEditDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>Update</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
};
