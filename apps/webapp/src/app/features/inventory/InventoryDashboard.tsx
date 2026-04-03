import { useGetInventoryQuery } from '@/app/api/apiSlice';
import { PageHeader } from '@/app/components/ui/PageHeader';
import { EditHardwareModal } from './components/EditHardwareModal';
import { HardwareTable } from './components/HardwareTable';
import { InventoryToolbar } from './components/InventoryToolbar';
import { Loader2, RefreshCcw } from 'lucide-react';
import { useInventoryPageState } from './hooks/useInventoryPageState';

export const InventoryDashboard = () => {
  const { data: items, isLoading, isError, refetch } = useGetInventoryQuery();
  const {
    form,
    searchTerm,
    editingItem,
    filteredItems,
    setSearchTerm,
    setEditingItem,
    updateField,
    submitCreate,
    removeItem,
  } = useInventoryPageState(items ?? []);

  if (isLoading) {
    return (
      <div className="flex min-h-[60vh] flex-col items-center justify-center gap-4">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
        <span className="font-medium text-zinc-500">Syncing Fleet Data...</span>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="glass flex min-h-[60vh] flex-col items-center justify-center gap-6 rounded-3xl border-red-500/10 p-12">
        <div className="rounded-full bg-red-500/10 p-4 text-red-400">
          <RefreshCcw className="h-8 w-8" />
        </div>
        <div className="text-center">
          <h2 className="mb-2 text-xl font-semibold">Fleet Sync Failed</h2>
          <p className="text-zinc-500">
            Connection to the hardware management API interrupted.
          </p>
        </div>
        <button
          onClick={() => void refetch()}
          className="rounded-xl border border-red-500/30 bg-red-500/20 px-6 py-2.5 font-medium text-red-100 transition-all hover:bg-red-500/30"
        >
          Retry Connection
        </button>
      </div>
    );
  }

  return (
    <section className="mx-auto max-w-7xl space-y-6">
      <PageHeader
        title="Hardware Inventory Maintenance"
        description="List, search, add, edit, and delete hardware inventory records."
        meta={
          <div className="flex items-center gap-2 rounded-full border border-zinc-800/50 bg-zinc-900 px-3 py-1.5 font-mono text-xs text-zinc-500">
            <span className="h-1.5 w-1.5 rounded-full bg-green-500 animate-pulse" />
            ASSETS: {items?.length ?? 0}
          </div>
        }
      />

      <InventoryToolbar
        searchTerm={searchTerm}
        form={form}
        onSearchChange={setSearchTerm}
        onFieldChange={updateField}
        onSubmit={submitCreate}
      />

      <HardwareTable
        items={filteredItems}
        onEdit={setEditingItem}
        onDelete={removeItem}
      />

      <EditHardwareModal
        item={editingItem}
        onClose={() => setEditingItem(null)}
      />
    </section>
  );
};
