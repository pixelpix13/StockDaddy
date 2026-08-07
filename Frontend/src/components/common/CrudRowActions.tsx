import { Pencil, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';

interface CrudRowActionsProps {
  onEdit: () => void;
  onDelete: () => void;
  deleteLabel?: string;
}

export function CrudRowActions({ onEdit, onDelete, deleteLabel = 'Delete' }: CrudRowActionsProps) {
  return (
    <div className="flex items-center gap-1">
      <Button type="button" variant="ghost" size="sm" onClick={onEdit} title="Edit">
        <Pencil className="w-4 h-4" />
      </Button>
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
    </div>
  );
}
