import { useState } from 'react';
import ProductSearch from '../products/ProductSearch';

export interface SearchBarProps {
  placeholder?: string;
  className?: string;
}

export function SearchBar({
  placeholder = 'Search products, brands and categories...',
  className = '',
}: SearchBarProps) {
  const [isSearchOpen, setIsSearchOpen] = useState(false);

  return (
    <>
      <div
        onClick={() => setIsSearchOpen(true)}
        className={`relative flex items-center w-full cursor-pointer ${className}`}
      >
        <div className="flex items-center w-full bg-[#f8f9fa] hover:bg-neutral-100 border border-neutral-200/90 text-neutral-800 rounded-xl py-2 px-3.5 transition-colors shadow-2xs">
          <svg
            className="w-4 h-4 text-neutral-400 mr-2.5 flex-shrink-0"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z"
            />
          </svg>
          <span className="text-xs sm:text-sm text-neutral-400 select-none truncate">
            {placeholder}
          </span>
        </div>
      </div>

      <ProductSearch
        isOpen={isSearchOpen}
        onClose={() => setIsSearchOpen(false)}
        placeholder={placeholder}
      />
    </>
  );
}

export default SearchBar;
