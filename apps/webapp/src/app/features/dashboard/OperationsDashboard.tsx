import {
  useGetInventoryQuery,
  useGetProductsQuery,
  useGetCategoriesQuery,
  useGetManufacturersQuery,
  useGetAssetConfigurationsQuery,
} from '../../api/apiSlice';
import { Activity, Boxes, Building2, Layers, Server, Tags } from 'lucide-react';

export const OperationsDashboard = () => {
  const { data: inventory } = useGetInventoryQuery();
  const { data: products } = useGetProductsQuery();
  const { data: categories } = useGetCategoriesQuery();
  const { data: manufacturers } = useGetManufacturersQuery();
  const { data: assetConfigurations } = useGetAssetConfigurationsQuery();

  const cards = [
    {
      title: 'Computer Catalog',
      value: inventory?.length ?? 0,
      helper: 'Tracked composed computers',
      icon: Server,
    },
    {
      title: 'Component Catalog',
      value: products?.length ?? 0,
      helper: 'Reusable CPU/GPU/RAM/etc. records',
      icon: Boxes,
    },
    {
      title: 'Component Types',
      value: categories?.length ?? 0,
      helper: 'Technical slot definitions',
      icon: Tags,
    },
    {
      title: 'Manufacturers',
      value: manufacturers?.length ?? 0,
      helper: 'Registered vendors',
      icon: Building2,
    },
    {
      title: 'Asset Configurations',
      value: assetConfigurations?.length ?? 0,
      helper: 'Machine-product relationships',
      icon: Layers,
    },
  ];

  return (
    <section className="space-y-8">
      <header>
        <h1 className="text-3xl font-bold text-white tracking-tight">
          Operations Dashboard
        </h1>
        <p className="mt-2 text-sm text-zinc-400">
          Centralized maintenance hub for browsing complete computers, managing
          reusable components, and composing new device builds.
        </p>
      </header>

      <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
        {cards.map((card) => (
          <article
            key={card.title}
            className="glass-card p-5 border border-zinc-800/60 rounded-2xl"
          >
            <div className="flex items-center justify-between">
              <p className="text-xs font-semibold uppercase tracking-wider text-zinc-500">
                {card.title}
              </p>
              <card.icon className="w-4 h-4 text-primary" />
            </div>
            <p className="mt-3 text-3xl font-bold text-white">{card.value}</p>
            <p className="mt-1 text-xs text-zinc-500">{card.helper}</p>
          </article>
        ))}
      </div>

      <div className="rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-6">
        <h2 className="text-lg font-semibold text-white flex items-center gap-2">
          <Activity className="w-4 h-4 text-primary" />
          UX and UI Baseline
        </h2>
        <ul className="mt-4 space-y-2 text-sm text-zinc-300">
          <li>Use search before editing to reduce accidental updates.</li>
          <li>
            Use explicit add/edit forms with required fields and clear labels.
          </li>
          <li>
            Keep destructive actions visible and always paired with clear
            affordances.
          </li>
          <li>
            Use keyboard-focusable controls and semantic tables for
            maintainability.
          </li>
        </ul>
      </div>
    </section>
  );
};
