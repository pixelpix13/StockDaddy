import React, { useEffect, useState } from 'react';
import { Boxes, Plus, RefreshCw } from 'lucide-react';
import { orchestrationService } from '@/services';
import { VariantStockDto } from '@/dtos';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
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

  const [variants, setVariants] = useState<VariantStockDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedVariantId, setSelectedVariantId] = useState('');
  const [quantityChange, setQuantityChange] = useState('');
  const [reason, setReason] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadInventory = async () => {
    setIsLoading(true);
    try {
      const data = await orchestrationService.getVariantStock(storeId);
      setVariants(data);
      if (data.length > 0 && !selectedVariantId) {
        setSelectedVariantId(String(data[0].id));
      }
    } catch {
      showToast('error', 'Load Failed', 'Could not load inventory.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadInventory();
  }, []);

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
      loadInventory();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not adjust stock.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const selected = variants.find((v) => String(v.id) === selectedVariantId);

  return (
    <div className="space-y-6">
      <div className="glass-panel p-6 rounded-3xl border border-slate-800 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
            Stock & Inventory <Boxes className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-slate-400 mt-1">
            Variant-level stock synced with warehouse records
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={loadInventory} disabled={isLoading}>
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
          <CardTitle>Variant Stock Levels ({variants.length})</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-sm text-slate-400">Loading...</p>
          ) : variants.length === 0 ? (
            <p className="text-sm text-slate-400">No variants found. Add products first.</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-slate-800 text-slate-400 text-left">
                    <th className="pb-3 pr-4">Product</th>
                    <th className="pb-3 pr-4">SKU</th>
                    <th className="pb-3 pr-4">Category</th>
                    <th className="pb-3 pr-4">Available</th>
                    <th className="pb-3 pr-4">Price</th>
                    <th className="pb-3">Status</th>
                  </tr>
                </thead>
                <tbody>
                  {variants.map((row) => (
                    <tr key={row.id} className="border-b border-slate-800/60">
                      <td className="py-3 pr-4 font-medium text-slate-100">{row.productName}</td>
                      <td className="py-3 pr-4 font-mono text-xs">{row.skuCode}</td>
                      <td className="py-3 pr-4 text-slate-400">{row.subcategoryName || '—'}</td>
                      <td className="py-3 pr-4 font-bold">{row.quantity}</td>
                      <td className="py-3 pr-4">${row.price.toFixed(2)}</td>
                      <td className="py-3">
                        <Badge variant={row.quantity <= 5 ? 'destructive' : row.quantity <= 20 ? 'warning' : 'success'}>
                          {row.quantity === 0 ? 'Out of Stock' : row.quantity <= 5 ? 'Low' : 'In Stock'}
                        </Badge>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
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
              variants={variants}
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
