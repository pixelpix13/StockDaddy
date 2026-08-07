import React, { useEffect, useState } from 'react';
import { FolderTree, Plus, Tag } from 'lucide-react';
import { catalogService } from '@/services';
import { CategoryDto, SubcategoryDto, HsnMasterDto, TaxRegionDto } from '@/dtos';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Badge } from '@/components/ui/badge';
import { CrudRowActions } from '@/components/common/CrudRowActions';

export const CatalogPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;
  const storeId = user?.storeId || 1;

  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [subcategories, setSubcategories] = useState<SubcategoryDto[]>([]);
  const [hsnCodes, setHsnCodes] = useState<HsnMasterDto[]>([]);
  const [taxRegions, setTaxRegions] = useState<TaxRegionDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [categoryDialogOpen, setCategoryDialogOpen] = useState(false);
  const [subcategoryDialogOpen, setSubcategoryDialogOpen] = useState(false);
  const [hsnDialogOpen, setHsnDialogOpen] = useState(false);
  const [taxDialogOpen, setTaxDialogOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<CategoryDto | null>(null);
  const [editingSubcategory, setEditingSubcategory] = useState<SubcategoryDto | null>(null);
  const [editingHsn, setEditingHsn] = useState<HsnMasterDto | null>(null);
  const [editingTaxRegion, setEditingTaxRegion] = useState<TaxRegionDto | null>(null);
  const [categoryName, setCategoryName] = useState('');
  const [subcategoryName, setSubcategoryName] = useState('');
  const [hsnCode, setHsnCode] = useState('');
  const [hsnDescription, setHsnDescription] = useState('');
  const [cgstPercent, setCgstPercent] = useState('9');
  const [sgstPercent, setSgstPercent] = useState('9');
  const [regionName, setRegionName] = useState('');
  const [taxPercent, setTaxPercent] = useState('18');
  const [selectedCategoryId, setSelectedCategoryId] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadCatalog = async () => {
    setIsLoading(true);
    try {
      const [cats, subs, hsn, tax] = await Promise.all([
        catalogService.getCategories(),
        catalogService.getSubcategories(),
        catalogService.getHsnCodes(),
        catalogService.getTaxRegions(),
      ]);
      setCategories(cats);
      setSubcategories(subs);
      setHsnCodes(hsn);
      setTaxRegions(tax);
      if (cats.length > 0 && !selectedCategoryId) {
        setSelectedCategoryId(String(cats[0].id));
      }
    } catch {
      showToast('error', 'Load Failed', 'Could not load catalog data.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadCatalog();
  }, []);

  const openCreateCategory = () => {
    setEditingCategory(null);
    setCategoryName('');
    setCategoryDialogOpen(true);
  };

  const openEditCategory = (cat: CategoryDto) => {
    setEditingCategory(cat);
    setCategoryName(cat.name);
    setCategoryDialogOpen(true);
  };

  const openCreateSubcategory = () => {
    setEditingSubcategory(null);
    setSubcategoryName('');
    setSubcategoryDialogOpen(true);
  };

  const openEditSubcategory = (sub: SubcategoryDto) => {
    setEditingSubcategory(sub);
    setSubcategoryName(sub.name);
    setSelectedCategoryId(String(sub.categoryId));
    setSubcategoryDialogOpen(true);
  };

  const handleSaveCategory = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!categoryName.trim()) return;
    setIsSubmitting(true);
    try {
      if (editingCategory) {
        await catalogService.updateCategory(editingCategory.id, { name: categoryName.trim() });
        showToast('success', 'Updated', 'Category updated.');
      } else {
        await catalogService.createCategory({ tenantId, storeId, name: categoryName.trim() });
        showToast('success', 'Created', 'Category created.');
      }
      setCategoryDialogOpen(false);
      loadCatalog();
    } catch {
      showToast('error', 'Failed', 'Could not save category.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteCategory = async (cat: CategoryDto) => {
    if (!confirm(`Delete category "${cat.name}"? Subcategories may become orphaned.`)) return;
    try {
      await catalogService.deleteCategory(cat.id);
      showToast('success', 'Deleted', 'Category removed.');
      loadCatalog();
    } catch {
      showToast('error', 'Failed', 'Could not delete category.');
    }
  };

  const handleSaveSubcategory = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!subcategoryName.trim() || !selectedCategoryId) return;
    setIsSubmitting(true);
    try {
      if (editingSubcategory) {
        await catalogService.updateSubcategory(editingSubcategory.id, {
          name: subcategoryName.trim(),
          categoryId: parseInt(selectedCategoryId, 10),
        });
        showToast('success', 'Updated', 'Subcategory updated.');
      } else {
        await catalogService.createSubcategory({
          tenantId,
          storeId,
          categoryId: parseInt(selectedCategoryId, 10),
          name: subcategoryName.trim(),
        });
        showToast('success', 'Created', 'Subcategory created.');
      }
      setSubcategoryDialogOpen(false);
      loadCatalog();
    } catch {
      showToast('error', 'Failed', 'Could not save subcategory.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteSubcategory = async (sub: SubcategoryDto) => {
    if (!confirm(`Delete subcategory "${sub.name}"?`)) return;
    try {
      await catalogService.deleteSubcategory(sub.id);
      showToast('success', 'Deleted', 'Subcategory removed.');
      loadCatalog();
    } catch {
      showToast('error', 'Failed', 'Could not delete subcategory.');
    }
  };

  const getCategoryName = (categoryId: number) =>
    categories.find((c) => c.id === categoryId)?.name || `Category #${categoryId}`;

  const openCreateHsn = () => {
    setEditingHsn(null);
    setHsnCode('');
    setHsnDescription('');
    setCgstPercent('9');
    setSgstPercent('9');
    setHsnDialogOpen(true);
  };

  const openEditHsn = (row: HsnMasterDto) => {
    setEditingHsn(row);
    setHsnCode(row.hsnCode);
    setHsnDescription(row.description);
    setCgstPercent(String(row.cgstPercent));
    setSgstPercent(String(row.sgstPercent));
    setHsnDialogOpen(true);
  };

  const handleSaveHsn = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!hsnDescription.trim()) return;
    setIsSubmitting(true);
    try {
      const cgst = parseFloat(cgstPercent) || 0;
      const sgst = parseFloat(sgstPercent) || 0;
      if (editingHsn) {
        await catalogService.updateHsnCode(editingHsn.id, {
          description: hsnDescription.trim(),
          cgstPercent: cgst,
          sgstPercent: sgst,
        });
        showToast('success', 'Updated', 'HSN code updated.');
      } else {
        if (!hsnCode.trim()) return;
        await catalogService.createHsnCode({
          hsnCode: hsnCode.trim(),
          description: hsnDescription.trim(),
          cgstPercent: cgst,
          sgstPercent: sgst,
        });
        showToast('success', 'Created', 'HSN code created.');
      }
      setHsnDialogOpen(false);
      loadCatalog();
    } catch {
      showToast('error', 'Failed', 'Could not save HSN code.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteHsn = async (row: HsnMasterDto) => {
    if (!confirm(`Delete HSN "${row.hsnCode}"?`)) return;
    try {
      await catalogService.deleteHsnCode(row.id);
      showToast('success', 'Deleted', 'HSN code removed.');
      loadCatalog();
    } catch {
      showToast('error', 'Failed', 'Could not delete HSN code.');
    }
  };

  const openCreateTaxRegion = () => {
    setEditingTaxRegion(null);
    setRegionName('');
    setTaxPercent('18');
    setTaxDialogOpen(true);
  };

  const openEditTaxRegion = (row: TaxRegionDto) => {
    setEditingTaxRegion(row);
    setRegionName(row.regionName);
    setTaxPercent(String(row.taxPercent));
    setTaxDialogOpen(true);
  };

  const handleSaveTaxRegion = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!regionName.trim()) return;
    setIsSubmitting(true);
    try {
      const percent = parseFloat(taxPercent) || 0;
      if (editingTaxRegion) {
        await catalogService.updateTaxRegion(editingTaxRegion.id, {
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
      setTaxDialogOpen(false);
      loadCatalog();
    } catch {
      showToast('error', 'Failed', 'Could not save tax region.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteTaxRegion = async (row: TaxRegionDto) => {
    if (!confirm(`Delete tax region "${row.regionName}"?`)) return;
    try {
      await catalogService.deleteTaxRegion(row.id);
      showToast('success', 'Deleted', 'Tax region removed.');
      loadCatalog();
    } catch {
      showToast('error', 'Failed', 'Could not delete tax region.');
    }
  };

  return (
    <div className="space-y-6">
      <div className="glass-panel p-6 rounded-3xl border border-slate-800">
        <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
          Catalog Setup <FolderTree className="w-6 h-6 text-blue-400" />
        </h1>
        <p className="text-sm text-slate-400 mt-1">
          Full CRUD for categories, subcategories, HSN codes, and tax regions
        </p>
      </div>

      <Tabs defaultValue="categories">
        <TabsList>
          <TabsTrigger value="categories">Categories</TabsTrigger>
          <TabsTrigger value="subcategories">Subcategories</TabsTrigger>
          <TabsTrigger value="hsn">HSN Codes</TabsTrigger>
          <TabsTrigger value="tax">Tax Regions</TabsTrigger>
        </TabsList>

        <TabsContent value="categories">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <div>
                <CardTitle>Categories ({categories.length})</CardTitle>
                <CardDescription>Store #{storeId}</CardDescription>
              </div>
              <Button onClick={openCreateCategory}>
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
                      <CrudRowActions
                        onEdit={() => openEditCategory(cat)}
                        onDelete={() => handleDeleteCategory(cat)}
                      />
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="subcategories">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <div>
                <CardTitle>Subcategories ({subcategories.length})</CardTitle>
                <CardDescription>Nested under categories</CardDescription>
              </div>
              <Button onClick={openCreateSubcategory} disabled={categories.length === 0}>
                <Plus className="w-4 h-4" /> Add Subcategory
              </Button>
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
                        <CrudRowActions
                          onEdit={() => openEditSubcategory(sub)}
                          onDelete={() => handleDeleteSubcategory(sub)}
                        />
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="hsn">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <div>
                <CardTitle>HSN Master ({hsnCodes.length})</CardTitle>
                <CardDescription>GST tax codes for product classification</CardDescription>
              </div>
              <Button onClick={openCreateHsn}>
                <Plus className="w-4 h-4" /> Add HSN
              </Button>
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
                      <CrudRowActions onEdit={() => openEditHsn(row)} onDelete={() => handleDeleteHsn(row)} />
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="tax">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <div>
                <CardTitle>Tax Regions ({taxRegions.length})</CardTitle>
                <CardDescription>Regional tax rates for store #{storeId}</CardDescription>
              </div>
              <Button onClick={openCreateTaxRegion}>
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
                      <CrudRowActions
                        onEdit={() => openEditTaxRegion(row)}
                        onDelete={() => handleDeleteTaxRegion(row)}
                      />
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>

      <Dialog open={categoryDialogOpen} onOpenChange={setCategoryDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingCategory ? 'Edit Category' : 'Add Category'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSaveCategory} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="categoryName">Category Name</Label>
              <Input
                id="categoryName"
                value={categoryName}
                onChange={(e) => setCategoryName(e.target.value)}
                required
              />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setCategoryDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {editingCategory ? 'Update' : 'Create'}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={subcategoryDialogOpen} onOpenChange={setSubcategoryDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingSubcategory ? 'Edit Subcategory' : 'Add Subcategory'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSaveSubcategory} className="space-y-4">
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
                value={subcategoryName}
                onChange={(e) => setSubcategoryName(e.target.value)}
                required
              />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setSubcategoryDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {editingSubcategory ? 'Update' : 'Create'}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={hsnDialogOpen} onOpenChange={setHsnDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingHsn ? 'Edit HSN Code' : 'Add HSN Code'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSaveHsn} className="space-y-4">
            <div className="space-y-2">
              <Label>HSN Code</Label>
              <Input
                value={hsnCode}
                onChange={(e) => setHsnCode(e.target.value)}
                required={!editingHsn}
                disabled={!!editingHsn}
              />
            </div>
            <div className="space-y-2">
              <Label>Description</Label>
              <Input value={hsnDescription} onChange={(e) => setHsnDescription(e.target.value)} required />
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
              <Button type="button" variant="secondary" onClick={() => setHsnDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>{editingHsn ? 'Update' : 'Create'}</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={taxDialogOpen} onOpenChange={setTaxDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingTaxRegion ? 'Edit Tax Region' : 'Add Tax Region'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSaveTaxRegion} className="space-y-4">
            <div className="space-y-2">
              <Label>Region Name</Label>
              <Input value={regionName} onChange={(e) => setRegionName(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>Tax Percent</Label>
              <Input type="number" step="0.01" value={taxPercent} onChange={(e) => setTaxPercent(e.target.value)} />
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => setTaxDialogOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={isSubmitting}>{editingTaxRegion ? 'Update' : 'Create'}</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
};
