import { useState, type FormEvent } from 'react';
import {
  useGetManufacturersQuery,
  useDeleteManufacturerMutation,
  useCreateManufacturerMutation,
  useGetCategoriesQuery,
  useDeleteCategoryMutation,
  useCreateCategoryMutation,
} from '../../api/apiSlice';
import { Shield, Plus, Trash2, Edit3, Tag } from 'lucide-react';

export const ReferenceData = () => {
  const { data: manufacturers, isLoading: mLoading } =
    useGetManufacturersQuery();
  const { data: categories, isLoading: cLoading } = useGetCategoriesQuery();
  const [deleteManufacturer] = useDeleteManufacturerMutation();
  const [deleteCategory] = useDeleteCategoryMutation();
  const [createManufacturer] = useCreateManufacturerMutation();
  const [createCategory] = useCreateCategoryMutation();

  const [newMName, setNewMName] = useState('');
  const [newCName, setNewCName] = useState('');

  const handleAddM = async (e: FormEvent) => {
    e.preventDefault();
    if (!newMName) return;
    await createManufacturer({ name: newMName });
    setNewMName('');
  };

  const handleAddC = async (e: FormEvent) => {
    e.preventDefault();
    if (!newCName) return;
    await createCategory({ name: newCName });
    setNewCName('');
  };

  return (
    <div className="space-y-12 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <header>
        <div className="flex items-center gap-3 mb-3">
          <span className="text-[10px] font-bold text-primary uppercase tracking-widest bg-primary/10 px-2 py-0.5 rounded">
            Core Logic
          </span>
        </div>
        <h1 className="text-4xl font-bold tracking-tight text-white mb-2">
          System <span className="text-primary italic">Definitions</span>
        </h1>
        <p className="text-zinc-500 font-medium tracking-tight">
          Configure core taxonomies and vendor registries.
        </p>
      </header>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
        {/* Manufacturers Section */}
        <div className="space-y-6">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="p-2 rounded-lg bg-zinc-800/50 text-zinc-400">
                <Shield className="w-5 h-5" />
              </div>
              <h2 className="text-xl font-bold text-white tracking-tight">
                Vendor Registry
              </h2>
            </div>
            <span className="text-[10px] font-mono text-zinc-600 bg-zinc-900 px-2 py-0.5 rounded border border-zinc-800/50">
              {manufacturers?.length || 0} RECORDS
            </span>
          </div>

          <form onSubmit={handleAddM} className="flex gap-2">
            <input
              type="text"
              placeholder="Add new manufacturer..."
              className="flex-1 bg-zinc-900/50 border border-zinc-800/50 rounded-xl px-4 py-2.5 text-sm outline-none focus:border-primary/50 transition-all font-medium"
              value={newMName}
              onChange={(e) => setNewMName(e.target.value)}
            />
            <button className="p-2.5 rounded-xl bg-primary text-white shadow-lg shadow-primary/20 hover:bg-primary/90 transition-all">
              <Plus className="w-5 h-5" />
            </button>
          </form>

          <div className="space-y-2 max-h-[500px] overflow-y-auto custom-scrollbar pr-2">
            {mLoading ? (
              <p className="p-4 text-xs italic text-zinc-600">
                Syncing registry...
              </p>
            ) : (
              manufacturers?.map((m) => (
                <div
                  key={m.id}
                  className="group flex items-center justify-between p-4 rounded-2xl bg-zinc-900/30 border border-zinc-800/30 hover:border-zinc-700/50 hover:bg-zinc-800/30 transition-all"
                >
                  <div className="flex items-center gap-4">
                    <div className="w-8 h-8 rounded-lg bg-zinc-800 flex items-center justify-center text-[10px] font-bold text-zinc-500 group-hover:text-primary transition-colors">
                      M
                    </div>
                    <span className="text-sm font-semibold text-zinc-300 group-hover:text-white transition-colors">
                      {m.name}
                    </span>
                  </div>
                  <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button className="p-2 rounded-lg hover:bg-zinc-700 text-zinc-500 hover:text-white transition-all">
                      <Edit3 className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => deleteManufacturer(m.id)}
                      className="p-2 rounded-lg hover:bg-red-500/10 text-zinc-500 hover:text-red-500 transition-all"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        {/* Categories Section */}
        <div className="space-y-6">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="p-2 rounded-lg bg-zinc-800/50 text-zinc-400">
                <Tag className="w-5 h-5" />
              </div>
              <h2 className="text-xl font-bold text-white tracking-tight">
                Classification Model
              </h2>
            </div>
            <span className="text-[10px] font-mono text-zinc-600 bg-zinc-900 px-2 py-0.5 rounded border border-zinc-800/50">
              {categories?.length || 0} RECORDS
            </span>
          </div>

          <form onSubmit={handleAddC} className="flex gap-2">
            <input
              type="text"
              placeholder="Add new category..."
              className="flex-1 bg-zinc-900/50 border border-zinc-800/50 rounded-xl px-4 py-2.5 text-sm outline-none focus:border-primary/50 transition-all font-medium"
              value={newCName}
              onChange={(e) => setNewCName(e.target.value)}
            />
            <button className="p-2.5 rounded-xl bg-zinc-200 text-black hover:bg-white transition-all">
              <Plus className="w-5 h-5" />
            </button>
          </form>

          <div className="space-y-2 max-h-[500px] overflow-y-auto custom-scrollbar pr-2">
            {cLoading ? (
              <p className="p-4 text-xs italic text-zinc-600">
                Syncing models...
              </p>
            ) : (
              categories?.map((c) => (
                <div
                  key={c.id}
                  className="group flex items-center justify-between p-4 rounded-2xl bg-zinc-900/30 border border-zinc-800/30 hover:border-zinc-700/50 hover:bg-zinc-800/30 transition-all"
                >
                  <div className="flex items-center gap-4">
                    <div className="w-8 h-8 rounded-lg bg-zinc-800 flex items-center justify-center text-[10px] font-bold text-zinc-500 group-hover:text-zinc-200 transition-colors">
                      C
                    </div>
                    <span className="text-sm font-semibold text-zinc-300 group-hover:text-white transition-colors">
                      {c.name}
                    </span>
                  </div>
                  <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button className="p-2 rounded-lg hover:bg-zinc-700 text-zinc-500 hover:text-white transition-all">
                      <Edit3 className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => deleteCategory(c.id)}
                      className="p-2 rounded-lg hover:bg-red-500/10 text-zinc-500 hover:text-red-500 transition-all"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  );
};
