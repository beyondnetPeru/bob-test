import React, { useState } from 'react';
import {
  useGetProductsQuery,
  useDeleteProductMutation,
  Product,
} from '../../api/apiSlice';
import {
  Package,
  Trash2,
  Edit3,
  Plus,
  Search,
  Filter,
  Cpu,
  Database,
  HardDrive,
  Monitor,
} from 'lucide-react';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const ProductCatalog: React.FC = () => {
  const { data: products, isLoading } = useGetProductsQuery();
  const [deleteProduct] = useDeleteProductMutation();
  const [searchTerm, setSearchTerm] = useState('');

  const filteredProducts = products?.filter(
    (p) =>
      p.modelName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.categoryName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.manufacturerName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getCategoryIcon = (category: string) => {
    switch (category.toUpperCase()) {
      case 'CPU':
        return <Cpu className="w-4 h-4" />;
      case 'RAM':
        return <Database className="w-4 h-4" />;
      case 'STORAGE':
        return <HardDrive className="w-4 h-4" />;
      default:
        return <Package className="w-4 h-4" />;
    }
  };

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <header className="flex flex-col md:flex-row md:items-end justify-between gap-6">
        <div>
          <div className="flex items-center gap-3 mb-3">
            <div className="p-2 rounded-lg bg-emerald-500/10 text-emerald-400">
              <Monitor className="w-5 h-5" />
            </div>
            <span className="text-[10px] font-bold text-emerald-500 uppercase tracking-widest">
              Global Asset Library
            </span>
          </div>
          <h1 className="text-4xl font-bold tracking-tight text-white mb-2">
            Technical <span className="text-emerald-400 italic">Catalog</span>
          </h1>
          <p className="text-zinc-500 font-medium">
            Standardized configuration templates for fleet orchestration.
          </p>
        </div>
        <button className="flex items-center gap-2 px-6 py-3 rounded-2xl bg-emerald-500 text-white shadow-lg shadow-emerald-500/20 hover:bg-emerald-400 transition-all font-semibold text-sm">
          <Plus className="w-4 h-4" />
          Add Specification
        </button>
      </header>

      <div className="flex gap-4">
        <div className="flex-1 relative group">
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500 group-focus-within:text-emerald-400 transition-colors" />
          <input
            type="text"
            placeholder="Search model, category, or vendor..."
            className="w-full bg-zinc-900/50 border border-zinc-800/50 rounded-2xl pl-12 pr-4 py-3.5 outline-none focus:border-emerald-500/50 transition-all text-sm font-medium"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        <button className="px-4 py-3.5 rounded-2xl border border-zinc-800/50 hover:bg-zinc-800/50 transition-all">
          <Filter className="w-4 h-4" />
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {isLoading
          ? Array.from({ length: 6 }).map((_, i) => (
              <div
                key={i}
                className="h-48 rounded-3xl bg-zinc-900/30 animate-pulse border border-zinc-800/50"
              />
            ))
          : filteredProducts?.map((product) => (
              <div
                key={product.id}
                className="group relative glass-card p-6 rounded-[2rem] border border-zinc-800/50 hover:border-emerald-500/30 transition-all duration-500 hover:shadow-2xl hover:shadow-emerald-500/5"
              >
                <div className="flex justify-between items-start mb-6">
                  <div
                    className={cn(
                      'p-3 rounded-2xl transition-colors',
                      product.categoryName === 'RAM'
                        ? 'bg-blue-500/10 text-blue-400'
                        : product.categoryName === 'CPU'
                        ? 'bg-amber-500/10 text-amber-400'
                        : 'bg-zinc-800/50 text-zinc-400'
                    )}
                  >
                    {getCategoryIcon(product.categoryName)}
                  </div>
                  <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button className="p-2 rounded-xl bg-zinc-800/50 hover:bg-zinc-700 text-zinc-400 hover:text-white transition-all">
                      <Edit3 className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => deleteProduct(product.id)}
                      className="p-2 rounded-xl bg-red-500/10 hover:bg-red-500 text-red-500 hover:text-white transition-all"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>

                <div>
                  <p className="text-[10px] font-bold text-zinc-500 uppercase tracking-[0.2em] mb-1">
                    {product.manufacturerName}
                  </p>
                  <h3 className="text-lg font-bold text-white mb-2 break-words">
                    {product.modelName}
                  </h3>
                  <p className="text-xs text-zinc-500 line-clamp-2 leading-relaxed">
                    {product.description ||
                      'No detailed technical documentation available.'}
                  </p>
                </div>

                <div className="mt-6 flex items-center justify-between pt-6 border-t border-zinc-800/50">
                  <span className="text-[10px] font-bold text-zinc-600 uppercase tracking-widest">
                    {product.categoryName}
                  </span>
                  <div className="flex items-center gap-1.5 px-2.5 py-1 rounded-lg bg-zinc-900/50 border border-zinc-800/50 text-[10px] font-bold text-zinc-400">
                    REF: {product.id}
                  </div>
                </div>
              </div>
            ))}
      </div>
    </div>
  );
};
