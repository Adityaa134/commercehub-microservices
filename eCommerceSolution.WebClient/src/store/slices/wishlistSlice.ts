import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { WishlistItemResponse } from '../../types/wishlist.types';

export interface WishlistState {
  items: WishlistItemResponse[];
  productIds: string[];
  isInitialized: boolean;
  isItemsLoaded: boolean;
}

const initialState: WishlistState = {
  items: [],
  productIds: [],
  isInitialized: false,
  isItemsLoaded: false,
};

const wishlistSlice = createSlice({
  name: 'wishlist',
  initialState,
  reducers: {
    setWishlistIds: (state, action: PayloadAction<string[]>) => {
      state.productIds = action.payload;
      state.isInitialized = true;
    },

    setWishlistItems: (state, action: PayloadAction<WishlistItemResponse[]>) => {
      state.items = action.payload;
      state.productIds = action.payload.map((item) => item.productId);
      state.isInitialized = true;
      state.isItemsLoaded = true;
    },

    toggleWishlistId: (state, action: PayloadAction<string>) => {
      const productId = action.payload;
      const index = state.productIds.indexOf(productId);
      if (index >= 0) {
        state.productIds.splice(index, 1);
        state.items = state.items.filter((item) => item.productId !== productId);
      } else {
        state.productIds.push(productId);
        state.isItemsLoaded = false;
      }
    },

    removeItemFromWishlist: (state, action: PayloadAction<string>) => {
      const productId = action.payload;
      state.productIds = state.productIds.filter((id) => id !== productId);
      state.items = state.items.filter((item) => item.productId !== productId);
    },

    clearWishlist: (state) => {
      state.items = [];
      state.productIds = [];
      state.isInitialized = false;
      state.isItemsLoaded = false;
    },
  },
});

export const {
  setWishlistIds,
  setWishlistItems,
  toggleWishlistId,
  removeItemFromWishlist,
  clearWishlist,
} = wishlistSlice.actions;

export default wishlistSlice.reducer;
