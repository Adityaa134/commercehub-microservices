import { useState } from 'react';
import { ProductResponse } from '../../types/product.types';

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
  isWishlisted = false,
}: ProductCardProps) {
  const [hover, setHover] = useState(false);
  const [wished, setWished] = useState(isWishlisted);
  const [imageError, setImageError] = useState(false);

  const isProductNew = (() => {
    if (!product.createdAt) return false;
    const createdTime = new Date(product.createdAt).getTime();
    if (isNaN(createdTime)) return false;
    const thirtyDaysMs = 30 * 24 * 60 * 60 * 1000;
    return Date.now() - createdTime <= thirtyDaysMs;
  })();

  const handleWishlistClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    const nextState = !wished;
    setWished(nextState);
    onToggleWishlist?.(product.productId);
  };

  const handleAddToCartClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    onAddToCart?.(product);
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

        <button
          type="button"
          onClick={handleWishlistClick}
          aria-label={wished ? 'Remove from wishlist' : 'Save to wishlist'}
          className="absolute top-1.5 right-1.5 sm:top-2.5 sm:right-2.5 w-6 h-6 sm:w-[30px] sm:h-[30px] rounded-full border-none bg-white/92 backdrop-blur-xs flex items-center justify-center cursor-pointer shadow-2xs hover:scale-105 transition-transform z-10"
        >
          <svg
            className="w-3 h-3 sm:w-3.5 sm:h-3.5"
            viewBox="0 0 24 24"
            fill={wished ? '#FF8A3D' : 'none'}
            stroke={wished ? '#FF8A3D' : '#16181C'}
            strokeWidth="2"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z"
            />
          </svg>
        </button>

        <button
          type="button"
          onClick={handleAddToCartClick}
          className={`absolute left-1.5 right-1.5 sm:left-2.5 sm:right-2.5 h-8 sm:h-9 rounded-[7px] sm:rounded-[9px] border-none bg-[#3355FF] hover:bg-[#2544DD] text-white text-[11.5px] sm:text-[13px] font-semibold cursor-pointer transition-all duration-200 z-10 shadow-sm flex items-center justify-center ${
            hover ? 'bottom-1.5 sm:bottom-2.5 opacity-100' : '-bottom-11 opacity-0'
          }`}
        >
          Add to cart
        </button>
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

        <div className="flex items-baseline gap-2 mt-auto pt-1">
          <span className="text-[13px] sm:text-[14.5px] font-bold text-[#16181C]">
            ₹{product.unitPrice.toLocaleString('en-IN')}
          </span>
        </div>
      </div>
    </div>
  );
}

export default ProductCard;
