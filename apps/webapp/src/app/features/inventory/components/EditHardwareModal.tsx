import React, { useEffect, useState } from 'react';
import {
  InventoryItem,
  useUpdateInventoryMutation,
} from '../../../api/apiSlice';
import { DEVICE_CATEGORY_OPTIONS } from '@/app/lib/computerCatalog';
import { X, Save, Loader2, Info } from 'lucide-react';

interface Props {
  item: InventoryItem | null;
  onClose: () => void;
}

export const EditHardwareModal: React.FC<Props> = ({ item, onClose }) => {
  const [updateInventory, { isLoading }] = useUpdateInventoryMutation();
  const [assetName, setAssetName] = useState('');
  const [deviceCategory, setDeviceCategory] = useState('Desktop PC');
  const [weightKg, setWeightKg] = useState<number>(0);

  useEffect(() => {
    if (item) {
      setAssetName(item.assetName);
      setDeviceCategory(item.deviceCategory);
      setWeightKg(item.weightKg);
    }
  }, [item]);

  if (!item) return null;

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await updateInventory({
        id: item.id,
        assetName,
        deviceCategory,
        weightKg,
      }).unwrap();
      onClose();
    } catch (err) {
      console.error('Failed to update inventory:', err);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 backdrop-blur-sm bg-black/40 animate-in fade-in duration-200">
      <div className="glass-card w-full max-w-md overflow-hidden animate-in zoom-in-95 duration-200 shadow-2xl border-zinc-700/50">
        <div className="flex items-center justify-between p-6 border-b border-zinc-800/50 bg-zinc-900/30">
          <h2 className="text-xl font-semibold flex items-center gap-2">
            <Info className="w-5 h-5 text-primary" />
            Edit Machine Specs
          </h2>
          <button
            onClick={onClose}
            className="p-2 rounded-lg hover:bg-zinc-800 transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        <form onSubmit={handleSave} className="p-6 space-y-6">
          <div className="space-y-2">
            <label className="text-xs font-semibold uppercase tracking-wider text-zinc-500">
              Asset Name
            </label>
            <input
              type="text"
              className="w-full bg-zinc-800/50 border border-zinc-700/50 rounded-lg px-4 py-2.5 outline-none focus:border-primary/50 transition-all text-sm"
              value={assetName}
              onChange={(e) => setAssetName(e.target.value)}
              disabled={isLoading}
              required
            />
          </div>

          <div className="space-y-2">
            <label className="text-xs font-semibold uppercase tracking-wider text-zinc-500">
              Device Category
            </label>
            <select
              className="w-full bg-zinc-800/50 border border-zinc-700/50 rounded-lg px-4 py-2.5 outline-none focus:border-primary/50 transition-all text-sm"
              value={deviceCategory}
              onChange={(e) => setDeviceCategory(e.target.value)}
              disabled={isLoading}
            >
              {DEVICE_CATEGORY_OPTIONS.map((category) => (
                <option key={category} value={category}>
                  {category}
                </option>
              ))}
            </select>
          </div>

          <div className="space-y-2">
            <label className="text-xs font-semibold uppercase tracking-wider text-zinc-500">
              Weight (kg)
            </label>
            <input
              type="number"
              step="0.01"
              className="w-full bg-zinc-800/50 border border-zinc-700/50 rounded-lg px-4 py-2.5 outline-none focus:border-primary/50 transition-all text-sm"
              value={weightKg}
              onChange={(e) => setWeightKg(parseFloat(e.target.value))}
              disabled={isLoading}
              required
            />
          </div>

          <div className="pt-4 flex gap-3">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-4 py-2.5 rounded-lg border border-zinc-700/50 hover:bg-zinc-800 transition-all text-sm font-medium"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="flex-1 flex items-center justify-center gap-2 px-4 py-2.5 rounded-lg bg-primary text-white shadow-lg shadow-primary/20 hover:bg-primary/90 transition-all text-sm font-medium disabled:opacity-50"
            >
              {isLoading ? (
                <Loader2 className="w-4 h-4 animate-spin" />
              ) : (
                <Save className="w-4 h-4" />
              )}
              Save Changes
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
