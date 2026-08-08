import React from 'react';
import { StoreDto } from '@/dtos/tenant.dto';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';

export interface StoreRoleAssignment {
  storeId: number;
  roleId: number;
  isDefault: boolean;
}

interface StoreRoleAssignmentsEditorProps {
  stores: StoreDto[];
  roles: Array<{ id: number; name: string }>;
  assignments: StoreRoleAssignment[];
  onChange: (assignments: StoreRoleAssignment[]) => void;
  disabled?: boolean;
  fallbackRoleId?: number;
}

export function StoreRoleAssignmentsEditor({
  stores,
  roles,
  assignments,
  onChange,
  disabled = false,
  fallbackRoleId = roles[0]?.id ?? 1,
}: StoreRoleAssignmentsEditorProps) {
  const isSelected = (storeId: number) => assignments.some((a) => a.storeId === storeId);

  const toggleStore = (storeId: number) => {
    if (disabled) return;
    if (isSelected(storeId)) {
      const next = assignments.filter((a) => a.storeId !== storeId);
      if (next.length > 0 && !next.some((a) => a.isDefault)) {
        next[0] = { ...next[0], isDefault: true };
      }
      onChange(next);
      return;
    }

    const next = [
      ...assignments,
      {
        storeId,
        roleId: fallbackRoleId,
        isDefault: assignments.length === 0,
      },
    ];
    onChange(next);
  };

  const setRole = (storeId: number, roleId: number) => {
    onChange(assignments.map((a) => (a.storeId === storeId ? { ...a, roleId } : a)));
  };

  const setLoginStore = (storeId: number) => {
    onChange(
      assignments.map((a) => ({
        ...a,
        isDefault: a.storeId === storeId,
      }))
    );
  };

  if (stores.length === 0) {
    return <p className="text-sm text-muted-foreground">No stores available.</p>;
  }

  const selectedCount = assignments.length;

  return (
    <div className="space-y-3">
      <div className="rounded-lg border border-border overflow-hidden">
        <div className="hidden sm:grid sm:grid-cols-[2.5rem_minmax(0,1fr)_10rem_5.5rem] gap-3 px-3 py-2 bg-muted/40 border-b border-border text-[11px] font-semibold uppercase tracking-wide text-muted-foreground">
          <span className="text-center">On</span>
          <span>Store</span>
          <span>Role at this store</span>
          <span className="text-center">Login</span>
        </div>

        <div className="divide-y divide-border max-h-64 overflow-y-auto">
          {stores.map((store) => {
            const assignment = assignments.find((a) => a.storeId === store.id);
            const selected = !!assignment;

            return (
              <div
                key={store.id}
                className={`grid grid-cols-1 sm:grid-cols-[2.5rem_minmax(0,1fr)_10rem_5.5rem] gap-3 p-3 items-center ${
                  selected ? 'bg-card' : 'bg-muted/20'
                }`}
              >
                <div className="flex sm:justify-center">
                  <input
                    type="checkbox"
                    checked={selected}
                    disabled={disabled}
                    onChange={() => toggleStore(store.id)}
                    aria-label={`Allow access to ${store.name}`}
                    className="h-4 w-4 rounded border-border"
                  />
                </div>

                <div className="min-w-0">
                  <p className="text-sm font-medium text-foreground truncate">{store.name}</p>
                  {store.location ? (
                    <p className="text-xs text-muted-foreground truncate">{store.location}</p>
                  ) : null}
                </div>

                {selected && assignment ? (
                  <>
                    <Select
                      value={String(assignment.roleId)}
                      disabled={disabled}
                      onValueChange={(value) => setRole(store.id, parseInt(value, 10) || fallbackRoleId)}
                    >
                      <SelectTrigger className="h-9 w-full">
                        <SelectValue placeholder="Select role" />
                      </SelectTrigger>
                      <SelectContent>
                        {roles.map((role) => (
                          <SelectItem key={role.id} value={String(role.id)}>
                            {role.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>

                    <div className="flex sm:justify-center">
                      <label className="inline-flex items-center gap-2 cursor-pointer">
                        <input
                          type="radio"
                          name="login-store"
                          checked={assignment.isDefault}
                          disabled={disabled}
                          onChange={() => setLoginStore(store.id)}
                          className="h-4 w-4 border-border text-primary focus:ring-ring"
                        />
                        <span className="text-xs text-muted-foreground sm:sr-only">Login store</span>
                      </label>
                    </div>
                  </>
                ) : (
                  <>
                    <p className="text-xs text-muted-foreground sm:col-span-2">No access</p>
                  </>
                )}
              </div>
            );
          })}
        </div>
      </div>

      <p className="text-xs text-muted-foreground leading-relaxed">
        {selectedCount === 0 ? (
          'Select at least one store. The user will only see data for stores they are assigned to.'
        ) : (
          <>
            <span className="font-medium text-foreground">Role at this store</span> controls what the user can do
            while that store is active in the header.{' '}
            <span className="font-medium text-foreground">Login</span> marks which store they open after sign-in
            ({selectedCount} store{selectedCount === 1 ? '' : 's'} selected).
          </>
        )}
      </p>
    </div>
  );
}

export function assignmentsFromLegacy(
  storeIds: number[],
  roleId: number,
  defaultStoreId?: number | null
): StoreRoleAssignment[] {
  if (storeIds.length === 0) return [];
  const fallbackDefault = defaultStoreId ?? storeIds[0];
  return storeIds.map((storeId) => ({
    storeId,
    roleId,
    isDefault: storeId === fallbackDefault,
  }));
}

export function summarizeAssignments(
  assignments: StoreRoleAssignment[],
  stores: StoreDto[],
  roles: Array<{ id: number; name: string }>
): string {
  if (assignments.length === 0) return 'No store access';
  return assignments
    .map((a) => {
      const store = stores.find((s) => s.id === a.storeId)?.name ?? `#${a.storeId}`;
      const role = roles.find((r) => r.id === a.roleId)?.name ?? `#${a.roleId}`;
      const login = a.isDefault ? ' · login' : '';
      return `${store} (${role}${login})`;
    })
    .join(', ');
}

export function resolveProfileRoleId(
  assignments: StoreRoleAssignment[],
  fallbackRoleId: number
): number {
  const loginStore = assignments.find((a) => a.isDefault) ?? assignments[0];
  return loginStore?.roleId ?? fallbackRoleId;
}
