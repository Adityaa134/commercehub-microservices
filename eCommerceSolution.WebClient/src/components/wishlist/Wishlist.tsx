import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../../store';
import {
  setWishlistItems,
  removeItemFromWishlist,
} from '../../store/slices/wishlistSlice';
import wishlistService from '../../services/wishlistService';
import WishlistCard from './WishlistCard';

export function Wishlist() {
  const dispatch = useDispatch();
  const items = useSelector((state: RootState) => state.wishlist.items);
  const isItemsLoaded = useSelector((state: RootState) => state.wishlist.isItemsLoaded);
  const [isLoading, setIsLoading] = useState(!isItemsLoaded);

  useEffect(() => {
    if (isItemsLoaded) {
      setIsLoading(false);
      return;
    }

    const fetchWishlist = async () => {
      setIsLoading(true);
      try {
        const data = await wishlistService.getWishlist();
        dispatch(setWishlistItems(data.items || []));
      } catch (err) {
        console.error('Failed to load wishlist:', err);
      } finally {
        setIsLoading(false);
      }
    };

    fetchWishlist();
  }, [isItemsLoaded, dispatch]);

  const handleRemove = async (productId: string) => {
    dispatch(removeItemFromWishlist(productId));
    try {
      await wishlistService.removeFromWishlist(productId);
    } catch (err) {
      console.error('Failed to remove from wishlist:', err);
    }
  };

  if (isLoading) {
    return (
      <div className="w-full max-w-5xl mx-auto px-4 sm:px-6 py-6 sm:py-8">
        <div className="flex items-center gap-2.5 mb-6">
          <div className="h-8 w-36 bg-neutral-200 rounded-lg animate-pulse" />
          <div className="h-6 w-8 bg-neutral-200 rounded-full animate-pulse" />
        </div>

        <div className="grid grid-cols-2 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {Array.from({ length: 8 }).map((_, index) => (
            <div
              key={index}
              className="bg-white border border-[#E9E6DD] rounded-[14px] overflow-hidden flex flex-col animate-pulse"
            >
              <div className="aspect-square bg-neutral-200" />
              <div className="p-3 space-y-2">
                <div className="h-3 bg-neutral-200 rounded-sm w-1/3" />
                <div className="h-4 bg-neutral-200 rounded-sm w-3/4" />
                <div className="h-4 bg-neutral-200 rounded-sm w-1/2" />
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (items.length === 0) {
    return (
      <div className="min-h-[70vh] flex items-center justify-center px-4 py-16 sm:py-24">
        <div className="max-w-md w-full mx-auto text-center flex flex-col items-center">
          <div className="relative mb-6">
            <div className="w-20 h-20 sm:w-24 sm:h-24 rounded-3xl bg-white border border-neutral-200/90 shadow-sm flex items-center justify-center">
              <svg
                className="w-9 h-9 sm:w-10 sm:h-10 text-[#16181C]"
                fill="none"
                stroke="currentColor"
                strokeWidth="1.8"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z"
                />
              </svg>
            </div>
            <span className="absolute -top-1.5 -right-1.5 w-6 h-6 rounded-full bg-neutral-100 text-[#16181C] text-xs font-bold flex items-center justify-center border-2 border-white">
              0
            </span>
          </div>

          <h2 className="text-2xl sm:text-3xl font-bold text-neutral-900 tracking-tight mb-2.5">
            Your wishlist is empty
          </h2>

          <p className="text-sm sm:text-base text-neutral-500 max-w-sm mb-8 leading-relaxed">
            You haven&apos;t saved any products yet. Browse the catalog and add items you like.
          </p>

          <Link
            to="/"
            className="w-full sm:w-auto min-w-[190px] bg-[#3355FF] hover:bg-[#2544DD] text-white font-semibold py-3 px-6 rounded-xl flex items-center justify-center gap-2 text-sm shadow-sm transition-colors"
          >
            <span>Start shopping</span>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
            </svg>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full max-w-5xl mx-auto px-4 sm:px-6 py-6 sm:py-8">
      <div className="flex items-center gap-2.5 mb-6">
        <h1 className="text-2xl sm:text-3xl font-bold text-[#16181C] m-0">
          My wishlist
        </h1>
        <span className="bg-[#16181C] text-white text-xs font-bold px-2 py-0.5 rounded-full">
          {items.length}
        </span>
      </div>

      <div className="grid grid-cols-2 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {items.map((item) => (
          <WishlistCard
            key={item.productId}
            item={item}
            onRemove={handleRemove}
          />
        ))}
      </div>
    </div>
  );
}

export default Wishlist;
