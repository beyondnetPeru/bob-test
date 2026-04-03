import type { FormEvent } from 'react';
import { Button } from '@/app/components/ui/Button';
import { TextInput } from '@/app/components/ui/Field';
import { SearchInput } from '@/app/components/ui/SearchInput';
import type { InventoryFormState } from '../hooks/useInventoryPageState';

interface InventoryToolbarProps {
  searchTerm: string;
  form: InventoryFormState;
  onSearchChange: (value: string) => void;
  onFieldChange: (field: keyof InventoryFormState, value: string) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
}

export const InventoryToolbar = ({
  searchTerm,
  form,
  onSearchChange,
  onFieldChange,
  onSubmit,
}: InventoryToolbarProps) => {
  return (
    <div className="grid gap-3 md:grid-cols-[1fr_420px]">
      <SearchInput
        value={searchTerm}
        onChange={onSearchChange}
        placeholder="Search by asset name or performance tier"
      />

      <form
        onSubmit={onSubmit}
        className="grid grid-cols-[1fr_140px_72px] gap-2"
      >
        <TextInput
          value={form.assetName}
          onChange={(event) => onFieldChange('assetName', event.target.value)}
          placeholder="Asset name"
          required
        />
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
