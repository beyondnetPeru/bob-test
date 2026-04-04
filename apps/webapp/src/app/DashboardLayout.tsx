import { lazy, Suspense, useState } from 'react';
import {
  LayoutDashboard,
  Server,
  Database,
  Tags,
  Building2,
  Layers,
  Activity,
} from 'lucide-react';
import { cn } from '@/app/lib/cn';

type Tab =
  | 'dashboard'
  | 'hardwareinventory'
  | 'products'
  | 'productcategory'
  | 'manufacturer'
  | 'assetconfiguration';

const OperationsDashboard = lazy(() =>
  import('./features/dashboard/OperationsDashboard').then((m) => ({
    default: m.OperationsDashboard,
  }))
);

const InventoryDashboard = lazy(() =>
  import('./features/inventory/InventoryDashboard').then((m) => ({
    default: m.InventoryDashboard,
  }))
);

const ProductCatalog = lazy(() =>
  import('./features/catalog/ProductCatalog').then((m) => ({
    default: m.ProductCatalog,
  }))
);

const ProductCategoryPage = lazy(() =>
  import('./features/categories/ProductCategoryPage').then((m) => ({
    default: m.ProductCategoryPage,
  }))
);

const ManufacturerPage = lazy(() =>
  import('./features/manufacturers/ManufacturerPage').then((m) => ({
    default: m.ManufacturerPage,
  }))
);

const AssetConfigurationPage = lazy(() =>
  import('./features/asset-configurations/AssetConfigurationPage').then(
    (m) => ({
      default: m.AssetConfigurationPage,
    })
  )
);

export const DashboardLayout = () => {
  const [activeTab, setActiveTab] = useState<Tab>('dashboard');

  const navigation = [
    {
      id: 'dashboard',
      name: 'Dashboard',
      icon: LayoutDashboard,
      description: 'Centralized UX and UI flow',
    },
    {
      id: 'hardwareinventory',
      name: 'Computer Catalog',
      icon: Server,
      description: 'Browse and compose computers',
    },
    {
      id: 'products',
      name: 'Component Catalog',
      icon: Database,
      description: 'Maintain reusable parts',
    },
    {
      id: 'productcategory',
      name: 'Component Types',
      icon: Tags,
      description: 'Maintain component groups',
    },
    {
      id: 'manufacturer',
      name: 'Manufacturer',
      icon: Building2,
      description: 'Maintain vendors',
    },
    {
      id: 'assetconfiguration',
      name: 'Computer Composition',
      icon: Layers,
      description: 'Maintain component mappings',
    },
  ] as const;

  return (
    <div className="flex min-h-screen bg-[#09090b] text-zinc-400 font-sans selection:bg-primary/30">
      {/* Dynamic Sidebar */}
      <aside className="w-72 border-r border-zinc-800/50 bg-[#09090b]/50 backdrop-blur-xl flex flex-col sticky top-0 h-screen">
        <div className="p-8">
          <div className="flex items-center gap-3 mb-8">
            <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary to-primary/50 flex items-center justify-center shadow-lg shadow-primary/20">
              <Activity className="w-6 h-6 text-white" />
            </div>
            <div>
              <h2 className="text-xl font-bold text-white tracking-tight">
                FleetOps
              </h2>
              <p className="text-[10px] font-bold text-primary uppercase tracking-[0.2em]">
                Management Suite
              </p>
            </div>
          </div>

          <nav className="space-y-1">
            {navigation.map((item) => (
              <button
                key={item.id}
                onClick={() => setActiveTab(item.id as Tab)}
                className={cn(
                  'w-full group flex items-start gap-4 p-4 rounded-2xl transition-all duration-300 text-left',
                  activeTab === item.id
                    ? 'bg-primary/10 text-primary shadow-sm ring-1 ring-primary/20'
                    : 'hover:bg-zinc-800/50 hover:text-zinc-200'
                )}
              >
                <item.icon
                  className={cn(
                    'w-5 h-5 mt-0.5 transition-colors',
                    activeTab === item.id
                      ? 'text-primary'
                      : 'text-zinc-500 group-hover:text-zinc-300'
                  )}
                />
                <div>
                  <div
                    className={cn(
                      'text-sm font-semibold transition-colors',
                      activeTab === item.id
                        ? 'text-white'
                        : 'text-zinc-400 group-hover:text-zinc-200'
                    )}
                  >
                    {item.name}
                  </div>
                  <p className="text-[11px] text-zinc-500 mt-0.5 leading-relaxed">
                    {item.description}
                  </p>
                </div>
              </button>
            ))}
          </nav>
        </div>

        <div className="mt-auto p-8 border-t border-zinc-800/50 bg-zinc-900/20">
          <div className="flex items-center gap-3 p-3 rounded-xl bg-zinc-800/30 border border-zinc-700/30">
            <div className="w-8 h-8 rounded-lg bg-zinc-700 flex items-center justify-center">
              <LayoutDashboard className="w-4 h-4 text-zinc-400" />
            </div>
            <div className="flex-1 overflow-hidden">
              <p className="text-xs font-semibold text-zinc-200 truncate truncate">
                Data Operations
              </p>
              <p className="text-[10px] text-zinc-500 truncate">
                Entity maintenance enabled
              </p>
            </div>
          </div>
        </div>
      </aside>

      {/* Main Content Area */}
      <main className="flex-1 h-screen overflow-y-auto custom-scrollbar">
        <div className="max-w-7xl mx-auto p-4 md:p-8">
          <Suspense
            fallback={
              <div className="h-64 rounded-2xl bg-zinc-900/30 border border-zinc-800/50 animate-pulse" />
            }
          >
            {activeTab === 'dashboard' && <OperationsDashboard />}
            {activeTab === 'hardwareinventory' && <InventoryDashboard />}
            {activeTab === 'products' && (
              <div className="animate-in fade-in slide-in-from-bottom-2 duration-500">
                <ProductCatalog />
              </div>
            )}
            {activeTab === 'productcategory' && (
              <div className="animate-in fade-in slide-in-from-bottom-2 duration-500">
                <ProductCategoryPage />
              </div>
            )}
            {activeTab === 'manufacturer' && (
              <div className="animate-in fade-in slide-in-from-bottom-2 duration-500">
                <ManufacturerPage />
              </div>
            )}
            {activeTab === 'assetconfiguration' && (
              <div className="animate-in fade-in slide-in-from-bottom-2 duration-500">
                <AssetConfigurationPage />
              </div>
            )}
          </Suspense>
        </div>
      </main>
    </div>
  );
};
