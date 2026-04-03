import { Button } from '@/app/components/ui/Button';
import type { Product } from '@/app/api/apiSlice';

interface ProductTableProps {
  products: Product[];
  isLoading: boolean;
  onEdit: (product: Product) => void;
  onDelete: (id: string) => void;
}

export const ProductTable = ({
  products,
  isLoading,
  onEdit,
  onDelete,
}: ProductTableProps) => {
  return (
    <div className="overflow-hidden rounded-2xl border border-zinc-800/60">
      <table className="w-full text-sm">
        <thead className="bg-zinc-900/50 text-zinc-400">
          <tr>
            <th className="px-4 py-3 text-left">Model</th>
            <th className="px-4 py-3 text-left">Description</th>
            <th className="px-4 py-3 text-left">Category</th>
            <th className="px-4 py-3 text-left">Manufacturer</th>
            <th className="px-4 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          {isLoading && (
            <tr>
              <td colSpan={5} className="px-4 py-4 text-zinc-500">
                Loading products...
              </td>
            </tr>
          )}
          {!isLoading && products.length === 0 && (
            <tr>
              <td colSpan={5} className="px-4 py-4 text-zinc-500">
                No products found.
              </td>
            </tr>
          )}
          {products.map((product) => (
            <tr key={product.id} className="border-t border-zinc-800/40">
              <td className="px-4 py-3 text-zinc-100">{product.modelName}</td>
              <td className="px-4 py-3 text-zinc-400">
                {product.description || '-'}
              </td>
              <td className="px-4 py-3 text-zinc-300">
                {product.categoryName}
              </td>
              <td className="px-4 py-3 text-zinc-300">
                {product.manufacturerName}
              </td>
              <td className="px-4 py-3">
                <div className="flex justify-end gap-2">
                  <Button
                    size="sm"
                    variant="secondary"
                    onClick={() => onEdit(product)}
                  >
                    Edit
                  </Button>
                  <Button
                    size="sm"
                    variant="danger"
                    onClick={() => void onDelete(product.id)}
                  >
                    Delete
                  </Button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
