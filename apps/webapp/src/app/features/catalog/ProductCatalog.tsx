import {
  useGetCategoriesQuery,
  useGetManufacturersQuery,
  useGetProductsQuery,
} from '@/app/api/apiSlice';
import { PageHeader } from '@/app/components/ui/PageHeader';
import { SearchInput } from '@/app/components/ui/SearchInput';
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
    editingId,
    filteredProducts,
    setSearchTerm,
    updateField,
    resetForm,
    startEdit,
    submitForm,
    removeProduct,
  } = useProductPageState(products ?? []);

  return (
    <section className="space-y-6">
      <PageHeader
        title="Product Maintenance"
        description="List, search, add, edit, and delete product records."
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

      <SearchInput
        value={searchTerm}
        onChange={setSearchTerm}
        placeholder="Search model, category, or manufacturer"
      />

      <ProductTable
        products={filteredProducts}
        isLoading={isLoading}
        onEdit={startEdit}
        onDelete={removeProduct}
      />
    </section>
  );
};
