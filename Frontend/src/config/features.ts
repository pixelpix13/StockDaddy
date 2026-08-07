/** Feature flags — set via VITE_* env vars. Remove modules by setting false and deleting their folders. */
export const FEATURES = {
  billAdjustment: import.meta.env.VITE_ENABLE_BILL_ADJUSTMENT !== 'false',
  /** Hidden route — not linked in nav. Override in .env.local (VITE_BILL_ADJUSTMENT_SECRET_PATH). */
  billAdjustmentSecretPath:
    import.meta.env.VITE_BILL_ADJUSTMENT_SECRET_PATH || '/x/sd-ba-8k2m',
} as const;
