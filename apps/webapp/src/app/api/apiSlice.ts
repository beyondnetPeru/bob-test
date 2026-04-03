import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5001/api';

export interface InventoryItem {
  id: string; // GUID
  assetName: string;
  weightKg: number;
  performanceTier: string | null;
  componentCount: number;
}

export interface Manufacturer {
  id: string; // GUID
  name: string;
}

export interface Category {
  id: string; // GUID
  name: string;
}

export interface Product {
  id: string; // GUID
  modelName: string;
  description: string;
  categoryId: string; // GUID
  categoryName: string;
  manufacturerId: string; // GUID
  manufacturerName: string;
}

export interface AssetConfiguration {
  id: string;
  inventoryId: string;
  inventoryName: string;
  productId: string;
  productModelName: string;
  categoryName: string;
  quantity: number;
  standardValue: number | null;
  location: string | null;
}

export interface ProductUpsertRequest {
  categoryId: string;
  manufacturerId: string;
  modelName: string;
  description: string;
}

export interface AssetConfigurationUpsertRequest {
  inventoryId: string;
  productId: string;
  quantity: number;
  standardValue: number | null;
  location: string | null;
}

export const apiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({ baseUrl: API_BASE_URL }),
  tagTypes: [
    'Inventory',
    'Manufacturer',
    'Category',
    'Product',
    'AssetConfiguration',
  ],
  endpoints: (builder) => ({
    // --- Inventory ---
    getInventory: builder.query<InventoryItem[], void>({
      query: () => '/inventory',
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ id }) => ({ type: 'Inventory' as const, id })),
              { type: 'Inventory', id: 'LIST' },
            ]
          : [{ type: 'Inventory', id: 'LIST' }],
    }),
    createInventory: builder.mutation<string, Partial<InventoryItem>>({
      query: (body) => ({ url: '/inventory', method: 'POST', body }),
      invalidatesTags: [{ type: 'Inventory', id: 'LIST' }],
    }),
    updateInventory: builder.mutation<void, Partial<InventoryItem>>({
      query: ({ id, ...patch }) => ({
        url: `/inventory/${id}`,
        method: 'PUT',
        body: patch,
      }),
      invalidatesTags: (result, error, { id }) => [{ type: 'Inventory', id }],
    }),
    deleteInventory: builder.mutation<void, string>({
      query: (id) => ({ url: `/inventory/${id}`, method: 'DELETE' }),
      invalidatesTags: [{ type: 'Inventory', id: 'LIST' }],
    }),

    // --- Manufacturers ---
    getManufacturers: builder.query<Manufacturer[], void>({
      query: () => '/manufacturer',
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ id }) => ({
                type: 'Manufacturer' as const,
                id,
              })),
              { type: 'Manufacturer', id: 'LIST' },
            ]
          : [{ type: 'Manufacturer', id: 'LIST' }],
    }),
    createManufacturer: builder.mutation<string, Partial<Manufacturer>>({
      query: (body) => ({ url: '/manufacturer', method: 'POST', body }),
      invalidatesTags: [{ type: 'Manufacturer', id: 'LIST' }],
    }),
    updateManufacturer: builder.mutation<void, Manufacturer>({
      query: ({ id, ...body }) => ({
        url: `/manufacturer/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (result, error, { id }) => [
        { type: 'Manufacturer', id },
      ],
    }),
    deleteManufacturer: builder.mutation<void, string>({
      query: (id) => ({ url: `/manufacturer/${id}`, method: 'DELETE' }),
      invalidatesTags: [{ type: 'Manufacturer', id: 'LIST' }],
    }),

    // --- Categories ---
    getCategories: builder.query<Category[], void>({
      query: () => '/category',
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ id }) => ({ type: 'Category' as const, id })),
              { type: 'Category', id: 'LIST' },
            ]
          : [{ type: 'Category', id: 'LIST' }],
    }),
    createCategory: builder.mutation<string, Partial<Category>>({
      query: (body) => ({ url: '/category', method: 'POST', body }),
      invalidatesTags: [{ type: 'Category', id: 'LIST' }],
    }),
    updateCategory: builder.mutation<void, Category>({
      query: ({ id, ...body }) => ({
        url: `/category/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (result, error, { id }) => [{ type: 'Category', id }],
    }),
    deleteCategory: builder.mutation<void, string>({
      query: (id) => ({ url: `/category/${id}`, method: 'DELETE' }),
      invalidatesTags: [{ type: 'Category', id: 'LIST' }],
    }),

    // --- Products ---
    getProducts: builder.query<Product[], void>({
      query: () => '/product',
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ id }) => ({ type: 'Product' as const, id })),
              { type: 'Product', id: 'LIST' },
            ]
          : [{ type: 'Product', id: 'LIST' }],
    }),
    createProduct: builder.mutation<string, ProductUpsertRequest>({
      query: (body) => ({ url: '/product', method: 'POST', body }),
      invalidatesTags: [{ type: 'Product', id: 'LIST' }],
    }),
    updateProduct: builder.mutation<
      void,
      { id: string } & ProductUpsertRequest
    >({
      query: ({ id, ...body }) => ({
        url: `/product/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (result, error, { id }) => [{ type: 'Product', id }],
    }),
    deleteProduct: builder.mutation<void, string>({
      query: (id) => ({ url: `/product/${id}`, method: 'DELETE' }),
      invalidatesTags: [{ type: 'Product', id: 'LIST' }],
    }),

    // --- Asset Configurations ---
    getAssetConfigurations: builder.query<AssetConfiguration[], void>({
      query: () => '/assetconfiguration',
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ id }) => ({
                type: 'AssetConfiguration' as const,
                id,
              })),
              { type: 'AssetConfiguration', id: 'LIST' },
            ]
          : [{ type: 'AssetConfiguration', id: 'LIST' }],
    }),
    createAssetConfiguration: builder.mutation<
      string,
      AssetConfigurationUpsertRequest
    >({
      query: (body) => ({ url: '/assetconfiguration', method: 'POST', body }),
      invalidatesTags: [{ type: 'AssetConfiguration', id: 'LIST' }],
    }),
    updateAssetConfiguration: builder.mutation<
      void,
      { id: string } & AssetConfigurationUpsertRequest
    >({
      query: ({ id, ...body }) => ({
        url: `/assetconfiguration/${id}`,
        method: 'PUT',
        body: { id, ...body },
      }),
      invalidatesTags: (result, error, { id }) => [
        { type: 'AssetConfiguration', id },
      ],
    }),
    deleteAssetConfiguration: builder.mutation<void, string>({
      query: (id) => ({ url: `/assetconfiguration/${id}`, method: 'DELETE' }),
      invalidatesTags: [{ type: 'AssetConfiguration', id: 'LIST' }],
    }),
  }),
});

export const {
  useGetInventoryQuery,
  useCreateInventoryMutation,
  useUpdateInventoryMutation,
  useDeleteInventoryMutation,
  useGetManufacturersQuery,
  useCreateManufacturerMutation,
  useUpdateManufacturerMutation,
  useDeleteManufacturerMutation,
  useGetCategoriesQuery,
  useCreateCategoryMutation,
  useUpdateCategoryMutation,
  useDeleteCategoryMutation,
  useGetProductsQuery,
  useCreateProductMutation,
  useUpdateProductMutation,
  useDeleteProductMutation,
  useGetAssetConfigurationsQuery,
  useCreateAssetConfigurationMutation,
  useUpdateAssetConfigurationMutation,
  useDeleteAssetConfigurationMutation,
} = apiSlice;
