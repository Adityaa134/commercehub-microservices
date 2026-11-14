import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { CategoryResponse } from '../../types/category.types';

export interface CategoryState {
  items: CategoryResponse[];
}

const initialState: CategoryState = {
  items: [],
};

const categorySlice = createSlice({
  name: 'categories',
  initialState,
  reducers: {
    setCategories: (state, action: PayloadAction<CategoryResponse[]>) => {
      state.items = action.payload;
    },
  },
});

export const { setCategories } = categorySlice.actions;
export default categorySlice.reducer;

