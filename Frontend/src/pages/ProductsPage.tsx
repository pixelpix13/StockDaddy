import React, { useCallback, useEffect, useState } from 'react';
import { Package, Plus } from 'lucide-react';
import { orchestrationService, catalogService, productService, productImageService } from '@/services';
import { VariantStockDto } from '@/dtos';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { STOCK_FILTER_OPTIONS, buildSubcategoryFilterOptions } from '@/config/list-filters';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
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
import { getApiErrorMessage } from '@/lib/api-error';

export const ProductsPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;
  const storeId = user?.storeId || 1;

  const list = usePagedList<VariantStockDto>({
    fetchFn: useCallback(
      (query) => orchestrationService.getVariantStockPaged(query, storeId),
      [storeId]
    ),
    defaultSortBy: 'productname',
    defaultSortDir: 'asc',
  });

  const [subcategories, setSubcategories] = useState<{ id: number; name: string; categoryId: number }[]>([]);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [editingRow, setEditingRow] = useState<VariantStockDto | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [name, setName] = useState('');
  const [sku, setSku] = useState('');
  const [price, setPrice] = useState('');
  const [costPrice, setCostPrice] = useState('');
  const [taxPercent, setTaxPercent] = useState('18');
  const [initialQuantity, setInitialQuantity] = useState('0');
  const [subcategoryId, setSubcategoryId] = useState('');
  const [description, setDescription] = useState('');
  const [imageUrl, setImageUrl] = useState('');

  useEffect(() => {
    catalogService.getSubcategories().then((subs) => {
      setSubcategories(subs.map((s) => ({ id: s.id, name: s.name, categoryId: s.categoryId })));
      if (subs.length > 0 && !subcategoryId) setSubcategoryId(String(subs[0].id));
    }).catch(() => {
      showToast('error', 'Load Failed', 'Could not load subcategories.');
    });
  }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      const result = await orchestrationService.createProductWithVariant({
        tenantId,
        storeId,
        subcategoryId: subcategoryId ? parseInt(subcategoryId, 10) : undefined,
        name,
        description,
        skuCode: sku,
        costPrice: parseFloat(costPrice),
        price: parseFloat(price),
        taxPercent: parseFloat(taxPercent),
        initialQuantity: parseInt(initialQuantity, 10) || 0,
        hsnCodeId: 1,
      });
      if (imageUrl.trim()) {
        await productImageService.createProductImage({
          productId: result.product.id,
          imageUrl: imageUrl.trim(),
          isPrimary: true,
        });
      }
      showToast('success', 'Product Created', `"${name}" added with variant and stock.`);
      setDialogOpen(false);
      setName('');
      setSku('');
      setPrice('');
      setCostPrice('');
      setDescription('');
      setImageUrl('');
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not create product.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (row: VariantStockDto) => {
    if (!confirm(`Delete product "${row.productName}"?`)) return;
    try {
      await productService.deleteProduct(row.productId);
      showToast('success', 'Deleted', 'Product removed.');
      list.reload();
    } catch {
      showToast('error', 'Failed', 'Could not delete product.');
    }
  };

  const openEdit = (row: VariantStockDto) => {
    setEditingRow(row);
    setName(row.productName);
    setSku(row.skuCode);
    setPrice(String(row.price));
    setCostPrice(String(row.costPrice));
    setTaxPercent(String(row.taxPercent));
    setInitialQuantity(String(row.quantity));
    setSubcategoryId(row.subcategoryId ? String(row.subcategoryId) : subcategoryId);
    setEditDialogOpen(true);
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingRow) return;
    setIsSubmitting(true);
    try {
      const cost = parseFloat(costPrice);
      const retail = parseFloat(price);
      await productService.updateProduct(editingRow.productId, {
        name,
        subcategoryId: subcategoryId ? parseInt(subcategoryId, 10) : undefined,
        storeId,
        description,
        unit: 'pcs',
      });
      await productService.updateProductVariant(editingRow.id, {
        variantName: name,
        skuCode: sku,
        barcode: sku,
        costPrice: cost,
        price: retail,
        taxPercent: parseFloat(taxPercent),
        quantity: parseInt(initialQuantity, 10) || editingRow.quantity,
        marginPercent: cost > 0 ? ((retail - cost) / cost) * 100 : 0,
        hsnCodeId: editingRow.hsnCodeId || 1,
      });
      showToast('success', 'Updated', 'Product and variant saved.');
      setEditDialogOpen(false);
      list.reload();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not update product.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const columns: Column<VariantStockDto>[] = [
    { header: 'ID', accessor: (row) => `#${row.id}`, sortKey: 'id', className: 'font-mono text-xs text-slate-500' },
    { header: 'Product', accessor: 'productName', sortKey: 'productname', className: 'font-medium text-slate-100' },
    { header: 'SKU', accessor: 'skuCode', sortKey: 'skucode', className: 'font-mono text-xs text-slate-400' },
    {
      header: 'Price',
      accessor: (row) => `$${row.price.toFixed(2)}`,
      sortKey: 'price',
      className: 'text-emerald-400 font-semibold',
    },
    { header: 'Tax', accessor: (row) => `${row.taxPercent}%` },
    {
      header: 'Stock',
      accessor: (row) => (
        <Badge variant={row.quantity <= 5 ? 'destructive' : 'success'}>
          {row.quantity} units
        </Badge>
      ),
      sortKey: 'quantity',
    },
    { header: 'Category', accessor: (row) => row.subcategoryName || '—', className: 'text-slate-400' },
    {
      header: 'Actions',
      accessor: (row) => (
        <CrudRowActions
          module={APP_MODULES.Product}
          onEdit={() => openEdit(row)}
          onDelete={() => handleDelete(row)}
        />
      ),
    },
  ];

  return (
    <div className="page-stack">
      <div className="page-hero flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="page-hero-title">
            Products & Variants <Package className="w-6 h-6 text-blue-400" />
          </h1>
          <p className="text-sm text-slate-400 mt-1">
            Products are created with a sellable variant, SKU, tax %, and opening stock
          </p>
        </div>
        <PermissionGate module={APP_MODULES.Product} action="Write">
          <Button onClick={() => setDialogOpen(true)}>
            <Plus className="w-4 h-4" /> Add Product
          </Button>
        </PermissionGate>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Catalog ({list.totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search all columns…"
            emptyMessage="No products yet. Add one from Catalog setup first if needed."
            filters={
              <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                <FilterSelect
                  label="Stock"
                  options={STOCK_FILTER_OPTIONS}
                  value={list.filters.stockFilter}
                  onChange={(value) => list.setFilter('stockFilter', value)}
                />
                {subcategories.length > 0 ? (
                  <FilterSelect
                    label="Subcategory"
                    options={buildSubcategoryFilterOptions(subcategories)}
                    value={list.filters.subcategoryId}
                    onChange={(value) => list.setFilter('subcategoryId', value)}
                  />
                ) : null}
              </ListFilterBar>
            }
          />
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Add Product with Variant</DialogTitle>
            <DialogDescription>
              Creates catalog product + sellable variant + optional opening stock in one step.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleCreate} className="space-y-4">
            <div className="space-y-2">
              <Label>Subcategory</Label>
              <Select value={subcategoryId} onValueChange={setSubcategoryId}>
                <SelectTrigger>
                  <SelectValue placeholder="Select subcategory" />
                </SelectTrigger>
                <SelectContent>
                  {subcategories.map((sub) => (
                    <SelectItem key={sub.id} value={String(sub.id)}>
                      {sub.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Product Name</Label>
              <Input value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>SKU Code</Label>
                <Input value={sku} onChange={(e) => setSku(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>Opening Stock</Label>
                <Input type="number" min="0" value={initialQuantity} onChange={(e) => setInitialQuantity(e.target.value)} />
              </div>
            </div>
            <div className="grid grid-cols-3 gap-4">
              <div className="space-y-2">
                <Label>Cost ($)</Label>
                <Input type="number" step="0.01" value={costPrice} onChange={(e) => setCostPrice(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>Price ($)</Label>
                <Input type="number" step="0.01" value={price} onChange={(e) => setPrice(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>Tax %</Label>
                <Input type="number" step="0.01" value={taxPercent} onChange={(e) => setTaxPercent(e.target.value)} />
              </div>
            </div>
            <div className="space-y-2">
              <Label>Description</Label>
              <Textarea value={description} onChange={(e) => setDescription(e.target.value)} rows={3} />
            </div>
            <div className="space-y-2">
              <Label>Product Image URL (optional)</Label>
              <Input
                type="url"
                value={imageUrl}
                onChange={(e) => setImageUrl(e.target.value)}
                placeholder="https://example.com/product.jpg"
              />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>Save Product</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Edit Product & Variant</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleUpdate} className="space-y-4">
            <div className="space-y-2">
              <Label>Subcategory</Label>
              <Select value={subcategoryId} onValueChange={setSubcategoryId}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                  {subcategories.map((sub) => (
                    <SelectItem key={sub.id} value={String(sub.id)}>{sub.name}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Product Name</Label>
              <Input value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>SKU</Label>
                <Input value={sku} onChange={(e) => setSku(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>Stock Qty</Label>
                <Input type="number" min="0" value={initialQuantity} onChange={(e) => setInitialQuantity(e.target.value)} />
              </div>
            </div>
            <div className="grid grid-cols-3 gap-4">
              <div className="space-y-2">
                <Label>Cost ($)</Label>
                <Input type="number" step="0.01" value={costPrice} onChange={(e) => setCostPrice(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>Price ($)</Label>
                <Input type="number" step="0.01" value={price} onChange={(e) => setPrice(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>Tax %</Label>
                <Input type="number" step="0.01" value={taxPercent} onChange={(e) => setTaxPercent(e.target.value)} />
              </div>
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setEditDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>Update</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
};
