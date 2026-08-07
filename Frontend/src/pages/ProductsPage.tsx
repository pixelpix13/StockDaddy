import React, { useEffect, useState } from 'react';
import { Package, Plus } from 'lucide-react';
import { orchestrationService, catalogService, productService, productImageService } from '@/services';
import { VariantStockDto } from '@/dtos';
import { CrudRowActions } from '@/components/common/CrudRowActions';
import { PermissionGate } from '@/components/common/PermissionGate';
import { APP_MODULES } from '@/config/permissions';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
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

  const [products, setProducts] = useState<VariantStockDto[]>([]);
  const [subcategories, setSubcategories] = useState<{ id: number; name: string; categoryId: number }[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [isLoading, setIsLoading] = useState(true);
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

  const loadProducts = async () => {
    setIsLoading(true);
    try {
      const [stock, subs] = await Promise.all([
        orchestrationService.getVariantStock(storeId),
        catalogService.getSubcategories(),
      ]);
      setProducts(stock);
      setSubcategories(subs.map((s) => ({ id: s.id, name: s.name, categoryId: s.categoryId })));
      if (subs.length > 0 && !subcategoryId) setSubcategoryId(String(subs[0].id));
    } catch {
      showToast('error', 'Load Failed', 'Could not load products.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadProducts();
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
      loadProducts();
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
      loadProducts();
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
      loadProducts();
    } catch (err: unknown) {
      showToast('error', 'Failed', getApiErrorMessage(err, 'Could not update product.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const filtered = products.filter(
    (p) =>
      p.productName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      p.skuCode.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div className="glass-panel p-6 rounded-3xl border border-slate-800 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
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
        <CardContent className="pt-6">
          <Input
            placeholder="Search by name or SKU..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="max-w-md"
          />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Catalog ({filtered.length})</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-sm text-slate-400">Loading...</p>
          ) : filtered.length === 0 ? (
            <p className="text-sm text-slate-400">No products yet. Add one from Catalog setup first if needed.</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-slate-800 text-slate-400 text-left">
                    <th className="pb-3 pr-4">Product</th>
                    <th className="pb-3 pr-4">SKU</th>
                    <th className="pb-3 pr-4">Price</th>
                    <th className="pb-3 pr-4">Tax</th>
                    <th className="pb-3 pr-4">Stock</th>
                    <th className="pb-3 pr-4">Category</th>
                    <th className="pb-3">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filtered.map((row) => (
                    <tr key={row.id} className="border-b border-slate-800/60">
                      <td className="py-3 pr-4 font-medium text-slate-100">{row.productName}</td>
                      <td className="py-3 pr-4 font-mono text-xs text-slate-400">{row.skuCode}</td>
                      <td className="py-3 pr-4 text-emerald-400 font-semibold">${row.price.toFixed(2)}</td>
                      <td className="py-3 pr-4">{row.taxPercent}%</td>
                      <td className="py-3 pr-4">
                        <Badge variant={row.quantity <= 5 ? 'destructive' : 'success'}>
                          {row.quantity} units
                        </Badge>
                      </td>
                      <td className="py-3 pr-4 text-slate-400">{row.subcategoryName || '—'}</td>
                      <td className="py-3">
                        <CrudRowActions
                          module={APP_MODULES.Product}
                          onEdit={() => openEdit(row)}
                          onDelete={() => handleDelete(row)}
                        />
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
