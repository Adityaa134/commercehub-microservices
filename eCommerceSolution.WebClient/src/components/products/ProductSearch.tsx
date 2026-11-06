import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import productService from '../../services/productService';
import { ProductResponse } from '../../types/product.types';

export interface ProductSearchProps {
  isOpen: boolean;
  onClose: () => void;
  placeholder?: string;
}

const STORAGE_KEY = 'recent_searches';

function loadRecentSearches(): string[] {
  try {
    const data = localStorage.getItem(STORAGE_KEY);
    return data ? JSON.parse(data) : [];
  } catch {
    return [];
  }
}

function saveRecentSearches(list: string[]) {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(list));
  } catch {}
}

function HighlightMatch({ text, query }: { text: string; query: string }) {
  const tokens = query
    .trim()
    .split(/\s+/)
    .filter(Boolean)
    .map((t) => t.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'));
  if (!tokens.length) return <>{text}</>;
  return (
    <>
      {text.split(new RegExp(`(${tokens.join('|')})`, 'ig')).map((part, i) =>
        i % 2 === 1 ? (
          <b key={i} className="font-bold text-[#16181C]">
            {part}
          </b>
        ) : (
          <span key={i} className="font-normal text-[#16181C]">
            {part}
          </span>
        )
      )}
    </>
  );
}

function ProductThumbnail({ src, name }: { src?: string; name: string }) {
  const [failed, setFailed] = useState(false);
  return (
    <div className="w-11 h-11 rounded-[10px] bg-[#F0EEE7] overflow-hidden flex-shrink-0 flex items-center justify-center text-[#3355FF] font-bold text-sm">
      {!failed && src ? (
        <img
          src={src}
          alt=""
          onError={() => setFailed(true)}
          className="w-full h-full object-cover"
        />
      ) : (
        <span>{name[0]?.toUpperCase() || 'P'}</span>
      )}
    </div>
  );
}

export function ProductSearch({
  isOpen,
  onClose,
  placeholder = 'Search products, brands and categories…',
}: ProductSearchProps) {
  const navigate = useNavigate();
  const [query, setQuery] = useState('');
  const [searchState, setSearchState] = useState<{
    loading: boolean;
    results: ProductResponse[] | null;
  }>({
    loading: false,
    results: null,
  });
  const [recentSearches, setRecentSearches] = useState<string[]>(loadRecentSearches);
  const [activeIndex, setActiveIndex] = useState(-1);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (!isOpen) return;
    setQuery('');
    setSearchState({ loading: false, results: null });
    setActiveIndex(-1);
    setRecentSearches(loadRecentSearches());

    const prevOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    const timer = setTimeout(() => inputRef.current?.focus(), 30);

    return () => {
      clearTimeout(timer);
      document.body.style.overflow = prevOverflow;
    };
  }, [isOpen]);

  useEffect(() => {
    const trimmed = query.trim();
    if (!trimmed) {
      setSearchState({ loading: false, results: null });
      return;
    }

    setSearchState((prev) => ({ ...prev, loading: true }));
    let isCurrent = true;

    const timer = setTimeout(async () => {
      try {
        const data = await productService.getProducts({
          search: trimmed,
          limit: 6,
        });
        if (isCurrent) {
          setSearchState({ loading: false, results: data.items || [] });
          setActiveIndex(-1);
        }
      } catch {
        if (isCurrent) {
          setSearchState({ loading: false, results: [] });
        }
      }
    }, 280);

    return () => {
      isCurrent = false;
      clearTimeout(timer);
    };
  }, [query]);

  const recordRecentSearch = (term: string) => {
    const trimmed = term.trim();
    if (!trimmed) return;
    const next = [
      trimmed,
      ...recentSearches.filter((r) => r.toLowerCase() !== trimmed.toLowerCase()),
    ].slice(0, 5);
    setRecentSearches(next);
    saveRecentSearches(next);
  };

  const handleSelectProduct = (product: ProductResponse) => {
    recordRecentSearch(product.productName);
    onClose();
    navigate(`/products?search=${encodeURIComponent(product.productName)}`);
  };

  const handleSearchSubmit = (searchTerm: string) => {
    const trimmed = searchTerm.trim();
    if (!trimmed) return;
    recordRecentSearch(trimmed);
    onClose();
    navigate(`/products?search=${encodeURIComponent(trimmed)}`);
  };

  const handleRemoveRecent = (term: string, e: React.MouseEvent) => {
    e.stopPropagation();
    const updated = recentSearches.filter((item) => item !== term);
    setRecentSearches(updated);
    saveRecentSearches(updated);
  };

  const handleClearAllRecent = () => {
    setRecentSearches([]);
    saveRecentSearches([]);
  };

  const currentProducts = searchState.results || [];
  const totalCount = !query.trim() ? recentSearches.length : currentProducts.length;

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'ArrowDown') {
      e.preventDefault();
      setActiveIndex((prev) => (prev + 1) % Math.max(totalCount, 1));
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      setActiveIndex((prev) => (prev <= 0 ? totalCount - 1 : prev - 1));
    } else if (e.key === 'Escape') {
      onClose();
    } else if (e.key === 'Enter') {
      e.preventDefault();
      if (!query.trim()) {
        if (activeIndex >= 0 && recentSearches[activeIndex]) {
          handleSearchSubmit(recentSearches[activeIndex]);
        }
      } else {
        if (activeIndex >= 0 && currentProducts[activeIndex]) {
          handleSelectProduct(currentProducts[activeIndex]);
        } else {
          handleSearchSubmit(query);
        }
      }
    }
  };

  if (!isOpen) return null;

  return (
    <>
      <style>{`
        @keyframes spFade { from { opacity: 0; } to { opacity: 1; } }
        @keyframes spPop { from { opacity: 0; transform: translate(-50%, -8px) scale(.98); } to { opacity: 1; transform: translate(-50%, 0) scale(1); } }
        @keyframes spShimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
        .sp-skel { background: linear-gradient(90deg, #F0EEE7 25%, #F8F7F2 50%, #F0EEE7 75%); background-size: 200% 100%; animation: spShimmer 1.2s infinite; }
      `}</style>
      <div
        onMouseDown={(e) => {
          if (e.target === e.currentTarget) onClose();
        }}
        style={{ animation: 'spFade 160ms ease' }}
        className="fixed inset-0 z-50 bg-[#16181C]/40 backdrop-blur-md"
      >
        <div
          role="dialog"
          aria-label="Search"
          style={{ animation: 'spPop 180ms cubic-bezier(.2,.8,.2,1)' }}
          className="fixed left-1/2 -translate-x-1/2 top-3 sm:top-[84px] w-[calc(100vw-24px)] sm:w-[min(640px,calc(100vw-32px))] max-h-[calc(100vh-24px)] sm:max-h-[min(560px,calc(100vh-120px))] bg-white rounded-[18px] border border-[#E9E6DD] shadow-[0_30px_80px_rgba(20,22,26,0.35)] overflow-hidden flex flex-col"
        >
          <div className="h-[58px] px-4 sm:px-4.5 border-b border-[#E9E6DD] flex items-center gap-2.5 flex-shrink-0">
            <svg
              className="w-[18px] h-[18px] text-[#3355FF] flex-shrink-0"
              fill="none"
              stroke="currentColor"
              strokeWidth="2.2"
              viewBox="0 0 24 24"
            >
              <circle cx="11" cy="11" r="8" />
              <line x1="21" y1="21" x2="16.65" y2="16.65" strokeLinecap="round" />
            </svg>

            <input
              ref={inputRef}
              type="text"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              onKeyDown={handleKeyDown}
              enterKeyHint="search"
              placeholder={placeholder}
              className="flex-1 min-w-0 bg-transparent border-none outline-none text-base text-[#16181C] placeholder-[#83887B]"
            />

            {query && (
              <button
                type="button"
                onClick={() => {
                  setQuery('');
                  inputRef.current?.focus();
                }}
                aria-label="Clear input"
                className="w-6 h-6 rounded-full bg-[#F0EEE7] hover:bg-[#E4E1D8] text-[#5F5E5A] flex items-center justify-center flex-shrink-0 transition-colors border-none cursor-pointer"
              >
                <svg className="w-3 h-3" fill="none" stroke="currentColor" strokeWidth="2.5" viewBox="0 0 24 24">
                  <line x1="18" y1="6" x2="6" y2="18" strokeLinecap="round" />
                  <line x1="6" y1="6" x2="18" y2="18" strokeLinecap="round" />
                </svg>
              </button>
            )}
          </div>

          <div className="overflow-y-auto flex-1 py-1 pb-3">
            {!query.trim() ? (
              recentSearches.length > 0 ? (
                <div>
                  <div className="flex items-center justify-between pr-[18px]">
                    <div className="px-[18px] pt-3 pb-1.5 text-[11px] font-bold text-[#83887B] tracking-[0.08em] uppercase select-none">
                      Recent searches
                    </div>
                    <button
                      type="button"
                      onClick={handleClearAllRecent}
                      className="text-xs font-semibold text-[#3355FF] hover:underline cursor-pointer border-none bg-transparent pt-1"
                    >
                      Clear
                    </button>
                  </div>

                  {recentSearches.map((term, index) => (
                    <div
                      key={term}
                      className={`flex items-center w-full transition-colors ${
                        activeIndex === index ? 'bg-[#F4F6FF]' : 'hover:bg-[#F4F6FF]'
                      }`}
                      onMouseEnter={() => setActiveIndex(index)}
                    >
                      <button
                        type="button"
                        onClick={() => handleSearchSubmit(term)}
                        className="flex-1 flex items-center gap-3 pl-[18px] py-[9px] min-w-0 text-left bg-transparent border-none cursor-pointer"
                      >
                        <svg
                          className="w-4 h-4 text-[#B4B2A9] flex-shrink-0"
                          fill="none"
                          stroke="currentColor"
                          strokeWidth="1.8"
                          viewBox="0 0 24 24"
                        >
                          <circle cx="12" cy="12" r="10" />
                          <polyline points="12 6 12 12 16 14" strokeLinecap="round" strokeLinejoin="round" />
                        </svg>
                        <span className="text-sm text-[#16181C] truncate">{term}</span>
                      </button>

                      <button
                        type="button"
                        aria-label={`Remove ${term}`}
                        onClick={(e) => handleRemoveRecent(term, e)}
                        className="px-4 h-[40px] flex items-center justify-center text-[#B4B2A9] hover:text-[#16181C] transition-colors bg-transparent border-none cursor-pointer flex-shrink-0"
                      >
                        <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                          <line x1="18" y1="6" x2="6" y2="18" strokeLinecap="round" />
                          <line x1="6" y1="6" x2="18" y2="18" strokeLinecap="round" />
                        </svg>
                      </button>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="py-8 px-5 text-center text-[13.5px] text-[#83887B]">
                  Search products by name, brand or category…
                </div>
              )
            ) : searchState.loading && !searchState.results ? (
              <div className="px-[18px] py-2.5 space-y-2">
                {[0, 1, 2, 3].map((i) => (
                  <div key={i} className="flex items-center gap-3 py-1.5">
                    <div className="w-11 h-11 rounded-[10px] sp-skel flex-shrink-0" />
                    <div className="flex-1 space-y-2">
                      <div
                        className="h-3 rounded-md sp-skel"
                        style={{ width: `${70 - i * 8}%` }}
                      />
                      <div className="h-2.5 w-[30%] rounded-md sp-skel" />
                    </div>
                  </div>
                ))}
              </div>
            ) : !searchState.loading && searchState.results && searchState.results.length === 0 ? (
              <div className="py-8 px-5 text-center">
                <p className="text-[15px] font-semibold text-[#16181C]">
                  No results for “{query}”
                </p>
                <p className="text-[13px] text-[#83887B] mt-1">
                  Check the spelling or try a broader word like “laptop” or “shoes”.
                </p>
              </div>
            ) : (
              <div className={searchState.loading ? 'opacity-60 transition-opacity duration-150' : ''}>
                <div className="px-[18px] pt-3 pb-1.5 text-[11px] font-bold text-[#83887B] tracking-[0.08em] uppercase select-none">
                  Products
                </div>

                {currentProducts.map((product, index) => (
                  <button
                    key={product.productId}
                    type="button"
                    onMouseEnter={() => setActiveIndex(index)}
                    onClick={() => handleSelectProduct(product)}
                    className={`flex items-center gap-3 w-full px-[18px] py-[9px] text-left transition-colors border-none cursor-pointer ${
                      activeIndex === index ? 'bg-[#F4F6FF]' : 'hover:bg-[#F4F6FF]'
                    }`}
                  >
                    <ProductThumbnail
                      src={product.productImagePath}
                      name={product.productName}
                    />

                    <span className="flex-1 min-w-0">
                      <span className="block truncate text-sm text-[#16181C]">
                        <HighlightMatch text={product.productName} query={query} />
                      </span>
                      <span className="flex items-center gap-2 text-xs text-[#83887B] mt-0.5">
                        <span>{product.categoryName || 'Product'}</span>
                        <span className="inline-flex items-center gap-1">
                          <svg className="w-2.5 h-2.5 text-[#FF8A3D] fill-[#FF8A3D] flex-shrink-0" viewBox="0 0 20 20">
                            <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                          </svg>
                          <span>{product.averageRating ? product.averageRating.toFixed(1) : '4.5'}</span>
                        </span>
                      </span>
                    </span>

                    <span className="text-sm font-bold text-[#16181C] flex-shrink-0 pl-3">
                      {'₹' + product.unitPrice.toLocaleString('en-IN')}
                    </span>
                  </button>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

export default ProductSearch;

