import { useMemo, useState, type FormEvent } from 'react';
import {
  type InventoryItem,
  useCreateInventoryMutation,
  useDeleteInventoryMutation,
} from '@/app/api/apiSlice';
import {
  compareNumber,
  compareText,
  matchesSearch,
  type SortDirection,
} from '@/app/lib/listUtils';

export interface InventoryFormState {
  assetName: string;
  deviceCategory: string;
  weightKg: string;
}

type InventorySortField =
  | 'assetName'
  | 'deviceCategory'
  | 'performanceTier'
  | 'weightKg'
  | 'componentCount';

const initialFormState: InventoryFormState = {
  assetName: '',
  deviceCategory: 'Desktop PC',
  weightKg: '',
};

export const useInventoryPageState = (items: InventoryItem[] = []) => {
  const [createInventory] = useCreateInventoryMutation();
  const [deleteInventory] = useDeleteInventoryMutation();

  const [searchTerm, setSearchTerm] = useState('');
  const [deviceCategoryFilter, setDeviceCategoryFilter] = useState('all');
  const [tierFilter, setTierFilter] = useState('all');
  const [sortBy, setSortBy] = useState<InventorySortField>('assetName');
  const [sortDirection, setSortDirection] = useState<SortDirection>('asc');
  const [editingItem, setEditingItem] = useState<InventoryItem | null>(null);
  const [form, setForm] = useState<InventoryFormState>(initialFormState);

  const filteredItems = useMemo(() => {
    const nextItems = items
      .filter((item) =>
        matchesSearch(searchTerm, [
          item.assetName,
          item.deviceCategory,
          item.performanceTier,
          item.cpu,
          item.ram,
          item.hardDisc,
          item.ports,
          item.video,
          item.powerSource,
          item.weightKg,
          item.componentCount,
          item.id.slice(0, 8),
        ])
      )
      .filter(
        (item) =>
          deviceCategoryFilter === 'all' ||
          item.deviceCategory === deviceCategoryFilter
      )
      .filter(
        (item) =>
          tierFilter === 'all' ||
          (item.performanceTier ?? 'Unknown') === tierFilter
      );

    return [...nextItems].sort((left, right) => {
      switch (sortBy) {
        case 'deviceCategory':
          return compareText(
            left.deviceCategory,
            right.deviceCategory,
            sortDirection
          );
        case 'performanceTier':
          return compareText(
            left.performanceTier ?? 'Unknown',
            right.performanceTier ?? 'Unknown',
            sortDirection
          );
        case 'weightKg':
          return compareNumber(left.weightKg, right.weightKg, sortDirection);
        case 'componentCount':
          return compareNumber(
            left.componentCount,
            right.componentCount,
            sortDirection
          );
        case 'assetName':
        default:
          return compareText(left.assetName, right.assetName, sortDirection);
      }
    });
  }, [
    items,
    searchTerm,
    deviceCategoryFilter,
    tierFilter,
    sortBy,
    sortDirection,
  ]);

  const updateField = (field: keyof InventoryFormState, value: string) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const resetFilters = () => {
    setSearchTerm('');
    setDeviceCategoryFilter('all');
    setTierFilter('all');
    setSortBy('assetName');
    setSortDirection('asc');
  };

  const submitCreate = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!form.assetName.trim() || !form.weightKg) return;

    await createInventory({
      assetName: form.assetName.trim(),
      deviceCategory: form.deviceCategory,
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
    deviceCategoryFilter,
    tierFilter,
    sortBy,
    sortDirection,
    editingItem,
    filteredItems,
    setSearchTerm,
    setDeviceCategoryFilter,
    setTierFilter,
    setSortBy,
    setSortDirection,
    setEditingItem,
    updateField,
    resetFilters,
    submitCreate,
    removeItem,
  };
};
