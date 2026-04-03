import type {
  InputHTMLAttributes,
  ReactNode,
  SelectHTMLAttributes,
} from 'react';
import { cn } from '../../lib/cn';

const fieldBaseClassName =
  'h-10 w-full rounded-xl border border-zinc-700 bg-zinc-900/60 px-3 text-sm text-zinc-100 outline-none transition-all focus:border-primary focus-visible:ring-2 focus-visible:ring-primary/50';

interface TextInputProps extends InputHTMLAttributes<HTMLInputElement> {
  startAdornment?: ReactNode;
}

export const TextInput = ({
  className,
  startAdornment,
  ...props
}: TextInputProps) => {
  if (!startAdornment) {
    return <input className={cn(fieldBaseClassName, className)} {...props} />;
  }

  return (
    <div className="relative">
      <span className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-zinc-500">
        {startAdornment}
      </span>
      <input
        className={cn(fieldBaseClassName, 'pl-10', className)}
        {...props}
      />
    </div>
  );
};

export const SelectField = ({
  className,
  children,
  ...props
}: SelectHTMLAttributes<HTMLSelectElement>) => (
  <select className={cn(fieldBaseClassName, className)} {...props}>
    {children}
  </select>
);
