import {
  useGetCategoriesQuery,
  useGetManufacturersQuery,
  useGetProductsQuery,
} from '@/app/api/apiSlice';
import { ListControls } from '@/app/components/ui/ListControls';
import { SelectField } from '@/app/components/ui/Field';
import { PageHeader } from '@/app/components/ui/PageHeader';
import { ProductForm } from './components/ProductForm';
import { ProductTable } from './components/ProductTable';
import { useProductPageState } from './hooks/useProductPageState';

export const ProductCatalog = () => {
  const { data: products, isLoading } = useGetProductsQuery();
  const { data: categories } = useGetCategoriesQuery();
  const { data: manufacturers } = useGetManufacturersQuery();

  const {
    form,
    searchTerm,
    categoryFilter,
    manufacturerFilter,
    sortBy,
    sortDirection,
    editingId,
    filteredProducts,
    setSearchTerm,
    setCategoryFilter,
    setManufacturerFilter,
    setSortBy,
    setSortDirection,
    updateField,
    resetForm,
    resetFilters,
    startEdit,
    submitForm,
    removeProduct,
  } = useProductPageState(products ?? []);

  return (
    <section className="space-y-6">
      <PageHeader
        title="Component Catalog Maintenance"
        description="List, search, filter, sort, add, edit, and delete reusable component records."
      />

      <ProductForm
        value={form}
        categories={categories ?? []}
        manufacturers={manufacturers ?? []}
        isEditing={editingId !== null}
        onSubmit={submitForm}
        onChange={updateField}
        onCancel={resetForm}
      />

      <ListControls
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        searchPlaceholder="Search model, description, category, or manufacturer"
        sortValue={sortBy}
        onSortChange={(value) =>
          setSortBy(value as 'modelName' | 'categoryName' | 'manufacturerName')
        }
        sortOptions={[
          { value: 'modelName', label: 'Model' },
          { value: 'categoryName', label: 'Category' },
          { value: 'manufacturerName', label: 'Manufacturer' },
        ]}
        direction={sortDirection}
        onDirectionChange={setSortDirection}
        onReset={resetFilters}
        resultCount={filteredProducts.length}
      >
        <SelectField
          value={categoryFilter}
          onChange={(event) => setCategoryFilter(event.target.value)}
          aria-label="Filter products by category"
        >
          <option value="all">All categories</option>
          {(categories ?? []).map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </SelectField>

        <SelectField
          value={manufacturerFilter}
          onChange={(event) => setManufacturerFilter(event.target.value)}
          aria-label="Filter products by manufacturer"
        >
          <option value="all">All manufacturers</option>
          {(manufacturers ?? []).map((manufacturer) => (
            <option key={manufacturer.id} value={manufacturer.id}>
              {manufacturer.name}
            </option>
          ))}
        </SelectField>
      </ListControls>

      <ProductTable
        products={filteredProducts}
        isLoading={isLoading}
        onEdit={startEdit}
        onDelete={removeProduct}
      />
    </section>
  );
};
