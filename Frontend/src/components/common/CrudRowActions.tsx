import { Pencil, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { usePermissions } from '@/hooks/usePermissions';
import type { AppModule } from '@/config/permissions';

interface CrudRowActionsProps {
  /** Permission module for edit/delete visibility. */
  module: AppModule | string;
  onEdit: () => void;
  onDelete: () => void;
  deleteLabel?: string;
}

/** Edit/delete row actions — only visible when user has Update/Delete on the module. */
export function CrudRowActions({ module, onEdit, onDelete, deleteLabel = 'Delete' }: CrudRowActionsProps) {
  const { hasPermission } = usePermissions();
  const canUpdate = hasPermission(module, 'Update');
  const canDelete = hasPermission(module, 'Delete');

  if (!canUpdate && !canDelete) {
    return null;
  }

  return (
    <div className="flex items-center gap-1.5 sm:gap-2">
      {canUpdate && (
        <Button type="button" variant="ghost" size="sm" onClick={onEdit} title="Edit">
          <Pencil className="w-4 h-4" />
        </Button>
      )}
      {canDelete && (
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={onDelete}
          title={deleteLabel}
          className="text-rose-400 hover:text-rose-300 hover:bg-rose-500/10"
        >
          <Trash2 className="w-4 h-4" />
        </Button>
      )}
    </div>
  );
}
