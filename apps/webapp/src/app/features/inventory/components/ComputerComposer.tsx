import { useMemo, useState, type FormEvent } from 'react';
import {
  useCreateAssetConfigurationMutation,
  useCreateInventoryMutation,
  useGetProductsQuery,
} from '@/app/api/apiSlice';
import { Button } from '@/app/components/ui/Button';
import { SelectField, TextInput } from '@/app/components/ui/Field';
import { DEVICE_CATEGORY_OPTIONS } from '@/app/lib/computerCatalog';
import { CheckCircle2, Cpu, HardDrive, Monitor, PlugZap } from 'lucide-react';

type MessageState = {
  tone: 'success' | 'error';
  text: string;
} | null;

const initialFormState = {
  assetName: '',
  deviceCategory: 'Desktop PC',
  weightKg: '',
  cpuId: '',
  gpuId: '',
  ramId: '',
  storageId: '',
  psuId: '',
};

export const ComputerComposer = () => {
  const { data: products } = useGetProductsQuery();
  const [createInventory] = useCreateInventoryMutation();
  const [createAssetConfiguration] = useCreateAssetConfigurationMutation();

  const [form, setForm] = useState(initialFormState);
  const [portQuantities, setPortQuantities] = useState<Record<string, string>>(
    {}
  );
  const [message, setMessage] = useState<MessageState>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const productGroups = useMemo(() => {
    const allProducts = products ?? [];

    return {
      cpu: allProducts.filter((product) => product.categoryName === 'CPU'),
      gpu: allProducts.filter((product) => product.categoryName === 'GPU'),
      ram: allProducts.filter((product) => product.categoryName === 'RAM'),
      storage: allProducts.filter(
        (product) => product.categoryName === 'STORAGE'
      ),
      psu: allProducts.filter((product) => product.categoryName === 'PSU'),
      ports: allProducts.filter((product) => product.categoryName === 'PORT'),
    };
  }, [products]);

  const updateField = (field: keyof typeof initialFormState, value: string) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const resetForm = () => {
    setForm(initialFormState);
    setPortQuantities({});
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setMessage(null);

    if (
      !form.assetName.trim() ||
      !form.weightKg ||
      !form.cpuId ||
      !form.gpuId ||
      !form.ramId ||
      !form.storageId ||
      !form.psuId
    ) {
      setMessage({
        tone: 'error',
        text: 'Select the required component types before composing the computer.',
      });
      return;
    }

    setIsSubmitting(true);

    try {
      const inventoryId = await createInventory({
        assetName: form.assetName.trim(),
        weightKg: Number(form.weightKg),
        deviceCategory: form.deviceCategory,
      }).unwrap();

      const selections = [
        { productId: form.cpuId, quantity: 1, location: 'CPU Socket' },
        { productId: form.gpuId, quantity: 1, location: 'PCIe Slot' },
        { productId: form.ramId, quantity: 1, location: 'DIMM Slot' },
        { productId: form.storageId, quantity: 1, location: 'Drive Bay' },
        { productId: form.psuId, quantity: 1, location: 'Power Bay' },
        ...Object.entries(portQuantities)
          .map(([productId, quantity]) => ({
            productId,
            quantity: Number(quantity),
            location: 'I/O Panel',
          }))
          .filter(
            (selection) =>
              Number.isFinite(selection.quantity) && selection.quantity > 0
          ),
      ];

      await Promise.all(
        selections.map((selection) =>
          createAssetConfiguration({
            inventoryId,
            productId: selection.productId,
            quantity: selection.quantity,
            standardValue: null,
            location: selection.location,
          }).unwrap()
        )
      );

      setMessage({
        tone: 'success',
        text: `Computer \"${form.assetName.trim()}\" created successfully.`,
      });
      resetForm();
    } catch {
      setMessage({
        tone: 'error',
        text: 'Unable to compose the computer. Verify the API is running and try again.',
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <section className="rounded-2xl border border-zinc-800/60 bg-zinc-900/30 p-4 md:p-5">
      <div className="mb-4 flex items-start justify-between gap-4">
        <div>
          <h2 className="text-lg font-semibold text-white">
            Computer Composer
          </h2>
          <p className="mt-1 text-sm text-zinc-400">
            Create a computer by selecting one component per type and optional
            port quantities.
          </p>
        </div>
        <div className="rounded-full border border-zinc-800/70 bg-zinc-950/70 px-3 py-1 text-xs text-zinc-400">
          Builder workflow enabled
        </div>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="grid gap-3 md:grid-cols-3">
          <TextInput
            value={form.assetName}
            onChange={(event) => updateField('assetName', event.target.value)}
            placeholder="Computer name"
            required
          />
          <SelectField
            value={form.deviceCategory}
            onChange={(event) =>
              updateField('deviceCategory', event.target.value)
            }
            aria-label="Device category"
          >
            {DEVICE_CATEGORY_OPTIONS.map((category) => (
              <option key={category} value={category}>
                {category}
              </option>
            ))}
          </SelectField>
          <TextInput
            type="number"
            min="0.1"
            step="0.01"
            value={form.weightKg}
            onChange={(event) => updateField('weightKg', event.target.value)}
            placeholder="Weight (kg)"
            required
          />
        </div>

        <div className="grid gap-3 xl:grid-cols-2">
          <div className="grid gap-3 md:grid-cols-2">
            <label className="space-y-1 text-sm text-zinc-300">
              <span className="flex items-center gap-2 font-medium">
                <Cpu className="h-4 w-4 text-primary" />
                CPU
              </span>
              <SelectField
                value={form.cpuId}
                onChange={(event) => updateField('cpuId', event.target.value)}
                required
              >
                <option value="">Select CPU</option>
                {productGroups.cpu.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.modelName}
                  </option>
                ))}
              </SelectField>
            </label>

            <label className="space-y-1 text-sm text-zinc-300">
              <span className="flex items-center gap-2 font-medium">
                <Monitor className="h-4 w-4 text-primary" />
                Video
              </span>
              <SelectField
                value={form.gpuId}
                onChange={(event) => updateField('gpuId', event.target.value)}
                required
              >
                <option value="">Select GPU</option>
                {productGroups.gpu.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.modelName}
                  </option>
                ))}
              </SelectField>
            </label>

            <label className="space-y-1 text-sm text-zinc-300">
              <span className="font-medium">RAM</span>
              <SelectField
                value={form.ramId}
                onChange={(event) => updateField('ramId', event.target.value)}
                required
              >
                <option value="">Select RAM</option>
                {productGroups.ram.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.modelName}
                  </option>
                ))}
              </SelectField>
            </label>

            <label className="space-y-1 text-sm text-zinc-300">
              <span className="flex items-center gap-2 font-medium">
                <HardDrive className="h-4 w-4 text-primary" />
                Hard Disc
              </span>
              <SelectField
                value={form.storageId}
                onChange={(event) =>
                  updateField('storageId', event.target.value)
                }
                required
              >
                <option value="">Select storage</option>
                {productGroups.storage.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.modelName}
                  </option>
                ))}
              </SelectField>
            </label>

            <label className="space-y-1 text-sm text-zinc-300 md:col-span-2">
              <span className="flex items-center gap-2 font-medium">
                <PlugZap className="h-4 w-4 text-primary" />
                Power Source
              </span>
              <SelectField
                value={form.psuId}
                onChange={(event) => updateField('psuId', event.target.value)}
                required
              >
                <option value="">Select PSU</option>
                {productGroups.psu.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.modelName}
                  </option>
                ))}
              </SelectField>
            </label>
          </div>

          <div className="rounded-2xl border border-zinc-800/60 bg-zinc-950/60 p-3">
            <h3 className="text-sm font-semibold text-white">Ports</h3>
            <p className="mt-1 text-xs text-zinc-500">
              Set a quantity for any port type you want on this computer.
            </p>
            <div className="mt-3 grid gap-2 sm:grid-cols-2">
              {productGroups.ports.map((product) => (
                <label
                  key={product.id}
                  className="rounded-xl border border-zinc-800/70 bg-zinc-900/60 p-3 text-sm text-zinc-300"
                >
                  <span className="mb-2 block font-medium text-zinc-100">
                    {product.modelName}
                  </span>
                  <TextInput
                    type="number"
                    min="0"
                    step="1"
                    value={portQuantities[product.id] ?? '0'}
                    onChange={(event) =>
                      setPortQuantities((current) => ({
                        ...current,
                        [product.id]: event.target.value,
                      }))
                    }
                    placeholder="Qty"
                  />
                </label>
              ))}
            </div>
          </div>
        </div>

        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          {message ? (
            <div
              className={`inline-flex items-center gap-2 rounded-full px-3 py-1.5 text-xs ${
                message.tone === 'success'
                  ? 'border border-green-500/30 bg-green-500/10 text-green-300'
                  : 'border border-red-500/30 bg-red-500/10 text-red-300'
              }`}
            >
              <CheckCircle2 className="h-3.5 w-3.5" />
              {message.text}
            </div>
          ) : (
            <span className="text-xs text-zinc-500">
              Required component types: CPU, Video, RAM, Hard Disc, and Power
              Source.
            </span>
          )}

          <div className="flex gap-2">
            <Button
              variant="secondary"
              onClick={resetForm}
              disabled={isSubmitting}
            >
              Reset
            </Button>
            <Button type="submit" variant="primary" disabled={isSubmitting}>
              {isSubmitting ? 'Composing…' : 'Compose Computer'}
            </Button>
          </div>
        </div>
      </form>
    </section>
  );
};
