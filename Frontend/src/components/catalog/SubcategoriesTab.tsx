/**
 * Subcategories tab — list + create/edit dialog with parent category picker.
 */
import React, { useEffect, useState } from 'react';
import { Plus } from 'lucide-react';
import { catalogService } from '@/services';
import { CategoryDto, SubcategoryDto } from '@/dtos';
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Badge } from '@/components/ui/badge';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';

interface SubcategoriesTabProps {
  tenantId: number;
  storeId: number;
  categories: CategoryDto[];
  subcategories: SubcategoryDto[];
  onChanged: () => void;
}

export function SubcategoriesTab({
  tenantId,
  storeId,
  categories,
  subcategories,
  onChanged,
}: SubcategoriesTabProps) {
  const { showToast } = useToast();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<SubcategoryDto | null>(null);
  const [name, setName] = useState('');
  const [selectedCategoryId, setSelectedCategoryId] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (categories.length > 0 && !selectedCategoryId) {
      setSelectedCategoryId(String(categories[0].id));
    }
  }, [categories, selectedCategoryId]);

  const getCategoryName = (categoryId: number) =>
    categories.find((c) => c.id === categoryId)?.name || `Category #${categoryId}`;

  const openCreate = () => {
    setEditing(null);
    setName('');
    if (categories.length > 0) setSelectedCategoryId(String(categories[0].id));
    setDialogOpen(true);
  };

  const openEdit = (row: SubcategoryDto) => {
    setEditing(row);
    setName(row.name);
    setSelectedCategoryId(String(row.categoryId));
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim() || !selectedCategoryId) return;
    setIsSubmitting(true);
    try {
      if (editing) {
        await catalogService.updateSubcategory(editing.id, {
          name: name.trim(),
          categoryId: parseInt(selectedCategoryId, 10),
        });
        showToast('success', 'Updated', 'Subcategory updated.');
      } else {
        await catalogService.createSubcategory({
          tenantId,
          storeId,
          categoryId: parseInt(selectedCategoryId, 10),
          name: name.trim(),
        });
        showToast('success', 'Created', 'Subcategory created.');
      }
      setDialogOpen(false);
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not save subcategory.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (row: SubcategoryDto) => {
    if (!confirm(`Delete subcategory "${row.name}"?`)) return;
    try {
      await catalogService.deleteSubcategory(row.id);
      showToast('success', 'Deleted', 'Subcategory removed.');
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete subcategory.'));
    }
  };

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <div>
            <CardTitle>Subcategories ({subcategories.length})</CardTitle>
            <CardDescription>Nested under categories</CardDescription>
          </div>
          <PermissionGate module={APP_MODULES.Catalog} action="Write">
            <Button onClick={openCreate} disabled={categories.length === 0}>
              <Plus className="w-4 h-4" /> Add Subcategory
            </Button>
          </PermissionGate>
        </CardHeader>
        <CardContent>
          {subcategories.length === 0 ? (
            <p className="text-sm text-slate-400">No subcategories yet.</p>
          ) : (
            <div className="space-y-2">
              {subcategories.map((sub) => (
                <div
                  key={sub.id}
                  className="flex items-center justify-between rounded-xl border border-slate-800 bg-slate-950/50 p-4"
                >
                  <div>
                    <p className="font-semibold text-slate-100">{sub.name}</p>
                    <p className="text-xs text-slate-500">
                      {getCategoryName(sub.categoryId)} · ID #{sub.id}
                    </p>
                  </div>
                  <div className="flex items-center gap-2">
                    <Badge variant="secondary">Subcategory</Badge>
                    <CrudRowActions module={APP_MODULES.Catalog} onEdit={() => openEdit(sub)} onDelete={() => handleDelete(sub)} />
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Subcategory' : 'Add Subcategory'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label>Parent Category</Label>
              <Select value={selectedCategoryId} onValueChange={setSelectedCategoryId}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                  {categories.map((cat) => (
                    <SelectItem key={cat.id} value={String(cat.id)}>{cat.name}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="subcategoryName">Subcategory Name</Label>
              <Input
                id="subcategoryName"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {editing ? 'Update' : 'Create'}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </>
  );
}
