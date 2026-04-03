import React, { useState } from 'react';
import { InventoryDashboard } from './features/inventory/InventoryDashboard';
import { ProductCatalog } from './features/catalog/ProductCatalog';
import { ReferenceData } from './features/reference/ReferenceData';
import {
  Layout,
  Server,
  Database,
  Settings,
  Shield,
  Activity,
} from 'lucide-react';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

type Tab = 'fleet' | 'catalog' | 'reference';

export const DashboardLayout: React.FC = () => {
  const [activeTab, setActiveTab] = useState<Tab>('fleet');

  const navigation = [
    {
      id: 'fleet',
      name: 'Fleet Inventory',
      icon: Server,
      description: 'Manage hardware assets',
    },
    {
      id: 'catalog',
      name: 'Product Catalog',
      icon: Database,
      description: 'Technical specifications',
    },
    {
      id: 'reference',
      name: 'Reference Data',
      icon: Settings,
      description: 'System definitions',
    },
  ];

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
              <Shield className="w-4 h-4 text-zinc-400" />
            </div>
            <div className="flex-1 overflow-hidden">
              <p className="text-xs font-semibold text-zinc-200 truncate truncate">
                Admin Terminal
              </p>
              <p className="text-[10px] text-zinc-500 truncate">
                v1.2.4-stable
              </p>
            </div>
          </div>
        </div>
      </aside>

      {/* Main Content Area */}
      <main className="flex-1 h-screen overflow-y-auto custom-scrollbar">
        <div className="max-w-7xl mx-auto p-4 md:p-8">
          {activeTab === 'fleet' && <InventoryDashboard />}
          {activeTab === 'catalog' && (
            <div className="animate-in fade-in slide-in-from-bottom-2 duration-500">
              <ProductCatalog />
            </div>
          )}
          {activeTab === 'reference' && (
            <div className="animate-in fade-in slide-in-from-bottom-2 duration-500">
              <ReferenceData />
            </div>
          )}
        </div>
      </main>
    </div>
  );
};
