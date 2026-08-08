import React, { useCallback, useEffect, useState } from 'react';
import { Truck, Plus, PackageCheck, Trash2 } from 'lucide-react';
import { orchestrationService, purchaseService } from '@/services';
import {
  PurchaseOrderDto,
  PurchaseItemDto,
  SupplierDto,
  VariantStockDto,
  PurchaseOrderStatus,
} from '@/dtos';
import { useAuth } from '@/context/AuthContext';
import { useActiveStoreId } from '@/context/StoreContext';
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
import { DatePicker, formatIsoDate, parseIsoDate } from '@/components/ui/date-picker';
import { Combobox } from '@/components/ui/combobox';

interface PoLineDraft {
  id?: number;
  productVariantId: number;
  quantity: number;
  unitCost: number;
  label: string;
}

interface ReceiveLineDraft {
  purchaseItemId: number;
  label: string;
  quantityOrdered: number;
  quantityReceived: string;
}

function lineLabel(variant: VariantStockDto) {
  return `${variant.productName} (${variant.skuCode})`;
}

function itemLabel(item: PurchaseItemDto) {
  const name = item.productName ?? `Variant #${item.productVariantId}`;
  const sku = item.skuCode ? ` (${item.skuCode})` : '';
  return `${name}${sku}`;
}

export const PurchasesPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;
  const storeId = useActiveStoreId();

  const list = usePagedList<PurchaseOrderDto>({
    fetchFn: useCallback(
      (query) => purchaseService.getPurchaseOrdersPaged({ ...query, storeId }),
      [storeId]
    ),
    defaultSortBy: 'id',
    defaultSortDir: 'desc',
  });

  const [suppliers, setSuppliers] = useState<SupplierDto[]>([]);
  const [variants, setVariants] = useState<VariantStockDto[]>([]);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [receiveDialogOpen, setReceiveDialogOpen] = useState(false);
  const [editingOrder, setEditingOrder] = useState<PurchaseOrderDto | null>(null);
  const [receivingOrder, setReceivingOrder] = useState<PurchaseOrderDto | null>(null);
  const [editSupplierId, setEditSupplierId] = useState('1');
  const [editStatus, setEditStatus] = useState<PurchaseOrderStatus>('Pending');
  const [editNotes, setEditNotes] = useState('');
  const [editExpectedDelivery, setEditExpectedDelivery] = useState('');
  const [editDueDate, setEditDueDate] = useState('');
  const [editLines, setEditLines] = useState<PoLineDraft[]>([]);
  const [receiveLines, setReceiveLines] = useState<ReceiveLineDraft[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isLoadingOrder, setIsLoadingOrder] = useState(false);

  const [supplierId, setSupplierId] = useState('1');
  const [status, setStatus] = useState<PurchaseOrderStatus>('Pending');
  const [notes, setNotes] = useState('');
  const [dueDate, setDueDate] = useState('');
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

  const addLineToDraft = (
    draftLines: PoLineDraft[],
    setDraftLines: React.Dispatch<React.SetStateAction<PoLineDraft[]>>
  ) => {
    const variant = variants.find((v) => String(v.id) === selectedVariantId);
    const qty = parseInt(lineQty, 10) || 1;
    const cost = parseFloat(lineCost) || variant?.costPrice || 0;
    if (!variant) return;
    setDraftLines([
      ...draftLines,
      {
        productVariantId: variant.id,
        quantity: qty,
        unitCost: cost,
        label: lineLabel(variant),
      },
    ]);
    setLineQty('1');
    setLineCost('');
  };

  const removeLineFromDraft = (
    index: number,
    setDraftLines: React.Dispatch<React.SetStateAction<PoLineDraft[]>>
  ) => {
    setDraftLines((prev) => prev.filter((_, i) => i !== index));
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (lines.length === 0) {
      showToast('warning', 'No Items', 'Add at least one line item.');
      return;
    }
    if (status === 'Unpaid' && !dueDate) {
      showToast('warning', 'Due Date Required', 'Set when you will pay the supplier for credit purchases.');
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
        dueDate: status === 'Unpaid' && dueDate ? new Date(dueDate).toISOString() : undefined,
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
      setDueDate('');
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not create PO.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const openReceiveOrder = async (order: PurchaseOrderDto) => {
    setIsLoadingOrder(true);
    try {
      const detail = await orchestrationService.getPurchaseOrderWithItems(order.id);
      setReceivingOrder(detail.order);
      setReceiveLines(
        detail.items.map((item) => ({
          purchaseItemId: item.id,
          label: itemLabel(item),
          quantityOrdered: item.quantity,
          quantityReceived: String(item.quantity),
        }))
      );
      setReceiveDialogOpen(true);
    } catch (err: unknown) {
      showToast('error', 'Load Failed', getApiErrorMessage(err, 'Could not load PO lines.'));
    } finally {
      setIsLoadingOrder(false);
    }
  };

  const handleReceive = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!receivingOrder) return;

    for (const line of receiveLines) {
      const qty = parseInt(line.quantityReceived, 10);
      if (Number.isNaN(qty) || qty < 0 || qty > line.quantityOrdered) {
        showToast(
          'warning',
          'Invalid Quantity',
          `${line.label}: enter 0–${line.quantityOrdered} received.`
        );
        return;
      }
    }

    setIsSubmitting(true);
    try {
      const result = await orchestrationService.receivePurchaseOrder(receivingOrder.id, {
        items: receiveLines.map((line) => ({
          purchaseItemId: line.purchaseItemId,
          quantityReceived: parseInt(line.quantityReceived, 10) || 0,
        })),
      });
      const partial = !result.order.fullyReceived;
      showToast(
        'success',
        'Received',
        partial
          ? `PO #${receivingOrder.id} received with partial quantities — stock updated for what arrived.`
          : `PO #${receivingOrder.id} fully received and stock updated.`
      );
      setReceiveDialogOpen(false);
      setReceivingOrder(null);
      setReceiveLines([]);
      list.reload();
      orchestrationService.getVariantStock(storeId).then(setVariants);
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not receive order.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const openEditOrder = async (order: PurchaseOrderDto) => {
    if (order.status === 'Delivered' || order.status === 'Cancelled') {
      showToast('warning', 'Cannot Edit', 'Delivered or cancelled orders cannot be edited.');
      return;
    }

    setIsLoadingOrder(true);
    try {
      const detail = await orchestrationService.getPurchaseOrderWithItems(order.id);
      setEditingOrder(detail.order);
      setEditSupplierId(String(detail.order.supplierId));
      setEditStatus(detail.order.status);
      setEditNotes(detail.order.notes || '');
      setEditExpectedDelivery(detail.order.expectedDelivery?.slice(0, 10) || '');
      setEditDueDate(detail.order.dueDate?.slice(0, 10) || '');
      setEditLines(
        detail.items.map((item) => ({
          id: item.id,
          productVariantId: item.productVariantId,
          quantity: item.quantity,
          unitCost: item.unitCost,
          label: itemLabel(item),
        }))
      );
      setEditDialogOpen(true);
    } catch (err: unknown) {
      showToast('error', 'Load Failed', getApiErrorMessage(err, 'Could not load PO for editing.'));
    } finally {
      setIsLoadingOrder(false);
    }
  };

  const handleUpdateOrder = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingOrder) return;
    if (editLines.length === 0) {
      showToast('warning', 'No Items', 'Add at least one line item.');
      return;
    }
    if (editStatus === 'Unpaid' && !editDueDate) {
      showToast('warning', 'Due Date Required', 'Set a due date for unpaid supplier credit.');
      return;
    }

    setIsSubmitting(true);
    try {
      await orchestrationService.updatePurchaseOrderWithItems(editingOrder.id, {
        supplierId: parseInt(editSupplierId, 10),
        expectedDelivery: editExpectedDelivery
          ? new Date(editExpectedDelivery).toISOString()
          : editingOrder.expectedDelivery,
        status: editStatus,
        notes: editNotes,
        dueDate: editStatus === 'Unpaid' && editDueDate ? new Date(editDueDate).toISOString() : undefined,
        items: editLines.map((line) => ({
          id: line.id,
          productVariantId: line.productVariantId,
          quantity: line.quantity,
          unitCost: line.unitCost,
        })),
      });
      showToast('success', 'Updated', `PO #${editingOrder.id} updated.`);
      setEditDialogOpen(false);
      setEditingOrder(null);
      setEditLines([]);
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

  const getSupplierName = (id: number) =>
    suppliers.find((s) => s.id === id)?.name ?? `Supplier #${id}`;

  const renderLineList = (
    draftLines: PoLineDraft[],
    onRemove: (index: number) => void
  ) => (
    <div className="space-y-2 max-h-40 overflow-y-auto">
      {draftLines.length === 0 ? (
        <p className="text-xs text-muted-foreground text-center py-2">No line items yet</p>
      ) : (
        draftLines.map((line, idx) => (
          <div
            key={line.id ?? `new-${idx}-${line.productVariantId}`}
            className="flex items-center justify-between rounded-lg border border-border px-3 py-2"
          >
            <p className="text-xs text-foreground/90 pr-2">
              {line.label} · {line.quantity} × ${line.unitCost.toFixed(2)}
            </p>
            <Button
              type="button"
              variant="ghost"
              size="icon"
              className="shrink-0 text-muted-foreground hover:text-rose-400"
              onClick={() => onRemove(idx)}
              aria-label="Remove line"
            >
              <Trash2 className="w-4 h-4" />
            </Button>
          </div>
        ))
      )}
    </div>
  );

  const renderLineEditor = (
    draftLines: PoLineDraft[],
    setDraftLines: React.Dispatch<React.SetStateAction<PoLineDraft[]>>
  ) => (
    <div className="rounded-xl border border-border p-4 space-y-3">
      <p className="text-xs font-semibold uppercase text-muted-foreground">Line Items</p>
      <VariantSelect variants={variants} value={selectedVariantId} onValueChange={setSelectedVariantId} />
      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-2">
          <Label>Qty</Label>
          <Input type="number" min="1" value={lineQty} onChange={(e) => setLineQty(e.target.value)} />
        </div>
        <div className="space-y-2">
          <Label>Unit Cost ($)</Label>
          <Input
            type="number"
            step="0.01"
            value={lineCost}
            onChange={(e) => setLineCost(e.target.value)}
            placeholder="Auto from variant"
          />
        </div>
      </div>
      <Button
        type="button"
        variant="secondary"
        className="w-full"
        onClick={() => addLineToDraft(draftLines, setDraftLines)}
      >
        <Plus className="w-4 h-4" /> Add Line
      </Button>
      {renderLineList(draftLines, (index) => removeLineFromDraft(index, setDraftLines))}
    </div>
  );

  const columns: Column<PurchaseOrderDto>[] = [
    {
      header: 'ID',
      accessor: (row) => <span className="font-mono text-sm text-blue-400">#{row.id}</span>,
      sortKey: 'id',
    },
    {
      header: 'Supplier',
      accessor: (row) => getSupplierName(row.supplierId),
      className: 'text-foreground/90',
    },
    {
      header: 'Order Date',
      accessor: (row) => new Date(row.orderDate).toLocaleDateString(),
      sortKey: 'createdat',
      className: 'text-muted-foreground',
    },
    {
      header: 'Status',
      accessor: (row) => (
        <div className="flex flex-col gap-1">
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
          {row.status === 'Delivered' && row.fullyReceived === false ? (
            <span className="text-[10px] text-amber-400">Partial receipt</span>
          ) : null}
        </div>
      ),
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <div className="flex items-center gap-2">
          {row.status !== 'Delivered' && row.status !== 'Cancelled' ? (
            <PermissionGate module={APP_MODULES.Purchase} action="Update">
              <Button
                size="sm"
                variant="outline"
                onClick={() => openReceiveOrder(row)}
                disabled={isLoadingOrder}
              >
                <PackageCheck className="w-4 h-4" /> Receive
              </Button>
            </PermissionGate>
          ) : null}
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
          <p className="text-sm text-muted-foreground mt-1">
            Create POs with line items, edit before delivery, and record partial or full receipts
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
        <DialogContent className="max-w-lg max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Create Purchase Order</DialogTitle>
            <DialogDescription>Add line items and submit to your supplier.</DialogDescription>
          </DialogHeader>
          <form onSubmit={handleCreate} className="space-y-4">
            <div className="space-y-2">
              <Label>Supplier</Label>
              <Combobox
                options={suppliers.map((s) => ({
                  value: String(s.id),
                  label: s.name,
                  keywords: s.email ?? s.phone ?? '',
                }))}
                value={supplierId}
                onValueChange={setSupplierId}
                placeholder="Select supplier"
                searchPlaceholder="Search suppliers…"
                emptyText="No matching suppliers."
              />
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
            {status === 'Unpaid' ? (
              <div className="space-y-2">
                <Label>Pay supplier by (credit due date)</Label>
                <DatePicker
                  value={parseIsoDate(dueDate)}
                  onChange={(date) => setDueDate(formatIsoDate(date))}
                  placeholder="Select due date"
                  fromDate={new Date()}
                />
              </div>
            ) : null}

            {renderLineEditor(lines, setLines)}

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
        <DialogContent className="max-w-lg max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Edit PO #{editingOrder?.id}</DialogTitle>
            <DialogDescription>Update supplier, lines, status, and notes before delivery.</DialogDescription>
          </DialogHeader>
          <form onSubmit={handleUpdateOrder} className="space-y-4">
            <div className="space-y-2">
              <Label>Supplier</Label>
              <Combobox
                options={suppliers.map((s) => ({
                  value: String(s.id),
                  label: s.name,
                  keywords: s.email ?? s.phone ?? '',
                }))}
                value={editSupplierId}
                onValueChange={setEditSupplierId}
                placeholder="Select supplier"
                searchPlaceholder="Search suppliers…"
                emptyText="No matching suppliers."
              />
            </div>
            <div className="space-y-2">
              <Label>Status</Label>
              <Select value={editStatus} onValueChange={(v) => setEditStatus(v as PurchaseOrderStatus)}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                  <SelectItem value="Pending">Pending</SelectItem>
                  <SelectItem value="Paid">Paid</SelectItem>
                  <SelectItem value="Unpaid">Unpaid</SelectItem>
                  <SelectItem value="Cancelled">Cancelled</SelectItem>
                </SelectContent>
              </Select>
            </div>
            {editStatus === 'Unpaid' ? (
              <div className="space-y-2">
                <Label>Pay supplier by</Label>
                <DatePicker
                  value={parseIsoDate(editDueDate)}
                  onChange={(date) => setEditDueDate(formatIsoDate(date))}
                  placeholder="Select due date"
                  fromDate={new Date()}
                />
              </div>
            ) : null}
            <div className="space-y-2">
              <Label>Expected Delivery</Label>
              <DatePicker
                value={parseIsoDate(editExpectedDelivery)}
                onChange={(date) => setEditExpectedDelivery(formatIsoDate(date))}
                placeholder="Select expected delivery"
              />
            </div>

            {renderLineEditor(editLines, setEditLines)}

            <div className="space-y-2">
              <Label>Notes</Label>
              <Input value={editNotes} onChange={(e) => setEditNotes(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setEditDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>Save Changes</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={receiveDialogOpen} onOpenChange={setReceiveDialogOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Receive PO #{receivingOrder?.id}</DialogTitle>
            <DialogDescription>
              Enter how many units you actually received for each line. Stock updates by received qty only.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleReceive} className="space-y-4">
            <div className="space-y-3 max-h-72 overflow-y-auto">
              {receiveLines.map((line, idx) => (
                <div key={line.purchaseItemId} className="rounded-lg border border-border p-3 space-y-2">
                  <p className="text-sm text-foreground">{line.label}</p>
                  <p className="text-xs text-muted-foreground">Ordered: {line.quantityOrdered}</p>
                  <div className="space-y-1">
                    <Label className="text-xs">Qty received</Label>
                    <Input
                      type="number"
                      min={0}
                      max={line.quantityOrdered}
                      value={line.quantityReceived}
                      onChange={(e) => {
                        const value = e.target.value;
                        setReceiveLines((prev) =>
                          prev.map((row, i) => (i === idx ? { ...row, quantityReceived: value } : row))
                        );
                      }}
                      required
                    />
                  </div>
                </div>
              ))}
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setReceiveDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                <PackageCheck className="w-4 h-4" /> Confirm Receipt
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
};
