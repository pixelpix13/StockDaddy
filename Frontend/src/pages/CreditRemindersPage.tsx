import React, { useCallback, useState } from 'react';
import { Wallet, DollarSign } from 'lucide-react';
import { creditService } from '@/services';
import { CreditLedgerDto } from '@/dtos/credit.dto';
import { usePagedList } from '@/hooks/usePagedList';
import { PagedDataTable, Column } from '@/components/common/PagedDataTable';
import { FilterSelect, ListFilterBar } from '@/components/common/ListFilters';
import { CREDIT_PARTY_TYPE_OPTIONS, CREDIT_STATUS_OPTIONS } from '@/config/list-filters';
import { PageHeader } from '@/components/common/PageHeader';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';

function statusVariant(status: string): 'default' | 'secondary' | 'destructive' {
  switch (status) {
    case 'Paid':
      return 'default';
    case 'Overdue':
      return 'destructive';
    default:
      return 'secondary';
  }
}

function formatDate(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return '—';
  return date.toLocaleDateString();
}

export const CreditRemindersPage: React.FC = () => {
  const list = usePagedList<CreditLedgerDto>({
    fetchFn: useCallback((query) => creditService.getCreditPaged(query), []),
    defaultSortBy: 'duedate',
    defaultSortDir: 'asc',
  });

  const [selected, setSelected] = useState<CreditLedgerDto | null>(null);
  const [paymentAmount, setPaymentAmount] = useState('');
  const [paymentNotes, setPaymentNotes] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const openPayment = (row: CreditLedgerDto) => {
    setSelected(row);
    setPaymentAmount(String(Math.max(row.balanceDue, 0)));
    setPaymentNotes('');
  };

  const handleRecordPayment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selected) return;
    const amount = parseFloat(paymentAmount);
    if (!amount || amount <= 0) return;
    setIsSubmitting(true);
    try {
      await creditService.recordPayment(selected.id, {
        amount,
        notes: paymentNotes.trim() || undefined,
      });
      setSelected(null);
      list.reload();
    } finally {
      setIsSubmitting(false);
    }
  };

  const columns: Column<CreditLedgerDto>[] = [
    {
      header: 'Party',
      accessor: (row) => (
        <div>
          <p className="font-medium text-slate-100">{row.partyName}</p>
          <p className="text-xs text-slate-500">
            {row.partyType === 'Customer' ? 'Collect from customer' : 'Pay supplier'}
            {row.partyPhone ? ` · ${row.partyPhone}` : ''}
          </p>
        </div>
      ),
      sortKey: 'partyname',
    },
    {
      header: 'Contact',
      accessor: (row) => (
        <div className="text-xs text-slate-400">
          <p>{row.partyEmail || '—'}</p>
          <p className="text-slate-500 truncate max-w-[180px]">{row.partyAddress || '—'}</p>
        </div>
      ),
    },
    {
      header: 'Linked',
      accessor: (row) => (
        <span className="text-xs font-mono text-slate-400">
          {row.saleId ? `Sale #${row.saleId}` : row.purchaseOrderId ? `PO #${row.purchaseOrderId}` : '—'}
        </span>
      ),
    },
    {
      header: 'Due',
      accessor: (row) => (
        <div>
          <p className="text-sm text-slate-200">{formatDate(row.dueDate)}</p>
          <p className={`text-xs ${row.isOverdue ? 'text-rose-400' : 'text-slate-500'}`}>
            {row.isOverdue
              ? `${Math.abs(row.daysUntilDue)} day(s) overdue`
              : row.daysUntilDue === 0
                ? 'Due today'
                : `${row.daysUntilDue} day(s) left`}
          </p>
        </div>
      ),
      sortKey: 'duedate',
    },
    {
      header: 'Balance',
      accessor: (row) => (
        <div>
          <p className="font-semibold text-emerald-400">${row.balanceDue.toFixed(2)}</p>
          <p className="text-xs text-slate-500">
            of ${row.amount.toFixed(2)} · paid ${row.amountPaid.toFixed(2)}
          </p>
        </div>
      ),
      sortKey: 'amount',
    },
    {
      header: 'Status',
      accessor: (row) => <Badge variant={statusVariant(row.status)}>{row.status}</Badge>,
    },
    {
      header: '',
      accessor: (row) =>
        row.status !== 'Paid' ? (
          <Button type="button" size="sm" variant="secondary" onClick={() => openPayment(row)}>
            Record payment
          </Button>
        ) : null,
      align: 'right',
    },
  ];

  const overdueCount = list.items.filter((row) => row.isOverdue && row.status !== 'Paid').length;

  return (
    <div className="page-stack">
      <PageHeader
        title="Credit & Reminders"
        description="Track money to collect from customers and pay to suppliers. Overdue entries are highlighted for admin follow-up."
        icon={<Wallet className="w-6 h-6 text-amber-400" />}
      />

      {overdueCount > 0 ? (
        <Card className="border-amber-500/30 bg-amber-500/5">
          <CardContent className="pt-6">
            <p className="text-sm text-amber-200">
              {overdueCount} credit {overdueCount === 1 ? 'entry is' : 'entries are'} overdue — follow up with the
              party to collect or pay.
            </p>
          </CardContent>
        </Card>
      ) : null}

      <Card>
        <CardHeader>
          <CardTitle>Credit ledger ({list.totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          <PagedDataTable
            columns={columns}
            list={list}
            keyExtractor={(row) => row.id}
            searchPlaceholder="Search party, phone, email…"
            emptyMessage="No credit entries yet. Credit sales and unpaid purchase orders appear here."
            filters={
              <ListFilterBar showClear={list.hasActiveFilters} onClear={list.clearFilters}>
                <FilterSelect
                  label="Party"
                  options={CREDIT_PARTY_TYPE_OPTIONS}
                  value={list.filters.partyType}
                  onChange={(value) => list.setFilter('partyType', value)}
                />
                <FilterSelect
                  label="Status"
                  options={CREDIT_STATUS_OPTIONS}
                  value={list.filters.status}
                  onChange={(value) => list.setFilter('status', value)}
                />
              </ListFilterBar>
            }
          />
        </CardContent>
      </Card>

      <Dialog open={!!selected} onOpenChange={(open) => !open && setSelected(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <DollarSign className="w-5 h-5" />
              Record payment — {selected?.partyName}
            </DialogTitle>
          </DialogHeader>
          {selected ? (
            <form onSubmit={handleRecordPayment} className="space-y-4">
              <div className="rounded-lg border border-slate-800 p-3 text-sm space-y-1">
                <p className="text-slate-300">
                  Balance due: <span className="font-semibold text-emerald-400">${selected.balanceDue.toFixed(2)}</span>
                </p>
                <p className="text-xs text-slate-500">Due {formatDate(selected.dueDate)}</p>
              </div>
              <div className="space-y-2">
                <Label>Amount</Label>
                <Input
                  type="number"
                  min="0.01"
                  step="0.01"
                  value={paymentAmount}
                  onChange={(e) => setPaymentAmount(e.target.value)}
                  required
                />
              </div>
              <div className="space-y-2">
                <Label>Notes (optional)</Label>
                <Input value={paymentNotes} onChange={(e) => setPaymentNotes(e.target.value)} />
              </div>
              <Button type="submit" className="w-full" disabled={isSubmitting}>
                Record payment
              </Button>
            </form>
          ) : null}
        </DialogContent>
      </Dialog>
    </div>
  );
};
