import { Monitor, Weight, Hash } from 'lucide-react';
import { Button } from '@/app/components/ui/Button';
import type { InventoryItem } from '@/app/api/apiSlice';

interface Props {
  items: InventoryItem[];
  onEdit: (item: InventoryItem) => void;
  onDelete: (id: string) => void;
}

export const HardwareTable = ({ items, onEdit, onDelete }: Props) => {
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
    <div className="w-full overflow-hidden rounded-2xl border border-zinc-800/60">
      <div className="overflow-x-auto">
        <table className="w-full border-collapse text-left text-sm">
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
              <th className="px-6 py-4 text-right text-xs font-semibold uppercase tracking-wider text-zinc-500">
                Actions
              </th>
            </tr>
          </thead>
          <tbody className="divide-y divide-zinc-800/30">
            {items.map((item) => (
              <tr
                key={item.id}
                className="transition-colors hover:bg-zinc-800/20"
              >
                <td className="px-6 py-4">
                  <div className="flex items-center gap-3">
                    <div className="rounded-lg bg-primary/10 p-2 text-primary">
                      <Monitor className="h-4 w-4" />
                    </div>
                    <div>
                      <div className="font-medium text-zinc-100">
                        {item.assetName}
                      </div>
                      <div className="font-mono text-xs uppercase tracking-tighter text-zinc-500">
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
                    <Weight className="h-3.5 w-3.5" />
                    {item.weightKg} kg
                  </div>
                </td>
                <td className="px-6 py-4">
                  <div className="flex items-center gap-2 font-mono text-sm text-zinc-400">
                    <Hash className="h-3.5 w-3.5 text-zinc-600" />
                    {item.componentCount}
                  </div>
                </td>
                <td className="px-6 py-4">
                  <div className="flex justify-end gap-2">
                    <Button
                      size="sm"
                      variant="secondary"
                      onClick={() => onEdit(item)}
                    >
                      Edit
                    </Button>
                    <Button
                      size="sm"
                      variant="danger"
                      onClick={() => void onDelete(item.id)}
                    >
                      Delete
                    </Button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};
