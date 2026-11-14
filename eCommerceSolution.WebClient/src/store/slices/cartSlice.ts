import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { CartItemResponse, CartResponse } from '../../types/cart.types';

export interface CartState {
  cartId: string | null;
  items: CartItemResponse[];
  totalItems: number;
  cartTotal: number;
  isInitialized: boolean;
}

const updateTotals = (state: CartState) => {
  state.totalItems = state.items.reduce((sum, item) => sum + item.quantity, 0);
  const total = state.items.reduce((sum, item) => sum + item.totalPrice, 0);
  state.cartTotal = Math.round(total * 100) / 100;
};

const initialState: CartState = {
  cartId: null,
  items: [],
  totalItems: 0,
  cartTotal: 0,
  isInitialized: false,
};

const cartSlice = createSlice({
  name: 'cart',
  initialState,
  reducers: {
    setCart: (state, action: PayloadAction<CartResponse>) => {
      state.cartId = action.payload.cartId;
      state.items = action.payload.cartItems;
      state.cartTotal = action.payload.cartTotal;
      state.totalItems = action.payload.totalItems;
      state.isInitialized = true;
    },

    addItem: (state, action: PayloadAction<CartItemResponse>) => {
      const incomingItem = action.payload;
      const existingIndex = state.items.findIndex(
        (item) => item.productId === incomingItem.productId
      );

      if (existingIndex >= 0) {
        state.items[existingIndex] = incomingItem;
      } else {
        state.items.push(incomingItem);
      }

      updateTotals(state);
    },

    updateQuantity: (state, action: PayloadAction<CartItemResponse>) => {
      const updatedItem = action.payload;
      const existingIndex = state.items.findIndex(
        (item) => item.productId === updatedItem.productId
      );

      if (existingIndex >= 0) {
        state.items[existingIndex] = updatedItem;
        updateTotals(state);
      }
    },

    removeItem: (state, action: PayloadAction<string>) => {
      state.items = state.items.filter((item) => item.productId !== action.payload);
      updateTotals(state);
    },

    clearCart: (state) => {
      state.cartId = null;
      state.items = [];
      state.totalItems = 0;
      state.cartTotal = 0;
      state.isInitialized = true;
    },
  },
});

export const { setCart, addItem, updateQuantity, removeItem, clearCart } = cartSlice.actions;
export default cartSlice.reducer;
