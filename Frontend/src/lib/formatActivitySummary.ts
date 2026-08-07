import { AuditLogDto } from '@/dtos/activity.dto';

interface ParsedPayload {
  operation?: string;
  data: Record<string, unknown>;
}

export interface ActivitySummary {
  /** One-line narrative, e.g. "Purav updated the Cashier role (#3)." */
  sentence: string;
  /** Extra context shown in the detail view. */
  details: string[];
  /** Raw operation name from the audit payload, if present. */
  operation?: string;
}

const ENTITY_SINGULAR: Record<string, string> = {
  Rbac: 'access control',
  Category: 'category',
  Subcategory: 'subcategory',
  HsnMaster: 'HSN code',
  TaxRegion: 'tax region',
  Product: 'product',
  ProductVariant: 'product variant',
  StockItem: 'inventory item',
  Sale: 'sale',
  SaleItem: 'sale line item',
  PurchaseOrder: 'purchase order',
  PurchaseItem: 'purchase line item',
  Supplier: 'supplier',
  Customer: 'customer',
  User: 'user',
  Store: 'store',
  Tenant: 'tenant',
  Orchestration: 'transaction',
  BillAdjustment: 'bill adjustment',
  Payment: 'payment',
  Return: 'return',
  Refund: 'refund',
  Shipment: 'shipment',
  GiftOption: 'gift option',
  ProductBundle: 'product bundle',
  ProductTag: 'product tag',
  ProductImage: 'product image',
  ProductAttribute: 'product attribute',
  ProductRestockAlert: 'restock alert',
  ScheduledPriceRevert: 'scheduled price revert',
  AdjustedInvoice: 'adjusted invoice',
  IntegrationEvent: 'integration event',
  Role: 'role',
  Permission: 'permission',
  RolePermission: 'role permission',
};

const ACTION_VERB: Record<string, string> = {
  Create: 'created',
  Update: 'updated',
  Delete: 'deleted',
};

const NAME_KEYS = [
  'name',
  'Name',
  'username',
  'Username',
  'productName',
  'ProductName',
  'variantName',
  'VariantName',
  'title',
  'Title',
  'description',
  'Description',
  'skuCode',
  'SkuCode',
  'barcode',
  'Barcode',
  'hsnCode',
  'HSNCode',
  'email',
  'Email',
  'location',
  'Location',
  'paymentMethod',
  'PaymentMethod',
  'status',
  'Status',
];

function parsePayload(raw: string): ParsedPayload {
  if (!raw.trim()) {
    return { data: {} };
  }

  const colonIndex = raw.indexOf(':');
  if (colonIndex < 0) {
    return { data: {} };
  }

  const operation = raw.slice(0, colonIndex).trim();
  const jsonPart = raw.slice(colonIndex + 1).trim();

  try {
    const data = JSON.parse(jsonPart) as Record<string, unknown>;
    return { operation, data };
  } catch {
    return { operation, data: {} };
  }
}

function isPlainObject(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null && !Array.isArray(value);
}

function findFirstString(obj: unknown, keys: string[] = NAME_KEYS): string | undefined {
  if (!isPlainObject(obj)) return undefined;

  for (const key of keys) {
    const value = obj[key];
    if (typeof value === 'string' && value.trim()) {
      return value.trim();
    }
  }

  for (const value of Object.values(obj)) {
    if (isPlainObject(value)) {
      const nested = findFirstString(value, keys);
      if (nested) return nested;
    }
  }

  return undefined;
}

function getEntityLabel(tableName: string): string {
  return ENTITY_SINGULAR[tableName] ?? tableName.replace(/([a-z])([A-Z])/g, '$1 $2').toLowerCase();
}

function getRecordRef(recordId: string): string {
  if (!recordId || recordId === 'n/a') return '';
  return ` (#${recordId})`;
}

function capitalizeFirst(text: string): string {
  if (!text) return text;
  return text.charAt(0).toUpperCase() + text.slice(1);
}

function displayUser(log: AuditLogDto): string {
  return log.username || (log.userId ? `User #${log.userId}` : 'System');
}

function countItems(data: Record<string, unknown>, key: string): number | undefined {
  const value = data[key] ?? (isPlainObject(data.request) ? data.request[key] : undefined);
  if (Array.isArray(value)) return value.length;
  return undefined;
}

function pickId(data: Record<string, unknown>, ...keys: string[]): string | undefined {
  for (const key of keys) {
    const value = data[key];
    if (value != null && value !== '') return String(value);
    if (isPlainObject(data.request) && data.request[key] != null) {
      return String(data.request[key]);
    }
  }
  return undefined;
}

function describeOperation(
  operation: string | undefined,
  action: string,
  tableName: string,
  recordId: string,
  data: Record<string, unknown>,
  label?: string
): { object: string; details: string[] } {
  const entity = getEntityLabel(tableName);
  const ref = getRecordRef(recordId);
  const details: string[] = [];

  const path = typeof data.path === 'string' ? data.path : undefined;
  if (path) details.push(`API: ${path}`);

  switch (operation) {
    case 'CreateRole':
      return {
        object: label ? `a new role "${label}"` : 'a new role',
        details: label ? [`Role name: ${label}`, ...details] : details,
      };

    case 'UpdateRole':
      return {
        object: label ? `the "${label}" role${ref}` : `role${ref}`,
        details: label ? [`Renamed or updated role to "${label}"`, ...details] : details,
      };

    case 'DeleteRole':
      return {
        object: label ? `the "${label}" role${ref}` : `role${ref}`,
        details,
      };

    case 'UpdateRolePermissions': {
      const roleId = pickId(data, 'roleId') ?? recordId;
      const permCount = countItems(data, 'permissionIds') ?? countItems(data, 'permissions');
      details.push(`Role ID: ${roleId}`);
      if (permCount != null) details.push(`${permCount} permission(s) assigned`);
      return {
        object: `permissions for role #${roleId}`,
        details,
      };
    }

    case 'AssignUserRole': {
      const userId = pickId(data, 'userId') ?? recordId;
      const roleId = pickId(data, 'roleId');
      if (roleId) details.push(`Assigned role ID: ${roleId}`);
      return {
        object: `the role for user #${userId}`,
        details,
      };
    }

    case 'Checkout': {
      const itemCount = countItems(data, 'items') ?? countItems(data, 'Items');
      const payment =
        findFirstString(data, ['paymentMethod', 'PaymentMethod']) ??
        (isPlainObject(data.request) ? findFirstString(data.request, ['paymentMethod', 'PaymentMethod']) : undefined);
      if (itemCount != null) details.push(`${itemCount} item(s) in cart`);
      if (payment) details.push(`Payment: ${payment}`);
      return {
        object: itemCount != null ? `a sale checkout with ${itemCount} item(s)${ref}` : `a sale checkout${ref}`,
        details,
      };
    }

    case 'CreateProductWithVariant': {
      if (label) details.push(`Product: ${label}`);
      const sku = findFirstString(data, ['skuCode', 'SkuCode']);
      if (sku) details.push(`SKU: ${sku}`);
      return {
        object: label ? `product "${label}" with its first variant${ref}` : `a new product with variant${ref}`,
        details,
      };
    }

    case 'AdjustStock': {
      const qty =
        data.quantityChange ??
        data.QuantityChange ??
        (isPlainObject(data.request) ? data.request.quantityChange ?? data.request.QuantityChange : undefined);
      const variantId = pickId(data, 'productVariantId', 'ProductVariantId');
      if (qty != null) {
        const n = Number(qty);
        if (!Number.isNaN(n) && n !== 0) {
          details.push(`${n > 0 ? 'Added' : 'Removed'} ${Math.abs(n)} unit(s)`);
          if (variantId) details.push(`Variant ID: ${variantId}`);
          return {
            object: n > 0 ? `stock — added ${Math.abs(n)} unit(s)${ref}` : `stock — removed ${Math.abs(n)} unit(s)${ref}`,
            details,
          };
        }
      }
      return { object: `stock levels${ref}`, details };
    }

    case 'CreatePurchaseOrderWithItems': {
      const itemCount = countItems(data, 'items') ?? countItems(data, 'Items');
      if (itemCount != null) details.push(`${itemCount} line item(s)`);
      return {
        object: itemCount != null ? `purchase order with ${itemCount} item(s)${ref}` : `a purchase order${ref}`,
        details,
      };
    }

    case 'ReceivePurchaseOrder':
      return { object: `purchase order${ref} as received`, details: ['Marked goods as received into inventory', ...details] };

    default:
      break;
  }

  const verb = ACTION_VERB[action] ?? action.toLowerCase();

  if (action === 'Create') {
    return {
      object: label ? `a new ${entity} "${label}"${ref}` : `a new ${entity}${ref}`,
      details: label ? [`Name: ${label}`, ...details] : details,
    };
  }

  if (action === 'Update') {
    return {
      object: label ? `${entity} "${label}"${ref}` : `${entity}${ref}`,
      details: label ? [`Updated: ${label}`, ...details] : details,
    };
  }

  if (action === 'Delete') {
    return {
      object: label ? `${entity} "${label}"${ref}` : `${entity}${ref}`,
      details,
    };
  }

  return {
    object: `${entity}${ref}`,
    details: [`Operation: ${operation ?? verb}`, ...details],
  };
}

/**
 * Builds a human-readable activity summary from an audit log row.
 */
export function formatActivitySummary(log: AuditLogDto): ActivitySummary {
  const user = displayUser(log);
  const verb = ACTION_VERB[log.action] ?? log.action.toLowerCase();
  const { operation, data } = parsePayload(log.newData);
  const label = findFirstString(data);

  const { object, details } = describeOperation(
    operation,
    log.action,
    log.tableName,
    log.recordId,
    data,
    label
  );

  const sentence = `${capitalizeFirst(user)} ${verb} ${object}.`;

  if (operation && !details.some((d) => d.startsWith('Operation:'))) {
    details.unshift(`Action: ${humanizeOperationName(operation)}`);
  }

  return { sentence, details, operation };
}

function humanizeOperationName(operation: string): string {
  return operation
    .replace(/([a-z])([A-Z])/g, '$1 $2')
    .replace(/_/g, ' ')
    .toLowerCase();
}

/** Short preview for table cells (truncated sentence). */
export function formatActivityPreview(log: AuditLogDto, maxLength = 120): string {
  const { sentence } = formatActivitySummary(log);
  if (sentence.length <= maxLength) return sentence;
  return `${sentence.slice(0, maxLength - 1)}…`;
}
