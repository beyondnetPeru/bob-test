import React, { useState } from 'react';
import { useGetInventoryQuery, InventoryItem } from '../../api/apiSlice';
import { HardwareTable } from './components/HardwareTable';
import { HardwareSearch } from './components/HardwareSearch';
import { EditHardwareModal } from './components/EditHardwareModal';
import { Loader2, RefreshCcw } from 'lucide-react';

export const InventoryDashboard: React.FC = () => {
  const { data: items, isLoading, isError, refetch } = useGetInventoryQuery();
  const [searchTerm, setSearchTerm] = useState('');
  const [editingItem, setEditingItem] = useState<InventoryItem | null>(null);

  const filteredItems = items?.filter(
    (item) =>
      item.assetName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      item.performanceTier?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleEdit = (item: InventoryItem) => {
    setEditingItem(item);
  };

  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[60vh] gap-4">
        <Loader2 className="w-8 h-8 text-primary animate-spin" />
        <span className="text-zinc-500 font-medium">Syncing Fleet Data...</span>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[60vh] gap-6 glass p-12 rounded-3xl border-red-500/10">
        <div className="p-4 rounded-full bg-red-500/10 text-red-400">
          <RefreshCcw className="w-8 h-8" />
        </div>
        <div className="text-center">
          <h2 className="text-xl font-semibold mb-2">Fleet Sync Failed</h2>
          <p className="text-zinc-500">
            Connection to the hardware management API interrupted.
          </p>
        </div>
        <button
          onClick={refetch}
          className="px-6 py-2.5 rounded-xl bg-red-500/20 text-red-100 border border-red-500/30 hover:bg-red-500/30 transition-all font-medium"
        >
          Retry Connection
        </button>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <header className="flex flex-col md:flex-row md:items-end justify-between gap-4 py-8">
        <div>
          <h1 className="text-4xl font-bold tracking-tight text-white mb-2">
            Hardware <span className="text-primary italic">Inventory</span>
          </h1>
          <p className="text-zinc-500 font-medium">
            Monitoring {items?.length || 0} production-ready assets across the
            fleet.
          </p>
        </div>
        <div className="flex items-center gap-2 text-xs font-mono text-zinc-500 bg-zinc-900 px-3 py-1.5 rounded-full border border-zinc-800/50">
          <span className="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse" />
          SYSTEMS_ONLINE
        </div>
      </header>

      <HardwareSearch value={searchTerm} onChange={setSearchTerm} />
      <HardwareTable items={filteredItems || []} onEdit={handleEdit} />

      <EditHardwareModal
        item={editingItem}
        onClose={() => setEditingItem(null)}
      />
    </div>
  );
};
