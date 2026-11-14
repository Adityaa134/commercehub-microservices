import { createApi, fakeBaseQuery } from '@reduxjs/toolkit/query/react';
import productService from '../../services/productService';
import {
  PagedResult,
  ProductResponse,
  ProductQueryParameters,
  ProductUpdateRequest,
  ProductAddRequest,
} from '../../types/product.types';

export const productApiSlice = createApi({
  reducerPath: 'productApi',
  baseQuery: fakeBaseQuery(),
  tagTypes: ['Product'],
  endpoints: (builder) => ({
    getProducts: builder.query<PagedResult<ProductResponse>, ProductQueryParameters | void>({
      async queryFn(params) {
        try {
          const data = await productService.getProducts(params || undefined);
          return { data };
        } catch (error) {
          return { error };
        }
      },
      providesTags: ['Product'],
      keepUnusedDataFor: 120,
    }),

    getProductById: builder.query<ProductResponse, string>({
      async queryFn(productId) {
        try {
          const data = await productService.getProductById(productId);
          return { data };
        } catch (error) {
          return { error };
        }
      },
      providesTags: (_result, _error, id) => [{ type: 'Product', id }],
    }),

    addProduct: builder.mutation<ProductResponse, ProductAddRequest | FormData>({
      async queryFn(data) {
        try {
          const result = await productService.addProduct(data);
          return { data: result };
        } catch (error) {
          return { error };
        }
      },
      invalidatesTags: ['Product'],
    }),

    updateProduct: builder.mutation<ProductResponse, { id: string; data: ProductUpdateRequest | FormData }>({
      async queryFn({ id, data }) {
        try {
          const result = await productService.updateProduct(id, data);
          return { data: result };
        } catch (error) {
          return { error };
        }
      },
      invalidatesTags: ['Product'],
    }),
  }),
});

export const {
  useGetProductsQuery,
  useGetProductByIdQuery,
  useAddProductMutation,
  useUpdateProductMutation,
} = productApiSlice;

