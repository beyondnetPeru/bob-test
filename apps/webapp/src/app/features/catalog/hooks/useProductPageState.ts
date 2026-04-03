import { useMemo, useState, type FormEvent } from 'react';
import {
  type Product,
  useCreateProductMutation,
  useDeleteProductMutation,
  useUpdateProductMutation,
} from '@/app/api/apiSlice';

export interface ProductFormState {
  modelName: string;
  description: string;
  categoryId: string;
  manufacturerId: string;
}

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
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<ProductFormState>(initialFormState);

  const filteredProducts = useMemo(
    () =>
      products.filter(
        (product) =>
          product.modelName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          product.categoryName
            .toLowerCase()
            .includes(searchTerm.toLowerCase()) ||
          product.manufacturerName
            .toLowerCase()
            .includes(searchTerm.toLowerCase())
      ),
    [products, searchTerm]
  );

  const updateField = (field: keyof ProductFormState, value: string) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const resetForm = () => {
    setEditingId(null);
    setForm(initialFormState);
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
    editingId,
    filteredProducts,
    setSearchTerm,
    updateField,
    resetForm,
    startEdit,
    submitForm,
    removeProduct,
  };
};
