import { Search } from 'lucide-react';
import { TextInput } from './Field';

interface SearchInputProps {
  value: string;
  onChange: (value: string) => void;
  placeholder: string;
}

export const SearchInput = ({
  value,
  onChange,
  placeholder,
}: SearchInputProps) => {
  return (
    <TextInput
      type="search"
      value={value}
      onChange={(event) => onChange(event.target.value)}
      placeholder={placeholder}
      startAdornment={<Search className="h-4 w-4" />}
    />
  );
};
