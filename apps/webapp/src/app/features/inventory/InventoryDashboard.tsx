import { FormEvent, useMemo, useState } from 'react';
import {
  InventoryItem,
  useCreateInventoryMutation,
  useDeleteInventoryMutation,
  useGetInventoryQuery,
} from '../../api/apiSlice';
import { EditHardwareModal } from './components/EditHardwareModal';
import { Loader2, RefreshCcw } from 'lucide-react';

export const InventoryDashboard = () => {
  const { data: items, isLoading, isError, refetch } = useGetInventoryQuery();
  const [createInventory] = useCreateInventoryMutation();
  const [deleteInventory] = useDeleteInventoryMutation();

  const [searchTerm, setSearchTerm] = useState('');
  const [assetName, setAssetName] = useState('');
  const [weightKg, setWeightKg] = useState('');
  const [editingItem, setEditingItem] = useState<InventoryItem | null>(null);

  const filteredItems = useMemo(
    () =>
      (items ?? []).filter(
        (item) =>
          item.assetName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          item.performanceTier?.toLowerCase().includes(searchTerm.toLowerCase())
      ),
    [items, searchTerm]
  );

  const handleCreate = async (event: FormEvent) => {
    event.preventDefault();
    if (!assetName.trim() || !weightKg) return;

    await createInventory({
      assetName: assetName.trim(),
      weightKg: Number(weightKg),
    }).unwrap();

    setAssetName('');
    setWeightKg('');
  };

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
      <header className="flex flex-col md:flex-row md:items-end justify-between gap-4 py-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-white mb-2">
            Hardware Inventory Maintenance
          </h1>
          <p className="text-zinc-400 font-medium text-sm">
            List, search, add, edit, and delete hardware inventory records.
          </p>
        </div>
        <div className="flex items-center gap-2 text-xs font-mono text-zinc-500 bg-zinc-900 px-3 py-1.5 rounded-full border border-zinc-800/50">
          <span className="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse" />
          ASSETS: {items?.length || 0}
        </div>
      </header>

      <div className="grid gap-3 md:grid-cols-[1fr_420px]">
        <input
          type="search"
          value={searchTerm}
          onChange={(event) => setSearchTerm(event.target.value)}
          placeholder="Search by asset name or performance tier"
          className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm outline-none focus:border-primary"
        />

        <form
          onSubmit={handleCreate}
          className="grid grid-cols-[1fr_140px_72px] gap-2"
        >
          <input
            type="text"
            value={assetName}
            onChange={(event) => setAssetName(event.target.value)}
            placeholder="Asset name"
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm outline-none focus:border-primary"
            required
          />
          <input
            type="number"
            step="0.01"
            min="0.01"
            value={weightKg}
            onChange={(event) => setWeightKg(event.target.value)}
            placeholder="Weight kg"
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm outline-none focus:border-primary"
            required
          />
          <button
            type="submit"
            className="h-10 rounded-xl bg-primary text-sm font-semibold text-white"
          >
            Add
          </button>
        </form>
      </div>

      <div className="overflow-hidden rounded-2xl border border-zinc-800/60">
        <table className="w-full text-sm">
          <thead className="bg-zinc-900/50 text-zinc-400">
            <tr>
              <th className="px-4 py-3 text-left">Asset Name</th>
              <th className="px-4 py-3 text-left">Tier</th>
              <th className="px-4 py-3 text-left">Weight (kg)</th>
              <th className="px-4 py-3 text-left">Components</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {filteredItems.map((item) => (
              <tr key={item.id} className="border-t border-zinc-800/40">
                <td className="px-4 py-3 text-zinc-100">{item.assetName}</td>
                <td className="px-4 py-3 text-zinc-300">
                  {item.performanceTier || '-'}
                </td>
                <td className="px-4 py-3 text-zinc-300">{item.weightKg}</td>
                <td className="px-4 py-3 text-zinc-400">
                  {item.componentCount}
                </td>
                <td className="px-4 py-3">
                  <div className="flex justify-end gap-2">
                    <button
                      onClick={() => handleEdit(item)}
                      className="h-8 rounded-lg border border-zinc-700 px-3 text-xs text-zinc-200"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => void deleteInventory(item.id)}
                      className="h-8 rounded-lg border border-red-700/50 px-3 text-xs text-red-300"
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <EditHardwareModal
        item={editingItem}
        onClose={() => setEditingItem(null)}
      />
    </div>
  );
};
