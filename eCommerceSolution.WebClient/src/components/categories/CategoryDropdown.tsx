import { Link } from 'react-router-dom';
import { CategoryResponse } from '../../types/category.types';

export function CategoryIcon({ category }: { category: CategoryResponse }) {
  if (category.categoryImagePath) {
    return (
      <img
        src={category.categoryImagePath}
        alt={category.categoryName}
        className="w-full h-full object-cover rounded-[7px]"
      />
    );
  }

  return (
    <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6A2.25 2.25 0 016 3.75h2.25A2.25 2.25 0 0110.5 6v2.25a2.25 2.25 0 01-2.25 2.25H6a2.25 2.25 0 01-2.25-2.25V6zM3.75 15.75A2.25 2.25 0 016 13.5h2.25a2.25 2.25 0 012.25 2.25V18a2.25 2.25 0 01-2.25 2.25H6A2.25 2.25 0 013.75 18v-2.25zM13.5 6a2.25 2.25 0 012.25-2.25H18A2.25 2.25 0 0120.25 6v2.25A2.25 2.25 0 0118 10.5h-2.25a2.25 2.25 0 01-2.25-2.25V6zM13.5 15.75a2.25 2.25 0 012.25-2.25H18a2.25 2.25 0 012.25 2.25V18A2.25 2.25 0 0118 20.25h-2.25A2.25 2.25 0 0113.5 18v-2.25z" />
    </svg>
  );
}

export interface CategoryDropdownProps {
  categories: CategoryResponse[];
  onClose: () => void;
}

export function CategoryDropdown({
  categories,
  onClose,
}: CategoryDropdownProps) {
  return (
    <div className="absolute top-full left-0 pt-[14px] z-50 animate-in fade-in zoom-in-95 duration-150">
      <div className="bg-white border border-[#E9E6DD] rounded-[14px] shadow-[0_20px_48px_rgba(20,22,26,0.12)] p-[22px] w-[min(640px,88vw)]">
        {categories.length === 0 ? (
          <div className="text-center py-6 text-xs text-neutral-400">
            No categories available
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-[22px]">
            {categories.map((category) => (
              <div key={category.categoryId}>
                <div className="flex items-center gap-2 mb-2.5">
                  <div className="w-7 h-7 rounded-[7px] bg-[#EAEEFF] text-[#3355FF] flex items-center justify-center flex-shrink-0">
                    <CategoryIcon category={category} />
                  </div>
                  <span className="text-[12.5px] font-bold text-[#16181C]">
                    {category.categoryName}
                  </span>
                </div>
                <div className="flex flex-col gap-1.5">
                  {category.subCategories && category.subCategories.length > 0 ? (
                    category.subCategories.map((sub) => (
                      <Link
                        key={sub.categoryId}
                        to={`/?categoryId=${sub.categoryId}`}
                        className="text-[12.5px] text-[#5F5E5A] hover:text-[#16181C] transition-colors block"
                        onClick={onClose}
                      >
                        {sub.categoryName}
                      </Link>
                    ))
                  ) : null}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
