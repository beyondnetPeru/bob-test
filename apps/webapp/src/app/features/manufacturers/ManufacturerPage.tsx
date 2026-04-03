import { FormEvent, useMemo, useState } from 'react';
import {
  useCreateManufacturerMutation,
  useDeleteManufacturerMutation,
  useGetManufacturersQuery,
  useUpdateManufacturerMutation,
} from '../../api/apiSlice';

export const ManufacturerPage = () => {
  const { data: manufacturers, isLoading } = useGetManufacturersQuery();
  const [createManufacturer] = useCreateManufacturerMutation();
  const [updateManufacturer] = useUpdateManufacturerMutation();
  const [deleteManufacturer] = useDeleteManufacturerMutation();

  const [search, setSearch] = useState('');
  const [newName, setNewName] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editingName, setEditingName] = useState('');

  const filtered = useMemo(
    () =>
      (manufacturers ?? []).filter((item) =>
        item.name.toLowerCase().includes(search.toLowerCase())
      ),
    [manufacturers, search]
  );

  const onCreate = async (event: FormEvent) => {
    event.preventDefault();
    if (!newName.trim()) return;

    await createManufacturer({ name: newName.trim() }).unwrap();
    setNewName('');
  };

  const onSaveEdit = async (id: string) => {
    if (!editingName.trim()) return;

    await updateManufacturer({ id, name: editingName.trim() }).unwrap();
    setEditingId(null);
    setEditingName('');
  };

  return (
    <section className="space-y-6">
      <header>
        <h1 className="text-3xl font-bold text-white tracking-tight">
          Manufacturer Maintenance
        </h1>
        <p className="mt-2 text-sm text-zinc-400">
          List, search, add, edit, and delete manufacturer records.
        </p>
      </header>

      <div className="grid gap-3 md:grid-cols-[1fr_360px]">
        <input
          type="search"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          placeholder="Search manufacturers"
          className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm outline-none focus:border-primary"
        />
        <form onSubmit={onCreate} className="flex gap-2">
          <input
            type="text"
            value={newName}
            onChange={(event) => setNewName(event.target.value)}
            placeholder="New manufacturer"
            className="h-10 flex-1 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm outline-none focus:border-primary"
          />
          <button
            type="submit"
            className="h-10 rounded-xl bg-primary px-4 text-sm font-semibold text-white"
          >
            Add
          </button>
        </form>
      </div>

      <div className="overflow-hidden rounded-2xl border border-zinc-800/60">
        <table className="w-full text-sm">
          <thead className="bg-zinc-900/50 text-zinc-400">
            <tr>
              <th className="px-4 py-3 text-left">Name</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && (
              <tr>
                <td colSpan={2} className="px-4 py-4 text-zinc-500">
                  Loading manufacturers...
                </td>
              </tr>
            )}
            {!isLoading && filtered.length === 0 && (
              <tr>
                <td colSpan={2} className="px-4 py-4 text-zinc-500">
                  No manufacturers found.
                </td>
              </tr>
            )}
            {filtered.map((item) => (
              <tr key={item.id} className="border-t border-zinc-800/40">
                <td className="px-4 py-3">
                  {editingId === item.id ? (
                    <input
                      type="text"
                      value={editingName}
                      onChange={(event) => setEditingName(event.target.value)}
                      className="h-9 w-full rounded-lg border border-zinc-700 bg-zinc-900/60 px-3 text-sm outline-none focus:border-primary"
                    />
                  ) : (
                    <span className="text-zinc-200">{item.name}</span>
                  )}
                </td>
                <td className="px-4 py-3">
                  <div className="flex justify-end gap-2">
                    {editingId === item.id ? (
                      <button
                        onClick={() => void onSaveEdit(item.id)}
                        className="h-8 rounded-lg bg-primary px-3 text-xs font-semibold text-white"
                      >
                        Save
                      </button>
                    ) : (
                      <button
                        onClick={() => {
                          setEditingId(item.id);
                          setEditingName(item.name);
                        }}
                        className="h-8 rounded-lg border border-zinc-700 px-3 text-xs text-zinc-200"
                      >
                        Edit
                      </button>
                    )}
                    <button
                      onClick={() => void deleteManufacturer(item.id)}
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
    </section>
  );
};
