import type { ReactNode } from 'react';

interface PageHeaderProps {
  title: string;
  description: string;
  meta?: ReactNode;
}

export const PageHeader = ({ title, description, meta }: PageHeaderProps) => {
  return (
    <header className="flex flex-col gap-4 py-2 md:flex-row md:items-end md:justify-between">
      <div>
        <h1 className="text-3xl font-bold tracking-tight text-white">
          {title}
        </h1>
        <p className="mt-2 text-sm font-medium text-zinc-400">{description}</p>
      </div>
      {meta}
    </header>
  );
};
