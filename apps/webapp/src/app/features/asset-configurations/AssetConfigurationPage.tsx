import { FormEvent, useMemo, useState } from 'react';
import {
  useCreateAssetConfigurationMutation,
  useDeleteAssetConfigurationMutation,
  useGetAssetConfigurationsQuery,
  useGetInventoryQuery,
  useGetProductsQuery,
  useUpdateAssetConfigurationMutation,
} from '@/app/api/apiSlice';
import { Button } from '@/app/components/ui/Button';
import { SelectField, TextInput } from '@/app/components/ui/Field';
import { ListControls } from '@/app/components/ui/ListControls';
import { PageHeader } from '@/app/components/ui/PageHeader';
import {
  compareNumber,
  compareText,
  matchesSearch,
  type SortDirection,
} from '@/app/lib/listUtils';

export const AssetConfigurationPage = () => {
  const { data: configurations, isLoading } = useGetAssetConfigurationsQuery();
  const { data: inventoryItems } = useGetInventoryQuery();
  const { data: products } = useGetProductsQuery();

  const [createConfiguration] = useCreateAssetConfigurationMutation();
  const [updateConfiguration] = useUpdateAssetConfigurationMutation();
  const [deleteConfiguration] = useDeleteAssetConfigurationMutation();

  const [search, setSearch] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('all');
  const [locationFilter, setLocationFilter] = useState('all');
  const [sortBy, setSortBy] = useState<
    | 'inventoryName'
    | 'productModelName'
    | 'categoryName'
    | 'quantity'
    | 'standardValue'
    | 'location'
  >('inventoryName');
  const [sortDirection, setSortDirection] = useState<SortDirection>('asc');

  const [inventoryId, setInventoryId] = useState('');
  const [productId, setProductId] = useState('');
  const [quantity, setQuantity] = useState(1);
  const [standardValue, setStandardValue] = useState('');
  const [location, setLocation] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);

  const locationOptions = useMemo(
    () =>
      [...new Set((configurations ?? []).map((item) => item.location?.trim()))]
        .filter((value): value is string => Boolean(value))
        .sort((left, right) =>
          left.localeCompare(right, undefined, { sensitivity: 'base' })
        ),
    [configurations]
  );

  const filtered = useMemo(() => {
    const nextConfigurations = (configurations ?? [])
      .filter((item) =>
        matchesSearch(search, [
          item.inventoryName,
          item.productModelName,
          item.categoryName,
          item.location,
          item.quantity,
          item.standardValue,
        ])
      )
      .filter(
        (item) =>
          categoryFilter === 'all' || item.categoryName === categoryFilter
      )
      .filter(
        (item) =>
          locationFilter === 'all' ||
          (item.location ?? 'Unassigned') === locationFilter
      );

    return [...nextConfigurations].sort((left, right) => {
      switch (sortBy) {
        case 'productModelName':
          return compareText(
            left.productModelName,
            right.productModelName,
            sortDirection
          );
        case 'categoryName':
          return compareText(
            left.categoryName,
            right.categoryName,
            sortDirection
          );
        case 'quantity':
          return compareNumber(left.quantity, right.quantity, sortDirection);
        case 'standardValue':
          return compareNumber(
            left.standardValue,
            right.standardValue,
            sortDirection
          );
        case 'location':
          return compareText(
            left.location ?? 'Unassigned',
            right.location ?? 'Unassigned',
            sortDirection
          );
        case 'inventoryName':
        default:
          return compareText(
            left.inventoryName,
            right.inventoryName,
            sortDirection
          );
      }
    });
  }, [
    configurations,
    search,
    categoryFilter,
    locationFilter,
    sortBy,
    sortDirection,
  ]);

  const resetForm = () => {
    setInventoryId('');
    setProductId('');
    setQuantity(1);
    setStandardValue('');
    setLocation('');
    setEditingId(null);
  };

  const resetFilters = () => {
    setSearch('');
    setCategoryFilter('all');
    setLocationFilter('all');
    setSortBy('inventoryName');
    setSortDirection('asc');
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
      <PageHeader
        title="Computer Composition Maintenance"
        description="Maintain machine-to-component mappings with faster search, filters, sorting, and inline maintenance."
      />

      <div className="rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-4">
        <form
          onSubmit={onSubmit}
          className="grid grid-cols-1 gap-3 md:grid-cols-2 xl:grid-cols-[1.1fr_1.1fr_110px_140px_1fr_auto]"
        >
          <SelectField
            value={inventoryId}
            onChange={(event) => setInventoryId(event.target.value)}
            required
          >
            <option value="">Inventory</option>
            {(inventoryItems ?? []).map((item) => (
              <option key={item.id} value={item.id}>
                {item.assetName}
              </option>
            ))}
          </SelectField>

          <SelectField
            value={productId}
            onChange={(event) => setProductId(event.target.value)}
            required
          >
            <option value="">Product</option>
            {(products ?? []).map((item) => (
              <option key={item.id} value={item.id}>
                {item.modelName}
              </option>
            ))}
          </SelectField>

          <TextInput
            type="number"
            min={1}
            value={quantity}
            onChange={(event) => setQuantity(Number(event.target.value))}
            placeholder="Qty"
            required
          />

          <TextInput
            type="number"
            step="0.01"
            value={standardValue}
            onChange={(event) => setStandardValue(event.target.value)}
            placeholder="Standard value"
          />

          <TextInput
            type="text"
            value={location}
            onChange={(event) => setLocation(event.target.value)}
            placeholder="Location"
          />

          <div className="flex gap-2">
            <Button type="submit" variant="primary" className="flex-1">
              {editingId ? 'Save' : 'Add'}
            </Button>
            {editingId && (
              <Button type="button" variant="secondary" onClick={resetForm}>
                Cancel
              </Button>
            )}
          </div>
        </form>
      </div>

      <ListControls
        searchValue={search}
        onSearchChange={setSearch}
        searchPlaceholder="Search inventory, product, category, location, qty, or value"
        sortValue={sortBy}
        onSortChange={(value) =>
          setSortBy(
            value as
              | 'inventoryName'
              | 'productModelName'
              | 'categoryName'
              | 'quantity'
              | 'standardValue'
              | 'location'
          )
        }
        sortOptions={[
          { value: 'inventoryName', label: 'Inventory' },
          { value: 'productModelName', label: 'Product' },
          { value: 'categoryName', label: 'Category' },
          { value: 'quantity', label: 'Quantity' },
          { value: 'standardValue', label: 'Value' },
          { value: 'location', label: 'Location' },
        ]}
        direction={sortDirection}
        onDirectionChange={setSortDirection}
        onReset={resetFilters}
        resultCount={filtered.length}
      >
        <SelectField
          value={categoryFilter}
          onChange={(event) => setCategoryFilter(event.target.value)}
          aria-label="Filter configurations by category"
        >
          <option value="all">All categories</option>
          {[...new Set((configurations ?? []).map((item) => item.categoryName))]
            .sort((left, right) =>
              left.localeCompare(right, undefined, { sensitivity: 'base' })
            )
            .map((category) => (
              <option key={category} value={category}>
                {category}
              </option>
            ))}
        </SelectField>

        <SelectField
          value={locationFilter}
          onChange={(event) => setLocationFilter(event.target.value)}
          aria-label="Filter configurations by location"
        >
          <option value="all">All locations</option>
          <option value="Unassigned">Unassigned</option>
          {locationOptions.map((locationOption) => (
            <option key={locationOption} value={locationOption}>
              {locationOption}
            </option>
          ))}
        </SelectField>
      </ListControls>

      <div className="overflow-hidden rounded-2xl border border-zinc-800/60">
        <div className="overflow-x-auto">
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
                    No asset configurations match the current filters.
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
                  <td className="px-4 py-3 text-zinc-400">
                    {item.categoryName}
                  </td>
                  <td className="px-4 py-3 text-zinc-200">{item.quantity}</td>
                  <td className="px-4 py-3 text-zinc-400">
                    {item.standardValue === null ? '-' : item.standardValue}
                  </td>
                  <td className="px-4 py-3 text-zinc-400">
                    {item.location ?? '-'}
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-2">
                      <Button
                        size="sm"
                        variant="secondary"
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
                      >
                        Edit
                      </Button>
                      <Button
                        size="sm"
                        variant="danger"
                        onClick={() => void deleteConfiguration(item.id)}
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
    </section>
  );
};
