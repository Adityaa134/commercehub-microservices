import { useSearchParams } from 'react-router-dom';
import { ProductShowcaseBanner, ProductGrid } from '../../components';

export function ProductsPage() {
  const [searchParams, setSearchParams] = useSearchParams();

  const handleShopNewArrivals = () => {
    const next = new URLSearchParams(searchParams);
    next.set('sort', 'newest');
    next.set('page', '1');
    setSearchParams(next);
    document.getElementById('products-section')?.scrollIntoView({ behavior: 'smooth' });
  };

  const handleExploreAll = () => {
    const next = new URLSearchParams(searchParams);
    next.delete('sort');
    next.set('page', '1');
    setSearchParams(next);
    document.getElementById('products-section')?.scrollIntoView({ behavior: 'smooth' });
  };

  return (
    <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 sm:py-10">
      <ProductShowcaseBanner
        onShopNewArrivals={handleShopNewArrivals}
        onExploreAll={handleExploreAll}
      />

      <div id="products-section" className="flex flex-col lg:flex-row gap-8 items-start pt-2">
        <aside className="hidden lg:block w-60 flex-shrink-0" />

        <div className="flex-1 w-full min-w-0">
          <ProductGrid />
        </div>
      </div>
    </div>
  );
}

export default ProductsPage;
