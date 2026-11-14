import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../../store';
import { updateQuantity, removeItem } from '../../store/slices/cartSlice';
import cartService from '../../services/cartService';
import { CartItemResponse, CartItemUpdateRequest } from '../../types/cart.types';
import Button from '../common/Button';
import Spinner from '../common/Spinner';

const GUEST_CART_STORAGE_KEY = 'commercehub_guest_cart';
const MAX_ITEM_LIMIT = 5;

function saveGuestCartItems(items: CartItemResponse[]) {
  try {
    localStorage.setItem(GUEST_CART_STORAGE_KEY, JSON.stringify(items));
  } catch (err) {
    console.error('Failed to save cart to localStorage', err);
  }
}

export function Cart() {
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const { items, totalItems, cartTotal } = useSelector((state: RootState) => state.cart);
  const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);

  const [loadingProductId, setLoadingProductId] = useState<string | null>(null);
  const [isCheckingOut, setIsCheckingOut] = useState(false);

  const handleUpdateQuantity = async (item: CartItemResponse, newQuantity: number) => {
    if (newQuantity <= 0) {
      handleRemoveItem(item.productId);
      return;
    }

    if (newQuantity > MAX_ITEM_LIMIT) {
      return;
    }

    setLoadingProductId(item.productId);
    try {
      if (isAuthenticated) {
        const updateRequest: CartItemUpdateRequest = {
          productId: item.productId,
          quantity: newQuantity,
        };
        const updatedItem: CartItemResponse = await cartService.updateCartItemQuantity(updateRequest);
        dispatch(updateQuantity(updatedItem));
      } else {
        const updatedItem: CartItemResponse = {
          ...item,
          quantity: newQuantity,
          totalPrice: Math.round(newQuantity * item.unitPrice * 100) / 100,
        };
        dispatch(updateQuantity(updatedItem));

        const updatedList = items.map((i) =>
          i.productId === item.productId ? updatedItem : i
        );
        saveGuestCartItems(updatedList);
      }
    } catch (error) {
      console.error('Failed to update quantity:', error);
    } finally {
      setLoadingProductId(null);
    }
  };

  const handleRemoveItem = async (productId: string) => {
    setLoadingProductId(productId);
    try {
      if (isAuthenticated) {
        await cartService.removeItemFromCart(productId);
        dispatch(removeItem(productId));
      } else {
        dispatch(removeItem(productId));
        const filtered = items.filter((i) => i.productId !== productId);
        saveGuestCartItems(filtered);
      }
    } catch (error) {
      console.error('Failed to remove item from cart:', error);
    } finally {
      setLoadingProductId(null);
    }
  };

  const handleProceedToCheckout = () => {
    setIsCheckingOut(true);
    navigate('/checkout');
  };

  if (!items || items.length === 0) {
    return (
      <div className="min-h-[75vh] flex items-center justify-center px-4 py-16 sm:py-24">
        <div className="max-w-md w-full mx-auto text-center flex flex-col items-center">
          <div className="relative mb-6">
            <div className="w-20 h-20 sm:w-24 sm:h-24 rounded-3xl bg-white border border-neutral-200/90 shadow-sm flex items-center justify-center">
              <svg
                className="w-9 h-9 sm:w-10 sm:h-10 text-[#3355FF]"
                fill="none"
                stroke="currentColor"
                strokeWidth="1.8"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M15.75 10.5V6a3.75 3.75 0 10-7.5 0v4.5m11.356-1.993l1.263 12c.07.665-.45 1.243-1.119 1.243H4.25c-.669 0-1.189-.578-1.119-1.243l1.263-12A1.125 1.125 0 015.513 7.5h12.974c.576 0 1.059.435 1.119.993z"
                />
              </svg>
            </div>
            <span className="absolute -top-1.5 -right-1.5 w-6 h-6 rounded-full bg-blue-100 text-[#3355FF] text-xs font-bold flex items-center justify-center border-2 border-white">
              0
            </span>
          </div>

          <h2 className="text-2xl sm:text-3xl font-bold text-neutral-900 tracking-tight mb-2.5">
            Your cart is empty
          </h2>

          <p className="text-sm sm:text-base text-neutral-500 max-w-sm mb-8 leading-relaxed">
            Looks like you haven't added anything yet. Browse the store and add something you like.
          </p>

          <div className="flex flex-col sm:flex-row items-center gap-3 w-full sm:w-auto">
            <Link
              to="/"
              className="w-full sm:w-auto min-w-[190px] bg-[#3355FF] hover:bg-[#2544DD] text-white font-semibold py-3 px-6 rounded-xl flex items-center justify-center gap-2 text-sm shadow-sm transition-colors"
            >
              <span>Start shopping</span>
              <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
              </svg>
            </Link>

            <Link
              to={isAuthenticated ? '/wishlist' : '/login?redirect=/wishlist'}
              className="w-full sm:w-auto min-w-[170px] bg-white border border-neutral-200 hover:bg-neutral-50 text-neutral-800 font-semibold py-3 px-6 rounded-xl flex items-center justify-center gap-2 text-sm transition-colors"
            >
              <svg className="w-4 h-4 text-[#16181C]" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" />
              </svg>
              <span>View wishlist</span>
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 sm:py-12 pb-24 sm:pb-12">
      <div className="flex items-start justify-between mb-6 sm:mb-8">
        <div>
          <h1 className="text-2xl sm:text-3xl font-bold tracking-tight text-neutral-900">
            Your cart
          </h1>
          <p className="text-xs sm:text-sm text-neutral-500 mt-1 font-medium">
            {totalItems} {totalItems === 1 ? 'item' : 'items'}
          </p>
        </div>

        <Link
          to="/"
          className="text-xs sm:text-sm font-semibold text-[#3355FF] hover:underline flex items-center gap-1.5 transition-colors"
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
          </svg>
          <span>Continue shopping</span>
        </Link>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6 lg:gap-8 items-start">
        <div className="lg:col-span-8 space-y-4">
          {items.map((item) => {
            const isProcessing = loadingProductId === item.productId;
            const isMaxReached = item.quantity >= MAX_ITEM_LIMIT;

            return (
              <div
                key={item.productId}
                className="bg-white border border-neutral-200/90 rounded-2xl sm:rounded-3xl p-4 sm:p-5 shadow-xs transition-all hover:border-neutral-300"
              >
                <div className="flex items-start gap-4 sm:gap-5">
                  <div className="w-20 h-20 sm:w-24 sm:h-24 rounded-2xl bg-[#F0EEE7] overflow-hidden flex-shrink-0 flex items-center justify-center">
                    {item.productImagePath ? (
                      <img
                        src={item.productImagePath}
                        alt={item.productName}
                        className="w-full h-full object-cover"
                        onError={(e) => {
                          e.currentTarget.style.display = 'none';
                        }}
                      />
                    ) : (
                      <span className="text-[#3355FF] font-bold text-2xl sm:text-3xl">
                        {item.productName[0]?.toUpperCase() || 'P'}
                      </span>
                    )}
                  </div>

                  <div className="flex-1 min-w-0">
                    <h3 className="text-sm sm:text-base font-bold text-neutral-900 truncate">
                      {item.productName}
                    </h3>

                    <p className="text-xs sm:text-sm text-neutral-500 mt-0.5">
                      ₹{item.unitPrice.toLocaleString('en-IN')} each
                    </p>

                    <div className="flex items-center gap-3 sm:gap-4 mt-3 sm:mt-4">
                      <div className="inline-flex items-center border border-neutral-200 rounded-lg sm:rounded-xl px-2 py-1 bg-white shadow-2xs">
                        <button
                          type="button"
                          disabled={isProcessing}
                          onClick={() => handleUpdateQuantity(item, item.quantity - 1)}
                          className="w-6 h-6 flex items-center justify-center text-neutral-500 hover:text-neutral-900 disabled:opacity-40 cursor-pointer"
                          aria-label="Decrease quantity"
                        >
                          <svg className="w-3 h-3" fill="none" stroke="currentColor" strokeWidth="2.5" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 12h-15" />
                          </svg>
                        </button>

                        <span className="text-xs sm:text-sm font-semibold text-neutral-900 min-w-[20px] text-center px-1">
                          {isProcessing ? <Spinner size="sm" /> : item.quantity}
                        </span>

                        <button
                          type="button"
                          disabled={isProcessing || isMaxReached}
                          onClick={() => handleUpdateQuantity(item, item.quantity + 1)}
                          className="w-6 h-6 flex items-center justify-center text-neutral-500 hover:text-neutral-900 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
                          aria-label="Increase quantity"
                        >
                          <svg className="w-3 h-3" fill="none" stroke="currentColor" strokeWidth="2.5" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
                          </svg>
                        </button>
                      </div>

                      {isMaxReached && (
                        <span className="text-[11px] font-bold text-[#E0583B]">
                          Max {MAX_ITEM_LIMIT}
                        </span>
                      )}

                      <button
                        type="button"
                        disabled={isProcessing}
                        onClick={() => handleRemoveItem(item.productId)}
                        className="inline-flex items-center gap-1.5 text-xs sm:text-sm text-neutral-500 hover:text-red-600 hover:bg-red-50/80 px-2.5 py-1.5 rounded-lg transition-colors cursor-pointer disabled:opacity-40"
                      >
                        <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                          <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0"
                          />
                        </svg>
                        <span>Remove</span>
                      </button>
                    </div>
                  </div>

                  <div className="hidden sm:block text-right flex-shrink-0">
                    <span className="text-base sm:text-lg font-bold text-neutral-900">
                      ₹{item.totalPrice.toLocaleString('en-IN')}
                    </span>
                  </div>
                </div>

                <div className="sm:hidden border-t border-dashed border-neutral-200 mt-3 pt-3 flex items-center justify-between">
                  <span className="text-xs text-neutral-500 font-medium">Item total</span>
                  <span className="text-sm font-bold text-neutral-900">
                    ₹{item.totalPrice.toLocaleString('en-IN')}
                  </span>
                </div>
              </div>
            );
          })}
        </div>

        <div className="lg:col-span-4 sticky top-24">
          <div className="bg-white border border-neutral-200/90 rounded-3xl p-5 sm:p-6 shadow-xs">
            <h2 className="text-lg font-bold text-neutral-900 mb-5">
              Order summary
            </h2>

            <div className="space-y-3.5 text-sm">
              <div className="flex items-center justify-between text-neutral-600">
                <span>Subtotal ({totalItems} items)</span>
                <span className="font-semibold text-neutral-900">
                  ₹{cartTotal.toLocaleString('en-IN')}
                </span>
              </div>

              <div className="flex items-center justify-between text-neutral-600">
                <span className="flex items-center gap-2">
                  <svg className="w-4 h-4 text-emerald-600" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 18.75a1.5 1.5 0 01-3 0m3 0a1.5 1.5 0 00-3 0m3 0h6m-9 0H3.375a1.125 1.125 0 01-1.125-1.125V14.25m17.25 4.5a1.5 1.5 0 01-3 0m3 0a1.5 1.5 0 00-3 0m3 0h1.125c.621 0 1.129-.504 1.09-1.124a17.902 17.902 0 00-3.213-9.193 2.056 2.056 0 00-1.58-.86H14.25M16.5 18.75h-2.25m0-11.177v-.948c0-.621-.504-1.125-1.125-1.125H4.5A1.125 1.125 0 003.375 6.625v7.625M14.25 7.575H17.25l2.25 3.375v3.3h-5.25" />
                  </svg>
                  Delivery charge
                </span>
                <span className="font-semibold text-emerald-600">Free</span>
              </div>
            </div>

            <div className="border-t border-neutral-100 my-4 sm:my-5" />

            <div className="flex items-baseline justify-between mb-6">
              <span className="text-base font-semibold text-neutral-900">Total</span>
              <span className="text-2xl font-extrabold text-neutral-900">
                ₹{cartTotal.toLocaleString('en-IN')}
              </span>
            </div>

            <Button
              variant="none"
              isLoading={isCheckingOut}
              disabled={isCheckingOut}
              onClick={handleProceedToCheckout}
              className="w-full bg-[#3355FF] hover:bg-[#2544DD] text-white font-semibold py-3.5 px-4 rounded-xl sm:rounded-2xl flex items-center justify-center gap-2 text-sm sm:text-base shadow-sm transition-colors cursor-pointer disabled:opacity-60"
            >
              <span>Proceed to checkout</span>
              {!isCheckingOut && (
                <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
                </svg>
              )}
            </Button>

            <div className="flex items-center justify-center gap-1.5 text-xs text-neutral-500 mt-4">
              <svg className="w-3.5 h-3.5 text-neutral-400" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" d="M9 12.75L11.25 15 15 9.75m-3-7.036A11.959 11.959 0 013.598 6 11.99 11.99 0 003 9.749c0 5.592 3.824 10.29 9 11.623 5.176-1.332 9-6.03 9-11.622 0-1.31-.21-2.571-.598-3.751h-.152c-3.196 0-6.1-1.248-8.25-3.285z" />
              </svg>
              <span>Prices include all taxes</span>
            </div>
          </div>
        </div>
      </div>

      <div className="sm:hidden fixed bottom-0 left-0 right-0 bg-white border-t border-neutral-200/90 px-4 py-3 flex items-center justify-between z-30 shadow-[0_-4px_20px_rgba(0,0,0,0.06)]">
        <div>
          <span className="text-[11px] text-neutral-500 font-medium block">Total</span>
          <span className="text-base font-bold text-neutral-900 block">
            ₹{cartTotal.toLocaleString('en-IN')}
          </span>
        </div>

        <Button
          variant="none"
          isLoading={isCheckingOut}
          disabled={isCheckingOut}
          onClick={handleProceedToCheckout}
          className="bg-[#3355FF] hover:bg-[#2544DD] text-white font-semibold py-2.5 px-6 rounded-xl flex items-center gap-2 text-sm shadow-sm transition-colors cursor-pointer disabled:opacity-60"
        >
          <span>Checkout</span>
          {!isCheckingOut && (
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
            </svg>
          )}
        </Button>
      </div>
    </div>
  );
}

export default Cart;