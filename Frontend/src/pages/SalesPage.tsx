import React, { useEffect, useMemo, useRef, useState } from 'react';
import { ShoppingCart, Plus, Trash2, Receipt, ScanBarcode } from 'lucide-react';
import { orchestrationService, saleService } from '@/services';
import { VariantStockDto, PaymentMethod } from '@/dtos';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Badge } from '@/components/ui/badge';
import { VariantSelect } from '@/components/catalog/VariantSelect';
import { getApiErrorMessage } from '@/lib/api-error';

interface CartLine {
  variant: VariantStockDto;
  quantity: number;
}

export const SalesPage: React.FC = () => {
  const { user } = useAuth();
  const { showToast } = useToast();
  const tenantId = user?.tenantId || 1;
  const storeId = user?.storeId || 1;
  const barcodeRef = useRef<HTMLInputElement>(null);

  const [variants, setVariants] = useState<VariantStockDto[]>([]);
  const [sales, setSales] = useState<Awaited<ReturnType<typeof saleService.getSales>>>([]);
  const [cart, setCart] = useState<CartLine[]>([]);
  const [selectedVariantId, setSelectedVariantId] = useState('');
  const [quantity, setQuantity] = useState('1');
  const [barcodeInput, setBarcodeInput] = useState('');
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>('Cash');
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadData = async () => {
    setIsLoading(true);
    try {
      const [stock, salesData] = await Promise.all([
        orchestrationService.getVariantStock(storeId),
        saleService.getSales(),
      ]);
      setVariants(stock);
      setSales(salesData);
      if (stock.length > 0 && !selectedVariantId) setSelectedVariantId(String(stock[0].id));
    } catch {
      showToast('error', 'Load Failed', 'Could not load POS data.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const totals = useMemo(() => {
    let subtotal = 0;
    let tax = 0;
    cart.forEach((line) => {
      const lineSub = line.variant.price * line.quantity;
      const lineTax = lineSub * (line.variant.taxPercent / 100);
      subtotal += lineSub;
      tax += lineTax;
    });
    return { subtotal, tax, total: subtotal + tax };
  }, [cart]);

  const addVariantToCart = (variant: VariantStockDto, qty = 1) => {
    if (qty > variant.quantity) {
      showToast('warning', 'Insufficient Stock', `Only ${variant.quantity} units available.`);
      return;
    }
    setCart((prev) => {
      const existing = prev.find((l) => l.variant.id === variant.id);
      if (existing) {
        const newQty = existing.quantity + qty;
        if (newQty > variant.quantity) {
          showToast('warning', 'Insufficient Stock', `Only ${variant.quantity} units available.`);
          return prev;
        }
        return prev.map((l) =>
          l.variant.id === variant.id ? { ...l, quantity: newQty } : l
        );
      }
      return [...prev, { variant, quantity: qty }];
    });
  };

  const addToCart = () => {
    const variant = variants.find((v) => String(v.id) === selectedVariantId);
    const qty = parseInt(quantity, 10) || 1;
    if (!variant) return;
    addVariantToCart(variant, qty);
    setQuantity('1');
  };

  const handleBarcodeSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const code = barcodeInput.trim();
    if (!code) return;
    try {
      const variant = await orchestrationService.getVariantByBarcode(code, storeId);
      addVariantToCart(variant, 1);
      setBarcodeInput('');
      showToast('success', 'Scanned', `${variant.productName} added to cart.`);
    } catch {
      showToast('error', 'Not Found', `No product for barcode/SKU "${code}".`);
    } finally {
      barcodeRef.current?.focus();
    }
  };

  const removeFromCart = (variantId: number) => {
    setCart((prev) => prev.filter((l) => l.variant.id !== variantId));
  };

  const handleCheckout = async () => {
    if (cart.length === 0) {
      showToast('warning', 'Empty Cart', 'Add items before checkout.');
      return;
    }
    setIsSubmitting(true);
    try {
      const result = await orchestrationService.checkout({
        tenantId,
        storeId,
        soldBy: user?.id || 1,
        paymentMethod,
        items: cart.map((line) => ({
          productVariantId: line.variant.id,
          quantity: line.quantity,
        })),
      });
      showToast(
        'success',
        'Sale Complete',
        `#${result.saleId} · Total $${result.totalAmount.toFixed(2)} (Tax $${result.taxAmount.toFixed(2)})`
      );
      setCart([]);
      loadData();
      barcodeRef.current?.focus();
    } catch (err: unknown) {
      showToast('error', 'Checkout Failed', getApiErrorMessage(err, 'Transaction failed.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="glass-panel p-6 rounded-3xl border border-slate-800">
        <h1 className="text-2xl font-extrabold text-white flex items-center gap-2">
          Sales & POS <ShoppingCart className="w-6 h-6 text-blue-400" />
        </h1>
        <p className="text-sm text-slate-400 mt-1">
          Scan barcodes, build a cart, and checkout with automatic stock deduction
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>POS Cart</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <form onSubmit={handleBarcodeSubmit} className="space-y-2">
              <Label className="flex items-center gap-2">
                <ScanBarcode className="w-4 h-4 text-blue-400" />
                Barcode / SKU Scanner
              </Label>
              <Input
                ref={barcodeRef}
                value={barcodeInput}
                onChange={(e) => setBarcodeInput(e.target.value)}
                placeholder="Scan or type SKU, press Enter"
                autoComplete="off"
              />
              <p className="text-[11px] text-slate-500">
                USB scanners work here — focus this field and scan. Camera scanning can be added in Phase 2.
              </p>
            </form>

            <VariantSelect
              variants={variants}
              value={selectedVariantId}
              onValueChange={setSelectedVariantId}
            />
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>Quantity</Label>
                <Input type="number" min="1" value={quantity} onChange={(e) => setQuantity(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>Payment</Label>
                <Select value={paymentMethod} onValueChange={(v) => setPaymentMethod(v as PaymentMethod)}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Cash">Cash</SelectItem>
                    <SelectItem value="Card">Card</SelectItem>
                    <SelectItem value="UPI">UPI</SelectItem>
                    <SelectItem value="BankTransfer">Bank Transfer</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
            <Button type="button" variant="secondary" className="w-full" onClick={addToCart}>
              <Plus className="w-4 h-4" /> Add to Cart
            </Button>

            {cart.length === 0 ? (
              <p className="text-sm text-slate-500 text-center py-6">Cart is empty — scan a barcode to start</p>
            ) : (
              <div className="space-y-2">
                {cart.map((line) => {
                  const lineSub = line.variant.price * line.quantity;
                  const lineTax = lineSub * (line.variant.taxPercent / 100);
                  return (
                    <div
                      key={line.variant.id}
                      className="flex items-center justify-between rounded-lg border border-slate-800 p-3"
                    >
                      <div>
                        <p className="text-sm font-medium text-slate-100">{line.variant.productName}</p>
                        <p className="text-xs text-slate-400">
                          {line.variant.skuCode} · {line.quantity} × ${line.variant.price.toFixed(2)} +{' '}
                          {line.variant.taxPercent}% tax
                        </p>
                      </div>
                      <div className="flex items-center gap-3">
                        <span className="text-sm font-semibold text-emerald-400">
                          ${(lineSub + lineTax).toFixed(2)}
                        </span>
                        <button
                          onClick={() => removeFromCart(line.variant.id)}
                          className="text-slate-500 hover:text-rose-400"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}

            <div className="rounded-xl border border-slate-800 p-4 space-y-2 text-sm">
              <div className="flex justify-between text-slate-400">
                <span>Subtotal</span>
                <span>${totals.subtotal.toFixed(2)}</span>
              </div>
              <div className="flex justify-between text-slate-400">
                <span>Tax</span>
                <span>${totals.tax.toFixed(2)}</span>
              </div>
              <div className="flex justify-between font-bold text-white text-base pt-2 border-t border-slate-800">
                <span>Total</span>
                <span>${totals.total.toFixed(2)}</span>
              </div>
            </div>

            <Button className="w-full" onClick={handleCheckout} disabled={isSubmitting || cart.length === 0}>
              <Receipt className="w-4 h-4" /> Process Checkout
            </Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Sales History ({sales.length})</CardTitle>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <p className="text-sm text-slate-400">Loading...</p>
            ) : sales.length === 0 ? (
              <p className="text-sm text-slate-400">No sales yet.</p>
            ) : (
              <div className="space-y-2 max-h-[520px] overflow-y-auto">
                {sales.map((sale) => (
                  <div
                    key={sale.id}
                    className="flex items-center justify-between rounded-lg border border-slate-800 p-3 gap-3"
                  >
                    <div>
                      <p className="font-mono text-xs text-blue-400">#SALE-{sale.id}</p>
                      <p className="text-xs text-slate-500">
                        {new Date(sale.createdAt).toLocaleString()} · Cashier #{sale.soldBy}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="font-bold text-emerald-400">${sale.totalAmount.toFixed(2)}</p>
                      <Badge variant="secondary">{sale.paymentMethod}</Badge>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
};
