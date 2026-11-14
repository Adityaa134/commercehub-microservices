export interface WishlistItemResponse {
  productId: string;
  productName: string;
  unitPrice: number;
  productImagePath: string | null;
  categoryName: string | null;
  averageRating: number;
  totalRatings: number;
  inStock: boolean;
  addedAt: string;
}

export interface WishlistResponse {
  userId: string;
  totalItems: number;
  items: WishlistItemResponse[];
}

export interface WishlistToggleResponse {
  productId: string;
  isInWishlist: boolean;
}

