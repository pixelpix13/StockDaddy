/**
 * Categories tab for Catalog Setup — list + create/edit dialog.
 */
import React, { useCallback, useState } from 'react';
import { Plus } from 'lucide-react';
import { catalogService } from '@/services';
import { CategoryDto } from '@/dtos';
import { useToast } from '@/context/ToastContext';
import { getApiErrorMessage } from '@/lib/api-error';
import { usePagedList } from '@/hooks/usePagedList';
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
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';

interface CategoriesTabProps {
  tenantId: number;
  storeId: number;
}

export function CategoriesTab({ tenantId, storeId }: CategoriesTabProps) {
  const { showToast } = useToast();
  const list = usePagedList<CategoryDto>({
    fetchFn: useCallback((query) => catalogService.getCategoriesPaged(query), []),
    defaultSortBy: 'name',
    defaultSortDir: 'asc',
  });

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
      list.reload();
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
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete category.'));
    }
  };

  const columns: Column<CategoryDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-muted-foreground' },
    { header: 'Name', accessor: 'name', sortKey: 'name', className: 'font-medium text-foreground' },
    {
      header: 'Actions',
      accessor: (row) => (
        <CrudRowActions module={APP_MODULES.Catalog} onEdit={() => openEdit(row)} onDelete={() => handleDelete(row)} />
      ),
    },
  ];

  return (
    <>
      <Card>
        <CardHeader className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <CardTitle>Categories ({list.totalCount})</CardTitle>
            <CardDescription>Store #{storeId}</CardDescription>
          </div>
          <PermissionGate module={APP_MODULES.Catalog} action="Write">
            <Button onClick={openCreate}>
              <Plus className="w-4 h-4" /> Add Category
            </Button>
          </PermissionGate>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search categories..."
            emptyMessage="No categories yet."
          />
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
