import axiosInstance from './axios';
import {
  CategoryAddRequest,
  CategoryUpdateRequest,
  CategoryStatusUpdateRequest,
  CategoryResponse,
} from '../types/category.types';

export const categoryService = {
  getCategories: async (): Promise<CategoryResponse[]> => {
    const response = await axiosInstance.get<CategoryResponse[]>('/api/categories');
    return response.data;
  },

  getCategoriesAdmin: async (): Promise<CategoryResponse[]> => {
    const response = await axiosInstance.get<CategoryResponse[]>('/api/categories/admin');
    return response.data;
  },

  getCategoryById: async (categoryId: string): Promise<CategoryResponse> => {
    const response = await axiosInstance.get<CategoryResponse>(`/api/categories/${categoryId}`);
    return response.data;
  },

  getCategoryByIdAdmin: async (categoryId: string): Promise<CategoryResponse> => {
    const response = await axiosInstance.get<CategoryResponse>(`/api/categories/admin/${categoryId}`);
    return response.data;
  },

  createCategory: async (data: CategoryAddRequest | FormData): Promise<CategoryResponse> => {
    const response = await axiosInstance.post<CategoryResponse>('/api/categories', data, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  updateCategory: async (
    categoryId: string,
    data: CategoryUpdateRequest | FormData
  ): Promise<CategoryResponse> => {
    const response = await axiosInstance.put<CategoryResponse>(`/api/categories/${categoryId}`, data, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  updateCategoryStatus: async (
    categoryId: string,
    data: CategoryStatusUpdateRequest
  ): Promise<CategoryResponse> => {
    const response = await axiosInstance.patch<CategoryResponse>(
      `/api/categories/${categoryId}/status`,
      data
    );
    return response.data;
  },
};

export default categoryService;
