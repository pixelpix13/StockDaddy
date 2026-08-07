import React, { useEffect, useState } from 'react';
import { FilePenLine } from 'lucide-react';
import { saleService } from '@/services';
import { billAdjustmentService } from './bill-adjustment.service';
import { SaleDto, PaymentMethod } from '@/dtos';
import { useToast } from '@/context/ToastContext';
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
  const [sales, setSales] = useState<SaleDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [editingSale, setEditingSale] = useState<SaleDto | null>(null);
  const [editTotalAmount, setEditTotalAmount] = useState('');
  const [editPaymentMethod, setEditPaymentMethod] = useState<PaymentMethod>('Cash');
  const [editNotes, setEditNotes] = useState('');

  const loadSales = async () => {
    setIsLoading(true);
    try {
      setSales(await saleService.getSales());
    } catch {
      showToast('error', 'Load Failed', 'Could not load sales for adjustment.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadSales();
  }, []);

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
      loadSales();
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
      loadSales();
    } catch {
      showToast('error', 'Failed', 'Could not void sale.');
    }
  };

  return (
    <div className="space-y-6">
      <div className="glass-panel p-6 rounded-3xl border border-slate-800">
        <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
          Sales Corrections <FilePenLine className="w-6 h-6 text-blue-400" />
        </h1>
        <p className="text-sm text-slate-400 mt-1">
          Adjust posted sale totals, payment method, or notes.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Posted Sales ({sales.length})</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-sm text-slate-400">Loading...</p>
          ) : sales.length === 0 ? (
            <p className="text-sm text-slate-400">No sales to adjust.</p>
          ) : (
            <div className="space-y-2 max-h-[600px] overflow-y-auto">
              {sales.map((sale) => (
                <div
                  key={sale.id}
                  className="flex items-center justify-between rounded-lg border border-slate-800 p-3 gap-3"
                >
                  <div>
                    <p className="font-mono text-xs text-blue-400">#SALE-{sale.id}</p>
                    <p className="text-xs text-slate-500">
                      {new Date(sale.createdAt).toLocaleString()} · Cashier #{sale.soldBy}
                    </p>
                  </div>
                  <div className="flex items-center gap-3">
                    <div className="text-right">
                      <p className="font-bold text-emerald-400">${sale.totalAmount.toFixed(2)}</p>
                      <Badge variant="secondary">{sale.paymentMethod}</Badge>
                    </div>
                    <CrudRowActions
                      module={APP_MODULES.BillAdjustment}
                      onEdit={() => openEdit(sale)}
                      onDelete={() => handleVoid(sale)}
                      deleteLabel="Void sale"
                    />
                  </div>
                </div>
              ))}
            </div>
          )}
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
