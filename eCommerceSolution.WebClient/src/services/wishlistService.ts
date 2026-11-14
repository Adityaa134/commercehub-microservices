import axiosInstance from './axios';
import {
  WishlistResponse,
  WishlistToggleResponse,
} from '../types/wishlist.types';

export const wishlistService = {
  getWishlist: async (): Promise<WishlistResponse> => {
    const response = await axiosInstance.get<WishlistResponse>('/api/wishlist');
    return response.data;
  },

  getWishlistProductIds: async (): Promise<string[]> => {
    const response = await axiosInstance.get<string[]>('/api/wishlist/ids');
    return response.data;
  },

  toggleWishlist: async (productId: string): Promise<WishlistToggleResponse> => {
    const response = await axiosInstance.post<WishlistToggleResponse>(`/api/wishlist/toggle/${productId}`);
    return response.data;
  },

  removeFromWishlist: async (productId: string): Promise<void> => {
    await axiosInstance.delete(`/api/wishlist/${productId}`);
  },
};

export default wishlistService;

