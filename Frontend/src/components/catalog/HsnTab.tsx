/**
 * HSN master tab — GST classification codes with CGST/SGST percentages.
 */
import React, { useState } from 'react';
import { Plus } from 'lucide-react';
import { catalogService } from '@/services';
import { HsnMasterDto } from '@/dtos';
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
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';

interface HsnTabProps {
  hsnCodes: HsnMasterDto[];
  onChanged: () => void;
}

export function HsnTab({ hsnCodes, onChanged }: HsnTabProps) {
  const { showToast } = useToast();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<HsnMasterDto | null>(null);
  const [hsnCode, setHsnCode] = useState('');
  const [description, setDescription] = useState('');
  const [cgstPercent, setCgstPercent] = useState('9');
  const [sgstPercent, setSgstPercent] = useState('9');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const openCreate = () => {
    setEditing(null);
    setHsnCode('');
    setDescription('');
    setCgstPercent('9');
    setSgstPercent('9');
    setDialogOpen(true);
  };

  const openEdit = (row: HsnMasterDto) => {
    setEditing(row);
    setHsnCode(row.hsnCode);
    setDescription(row.description);
    setCgstPercent(String(row.cgstPercent));
    setSgstPercent(String(row.sgstPercent));
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!description.trim()) return;
    setIsSubmitting(true);
    try {
      const cgst = parseFloat(cgstPercent) || 0;
      const sgst = parseFloat(sgstPercent) || 0;
      if (editing) {
        await catalogService.updateHsnCode(editing.id, {
          description: description.trim(),
          cgstPercent: cgst,
          sgstPercent: sgst,
        });
        showToast('success', 'Updated', 'HSN code updated.');
      } else {
        if (!hsnCode.trim()) return;
        await catalogService.createHsnCode({
          hsnCode: hsnCode.trim(),
          description: description.trim(),
          cgstPercent: cgst,
          sgstPercent: sgst,
        });
        showToast('success', 'Created', 'HSN code created.');
      }
      setDialogOpen(false);
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not save HSN code.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (row: HsnMasterDto) => {
    if (!confirm(`Delete HSN "${row.hsnCode}"?`)) return;
    try {
      await catalogService.deleteHsnCode(row.id);
      showToast('success', 'Deleted', 'HSN code removed.');
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete HSN code.'));
    }
  };

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <div>
            <CardTitle>HSN Master ({hsnCodes.length})</CardTitle>
            <CardDescription>GST tax codes for product classification</CardDescription>
          </div>
          <PermissionGate module={APP_MODULES.Catalog} action="Write">
            <Button onClick={openCreate}>
              <Plus className="w-4 h-4" /> Add HSN
            </Button>
          </PermissionGate>
        </CardHeader>
        <CardContent>
          {hsnCodes.length === 0 ? (
            <p className="text-sm text-slate-400">No HSN codes yet.</p>
          ) : (
            <div className="space-y-2">
              {hsnCodes.map((row) => (
                <div
                  key={row.id}
                  className="flex items-center justify-between rounded-xl border border-slate-800 bg-slate-950/50 p-4"
                >
                  <div>
                    <p className="font-semibold text-slate-100">{row.hsnCode}</p>
                    <p className="text-xs text-slate-500">{row.description}</p>
                    <p className="text-xs text-emerald-400 mt-1">
                      CGST {row.cgstPercent}% · SGST {row.sgstPercent}%
                    </p>
                  </div>
                  <CrudRowActions module={APP_MODULES.Catalog} onEdit={() => openEdit(row)} onDelete={() => handleDelete(row)} />
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit HSN Code' : 'Add HSN Code'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label>HSN Code</Label>
              <Input
                value={hsnCode}
                onChange={(e) => setHsnCode(e.target.value)}
                required={!editing}
                disabled={!!editing}
              />
            </div>
            <div className="space-y-2">
              <Label>Description</Label>
              <Input value={description} onChange={(e) => setDescription(e.target.value)} required />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>CGST %</Label>
                <Input type="number" step="0.01" value={cgstPercent} onChange={(e) => setCgstPercent(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>SGST %</Label>
                <Input type="number" step="0.01" value={sgstPercent} onChange={(e) => setSgstPercent(e.target.value)} />
              </div>
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
