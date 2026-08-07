import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
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
  placeholder = 'Select a variant',
}: VariantSelectProps) {
  return (
    <div className="space-y-2">
      <Label>{label}</Label>
      <Select value={value} onValueChange={onValueChange}>
        <SelectTrigger>
          <SelectValue placeholder={placeholder} />
        </SelectTrigger>
        <SelectContent>
          {variants.map((variant) => (
            <SelectItem key={variant.id} value={String(variant.id)}>
              {variant.productName} · {variant.skuCode} · ${variant.price.toFixed(2)} · Qty{' '}
              {variant.quantity}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
