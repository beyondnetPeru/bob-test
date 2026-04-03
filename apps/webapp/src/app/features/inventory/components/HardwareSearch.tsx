import React from 'react';
import { Search, SlidersHorizontal } from 'lucide-react';

interface Props {
  value: string;
  onChange: (value: string) => void;
}

export const HardwareSearch: React.FC<Props> = ({ value, onChange }) => {
  return (
    <div className="flex items-center gap-4 p-6 glass-card mb-4 border-zinc-500/20 shadow-lg group">
      <div className="relative flex-1">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-zinc-500 transition-colors group-focus-within:text-primary" />
        <input
          type="text"
          placeholder="Search by machine name, tier, or component..."
          className="w-full bg-zinc-800/50 border border-zinc-700/50 rounded-lg pl-10 pr-4 py-2.5 outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/20 transition-all text-sm"
          value={value}
          onChange={(e) => onChange(e.target.value)}
        />
      </div>
      <button className="flex items-center gap-2 px-4 py-2.5 rounded-lg bg-zinc-800/30 border border-zinc-700/50 hover:bg-zinc-800/60 hover:text-white transition-all text-sm font-medium">
        <SlidersHorizontal className="w-4 h-4" />
        Filters
      </button>
    </div>
  );
};
