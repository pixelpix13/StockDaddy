import { useCallback, useEffect, useState } from 'react';
import { catalogService } from '@/services';
import { CategoryDto, HsnMasterDto, SubcategoryDto, TaxRegionDto } from '@/dtos';
import { useToast } from '@/context/ToastContext';
import { getApiErrorMessage } from '@/lib/api-error';

/**
 * Loads all catalog entities used on the Catalog Setup page.
 * Each tab owns its own create/edit/delete dialogs but shares this data source.
 */
export function useCatalogData() {
  const { showToast } = useToast();
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [subcategories, setSubcategories] = useState<SubcategoryDto[]>([]);
  const [hsnCodes, setHsnCodes] = useState<HsnMasterDto[]>([]);
  const [taxRegions, setTaxRegions] = useState<TaxRegionDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const reload = useCallback(async () => {
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
    } catch (err: unknown) {
      showToast('error', 'Load Failed', getApiErrorMessage(err, 'Could not load catalog data.'));
    } finally {
      setIsLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    reload();
  }, [reload]);

  return {
    categories,
    subcategories,
    hsnCodes,
    taxRegions,
    isLoading,
    reload,
  };
}
