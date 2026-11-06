import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import productService from '../../services/productService';
import { PagedResult, ProductResponse, SORT_OPTIONS } from '../../types/product.types';
import { ProductCard } from './ProductCard';
import { Pagination } from '../common/Pagination';

export function ProductGrid() {
  const [searchParams, setSearchParams] = useSearchParams();
  const page = Number(searchParams.get('page')) || 1;
  const sortBy = searchParams.get('sort') || 'featured';
  const search = searchParams.get('search') || undefined;
  const categoryId = searchParams.get('category') || searchParams.get('categoryId') || undefined;

  const [data, setData] = useState<PagedResult<ProductResponse> | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSortOpen, setIsSortOpen] = useState(false);
  const [addedNotice, setAddedNotice] = useState<string | null>(null);

  const currentSortLabel =
    SORT_OPTIONS.find((s) => s.id === sortBy)?.label || 'Featured';

  useEffect(() => {
    let isCurrent = true;

    async function loadProducts() {
      setIsLoading(true);
      try {
        const result = await productService.getProducts({
          page,
          limit: 12,
          sortBy,
          search,
          categoryId,
        });
        if (isCurrent) {
          setData(result);
        }
      } catch {
        if (isCurrent) {
          setData(null);
        }
      } finally {
        if (isCurrent) {
          setIsLoading(false);
        }
      }
    }

    loadProducts();

    return () => {
      isCurrent = false;
    };
  }, [page, sortBy, search, categoryId]);

  const handlePageChange = (newPage: number) => {
    const next = new URLSearchParams(searchParams);
    next.set('page', String(newPage));
    setSearchParams(next);
    document.getElementById('products-section')?.scrollIntoView({ behavior: 'smooth' });
  };

  const handleSortChange = (newSort: string) => {
    const next = new URLSearchParams(searchParams);
    next.set('sort', newSort);
    next.set('page', '1');
    setSearchParams(next);
    setIsSortOpen(false);
  };

  const handleAddToCart = (product: ProductResponse) => {
    setAddedNotice(`Added "${product.productName}" to cart!`);
    window.setTimeout(() => setAddedNotice(null), 2500);
  };

  const products = data?.items || [];
  const totalCount = data?.totalCount || 0;
  const currentPage = data?.page || 1;
  const totalPages = data?.totalPages || 1;
  const pageSize = data?.limit || 12;

  return (
    <div className="w-full">
      {addedNotice && (
        <div className="fixed top-20 right-6 z-50 bg-[#16181C] text-white px-4 py-2.5 rounded-xl shadow-xl text-xs font-semibold flex items-center gap-2 animate-in fade-in slide-in-from-top-2 duration-200">
          <svg className="w-4 h-4 text-emerald-400 flex-shrink-0" fill="none" stroke="currentColor" strokeWidth="2.5" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.75l6 6 9-13.5" />
          </svg>
          <span>{addedNotice}</span>
        </div>
      )}

      <div className="flex items-center justify-between mb-6 flex-wrap gap-3">
        <div>
          <h2 className="text-xl font-semibold text-[#16181C] m-0">
            All gear
          </h2>
          <p className="text-[13px] text-[#83887B] mt-1 m-0">
            {totalCount} products
          </p>
        </div>

        <div className="relative">
          <button
            type="button"
            onClick={() => setIsSortOpen((prev) => !prev)}
            className="flex items-center gap-1.5 px-3.5 py-2.5 bg-white border border-[#E4E1D8] rounded-[9px] text-[13px] font-semibold text-[#16181C] cursor-pointer hover:border-neutral-400 transition-colors"
          >
            <span>Sort: {currentSortLabel}</span>
            <svg
              className={`w-3.5 h-3.5 text-[#83887B] transition-transform duration-150 ${
                isSortOpen ? 'rotate-180' : ''
              }`}
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              viewBox="0 0 24 24"
            >
              <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
            </svg>
          </button>

          {isSortOpen && (
            <div className="absolute right-0 top-11 bg-white border border-[#E9E6DD] rounded-[10px] shadow-[0_12px_28px_rgba(20,22,26,0.12)] overflow-hidden z-20 w-[180px]">
              {SORT_OPTIONS.map((option) => (
                <button
                  key={option.id}
                  type="button"
                  onClick={() => handleSortChange(option.id)}
                  className={`block w-full text-left px-3.5 py-2.5 border-none text-[13px] cursor-pointer transition-colors ${
                    sortBy === option.id
                      ? 'bg-[#F0EEE7] font-semibold text-[#16181C]'
                      : 'bg-white text-[#16181C] hover:bg-[#F4F6FF]'
                  }`}
                >
                  {option.label}
                </button>
              ))}
            </div>
          )}
        </div>
      </div>

      {isLoading ? (
        <div className="grid grid-cols-2 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-3 sm:gap-[18px]">
          {Array.from({ length: pageSize }).map((_, i) => (
            <div
              key={i}
              className="bg-white border border-[#E9E6DD] rounded-[14px] overflow-hidden flex flex-col"
            >
              <div className="aspect-square bg-gradient-to-r from-[#F0EEE7] via-[#F8F7F2] to-[#F0EEE7] bg-[length:200%_100%] animate-pulse" />
              <div className="p-2.5 sm:p-3.5 space-y-2 sm:space-y-2.5">
                <div className="h-2.5 w-1/3 bg-neutral-200/70 rounded-sm animate-pulse" />
                <div className="h-3.5 w-4/5 bg-neutral-200/70 rounded-sm animate-pulse" />
                <div className="h-3 w-1/2 bg-neutral-200/70 rounded-sm animate-pulse" />
                <div className="h-4 w-1/4 bg-neutral-200/70 rounded-sm animate-pulse mt-2" />
              </div>
            </div>
          ))}
        </div>
      ) : products.length === 0 ? (
        <div className="text-center py-20 px-5 text-[#83887B]">
          <p className="text-[17px] font-semibold text-[#16181C] mb-1.5">
            No products match your criteria
          </p>
          <p className="text-[13.5px] m-0">Try adjusting your search or filters.</p>
        </div>
      ) : (
        <>
          <div className="grid grid-cols-2 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-3 sm:gap-[18px]">
            {products.map((product) => (
              <ProductCard
                key={product.productId}
                product={product}
                onAddToCart={handleAddToCart}
              />
            ))}
          </div>

          <Pagination
            page={currentPage}
            totalPages={totalPages}
            totalItems={totalCount}
            pageSize={pageSize}
            onChange={handlePageChange}
          />
        </>
      )}
    </div>
  );
}

export default ProductGrid;
