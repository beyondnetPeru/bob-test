import React from 'react';
import { InventoryItem } from '../../../api/apiSlice';
import { Monitor, Weight, Hash, ChevronRight } from 'lucide-react';

interface Props {
  items: InventoryItem[];
  onEdit: (item: InventoryItem) => void;
}

export const HardwareTable: React.FC<Props> = ({ items, onEdit }) => {
  const getTierClass = (tier: string | null) => {
    switch (tier?.toLowerCase()) {
      case 'workstation':
        return 'tier-workstation';
      case 'high-end':
        return 'tier-high-end';
      case 'mid-range':
        return 'tier-mid-range';
      default:
        return 'tier-entry';
    }
  };

  return (
    <div className="w-full overflow-hidden glass-card">
      <div className="overflow-x-auto">
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="border-b border-zinc-800/50 bg-zinc-900/30">
              <th className="px-6 py-4 text-xs font-semibold uppercase tracking-wider text-zinc-500">
                Machine
              </th>
              <th className="px-6 py-4 text-xs font-semibold uppercase tracking-wider text-zinc-500">
                Tier
              </th>
              <th className="px-6 py-4 text-xs font-semibold uppercase tracking-wider text-zinc-500">
                Weight
              </th>
              <th className="px-6 py-4 text-xs font-semibold uppercase tracking-wider text-zinc-500">
                Components
              </th>
              <th className="px-6 py-4 text-right"></th>
            </tr>
          </thead>
          <tbody className="divide-y divide-zinc-800/30">
            {items.map((item) => (
              <tr
                key={item.id}
                className="group transition-colors hover:bg-zinc-800/20 cursor-pointer"
                onClick={() => onEdit(item)}
              >
                <td className="px-6 py-4">
                  <div className="flex items-center gap-3">
                    <div className="p-2 rounded-lg bg-primary/10 text-primary">
                      <Monitor className="w-4 h-4" />
                    </div>
                    <div>
                      <div className="font-medium text-zinc-100">
                        {item.assetName}
                      </div>
                      <div className="text-xs text-zinc-500 font-mono uppercase tracking-tighter">
                        {item.id.slice(0, 8)}
                      </div>
                    </div>
                  </div>
                </td>
                <td className="px-6 py-4">
                  <span
                    className={`badge ${getTierClass(item.performanceTier)}`}
                  >
                    {item.performanceTier || 'Unknown'}
                  </span>
                </td>
                <td className="px-6 py-4">
                  <div className="flex items-center gap-2 text-sm text-zinc-400">
                    <Weight className="w-3.5 h-3.5" />
                    {item.weightKg} kg
                  </div>
                </td>
                <td className="px-6 py-4">
                  <div className="flex items-center gap-2 text-sm text-zinc-400 font-mono">
                    <Hash className="w-3.5 h-3.5 text-zinc-600" />
                    {item.componentCount}
                  </div>
                </td>
                <td className="px-6 py-4 text-right">
                  <button className="p-1 rounded-md text-zinc-600 group-hover:text-zinc-200 transition-colors">
                    <ChevronRight className="w-5 h-5" />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};
