export type SortDirection = 'asc' | 'desc';

const collator = new Intl.Collator(undefined, {
  numeric: true,
  sensitivity: 'base',
});

export const normalizeSearchValue = (value: unknown) =>
  String(value ?? '')
    .toLowerCase()
    .normalize('NFKD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/\s+/g, ' ')
    .trim();

export const matchesSearch = (query: string, values: unknown[]) => {
  const normalizedQuery = normalizeSearchValue(query);

  if (!normalizedQuery) {
    return true;
  }

  return values.some((value) =>
    normalizeSearchValue(value).includes(normalizedQuery)
  );
};

export const compareText = (
  left: unknown,
  right: unknown,
  direction: SortDirection = 'asc'
) => {
  const result = collator.compare(
    normalizeSearchValue(left),
    normalizeSearchValue(right)
  );

  return direction === 'asc' ? result : result * -1;
};

export const compareNumber = (
  left: number | null | undefined,
  right: number | null | undefined,
  direction: SortDirection = 'asc'
) => {
  const safeLeft = left ?? Number.NEGATIVE_INFINITY;
  const safeRight = right ?? Number.NEGATIVE_INFINITY;
  const result = safeLeft - safeRight;

  return direction === 'asc' ? result : result * -1;
};
