export interface CategoryAddRequest {
  categoryName: string;
  description: string;
  isActive: boolean;
  parentCategoryId?: string | null;
  categoryImage?: File | null;
}

export interface CategoryUpdateRequest {
  categoryName: string;
  description: string;
  parentCategoryId?: string | null;
  categoryImage?: File | null;
}

export interface CategoryStatusUpdateRequest {
  isActive: boolean;
}

export interface CategoryResponse {
  categoryId: string;
  categoryName: string;
  description: string;
  categoryImagePath: string;
  isActive: boolean;
  parentCategoryId: string | null;
  displayOrder: number;
  subCategories: CategoryResponse[];
}
