import { useMemo, useState, type FormEvent } from 'react';
import {
  type InventoryItem,
  useCreateInventoryMutation,
  useDeleteInventoryMutation,
} from '@/app/api/apiSlice';

export interface InventoryFormState {
  assetName: string;
  weightKg: string;
}

const initialFormState: InventoryFormState = {
  assetName: '',
  weightKg: '',
};

export const useInventoryPageState = (items: InventoryItem[] = []) => {
  const [createInventory] = useCreateInventoryMutation();
  const [deleteInventory] = useDeleteInventoryMutation();

  const [searchTerm, setSearchTerm] = useState('');
  const [editingItem, setEditingItem] = useState<InventoryItem | null>(null);
  const [form, setForm] = useState<InventoryFormState>(initialFormState);

  const filteredItems = useMemo(
    () =>
      items.filter(
        (item) =>
          item.assetName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          item.performanceTier?.toLowerCase().includes(searchTerm.toLowerCase())
      ),
    [items, searchTerm]
  );

  const updateField = (field: keyof InventoryFormState, value: string) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const submitCreate = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!form.assetName.trim() || !form.weightKg) return;

    await createInventory({
      assetName: form.assetName.trim(),
      weightKg: Number(form.weightKg),
    }).unwrap();

    setForm(initialFormState);
  };

  const removeItem = async (id: string) => {
    await deleteInventory(id).unwrap();
  };

  return {
    form,
    searchTerm,
    editingItem,
    filteredItems,
    setSearchTerm,
    setEditingItem,
    updateField,
    submitCreate,
    removeItem,
  };
};
