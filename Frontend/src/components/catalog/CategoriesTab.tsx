/**
 * Categories tab for Catalog Setup — list + create/edit dialog.
 */
import React, { useState } from 'react';
import { Plus, Tag } from 'lucide-react';
import { catalogService } from '@/services';
import { CategoryDto } from '@/dtos';
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

interface CategoriesTabProps {
  tenantId: number;
  storeId: number;
  categories: CategoryDto[];
  isLoading: boolean;
  onChanged: () => void;
}

export function CategoriesTab({
  tenantId,
  storeId,
  categories,
  isLoading,
  onChanged,
}: CategoriesTabProps) {
  const { showToast } = useToast();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<CategoryDto | null>(null);
  const [name, setName] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const openCreate = () => {
    setEditing(null);
    setName('');
    setDialogOpen(true);
  };

  const openEdit = (row: CategoryDto) => {
    setEditing(row);
    setName(row.name);
    setDialogOpen(true);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    setIsSubmitting(true);
    try {
      if (editing) {
        await catalogService.updateCategory(editing.id, { name: name.trim() });
        showToast('success', 'Updated', 'Category updated.');
      } else {
        await catalogService.createCategory({ tenantId, storeId, name: name.trim() });
        showToast('success', 'Created', 'Category created.');
      }
      setDialogOpen(false);
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not save category.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (row: CategoryDto) => {
    if (!confirm(`Delete category "${row.name}"? Subcategories may become orphaned.`)) return;
    try {
      await catalogService.deleteCategory(row.id);
      showToast('success', 'Deleted', 'Category removed.');
      onChanged();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete category.'));
    }
  };

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <div>
            <CardTitle>Categories ({categories.length})</CardTitle>
            <CardDescription>Store #{storeId}</CardDescription>
          </div>
          <Button onClick={openCreate}>
            <Plus className="w-4 h-4" /> Add Category
          </Button>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-sm text-slate-400">Loading...</p>
          ) : categories.length === 0 ? (
            <p className="text-sm text-slate-400">No categories yet.</p>
          ) : (
            <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
              {categories.map((cat) => (
                <div
                  key={cat.id}
                  className="flex items-start justify-between rounded-xl border border-slate-800 bg-slate-950/50 p-4"
                >
                  <div>
                    <div className="flex items-center gap-2">
                      <Tag className="w-4 h-4 text-blue-400" />
                      <span className="font-semibold text-slate-100">{cat.name}</span>
                    </div>
                    <p className="text-xs text-slate-500 mt-2">ID #{cat.id}</p>
                  </div>
                  <CrudRowActions onEdit={() => openEdit(cat)} onDelete={() => handleDelete(cat)} />
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Category' : 'Add Category'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="categoryName">Category Name</Label>
              <Input
                id="categoryName"
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
