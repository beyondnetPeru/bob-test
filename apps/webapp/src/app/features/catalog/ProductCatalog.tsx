import { FormEvent, useMemo, useState } from 'react';
import {
  useCreateProductMutation,
  useGetCategoriesQuery,
  useGetManufacturersQuery,
  useGetProductsQuery,
  useDeleteProductMutation,
  useUpdateProductMutation,
} from '../../api/apiSlice';
import { Search } from 'lucide-react';

export const ProductCatalog = () => {
  const { data: products, isLoading } = useGetProductsQuery();
  const { data: categories } = useGetCategoriesQuery();
  const { data: manufacturers } = useGetManufacturersQuery();

  const [createProduct] = useCreateProductMutation();
  const [updateProduct] = useUpdateProductMutation();
  const [deleteProduct] = useDeleteProductMutation();

  const [searchTerm, setSearchTerm] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);

  const [modelName, setModelName] = useState('');
  const [description, setDescription] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [manufacturerId, setManufacturerId] = useState('');

  const filteredProducts = useMemo(
    () =>
      (products ?? []).filter(
        (p) =>
          p.modelName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          p.categoryName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          p.manufacturerName.toLowerCase().includes(searchTerm.toLowerCase())
      ),
    [products, searchTerm]
  );

  const resetForm = () => {
    setEditingId(null);
    setModelName('');
    setDescription('');
    setCategoryId('');
    setManufacturerId('');
  };

  const onSubmit = async (event: FormEvent) => {
    event.preventDefault();
    if (!modelName.trim() || !categoryId || !manufacturerId) return;

    const payload = {
      modelName: modelName.trim(),
      description: description.trim(),
      categoryId,
      manufacturerId,
    };

    if (editingId) {
      await updateProduct({ id: editingId, ...payload }).unwrap();
    } else {
      await createProduct(payload).unwrap();
    }

    resetForm();
  };

  return (
    <section className="space-y-6">
      <header>
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-white mb-2">
            Product Maintenance
          </h1>
          <p className="text-zinc-400 font-medium text-sm">
            List, search, add, edit, and delete product records.
          </p>
        </div>
      </header>

      <form
        onSubmit={onSubmit}
        className="rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-4"
      >
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2 xl:grid-cols-5">
          <input
            type="text"
            value={modelName}
            onChange={(event) => setModelName(event.target.value)}
            placeholder="Model name"
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
            required
          />
          <input
            type="text"
            value={description}
            onChange={(event) => setDescription(event.target.value)}
            placeholder="Description"
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
          />
          <select
            value={categoryId}
            onChange={(event) => setCategoryId(event.target.value)}
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
            required
          >
            <option value="">Category</option>
            {(categories ?? []).map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
          <select
            value={manufacturerId}
            onChange={(event) => setManufacturerId(event.target.value)}
            className="h-10 rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm"
            required
          >
            <option value="">Manufacturer</option>
            {(manufacturers ?? []).map((manufacturer) => (
              <option key={manufacturer.id} value={manufacturer.id}>
                {manufacturer.name}
              </option>
            ))}
          </select>
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
        </div>
      </form>

      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-zinc-500" />
        <input
          type="search"
          placeholder="Search model, category, or manufacturer"
          className="h-10 w-full rounded-xl border border-zinc-700 bg-zinc-900/60 pl-10 pr-3 text-sm outline-none focus:border-primary"
          value={searchTerm}
          onChange={(event) => setSearchTerm(event.target.value)}
        />
      </div>

      <div className="overflow-hidden rounded-2xl border border-zinc-800/60">
        <table className="w-full text-sm">
          <thead className="bg-zinc-900/50 text-zinc-400">
            <tr>
              <th className="px-4 py-3 text-left">Model</th>
              <th className="px-4 py-3 text-left">Description</th>
              <th className="px-4 py-3 text-left">Category</th>
              <th className="px-4 py-3 text-left">Manufacturer</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && (
              <tr>
                <td colSpan={5} className="px-4 py-4 text-zinc-500">
                  Loading products...
                </td>
              </tr>
            )}
            {!isLoading && filteredProducts.length === 0 && (
              <tr>
                <td colSpan={5} className="px-4 py-4 text-zinc-500">
                  No products found.
                </td>
              </tr>
            )}
            {filteredProducts.map((product) => (
              <tr key={product.id} className="border-t border-zinc-800/40">
                <td className="px-4 py-3 text-zinc-100">{product.modelName}</td>
                <td className="px-4 py-3 text-zinc-400">
                  {product.description || '-'}
                </td>
                <td className="px-4 py-3 text-zinc-300">
                  {product.categoryName}
                </td>
                <td className="px-4 py-3 text-zinc-300">
                  {product.manufacturerName}
                </td>
                <td className="px-4 py-3">
                  <div className="flex justify-end gap-2">
                    <button
                      onClick={() => {
                        setEditingId(product.id);
                        setModelName(product.modelName);
                        setDescription(product.description || '');
                        setCategoryId(product.categoryId);
                        setManufacturerId(product.manufacturerId);
                      }}
                      className="h-8 rounded-lg border border-zinc-700 px-3 text-xs text-zinc-200"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => void deleteProduct(product.id)}
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
