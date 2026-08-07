/**
 * Tax regions tab — store-scoped regional tax rates.
 */
import React, { useState } from 'react';
import { Plus } from 'lucide-react';
import { catalogService } from '@/services';
import { TaxRegionDto } from '@/dtos';
import { useToast } from '@/context/ToastContext';
import { getApiErrorMessage } from '@/lib/api-error';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { CrudRowActions } from '@/components/common/CrudRowActions';

interface TaxRegionsTabProps {
  tenantId: number;
  storeId: number;
  taxRegions: TaxRegionDto[];
  onChanged: () => void;
}

export function TaxRegionsTab({ tenantId, storeId, taxRegions, onChanged }: TaxRegionsTabProps) {
  const { showToast } = useToast();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<TaxRegionDto | null>(null);
  const [regionName, setRegionName] = useState('');
  const [taxPercent, setTaxPercent] = useState('18');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const openCreate = () => {
    setEditing(null);
    setRegionName('');
    setTaxPercent('18');
    setDialogOpen(true);
  };

  const openEdit = (row: TaxRegionDto) => {
    setEditing(row);
    setRegionName(row.regionName);
    setTaxPercent(String(row.taxPercent));
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!regionName.trim()) return;
    setIsSubmitting(true);
    try {
      const percent = parseFloat(taxPercent) || 0;
      if (editing) {
        await catalogService.updateTaxRegion(editing.id, {
          regionName: regionName.trim(),
          taxPercent: percent,
        });
        showToast('success', 'Updated', 'Tax region updated.');
      } else {
        await catalogService.createTaxRegion({
          tenantId,
          storeId,
          regionName: regionName.trim(),
          taxPercent: percent,
        });
        showToast('success', 'Created', 'Tax region created.');
      }
      setDialogOpen(false);
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not save tax region.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (row: TaxRegionDto) => {
    if (!confirm(`Delete tax region "${row.regionName}"?`)) return;
    try {
      await catalogService.deleteTaxRegion(row.id);
      showToast('success', 'Deleted', 'Tax region removed.');
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete tax region.'));
    }
  };

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <div>
            <CardTitle>Tax Regions ({taxRegions.length})</CardTitle>
            <CardDescription>Regional tax rates for store #{storeId}</CardDescription>
          </div>
          <Button onClick={openCreate}>
            <Plus className="w-4 h-4" /> Add Region
          </Button>
        </CardHeader>
        <CardContent>
          {taxRegions.length === 0 ? (
            <p className="text-sm text-slate-400">No tax regions yet.</p>
          ) : (
            <div className="space-y-2">
              {taxRegions.map((row) => (
                <div
                  key={row.id}
                  className="flex items-center justify-between rounded-xl border border-slate-800 bg-slate-950/50 p-4"
                >
                  <div>
                    <p className="font-semibold text-slate-100">{row.regionName}</p>
                    <p className="text-xs text-slate-500">
                      {row.taxPercent}% tax · Store #{row.storeId ?? 'All'}
                    </p>
                  </div>
                  <CrudRowActions onEdit={() => openEdit(row)} onDelete={() => handleDelete(row)} />
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Tax Region' : 'Add Tax Region'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label>Region Name</Label>
              <Input value={regionName} onChange={(e) => setRegionName(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>Tax Percent</Label>
              <Input type="number" step="0.01" value={taxPercent} onChange={(e) => setTaxPercent(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>{editing ? 'Update' : 'Create'}</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </>
  );
}
