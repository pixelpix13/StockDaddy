/**
 * Catalog Setup page — thin shell that wires shared data to tab components.
 * Each tab owns its CRUD dialogs; see `components/catalog/*Tab.tsx`.
 */
import React from 'react';
import { FolderTree } from 'lucide-react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { PageHeader } from '@/components/common/PageHeader';
import { CategoriesTab } from '@/components/catalog/CategoriesTab';
import { SubcategoriesTab } from '@/components/catalog/SubcategoriesTab';
import { HsnTab } from '@/components/catalog/HsnTab';
import { TaxRegionsTab } from '@/components/catalog/TaxRegionsTab';
import { useTenantScope } from '@/hooks/useTenantScope';

export const CatalogPage: React.FC = () => {
  const { tenantId, storeId } = useTenantScope();

  return (
    <div className="page-stack">
      <PageHeader
        title="Catalog Setup"
        description="Full CRUD for categories, subcategories, HSN codes, and tax regions"
        icon={<FolderTree className="w-6 h-6 text-blue-400" />}
      />

      <Tabs defaultValue="categories">
        <TabsList>
          <TabsTrigger value="categories">Categories</TabsTrigger>
          <TabsTrigger value="subcategories">Subcategories</TabsTrigger>
          <TabsTrigger value="hsn">HSN Codes</TabsTrigger>
          <TabsTrigger value="tax">Tax Regions</TabsTrigger>
        </TabsList>

        <TabsContent value="categories">
          <CategoriesTab tenantId={tenantId} storeId={storeId} />
        </TabsContent>

        <TabsContent value="subcategories">
          <SubcategoriesTab tenantId={tenantId} storeId={storeId} />
        </TabsContent>

        <TabsContent value="hsn">
          <HsnTab />
        </TabsContent>

        <TabsContent value="tax">
          <TaxRegionsTab tenantId={tenantId} storeId={storeId} />
        </TabsContent>
      </Tabs>
    </div>
  );
};
