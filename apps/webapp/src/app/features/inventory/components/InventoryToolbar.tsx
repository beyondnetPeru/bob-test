import type { FormEvent } from 'react';
import { Button } from '@/app/components/ui/Button';
import { SelectField, TextInput } from '@/app/components/ui/Field';
import { ListControls } from '@/app/components/ui/ListControls';
import { DEVICE_CATEGORY_OPTIONS } from '@/app/lib/computerCatalog';
import type { SortDirection } from '@/app/lib/listUtils';
import type { InventoryFormState } from '../hooks/useInventoryPageState';

interface InventoryToolbarProps {
  searchTerm: string;
  form: InventoryFormState;
  sortBy:
    | 'assetName'
    | 'deviceCategory'
    | 'performanceTier'
    | 'weightKg'
    | 'componentCount';
  sortDirection: SortDirection;
  deviceCategoryFilter: string;
  tierFilter: string;
  resultCount: number;
  onSearchChange: (value: string) => void;
  onSortChange: (
    value:
      | 'assetName'
      | 'deviceCategory'
      | 'performanceTier'
      | 'weightKg'
      | 'componentCount'
  ) => void;
  onDirectionChange: (value: SortDirection) => void;
  onDeviceCategoryFilterChange: (value: string) => void;
  onTierChange: (value: string) => void;
  onFieldChange: (field: keyof InventoryFormState, value: string) => void;
  onResetFilters: () => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
}

export const InventoryToolbar = ({
  searchTerm,
  form,
  sortBy,
  sortDirection,
  deviceCategoryFilter,
  tierFilter,
  resultCount,
  onSearchChange,
  onSortChange,
  onDirectionChange,
  onDeviceCategoryFilterChange,
  onTierChange,
  onFieldChange,
  onResetFilters,
  onSubmit,
}: InventoryToolbarProps) => {
  return (
    <div className="grid gap-4 xl:grid-cols-[minmax(0,1.35fr)_520px]">
      <ListControls
        searchValue={searchTerm}
        onSearchChange={onSearchChange}
        searchPlaceholder="Search computer, category, CPU, RAM, storage, video, ports, or id"
        sortValue={sortBy}
        onSortChange={(value) =>
          onSortChange(
            value as
              | 'assetName'
              | 'deviceCategory'
              | 'performanceTier'
              | 'weightKg'
              | 'componentCount'
          )
        }
        sortOptions={[
          { value: 'assetName', label: 'Computer' },
          { value: 'deviceCategory', label: 'Category' },
          { value: 'performanceTier', label: 'Tier' },
          { value: 'weightKg', label: 'Weight' },
          { value: 'componentCount', label: 'Components' },
        ]}
        direction={sortDirection}
        onDirectionChange={onDirectionChange}
        onReset={onResetFilters}
        resultCount={resultCount}
      >
        <SelectField
          value={deviceCategoryFilter}
          onChange={(event) => onDeviceCategoryFilterChange(event.target.value)}
          aria-label="Filter inventory by device category"
        >
          <option value="all">All device categories</option>
          {DEVICE_CATEGORY_OPTIONS.map((category) => (
            <option key={category} value={category}>
              {category}
            </option>
          ))}
        </SelectField>

        <SelectField
          value={tierFilter}
          onChange={(event) => onTierChange(event.target.value)}
          aria-label="Filter inventory by performance tier"
        >
          <option value="all">All tiers</option>
          <option value="Entry">Entry</option>
          <option value="Mid-Range">Mid-Range</option>
          <option value="High-End">High-End</option>
          <option value="Workstation">Workstation</option>
          <option value="Unknown">Unknown</option>
        </SelectField>
      </ListControls>

      <form
        onSubmit={onSubmit}
        className="grid gap-2 rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-3 md:grid-cols-[1fr_150px_120px_72px]"
      >
        <TextInput
          value={form.assetName}
          onChange={(event) => onFieldChange('assetName', event.target.value)}
          placeholder="Computer name"
          required
        />
        <SelectField
          value={form.deviceCategory}
          onChange={(event) =>
            onFieldChange('deviceCategory', event.target.value)
          }
          aria-label="Quick create device category"
        >
          {DEVICE_CATEGORY_OPTIONS.map((category) => (
            <option key={category} value={category}>
              {category}
            </option>
          ))}
        </SelectField>
        <TextInput
          type="number"
          step="0.01"
          min="0.01"
          value={form.weightKg}
          onChange={(event) => onFieldChange('weightKg', event.target.value)}
          placeholder="Weight kg"
          required
        />
        <Button type="submit" variant="primary">
          Add
        </Button>
      </form>
    </div>
  );
};
