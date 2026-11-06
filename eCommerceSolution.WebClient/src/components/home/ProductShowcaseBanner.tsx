import { useState, useEffect, useRef } from 'react';
import { Button } from '../common/Button';
import productService from '../../services/productService';
import { ProductResponse } from '../../types/product.types';

export interface ProductShowcaseBannerProps {
  onShopNewArrivals?: () => void;
  onExploreAll?: () => void;
}

export function ProductShowcaseBanner({
  onShopNewArrivals,
  onExploreAll,
}: ProductShowcaseBannerProps) {
  const [products, setProducts] = useState<ProductResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [currentSlide, setCurrentSlide] = useState(0);
  const [isPaused, setIsPaused] = useState(false);
  const [imgScale, setImgScale] = useState<number>(1.2);
  const timerRef = useRef<number | null>(null);

  const handleImageLoad = (e: React.SyntheticEvent<HTMLImageElement>) => {
    const { naturalWidth, naturalHeight } = e.currentTarget;
    if (!naturalWidth || !naturalHeight) return;
    const ratio = naturalWidth / naturalHeight;

    if (ratio < 0.85) {
      setImgScale(1.04);
    } else if (ratio <= 1.3) {
      setImgScale(1.32);
    } else {
      setImgScale(1.15);
    }
  };

  useEffect(() => {
    let isMounted = true;
    const fetchTrendingProducts = async () => {
      try {
        const data = await productService.getProducts({
          sortBy: 'trending',
          limit: 4,
        });
        if (isMounted && data.items && data.items.length > 0) {
          setProducts(data.items);
        }
      } catch (error) {
        console.error('Failed to load banner products', error);
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    };

    fetchTrendingProducts();

    return () => {
      isMounted = false;
    };
  }, []);

  useEffect(() => {
    if (isPaused || products.length <= 1) {
      if (timerRef.current) {
        clearInterval(timerRef.current);
      }
      return;
    }

    timerRef.current = window.setInterval(() => {
      setCurrentSlide((prev) => (prev + 1) % products.length);
    }, 4500);

    return () => {
      if (timerRef.current) {
        clearInterval(timerRef.current);
      }
    };
  }, [isPaused, products.length]);

  const handlePrev = () => {
    if (products.length === 0) return;
    setCurrentSlide((prev) => (prev === 0 ? products.length - 1 : prev - 1));
  };

  const handleNext = () => {
    if (products.length === 0) return;
    setCurrentSlide((prev) => (prev + 1) % products.length);
  };

  const activeProduct = products[currentSlide];

  return (
    <section className="w-full pb-10 sm:pb-14">
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-12 items-center">
        <div className="lg:col-span-6 xl:col-span-5 flex flex-col justify-center">
          <span className="text-xs font-bold text-blue-600 tracking-wider uppercase">
            Curated Collection · 2026
          </span>

          <h1 className="text-3xl sm:text-5xl lg:text-6xl font-extrabold text-neutral-900 tracking-tight leading-[1.08] mt-3 sm:mt-4">
            Quality gear, tuned to how you live.
          </h1>

          <p className="mt-4 sm:mt-5 text-sm sm:text-base text-neutral-600 leading-relaxed max-w-lg">
            Everyday essentials across electronics, lifestyle, and home — handpicked for durability, priced fairly, and delivered fast.
          </p>

          <div className="mt-7 sm:mt-8 flex flex-col sm:flex-row items-stretch sm:items-center gap-3">
            <Button
              variant="dark"
              size="md"
              onClick={onShopNewArrivals}
              className="px-6 py-3 rounded-xl text-sm font-semibold shadow-xs hover:bg-neutral-800 transition-colors justify-center"
            >
              Shop new arrivals
            </Button>
            <Button
              variant="outline"
              size="md"
              onClick={onExploreAll}
              className="px-6 py-3 rounded-xl text-sm font-semibold border-neutral-300 bg-white hover:bg-neutral-100 hover:text-neutral-900 text-neutral-800 transition-colors shadow-2xs justify-center"
            >
              Explore all
            </Button>
          </div>
        </div>

        <div className="lg:col-span-6 xl:col-span-7 flex flex-col items-center">
          {isLoading ? (
            <div className="w-full h-[320px] sm:h-[380px] lg:h-[420px] rounded-3xl bg-neutral-200/70 animate-pulse" />
          ) : products.length > 0 && activeProduct ? (
            <>
              <div
                onMouseEnter={() => setIsPaused(true)}
                onMouseLeave={() => setIsPaused(false)}
                className="relative w-full h-[320px] sm:h-[380px] lg:h-[420px] rounded-3xl overflow-hidden bg-white border border-neutral-200/70 select-none shadow-sm group flex items-center justify-center"
              >
                <img
                  key={activeProduct.productId}
                  src={activeProduct.productImagePath}
                  alt={activeProduct.productName}
                  onLoad={handleImageLoad}
                  style={{ transform: `scale(${imgScale})` }}
                  className="w-full h-full object-contain transition-transform duration-500 ease-out animate-in fade-in"
                />

                {products.length > 1 && (
                  <>
                    <Button
                      variant="none"
                      onClick={handlePrev}
                      aria-label="Previous slide"
                      className="absolute left-3.5 sm:left-4 top-1/2 -translate-y-1/2 w-9 h-9 rounded-full bg-white/95 hover:bg-white shadow-md border border-neutral-200 flex items-center justify-center text-neutral-700 hover:text-neutral-900 transition-colors z-20"
                    >
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.5" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5L8.25 12l7.5-7.5" />
                      </svg>
                    </Button>

                    <Button
                      variant="none"
                      onClick={handleNext}
                      aria-label="Next slide"
                      className="absolute right-3.5 sm:right-4 top-1/2 -translate-y-1/2 w-9 h-9 rounded-full bg-white/95 hover:bg-white shadow-md border border-neutral-200 flex items-center justify-center text-neutral-700 hover:text-neutral-900 transition-colors z-20"
                    >
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.5" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5" />
                      </svg>
                    </Button>
                  </>
                )}
              </div>

              {products.length > 1 && (
                <div className="flex items-center justify-center gap-1.5 mt-4">
                  {products.map((p, index) => {
                    const isActive = index === currentSlide;
                    return (
                      <Button
                        key={p.productId}
                        variant="none"
                        onClick={() => setCurrentSlide(index)}
                        aria-label={`Go to slide ${index + 1}`}
                        className={`transition-all duration-300 rounded-full ${
                          isActive
                            ? 'w-6 h-1.5 bg-blue-600'
                            : 'w-1.5 h-1.5 bg-neutral-300 hover:bg-neutral-400'
                        }`}
                      />
                    );
                  })}
                </div>
              )}
            </>
          ) : (
            <div className="w-full h-[320px] sm:h-[380px] lg:h-[420px] rounded-3xl bg-neutral-100 flex items-center justify-center text-neutral-400 text-sm">
              No featured products available
            </div>
          )}
        </div>
      </div>
    </section>
  );
}

export default ProductShowcaseBanner;
