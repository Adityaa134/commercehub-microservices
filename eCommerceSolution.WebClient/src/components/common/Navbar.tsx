import { useState, useRef, useEffect } from 'react';
import { Link } from 'react-router-dom';
import SearchBar from './SearchBar';
import Logo from './Logo';
import { Button } from './Button';
import categoryService from '../../services/categoryService';
import { CategoryResponse } from '../../types/category.types';
import { CategoryDropdown, CategoryIcon } from '../categories/CategoryDropdown';

export interface UserNavProfile {
  name: string;
  email: string;
  avatarUrl?: string;
}

export interface NavbarProps {
  user?: UserNavProfile | null;
  onLogout?: () => void;
}

export function Navbar({
  user,
  onLogout,
}: NavbarProps) {
  const [categories, setCategories] = useState<CategoryResponse[]>([]);
  const [isShopOpen, setIsShopOpen] = useState(false);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [expandedMobileCategory, setExpandedMobileCategory] = useState<string | null>(null);

  const shopMenuRef = useRef<HTMLDivElement>(null);
  const userMenuRef = useRef<HTMLDivElement>(null);
  const hoverTimeoutRef = useRef<number | null>(null);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const data = await categoryService.getCategories();
        setCategories(data);
      } catch (error) {
        console.error('Failed to load categories', error);
      }
    };

    fetchCategories();
  }, []);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (shopMenuRef.current && !shopMenuRef.current.contains(event.target as Node)) {
        setIsShopOpen(false);
      }
      if (userMenuRef.current && !userMenuRef.current.contains(event.target as Node)) {
        setIsUserMenuOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
      if (hoverTimeoutRef.current) {
        clearTimeout(hoverTimeoutRef.current);
      }
    };
  }, []);

  const handleMouseEnter = () => {
    if (hoverTimeoutRef.current) {
      clearTimeout(hoverTimeoutRef.current);
    }
    setIsShopOpen(true);
  };

  const handleMouseLeave = () => {
    hoverTimeoutRef.current = window.setTimeout(() => {
      setIsShopOpen(false);
    }, 150);
  };

  const toggleMobileCategory = (id: string) => {
    setExpandedMobileCategory((prev) => (prev === id ? null : id));
  };

  const getInitials = (name?: string) => {
    if (!name) return 'U';
    const parts = name.trim().split(/\s+/);
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase();
    }
    return parts[0].slice(0, 2).toUpperCase();
  };

  return (
    <header className="relative bg-[#f8f9fa] border-b border-neutral-200/80 sticky top-0 z-40">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16 gap-4">
          <div className="flex items-center gap-3 sm:gap-6 lg:gap-8 flex-shrink-0">
            <Button
              variant="none"
              onClick={() => setIsMobileMenuOpen(true)}
              aria-label="Open Mobile Menu"
              className="md:hidden p-2 -ml-2 rounded-lg hover:bg-neutral-100 text-neutral-800 transition-colors"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
              </svg>
            </Button>

            <Link
              to="/"
              className="cursor-pointer text-left"
            >
              <Logo />
            </Link>

            <nav className="hidden md:flex items-center gap-6 lg:gap-7 text-sm font-medium text-neutral-700">
              <div
                className="relative"
                ref={shopMenuRef}
                onMouseEnter={handleMouseEnter}
                onMouseLeave={handleMouseLeave}
              >
                <Button
                  variant="none"
                  onClick={() => setIsShopOpen(!isShopOpen)}
                  className={`flex items-center gap-1.5 py-1 text-sm font-semibold transition-colors ${
                    isShopOpen ? 'text-blue-600' : 'hover:text-blue-600'
                  }`}
                >
                  <span>Shop</span>
                  <svg
                    className={`w-3.5 h-3.5 transition-transform duration-200 ${
                    isShopOpen ? 'rotate-180 text-blue-600' : 'text-neutral-400'
                  }`}
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2.5"
                    viewBox="0 0 24 24"
                  >
                    <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
                  </svg>
                </Button>

                {isShopOpen && (
                  <CategoryDropdown
                    categories={categories}
                    onClose={() => setIsShopOpen(false)}
                  />
                )}
              </div>

              <Link to="/trending" className="hover:text-blue-600 transition-colors">
                Trending
              </Link>
            </nav>
          </div>

          <div className="hidden sm:block flex-1 max-w-md lg:max-w-lg xl:max-w-xl mx-4 lg:mx-8">
            <SearchBar />
          </div>

          <div className="flex items-center gap-2 sm:gap-3 text-neutral-700 flex-shrink-0">
              <div className="relative" ref={userMenuRef}>
                <Button
                  variant="none"
                  onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
                  aria-label="User Account"
                  aria-expanded={isUserMenuOpen}
                  className="w-9 h-9 flex items-center justify-center rounded-lg hover:bg-neutral-100 hover:text-neutral-900 transition-colors"
                >
                  {user ? (
                    <div className="w-7 h-7 rounded-full bg-blue-100 text-blue-600 font-bold text-xs flex items-center justify-center">
                      {getInitials(user.name)}
                    </div>
                  ) : (
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" />
                    </svg>
                  )}
                </Button>

                {isUserMenuOpen && (
                  <div className="absolute right-0 top-full mt-2 w-72 sm:w-80 bg-white rounded-3xl border border-neutral-100 shadow-[0_12px_40px_rgba(0,0,0,0.08)] z-50 overflow-hidden animate-in fade-in zoom-in-95 duration-150">
                    {!user ? (
                      <div className="p-6 text-left">
                        <h3 className="font-bold text-neutral-900 text-base sm:text-lg tracking-tight">
                          Welcome to CommerceHub
                        </h3>
                        <p className="text-neutral-500 text-xs sm:text-sm mt-1.5 leading-relaxed">
                          Sign in for faster checkout, order tracking and your wishlist.
                        </p>

                        <Link
                          to="/login"
                          onClick={() => setIsUserMenuOpen(false)}
                          className="w-full bg-[#16181f] hover:bg-black text-white font-semibold py-2.5 rounded-xl text-sm mt-5 transition-colors cursor-pointer text-center block shadow-xs"
                        >
                          Sign in
                        </Link>

                        <p className="text-xs text-neutral-500 mt-4 text-center">
                          New here?{' '}
                          <Link
                            to="/register"
                            onClick={() => setIsUserMenuOpen(false)}
                            className="text-blue-600 font-semibold hover:text-blue-500 cursor-pointer"
                          >
                            Create an account
                          </Link>
                        </p>
                      </div>
                    ) : (
                      <div className="text-left">
                        <div className="flex items-center gap-3 p-4 sm:p-5">
                          <div className="w-10 h-10 rounded-full bg-blue-100 text-blue-600 font-bold text-sm flex items-center justify-center flex-shrink-0">
                            {getInitials(user.name)}
                          </div>
                          <div className="min-w-0 flex-1">
                            <p className="font-semibold text-neutral-900 text-sm truncate">
                              {user.name}
                            </p>
                            <p className="text-xs text-neutral-500 truncate mt-0.5">
                              {user.email}
                            </p>
                          </div>
                        </div>

                        <div className="border-t border-neutral-100" />

                        <div className="py-2">
                          <a
                            href="#orders"
                            onClick={() => setIsUserMenuOpen(false)}
                            className="flex items-center gap-3 px-5 py-2.5 text-sm text-neutral-700 hover:bg-neutral-50 transition-colors"
                          >
                            <svg className="w-4 h-4 text-neutral-500 flex-shrink-0" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" d="M21 7.5l-9-5.25L3 7.5m18 0l-9 5.25m9-5.25v9l-9 5.25M3 7.5l9 5.25M3 7.5v9l9 5.25m0-9v9" />
                            </svg>
                            <span>My orders</span>
                          </a>

                          <a
                            href="#wishlist"
                            onClick={() => setIsUserMenuOpen(false)}
                            className="flex items-center gap-3 px-5 py-2.5 text-sm text-neutral-700 hover:bg-neutral-50 transition-colors"
                          >
                            <svg className="w-4 h-4 text-neutral-500 flex-shrink-0" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" />
                            </svg>
                            <span>Wishlist</span>
                          </a>

                          <a
                            href="#settings"
                            onClick={() => setIsUserMenuOpen(false)}
                            className="flex items-center gap-3 px-5 py-2.5 text-sm text-neutral-700 hover:bg-neutral-50 transition-colors"
                          >
                            <svg className="w-4 h-4 text-neutral-500 flex-shrink-0" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.964 0a9 9 0 10-11.963 0m11.964 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
                            </svg>
                            <span>Account settings</span>
                          </a>
                        </div>

                        <div className="border-t border-neutral-100" />

                        <div className="p-2 sm:p-3">
                          <Button
                            variant="none"
                            onClick={() => {
                              setIsUserMenuOpen(false);
                              onLogout?.();
                            }}
                            className="w-full flex items-center gap-3 px-3 py-2 text-sm text-[#e0583b] hover:bg-red-50/60 rounded-xl transition-colors text-left font-medium"
                          >
                            <svg className="w-4 h-4 text-[#e0583b] flex-shrink-0" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15M12 9l-3 3m0 0l3 3m-3-3h12.75" />
                            </svg>
                            <span>Log out</span>
                          </Button>
                        </div>
                      </div>
                    )}
                  </div>
                )}
              </div>

              <a
                href="#wishlist"
                aria-label="Wishlist"
                className="w-9 h-9 flex items-center justify-center rounded-lg hover:bg-neutral-100 hover:text-neutral-900 transition-colors"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" />
                </svg>
              </a>

              <a
                href="#cart"
                aria-label="Shopping Cart"
                className="w-9 h-9 flex items-center justify-center rounded-lg hover:bg-neutral-100 hover:text-neutral-900 transition-colors"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437M7.5 14.25a3 3 0 00-3 3h15.75m-12.75-3h11.218c1.121-2.3 2.1-4.684 2.924-7.138a60.114 60.114 0 00-16.536-1.84M7.5 14.25L5.106 5.272M6 20.25a.75.75 0 11-1.5 0 .75.75 0 011.5 0zm12.75 0a.75.75 0 11-1.5 0 .75.75 0 011.5 0z" />
                </svg>
              </a>
            </div>
        </div>

        <div className="sm:hidden pb-3">
          <SearchBar />
        </div>
      </div>

      {isMobileMenuOpen && (
        <div className="fixed inset-0 z-50 flex justify-start md:hidden">
          <div
            className="fixed inset-0 bg-black/40 backdrop-blur-sm transition-opacity"
            onClick={() => setIsMobileMenuOpen(false)}
          />

          <div className="relative w-full max-w-[280px] sm:max-w-xs bg-white h-[100dvh] shadow-2xl flex flex-col z-10 animate-in slide-in-from-left duration-200">
            <div className="flex items-center justify-between px-5 py-4 border-b border-neutral-100 flex-shrink-0">
              <h3 className="text-base font-bold text-neutral-900 tracking-tight">
                Shop by category
              </h3>
              <Button
                variant="none"
                onClick={() => setIsMobileMenuOpen(false)}
                aria-label="Close menu"
                className="p-1 rounded-lg text-neutral-600 hover:text-neutral-900 hover:bg-neutral-100"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </Button>
            </div>

            {user ? (
              <div className="p-4 border-b border-neutral-100 bg-neutral-50/70">
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 rounded-full bg-blue-100 text-blue-600 font-bold text-sm flex items-center justify-center flex-shrink-0">
                    {getInitials(user.name)}
                  </div>
                  <div className="min-w-0 flex-1">
                    <p className="font-semibold text-neutral-900 text-sm truncate">{user.name}</p>
                    <p className="text-xs text-neutral-500 truncate mt-0.5">{user.email}</p>
                  </div>
                </div>
              </div>
            ) : null}

            <div className="flex-1 overflow-y-auto px-5 py-2">
              <div className="divide-y divide-neutral-100">
                {categories.map((category) => {
                  const isExpanded = expandedMobileCategory === category.categoryId;
                  return (
                    <div key={category.categoryId} className="py-3">
                      <Button
                        variant="none"
                        onClick={() => toggleMobileCategory(category.categoryId)}
                        className="w-full flex items-center justify-between text-left py-3 text-[#16181C] hover:text-[#3355FF]"
                      >
                        <div className="flex items-center gap-2.5">
                          <div className="w-7 h-7 rounded-[7px] bg-[#EAEEFF] text-[#3355FF] flex items-center justify-center flex-shrink-0">
                            <CategoryIcon category={category} />
                          </div>
                          <span className="text-[13.5px] font-semibold text-[#16181C]">
                            {category.categoryName}
                          </span>
                        </div>
                        <svg
                          className={`w-3.5 h-3.5 text-[#83887B] transition-transform duration-150 ${
                            isExpanded ? 'rotate-180' : ''
                          }`}
                          fill="none"
                          stroke="currentColor"
                          strokeWidth="2"
                          viewBox="0 0 24 24"
                        >
                          <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
                        </svg>
                      </Button>

                      {isExpanded && (
                        <div className="pb-3 flex flex-col gap-2 pl-9">
                          {category.subCategories && category.subCategories.length > 0 ? (
                            category.subCategories.map((sub) => (
                              <Link
                                key={sub.categoryId}
                                to={`/?categoryId=${sub.categoryId}`}
                                onClick={() => setIsMobileMenuOpen(false)}
                                className="text-[12.5px] text-[#5F5E5A] hover:text-[#3355FF] transition-colors block"
                              >
                                {sub.categoryName}
                              </Link>
                            ))
                          ) : null}
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>

              <div className="pt-5 pb-8 space-y-4 border-t border-neutral-100">
                <Link
                  to="/trending"
                  onClick={() => setIsMobileMenuOpen(false)}
                  className="block text-sm font-semibold text-neutral-900 hover:text-blue-600 transition-colors"
                >
                  Trending
                </Link>
              </div>
            </div>

            {!user ? (
              <div className="p-4 border-t border-neutral-100 bg-neutral-50/50 space-y-2 mt-auto">
                <Link
                  to="/login"
                  onClick={() => setIsMobileMenuOpen(false)}
                  className="w-full bg-[#16181f] hover:bg-black text-white font-semibold py-2.5 rounded-xl text-sm transition-colors text-center block cursor-pointer"
                >
                  Sign in
                </Link>
                <Link
                  to="/register"
                  onClick={() => setIsMobileMenuOpen(false)}
                  className="w-full border border-neutral-300 hover:bg-neutral-100 text-neutral-800 font-semibold py-2.5 rounded-xl text-sm transition-colors text-center block cursor-pointer"
                >
                  Create account
                </Link>
              </div>
            ) : (
              <div className="p-4 border-t border-neutral-100 mt-auto">
                <Button
                  variant="none"
                  onClick={() => {
                    setIsMobileMenuOpen(false);
                    onLogout?.();
                  }}
                  className="w-full flex items-center justify-center gap-2 text-sm text-[#e0583b] font-semibold py-2.5 rounded-xl border border-red-200 hover:bg-red-50 transition-colors"
                >
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15M12 9l-3 3m0 0l3 3m-3-3h12.75" />
                  </svg>
                  <span>Log out</span>
                </Button>
              </div>
            )}
          </div>
        </div>
      )}
    </header>
  );
}

export default Navbar;
