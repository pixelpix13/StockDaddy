import React from 'react';
import { StoreDto } from '@/dtos/tenant.dto';

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

  const setDefault = (storeId: number) => {
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

  return (
    <div className="max-h-56 overflow-y-auto rounded-lg border border-border divide-y divide-border">
      {stores.map((store) => {
        const assignment = assignments.find((a) => a.storeId === store.id);
        const selected = !!assignment;
        return (
          <div key={store.id} className="flex flex-wrap items-center gap-3 p-3">
            <label className="flex items-center gap-2 min-w-[140px]">
              <input
                type="checkbox"
                checked={selected}
                disabled={disabled}
                onChange={() => toggleStore(store.id)}
              />
              <span className="text-sm text-foreground">
                {store.name}
                {store.location ? <span className="text-muted-foreground"> · {store.location}</span> : null}
              </span>
            </label>
            {selected ? (
              <>
                <select
                  value={assignment.roleId}
                  disabled={disabled}
                  onChange={(e) => setRole(store.id, parseInt(e.target.value, 10) || fallbackRoleId)}
                  className="flex-1 min-w-[120px] bg-card/80 border border-border rounded-lg px-2 py-1.5 text-sm"
                >
                  {roles.map((role) => (
                    <option key={role.id} value={role.id}>{role.name}</option>
                  ))}
                </select>
                <button
                  type="button"
                  disabled={disabled}
                  onClick={() => setDefault(store.id)}
                  className={`text-[10px] px-2 py-0.5 rounded-full border whitespace-nowrap ${
                    assignment.isDefault
                      ? 'border-primary text-primary'
                      : 'border-border text-muted-foreground hover:text-foreground'
                  }`}
                >
                  {assignment.isDefault ? 'Default store' : 'Set default'}
                </button>
              </>
            ) : null}
          </div>
        );
      })}
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
  if (assignments.length === 0) return '—';
  return assignments
    .map((a) => {
      const store = stores.find((s) => s.id === a.storeId)?.name ?? `#${a.storeId}`;
      const role = roles.find((r) => r.id === a.roleId)?.name ?? `#${a.roleId}`;
      return `${store} (${role})`;
    })
    .join(', ');
}
