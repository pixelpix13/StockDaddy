import { Combobox } from '@/components/ui/combobox';
import { Label } from '@/components/ui/label';
import { VariantStockDto } from '@/dtos';

interface VariantSelectProps {
  variants: VariantStockDto[];
  value: string;
  onValueChange: (value: string) => void;
  label?: string;
  placeholder?: string;
}

export function VariantSelect({
  variants,
  value,
  onValueChange,
  label = 'Product Variant',
  placeholder = 'Search product or SKU…',
}: VariantSelectProps) {
  const options = variants.map((variant) => ({
    value: String(variant.id),
    label: `${variant.productName} · ${variant.skuCode} · $${variant.price.toFixed(2)} · Qty ${variant.quantity}`,
    keywords: `${variant.productName} ${variant.skuCode}`,
  }));

  return (
    <div className="space-y-2">
      <Label>{label}</Label>
      <Combobox
        options={options}
        value={value}
        onValueChange={onValueChange}
        placeholder={placeholder}
        searchPlaceholder="Search by name or SKU…"
        emptyText="No matching variants."
      />
    </div>
  );
}
