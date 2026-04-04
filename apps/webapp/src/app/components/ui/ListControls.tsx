import type { ReactNode } from 'react';
import { ArrowUpDown, ListFilter } from 'lucide-react';
import { Button } from './Button';
import { SelectField } from './Field';
import { SearchInput } from './SearchInput';
import type { SortDirection } from '@/app/lib/listUtils';

interface ListControlsProps {
  searchValue: string;
  onSearchChange: (value: string) => void;
  searchPlaceholder: string;
  sortValue: string;
  onSortChange: (value: string) => void;
  sortOptions: Array<{ value: string; label: string }>;
  direction: SortDirection;
  onDirectionChange: (value: SortDirection) => void;
  onReset?: () => void;
  resultCount?: number;
  children?: ReactNode;
}

export const ListControls = ({
  searchValue,
  onSearchChange,
  searchPlaceholder,
  sortValue,
  onSortChange,
  sortOptions,
  direction,
  onDirectionChange,
  onReset,
  resultCount,
  children,
}: ListControlsProps) => {
  return (
    <div className="rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-3">
      <div className="grid gap-3 lg:grid-cols-[minmax(0,1.5fr)_220px_160px_auto]">
        <SearchInput
          value={searchValue}
          onChange={onSearchChange}
          placeholder={searchPlaceholder}
        />

        <SelectField
          value={sortValue}
          onChange={(event) => onSortChange(event.target.value)}
          aria-label="Sort records by"
        >
          {sortOptions.map((option) => (
            <option key={option.value} value={option.value}>
              Sort by: {option.label}
            </option>
          ))}
        </SelectField>

        <SelectField
          value={direction}
          onChange={(event) =>
            onDirectionChange(event.target.value as SortDirection)
          }
          aria-label="Sort direction"
        >
          <option value="asc">Ascending</option>
          <option value="desc">Descending</option>
        </SelectField>

        {onReset ? (
          <Button
            variant="ghost"
            className="gap-2 border border-zinc-800/80"
            onClick={onReset}
          >
            <ArrowUpDown className="h-4 w-4" />
            Reset
          </Button>
        ) : (
          <div className="hidden lg:block" />
        )}
      </div>

      {(children || typeof resultCount === 'number') && (
        <div className="mt-3 flex flex-col gap-3 xl:flex-row xl:items-center xl:justify-between">
          {children ? (
            <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-4">
              {children}
            </div>
          ) : (
            <div />
          )}

          {typeof resultCount === 'number' && (
            <div className="inline-flex items-center gap-2 rounded-full border border-zinc-800/70 bg-zinc-950/70 px-3 py-1 text-xs text-zinc-400">
              <ListFilter className="h-3.5 w-3.5" />
              {resultCount} matching record{resultCount === 1 ? '' : 's'}
            </div>
          )}
        </div>
      )}
    </div>
  );
};
