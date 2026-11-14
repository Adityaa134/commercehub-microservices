export interface CartItemAddRequest {
  productId: string;
  quantity: number;
}

export interface CartItemUpdateRequest {
  productId: string;
  quantity: number;
}

export interface CartItemResponse {
  productId: string;
  productName: string;
  productImagePath?: string | null;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface CartResponse {
  cartId: string;
  userId: string;
  cartItems: CartItemResponse[];
  cartTotal: number;
  totalItems: number;
}

