/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL?: string;
  readonly VITE_ENABLE_BILL_ADJUSTMENT?: string;
  readonly VITE_BILL_ADJUSTMENT_SECRET_PATH?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
