import { FormEvent, useMemo, useState } from 'react';
import {
  useCreateManufacturerMutation,
  useDeleteManufacturerMutation,
  useGetManufacturersQuery,
  useUpdateManufacturerMutation,
} from '@/app/api/apiSlice';
import { Button } from '@/app/components/ui/Button';
import { TextInput } from '@/app/components/ui/Field';
import { ListControls } from '@/app/components/ui/ListControls';
import { PageHeader } from '@/app/components/ui/PageHeader';
import {
  compareText,
  matchesSearch,
  type SortDirection,
} from '@/app/lib/listUtils';

export const ManufacturerPage = () => {
  const { data: manufacturers, isLoading } = useGetManufacturersQuery();
  const [createManufacturer] = useCreateManufacturerMutation();
  const [updateManufacturer] = useUpdateManufacturerMutation();
  const [deleteManufacturer] = useDeleteManufacturerMutation();

  const [search, setSearch] = useState('');
  const [sortDirection, setSortDirection] = useState<SortDirection>('asc');
  const [newName, setNewName] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editingName, setEditingName] = useState('');

  const filtered = useMemo(
    () =>
      [...(manufacturers ?? [])]
        .filter((item) =>
          matchesSearch(search, [item.name, item.id.slice(0, 8)])
        )
        .sort((left, right) =>
          compareText(left.name, right.name, sortDirection)
        ),
    [manufacturers, search, sortDirection]
  );

  const resetFilters = () => {
    setSearch('');
    setSortDirection('asc');
  };

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
      <PageHeader
        title="Manufacturer Maintenance"
        description="List, search, sort, add, edit, and delete component manufacturer records."
      />

      <div className="grid gap-3 xl:grid-cols-[minmax(0,1fr)_360px]">
        <ListControls
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search manufacturer name or short id"
          sortValue="name"
          onSortChange={() => undefined}
          sortOptions={[{ value: 'name', label: 'Name' }]}
          direction={sortDirection}
          onDirectionChange={setSortDirection}
          onReset={resetFilters}
          resultCount={filtered.length}
        />
        <form
          onSubmit={onCreate}
          className="flex gap-2 rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-3"
        >
          <TextInput
            value={newName}
            onChange={(event) => setNewName(event.target.value)}
            placeholder="New manufacturer"
          />
          <Button type="submit" variant="primary">
            Add
          </Button>
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
                  No manufacturers match the current filters.
                </td>
              </tr>
            )}
            {filtered.map((item) => (
              <tr key={item.id} className="border-t border-zinc-800/40">
                <td className="px-4 py-3">
                  {editingId === item.id ? (
                    <TextInput
                      value={editingName}
                      onChange={(event) => setEditingName(event.target.value)}
                      className="h-9"
                    />
                  ) : (
                    <span className="text-zinc-200">{item.name}</span>
                  )}
                </td>
                <td className="px-4 py-3">
                  <div className="flex justify-end gap-2">
                    {editingId === item.id ? (
                      <Button
                        size="sm"
                        variant="primary"
                        onClick={() => void onSaveEdit(item.id)}
                      >
                        Save
                      </Button>
                    ) : (
                      <Button
                        size="sm"
                        variant="secondary"
                        onClick={() => {
                          setEditingId(item.id);
                          setEditingName(item.name);
                        }}
                      >
                        Edit
                      </Button>
                    )}
                    <Button
                      size="sm"
                      variant="danger"
                      onClick={() => void deleteManufacturer(item.id)}
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
    </section>
  );
};
