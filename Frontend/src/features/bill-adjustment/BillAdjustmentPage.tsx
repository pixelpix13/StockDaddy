import React, { useCallback, useState } from 'react';
import { FilePenLine } from 'lucide-react';
import { saleService } from '@/services';
import { billAdjustmentService } from './bill-adjustment.service';
import { SaleDto, PaymentMethod } from '@/dtos';
import { useToast } from '@/context/ToastContext';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { PAYMENT_METHOD_FILTER_OPTIONS } from '@/config/list-filters';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
import { getApiErrorMessage } from '@/lib/api-error';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { APP_MODULES } from '@/config/permissions';

/** Isolated bill correction UI — delete this folder + disable Features:BillAdjustment to remove. */
export const BillAdjustmentPage: React.FC = () => {
  const { showToast } = useToast();
  const list = usePagedList<SaleDto>({
    fetchFn: useCallback((query) => saleService.getSalesPaged(query), []),
    defaultSortBy: 'id',
    defaultSortDir: 'desc',
  });

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [editingSale, setEditingSale] = useState<SaleDto | null>(null);
  const [editTotalAmount, setEditTotalAmount] = useState('');
  const [editPaymentMethod, setEditPaymentMethod] = useState<PaymentMethod>('Cash');
  const [editNotes, setEditNotes] = useState('');

  const openEdit = (sale: SaleDto) => {
    setEditingSale(sale);
    setEditTotalAmount(String(sale.totalAmount));
    setEditPaymentMethod(sale.paymentMethod);
    setEditNotes(sale.notes || '');
    setEditDialogOpen(true);
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingSale) return;
    setIsSubmitting(true);
    try {
      await billAdjustmentService.adjustSale(editingSale.id, {
        totalAmount: parseFloat(editTotalAmount) || editingSale.totalAmount,
        paymentMethod: editPaymentMethod,
        notes: editNotes,
      });
      showToast('success', 'Bill Adjusted', `Sale #${editingSale.id} updated.`);
      setEditDialogOpen(false);
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not adjust bill.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleVoid = async (sale: SaleDto) => {
    if (!confirm(`Void sale #${sale.id}? Stock is not automatically restored.`)) return;
    try {
      await billAdjustmentService.voidSale(sale.id);
      showToast('success', 'Voided', 'Sale removed.');
      list.reload();
    } catch {
      showToast('error', 'Failed', 'Could not void sale.');
    }
  };

  const columns: Column<SaleDto>[] = [
    {
      header: 'ID',
      accessor: (row) => <span className="font-mono text-xs text-blue-400">#{row.id}</span>,
      sortKey: 'id',
    },
    {
      header: 'Date',
      accessor: (row) => new Date(row.createdAt).toLocaleString(),
      sortKey: 'createdat',
      className: 'text-xs text-slate-500',
    },
    {
      header: 'Cashier',
      accessor: (row) => `#${row.soldBy}`,
      className: 'text-slate-400',
    },
    {
      header: 'Amount',
      accessor: (row) => <span className="font-bold text-emerald-400">${row.totalAmount.toFixed(2)}</span>,
    },
    {
      header: 'Payment',
      accessor: (row) => <Badge variant="secondary">{row.paymentMethod}</Badge>,
    },
    {
      header: 'Actions',
      accessor: (row) => (
        <CrudRowActions
          module={APP_MODULES.BillAdjustment}
          onEdit={() => openEdit(row)}
          onDelete={() => handleVoid(row)}
          deleteLabel="Void sale"
        />
      ),
    },
  ];

  return (
    <div className="page-stack">
      <div className="page-hero">
        <h1 className="page-hero-title">
          Sales Corrections <FilePenLine className="w-6 h-6 text-blue-400" />
        </h1>
        <p className="text-sm text-slate-400 mt-1">
          Adjust posted sale totals, payment method, or notes.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Posted Sales ({list.totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search all columns…"
            emptyMessage="No sales to adjust."
            filters={
              <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                <FilterSelect
                  label="Payment"
                  options={PAYMENT_METHOD_FILTER_OPTIONS}
                  value={list.filters.paymentMethod}
                  onChange={(value) => list.setFilter('paymentMethod', value)}
                />
              </ListFilterBar>
            }
          />
        </CardContent>
      </Card>

      <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Adjust Bill #{editingSale?.id}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleUpdate} className="space-y-4">
            <div className="space-y-2">
              <Label>Total Amount</Label>
              <Input
                type="number"
                step="0.01"
                value={editTotalAmount}
                onChange={(e) => setEditTotalAmount(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label>Payment Method</Label>
              <Select
                value={editPaymentMethod}
                onValueChange={(v) => setEditPaymentMethod(v as PaymentMethod)}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Cash">Cash</SelectItem>
                  <SelectItem value="Card">Card</SelectItem>
                  <SelectItem value="UPI">UPI</SelectItem>
                  <SelectItem value="BankTransfer">Bank Transfer</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Notes</Label>
              <Textarea value={editNotes} onChange={(e) => setEditNotes(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setEditDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                Save Adjustment
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
};
