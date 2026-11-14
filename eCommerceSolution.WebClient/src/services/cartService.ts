import axiosInstance from './axios';
import {
  CartResponse,
  CartItemResponse,
  CartItemAddRequest,
  CartItemUpdateRequest,
} from '../types/cart.types';

export const cartService = {
  getCart: async (): Promise<CartResponse> => {
    const response = await axiosInstance.get<CartResponse>('/api/cart');
    return response.data;
  },

  addItemToCart: async (data: CartItemAddRequest): Promise<CartItemResponse> => {
    const response = await axiosInstance.post<CartItemResponse>('/api/cart/items', data);
    return response.data;
  },

  updateCartItemQuantity: async (data: CartItemUpdateRequest): Promise<CartItemResponse> => {
    const response = await axiosInstance.put<CartItemResponse>('/api/cart/items', data);
    return response.data;
  },

  removeItemFromCart: async (productId: string): Promise<void> => {
    await axiosInstance.delete(`/api/cart/items/${productId}`);
  },

  mergeCart: async (cartItems: CartItemAddRequest[]): Promise<CartResponse> => {
    const response = await axiosInstance.post<CartResponse>('/api/cart/merge', cartItems);
    return response.data;
  },
};

export default cartService;

