/**
 * Subcategories tab — list + create/edit dialog with parent category picker.
 */
import React, { useCallback, useEffect, useState } from 'react';
import { Plus } from 'lucide-react';
import { catalogService } from '@/services';
import { CategoryDto, SubcategoryDto } from '@/dtos';
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
import { Combobox } from '@/components/ui/combobox';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { buildCategoryFilterOptions } from '@/config/list-filters';

interface SubcategoriesTabProps {
  tenantId: number;
  storeId: number;
}

export function SubcategoriesTab({ tenantId, storeId }: SubcategoriesTabProps) {
  const { showToast } = useToast();
  const list = usePagedList<SubcategoryDto>({
    fetchFn: useCallback((query) => catalogService.getSubcategoriesPaged(query), []),
    defaultSortBy: 'name',
    defaultSortDir: 'asc',
  });

  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<SubcategoryDto | null>(null);
  const [name, setName] = useState('');
  const [selectedCategoryId, setSelectedCategoryId] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    catalogService.getCategories().then(setCategories).catch(() => {
      showToast('error', 'Error', 'Failed to load categories for dropdown.');
    });
  }, [showToast]);

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
      list.reload();
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
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not delete subcategory.'));
    }
  };

  const columns: Column<SubcategoryDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-muted-foreground' },
    { header: 'Name', accessor: 'name', sortKey: 'name', className: 'font-medium text-foreground' },
    {
      header: 'Category',
      accessor: (row) => getCategoryName(row.categoryId),
      className: 'text-muted-foreground',
    },
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
            <CardTitle>Subcategories ({list.totalCount})</CardTitle>
            <CardDescription>Nested under categories</CardDescription>
          </div>
          <PermissionGate module={APP_MODULES.Catalog} action="Write">
            <Button onClick={openCreate} disabled={categories.length === 0}>
              <Plus className="w-4 h-4" /> Add Subcategory
            </Button>
          </PermissionGate>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search all columns…"
            emptyMessage="No subcategories yet."
            filters={
              categories.length > 0 ? (
                <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                  <FilterSelect
                    label="Category"
                    options={buildCategoryFilterOptions(categories)}
                    value={list.filters.categoryId}
                    onChange={(value) => list.setFilter('categoryId', value)}
                  />
                </ListFilterBar>
              ) : undefined
            }
          />
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
              <Combobox
                options={categories.map((cat) => ({
                  value: String(cat.id),
                  label: cat.name,
                }))}
                value={selectedCategoryId}
                onValueChange={setSelectedCategoryId}
                placeholder="Select category"
                searchPlaceholder="Search categories…"
                emptyText="No matching categories."
              />
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
