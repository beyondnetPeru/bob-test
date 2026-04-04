import { useMemo, useState, type FormEvent } from 'react';
import {
  type Product,
  useCreateProductMutation,
  useDeleteProductMutation,
  useUpdateProductMutation,
} from '@/app/api/apiSlice';
import {
  compareText,
  matchesSearch,
  type SortDirection,
} from '@/app/lib/listUtils';

export interface ProductFormState {
  modelName: string;
  description: string;
  categoryId: string;
  manufacturerId: string;
}

type ProductSortField = 'modelName' | 'categoryName' | 'manufacturerName';

const initialFormState: ProductFormState = {
  modelName: '',
  description: '',
  categoryId: '',
  manufacturerId: '',
};

export const useProductPageState = (products: Product[] = []) => {
  const [createProduct] = useCreateProductMutation();
  const [updateProduct] = useUpdateProductMutation();
  const [deleteProduct] = useDeleteProductMutation();

  const [searchTerm, setSearchTerm] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('all');
  const [manufacturerFilter, setManufacturerFilter] = useState('all');
  const [sortBy, setSortBy] = useState<ProductSortField>('modelName');
  const [sortDirection, setSortDirection] = useState<SortDirection>('asc');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<ProductFormState>(initialFormState);

  const filteredProducts = useMemo(() => {
    const nextProducts = products
      .filter((product) =>
        matchesSearch(searchTerm, [
          product.modelName,
          product.description,
          product.categoryName,
          product.manufacturerName,
        ])
      )
      .filter(
        (product) =>
          categoryFilter === 'all' || product.categoryId === categoryFilter
      )
      .filter(
        (product) =>
          manufacturerFilter === 'all' ||
          product.manufacturerId === manufacturerFilter
      );

    return [...nextProducts].sort((left, right) => {
      switch (sortBy) {
        case 'categoryName':
          return compareText(
            left.categoryName,
            right.categoryName,
            sortDirection
          );
        case 'manufacturerName':
          return compareText(
            left.manufacturerName,
            right.manufacturerName,
            sortDirection
          );
        case 'modelName':
        default:
          return compareText(left.modelName, right.modelName, sortDirection);
      }
    });
  }, [
    products,
    searchTerm,
    categoryFilter,
    manufacturerFilter,
    sortBy,
    sortDirection,
  ]);

  const updateField = (field: keyof ProductFormState, value: string) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const resetForm = () => {
    setEditingId(null);
    setForm(initialFormState);
  };

  const resetFilters = () => {
    setSearchTerm('');
    setCategoryFilter('all');
    setManufacturerFilter('all');
    setSortBy('modelName');
    setSortDirection('asc');
  };

  const startEdit = (product: Product) => {
    setEditingId(product.id);
    setForm({
      modelName: product.modelName,
      description: product.description,
      categoryId: product.categoryId,
      manufacturerId: product.manufacturerId,
    });
  };

  const submitForm = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!form.modelName.trim() || !form.categoryId || !form.manufacturerId) {
      return;
    }

    const payload = {
      modelName: form.modelName.trim(),
      description: form.description.trim(),
      categoryId: form.categoryId,
      manufacturerId: form.manufacturerId,
    };

    if (editingId) {
      await updateProduct({ id: editingId, ...payload }).unwrap();
    } else {
      await createProduct(payload).unwrap();
    }

    resetForm();
  };

  const removeProduct = async (id: string) => {
    await deleteProduct(id).unwrap();
  };

  return {
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
  };
};
