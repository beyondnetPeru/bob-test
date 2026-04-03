import type { FormEvent } from 'react';
import { Button } from '@/app/components/ui/Button';
import { SelectField, TextInput } from '@/app/components/ui/Field';
import type { Category, Manufacturer } from '@/app/api/apiSlice';
import type { ProductFormState } from '../hooks/useProductPageState';

interface ProductFormProps {
  value: ProductFormState;
  categories: Category[];
  manufacturers: Manufacturer[];
  isEditing: boolean;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
  onChange: (field: keyof ProductFormState, value: string) => void;
  onCancel: () => void;
}

export const ProductForm = ({
  value,
  categories,
  manufacturers,
  isEditing,
  onSubmit,
  onChange,
  onCancel,
}: ProductFormProps) => {
  return (
    <form
      onSubmit={onSubmit}
      className="rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-4"
    >
      <div className="grid grid-cols-1 gap-3 md:grid-cols-2 xl:grid-cols-5">
        <TextInput
          value={value.modelName}
          onChange={(event) => onChange('modelName', event.target.value)}
          placeholder="Model name"
          required
        />

        <TextInput
          value={value.description}
          onChange={(event) => onChange('description', event.target.value)}
          placeholder="Description"
        />

        <SelectField
          value={value.categoryId}
          onChange={(event) => onChange('categoryId', event.target.value)}
          required
        >
          <option value="">Category</option>
          {categories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </SelectField>

        <SelectField
          value={value.manufacturerId}
          onChange={(event) => onChange('manufacturerId', event.target.value)}
          required
        >
          <option value="">Manufacturer</option>
          {manufacturers.map((manufacturer) => (
            <option key={manufacturer.id} value={manufacturer.id}>
              {manufacturer.name}
            </option>
          ))}
        </SelectField>

        <div className="flex gap-2">
          <Button type="submit" variant="primary" className="flex-1">
            {isEditing ? 'Save' : 'Add'}
          </Button>
          {isEditing && (
            <Button type="button" variant="secondary" onClick={onCancel}>
              Cancel
            </Button>
          )}
        </div>
      </div>
    </form>
  );
};
