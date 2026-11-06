export interface ProductAddRequest {
  productName: string;
  description: string;
  isActive: boolean;
  unitPrice: number;
  categoryId: string;
  quantityInStock: number;
  productImage: File;
}

export interface ProductUpdateRequest {
  productName: string;
  description: string;
  unitPrice: number;
  categoryId: string;
  quantityInStock: number;
  productImage?: File | null;
}

export interface ProductResponse {
  productId: string;
  productName: string;
  description: string;
  productImagePath: string;
  unitPrice: number;
  quantityInStock: number;
  categoryId: string;
  categoryName: string;
  averageRating: number;
  totalRatings: number;
  isActive: boolean;
  createdAt: string;
}

export interface ProductQueryParameters {
  search?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  minRating?: number;
  sortBy?: string;
  page?: number;
  limit?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  limit: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface SortOption {
  id: string;
  label: string;
}

export const SORT_OPTIONS: SortOption[] = [
  { id: 'featured', label: 'Featured' },
  { id: 'rating', label: 'Rating' },
  { id: 'price-low', label: 'Price: low to high' },
  { id: 'price-high', label: 'Price: high to low' },
  { id: 'newest', label: 'Newest' },
];
