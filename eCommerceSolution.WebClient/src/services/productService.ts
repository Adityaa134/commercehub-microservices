import axiosInstance from './axios';
import {
  ProductAddRequest,
  ProductUpdateRequest,
  ProductResponse,
  ProductQueryParameters,
  PagedResult,
} from '../types/product.types';

export const productService = {
  getProducts: async (params?: ProductQueryParameters): Promise<PagedResult<ProductResponse>> => {
    const response = await axiosInstance.get<PagedResult<ProductResponse>>('/api/products', {
      params,
    });
    return response.data;
  },

  getProductById: async (productId: string): Promise<ProductResponse> => {
    const response = await axiosInstance.get<ProductResponse>(`/api/products/${productId}`);
    return response.data;
  },

  addProduct: async (data: ProductAddRequest | FormData): Promise<ProductResponse> => {
    const response = await axiosInstance.post<ProductResponse>('/api/products', data, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  updateProduct: async (
    productId: string,
    data: ProductUpdateRequest | FormData
  ): Promise<ProductResponse> => {
    const response = await axiosInstance.put<ProductResponse>(`/api/products/${productId}`, data, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },
};

export default productService;
