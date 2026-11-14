import { useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../../store';
import { addItem, updateQuantity, removeItem } from '../../store/slices/cartSlice';
import { toggleWishlistId } from '../../store/slices/wishlistSlice';
import cartService from '../../services/cartService';
import wishlistService from '../../services/wishlistService';
import { ProductResponse } from '../../types/product.types';
import {
  CartItemAddRequest,
  CartItemUpdateRequest,
  CartItemResponse,
} from '../../types/cart.types';
import Button from '../common/Button';

const GUEST_CART_STORAGE_KEY = 'commercehub_guest_cart';
const MAX_ITEM_LIMIT = 5;

function saveGuestCart(items: CartItemResponse[]): void {
  try {
    localStorage.setItem(GUEST_CART_STORAGE_KEY, JSON.stringify(items));
  } catch {}
}

interface ProductCardProps {
  product: ProductResponse;
  rank?: number;
  onAddToCart?: (product: ProductResponse) => void;
  onToggleWishlist?: (productId: string) => void;
  isWishlisted?: boolean;
}

export function ProductCard({
  product,
  rank,
  onAddToCart,
  onToggleWishlist,
  isWishlisted: propIsWishlisted,
}: ProductCardProps) {
  const dispatch = useDispatch();

  const cartItems = useSelector((state: RootState) => state.cart.items);
  const isCartInitialized = useSelector((state: RootState) => state.cart.isInitialized);
  const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);
  const wishlistIds = useSelector((state: RootState) => state.wishlist.productIds);

  const [hover, setHover] = useState(false);
  const [imageError, setImageError] = useState(false);
  const [isUpdating, setIsUpdating] = useState(false);

  const isWishlisted = propIsWishlisted !== undefined ? propIsWishlisted : wishlistIds.includes(product.productId);

  const cartItem = cartItems.find((item) => item.productId === product.productId);
  const quantity = cartItem?.quantity || 0;
  const isInCart = quantity > 0;

  const maxAllowedQuantity = Math.min(product.quantityInStock ?? MAX_ITEM_LIMIT, MAX_ITEM_LIMIT);
  const isMaxReached = quantity >= maxAllowedQuantity;

  const isProductNew = (() => {
    if (!product.createdAt) return false;
    const createdTime = new Date(product.createdAt).getTime();
    if (isNaN(createdTime)) return false;
    const thirtyDaysMs = 30 * 24 * 60 * 60 * 1000;
    return Date.now() - createdTime <= thirtyDaysMs;
  })();

  const handleWishlistClick = async (e: React.MouseEvent) => {
    e.stopPropagation();
    dispatch(toggleWishlistId(product.productId));
    onToggleWishlist?.(product.productId);
    try {
      await wishlistService.toggleWishlist(product.productId);
    } catch (err) {
      dispatch(toggleWishlistId(product.productId));
      console.error('Failed to toggle wishlist:', err);
    }
  };

  const handleRemove = async () => {
    setIsUpdating(true);
    try {
      if (isAuthenticated) {
        await cartService.removeItemFromCart(product.productId);
        dispatch(removeItem(product.productId));
      } else {
        dispatch(removeItem(product.productId));
        saveGuestCart(cartItems.filter((i) => i.productId !== product.productId));
      }
    } catch (err) {
      console.error('Failed to remove item:', err);
    } finally {
      setIsUpdating(false);
    }
  };

  const handleUpdateQuantity = async (newQuantity: number) => {
    if (newQuantity <= 0) {
      return handleRemove();
    }
    if (newQuantity > maxAllowedQuantity) {
      return;
    }

    setIsUpdating(true);
    try {
      if (isAuthenticated) {
        const updateRequest: CartItemUpdateRequest = {
          productId: product.productId,
          quantity: newQuantity,
        };
        const updatedItem: CartItemResponse = await cartService.updateCartItemQuantity(updateRequest);
        dispatch(updateQuantity(updatedItem));
      } else {
        const updatedItem: CartItemResponse = {
          productId: product.productId,
          productName: product.productName,
          productImagePath: product.productImagePath || null,
          quantity: newQuantity,
          unitPrice: product.unitPrice,
          totalPrice: Math.round(newQuantity * product.unitPrice * 100) / 100,
        };
        dispatch(updateQuantity(updatedItem));
        saveGuestCart(
          cartItems.map((i) => (i.productId === product.productId ? updatedItem : i))
        );
      }
    } catch (err) {
      console.error('Failed to update quantity:', err);
    } finally {
      setIsUpdating(false);
    }
  };

  const handleAddToCart = async () => {
    setIsUpdating(true);
    try {
      if (isAuthenticated) {
        const addRequest: CartItemAddRequest = {
          productId: product.productId,
          quantity: 1,
        };
        const res: CartItemResponse = await cartService.addItemToCart(addRequest);
        dispatch(addItem(res));
      } else {
        const newItem: CartItemResponse = {
          productId: product.productId,
          productName: product.productName,
          productImagePath: product.productImagePath || null,
          quantity: 1,
          unitPrice: product.unitPrice,
          totalPrice: product.unitPrice,
        };
        dispatch(addItem(newItem));
        saveGuestCart([...cartItems, newItem]);
      }
      onAddToCart?.(product);
    } catch (err) {
      console.error('Failed to add to cart:', err);
    } finally {
      setIsUpdating(false);
    }
  };

  return (
    <div
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      className="bg-white border border-[#E9E6DD] rounded-[14px] overflow-hidden flex flex-col transition-all duration-200 group hover:-translate-y-1 hover:shadow-[0_12px_28px_rgba(20,22,26,0.09)]"
    >
      <div className="relative aspect-square bg-[#F0EEE7] overflow-hidden">
        {!imageError && product.productImagePath ? (
          <img
            src={product.productImagePath}
            alt={product.productName}
            onError={() => setImageError(true)}
            className="w-full h-full object-cover transition-transform duration-400 ease-out group-hover:scale-105"
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center text-[#3355FF] font-bold text-3xl">
            {product.productName[0]?.toUpperCase() || 'P'}
          </div>
        )}

        {rank !== undefined ? (
          <span
            className={`absolute top-1.5 left-1.5 sm:top-2.5 sm:left-2.5 text-white text-[9.5px] sm:text-[11px] font-bold px-1.5 sm:px-2 py-0.5 rounded-full tracking-wide z-10 select-none shadow-xs ${
              rank <= 3 ? 'bg-[#FF8A3D]' : 'bg-[#16181C]'
            }`}
          >
            #{rank}
          </span>
        ) : (
          isProductNew && (
            <span className="absolute top-1.5 left-1.5 sm:top-2.5 sm:left-2.5 bg-[#16181C] text-white text-[9.5px] sm:text-[11px] font-semibold px-1.5 sm:px-2 py-0.5 sm:py-1 rounded-full tracking-[0.2px] z-10 select-none">
              New
            </span>
          )
        )}

        {isAuthenticated && (
          <button
            type="button"
            onClick={handleWishlistClick}
            aria-label={isWishlisted ? 'Remove from wishlist' : 'Save to wishlist'}
            className="absolute top-1.5 right-1.5 sm:top-2.5 sm:right-2.5 w-6 h-6 sm:w-[30px] sm:h-[30px] rounded-full border-none bg-white/92 backdrop-blur-xs flex items-center justify-center cursor-pointer shadow-2xs hover:scale-105 transition-transform z-10"
          >
            <svg
              className="w-3 h-3 sm:w-3.5 sm:h-3.5"
              viewBox="0 0 24 24"
              fill={isWishlisted ? '#16181C' : 'none'}
              stroke="#16181C"
              strokeWidth="2"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z"
              />
            </svg>
          </button>
        )}

        <div
          className={`absolute left-2 sm:left-2.5 right-2 sm:right-2.5 z-10 transition-all duration-200 ${
            isInCart
              ? 'bottom-2 sm:bottom-2.5 opacity-100'
              : hover
              ? 'bottom-2 sm:bottom-2.5 opacity-100'
              : '-bottom-12 opacity-0 pointer-events-none'
          }`}
        >
          {!isCartInitialized ? (
            <div className="w-full h-8 sm:h-9 rounded-[8px] sm:rounded-[10px] bg-neutral-300/80 animate-pulse" />
          ) : isInCart ? (
            <div className="w-full h-8 sm:h-9 rounded-[8px] sm:rounded-[10px] bg-[#3355FF] text-white flex items-center justify-between px-1.5 sm:px-2 shadow-sm select-none">
              <Button
                variant="none"
                disabled={isUpdating}
                onClick={(e) => {
                  e.stopPropagation();
                  handleUpdateQuantity(quantity - 1);
                }}
                aria-label="Decrease quantity"
                className="w-6 h-6 sm:w-7 sm:h-7 p-0 flex items-center justify-center text-white/90 hover:text-white hover:bg-white/15 rounded-md transition-colors text-sm font-bold disabled:opacity-50"
              >
                —
              </Button>

              <span className="text-[11px] sm:text-[12.5px] font-semibold text-white truncate px-1">
                {quantity} in cart
              </span>

              <Button
                variant="none"
                disabled={isUpdating || isMaxReached}
                onClick={(e) => {
                  e.stopPropagation();
                  handleUpdateQuantity(quantity + 1);
                }}
                aria-label="Increase quantity"
                className="w-6 h-6 sm:w-7 sm:h-7 p-0 flex items-center justify-center text-white/90 hover:text-white hover:bg-white/15 rounded-md transition-colors text-sm font-bold disabled:opacity-40 disabled:cursor-not-allowed"
              >
                +
              </Button>
            </div>
          ) : (
            <Button
              variant="none"
              isLoading={isUpdating}
              disabled={isUpdating}
              onClick={(e) => {
                e.stopPropagation();
                handleAddToCart();
              }}
              className="w-full h-8 sm:h-9 rounded-[8px] sm:rounded-[10px] bg-[#3355FF] hover:bg-[#2544DD] text-white text-[11.5px] sm:text-[13px] font-semibold flex items-center justify-center gap-1.5 sm:gap-2 shadow-sm transition-colors disabled:opacity-60 cursor-pointer"
            >
              {!isUpdating && (
                <svg
                  className="w-3.5 h-3.5 sm:w-4 sm:h-4"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="2"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437M7.5 14.25a3 3 0 00-3 3h15.75m-12.75-3h11.218c1.121-2.3 2.1-4.684 2.924-7.138a60.114 60.114 0 00-16.536-1.84M7.5 14.25L5.106 5.272M6 20.25a.75.75 0 11-1.5 0 .75.75 0 011.5 0zm12.75 0a.75.75 0 11-1.5 0 .75.75 0 011.5 0z"
                  />
                </svg>
              )}
              <span>Add to cart</span>
            </Button>
          )}
        </div>
      </div>

      <div className="p-2 sm:p-3 flex flex-col gap-0.5 flex-1">
        <span className="text-[9px] sm:text-[10.5px] text-[#83887B] uppercase tracking-[0.4px] select-none truncate">
          {product.categoryName || 'GENERAL'}
        </span>

        <h3 className="text-[12px] sm:text-[13.5px] font-semibold text-[#16181C] leading-[1.3] truncate">
          {product.productName}
        </h3>

        <div className="flex items-center gap-0.5 my-0.5">
          {[1, 2, 3, 4, 5].map((star) => (
            <svg
              key={star}
              className="w-2.5 h-2.5 sm:w-3 sm:h-3 flex-shrink-0"
              viewBox="0 0 20 20"
              fill={star <= Math.round(product.averageRating || 5) ? '#FF8A3D' : 'none'}
              stroke={star <= Math.round(product.averageRating || 5) ? '#FF8A3D' : '#D3D1C7'}
              strokeWidth="1.5"
            >
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
            </svg>
          ))}
        </div>

        <div className="flex items-baseline justify-between mt-auto pt-1">
          <span className="text-[13px] sm:text-[14.5px] font-bold text-[#16181C]">
            ₹{product.unitPrice.toLocaleString('en-IN')}
          </span>

          {isMaxReached && (
            <span className="text-[10px] sm:text-[11px] font-bold text-[#E0583B]">
              Max {maxAllowedQuantity}
            </span>
          )}
        </div>
      </div>
    </div>
  );
}

export default ProductCard;