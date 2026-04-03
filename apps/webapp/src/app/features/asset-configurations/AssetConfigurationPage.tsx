import { FormEvent, useMemo, useState } from 'react';
import {
  useCreateAssetConfigurationMutation,
  useDeleteAssetConfigurationMutation,
  useGetAssetConfigurationsQuery,
  useGetInventoryQuery,
  useGetProductsQuery,
  useUpdateAssetConfigurationMutation,
} from '../../api/apiSlice';

export const AssetConfigurationPage = () => {
  const { data: configurations, isLoading } = useGetAssetConfigurationsQuery();
  const { data: inventoryItems } = useGetInventoryQuery();
  const { data: products } = useGetProductsQuery();

  const [createConfiguration] = useCreateAssetConfigurationMutation();
  const [updateConfiguration] = useUpdateAssetConfigurationMutation();
  const [deleteConfiguration] = useDeleteAssetConfigurationMutation();

  const [search, setSearch] = useState('');

  const [inventoryId, setInventoryId] = useState('');
  const [productId, setProductId] = useState('');
  const [quantity, setQuantity] = useState(1);
  const [standardValue, setStandardValue] = useState('');
  const [location, setLocation] = useState('');

  const [editingId, setEditingId] = useState<string | null>(null);

  const filtered = useMemo(
    () =>
      (configurations ?? []).filter((item) => {
        const q = search.toLowerCase();
        return (
          item.inventoryName.toLowerCase().includes(q) ||
          item.productModelName.toLowerCase().includes(q) ||
          (item.location ?? '').toLowerCase().includes(q) ||
          item.categoryName.toLowerCase().includes(q)
        );
      }),
    [configurations, search]
  );

  const resetForm = () => {
    setInventoryId('');
    setProductId('');
    setQuantity(1);
    setStandardValue('');
    setLocation('');
    setEditingId(null);
  };

  const onSubmit = async (event: FormEvent) => {
    event.preventDefault();
    if (!inventoryId || !productId || quantity < 1) return;

    const payload = {
      inventoryId,
      productId,
      quantity,
      standardValue: standardValue.trim() ? Number(standardValue) : null,
      location: location.trim() || null,
    };

    if (editingId) {
      await updateConfiguration({ id: editingId, ...payload }).unwrap();
    } else {
      await createConfiguration(payload).unwrap();
    }

    resetForm();
  };

  return (
    <section className="space-y-6">
      <header>
        <h1 className="text-3xl font-bold text-white tracking-tight">
          Asset Configuration Maintenance
        </h1>
        <p className="mt-2 text-sm text-zinc-400">
          Maintain machine component configurations with search, add, edit, and
          delete.
        </p>
      </header>

      <div className="rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-4">
        <form
          onSubmit={onSubmit}
          className="grid grid-cols-1 gap-3 md:grid-cols-2 xl:grid-cols-6"
        >
          <select
            value={inventoryId}
            onChange={(event) => setInventoryId(event.target.value)}
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
            required
          >
            <option value="">Inventory</option>
            {(inventoryItems ?? []).map((item) => (
              <option key={item.id} value={item.id}>
                {item.assetName}
              </option>
            ))}
          </select>

          <select
            value={productId}
            onChange={(event) => setProductId(event.target.value)}
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
            required
          >
            <option value="">Product</option>
            {(products ?? []).map((item) => (
              <option key={item.id} value={item.id}>
                {item.modelName}
              </option>
            ))}
          </select>

          <input
            type="number"
            min={1}
            value={quantity}
            onChange={(event) => setQuantity(Number(event.target.value))}
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
            placeholder="Quantity"
            required
          />

          <input
            type="number"
            step="0.01"
            value={standardValue}
            onChange={(event) => setStandardValue(event.target.value)}
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
            placeholder="Standard value"
          />

          <input
            type="text"
            value={location}
            onChange={(event) => setLocation(event.target.value)}
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
            placeholder="Location"
          />

          <div className="flex gap-2">
            <button
              type="submit"
              className="h-10 flex-1 rounded-xl bg-primary px-3 text-xs font-semibold text-white"
            >
              {editingId ? 'Save' : 'Add'}
            </button>
            {editingId && (
              <button
                type="button"
                onClick={resetForm}
                className="h-10 rounded-xl border border-zinc-700 px-3 text-xs text-zinc-200"
              >
                Cancel
              </button>
            )}
          </div>
        </form>
      </div>

      <input
        type="search"
        value={search}
        onChange={(event) => setSearch(event.target.value)}
        placeholder="Search asset configurations"
        className="h-10 w-full rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm outline-none focus:border-primary"
      />

      <div className="overflow-hidden rounded-2xl border border-zinc-800/60">
        <table className="w-full text-sm">
          <thead className="bg-zinc-900/50 text-zinc-400">
            <tr>
              <th className="px-4 py-3 text-left">Inventory</th>
              <th className="px-4 py-3 text-left">Product</th>
              <th className="px-4 py-3 text-left">Category</th>
              <th className="px-4 py-3 text-left">Qty</th>
              <th className="px-4 py-3 text-left">Value</th>
              <th className="px-4 py-3 text-left">Location</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && (
              <tr>
                <td colSpan={7} className="px-4 py-4 text-zinc-500">
                  Loading asset configurations...
                </td>
              </tr>
            )}
            {!isLoading && filtered.length === 0 && (
              <tr>
                <td colSpan={7} className="px-4 py-4 text-zinc-500">
                  No asset configurations found.
                </td>
              </tr>
            )}
            {filtered.map((item) => (
              <tr key={item.id} className="border-t border-zinc-800/40">
                <td className="px-4 py-3 text-zinc-200">
                  {item.inventoryName}
                </td>
                <td className="px-4 py-3 text-zinc-200">
                  {item.productModelName}
                </td>
                <td className="px-4 py-3 text-zinc-400">{item.categoryName}</td>
                <td className="px-4 py-3 text-zinc-200">{item.quantity}</td>
                <td className="px-4 py-3 text-zinc-400">
                  {item.standardValue === null ? '-' : item.standardValue}
                </td>
                <td className="px-4 py-3 text-zinc-400">
                  {item.location ?? '-'}
                </td>
                <td className="px-4 py-3">
                  <div className="flex justify-end gap-2">
                    <button
                      onClick={() => {
                        setEditingId(item.id);
                        setInventoryId(item.inventoryId);
                        setProductId(item.productId);
                        setQuantity(item.quantity);
                        setStandardValue(
                          item.standardValue === null
                            ? ''
                            : String(item.standardValue)
                        );
                        setLocation(item.location ?? '');
                      }}
                      className="h-8 rounded-lg border border-zinc-700 px-3 text-xs text-zinc-200"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => void deleteConfiguration(item.id)}
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
