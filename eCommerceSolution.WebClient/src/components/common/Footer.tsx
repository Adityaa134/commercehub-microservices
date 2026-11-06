import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import Logo from './Logo';
import categoryService from '../../services/categoryService';
import { CategoryResponse } from '../../types/category.types';

export function Footer() {
  const [categories, setCategories] = useState<CategoryResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let isMounted = true;
    const fetchCategories = async () => {
      try {
        const data = await categoryService.getCategories();
        if (isMounted) {
          const parentCategories = data.filter((c) => !c.parentCategoryId);
          setCategories(parentCategories);
        }
      } catch (error) {
        console.error('Failed to load footer categories', error);
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    };
    fetchCategories();

    return () => {
      isMounted = false;
    };
  }, []);

  return (
    <footer className="bg-[#0e1015] text-neutral-400 text-sm border-t border-neutral-800/60">
      <div className="max-w-7xl mx-auto px-6 py-14">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-10 lg:gap-8">
          <div className="lg:col-span-2 space-y-6 max-w-sm">
            <Logo textColor="text-white" />

            <p className="text-neutral-400 text-sm leading-relaxed">
              Everyday gear across electronics, fashion and home — picked for quality, priced fairly, delivered fast.
            </p>

            <div className="flex items-center gap-2.5 pt-1">
              <a
                href="#github"
                aria-label="GitHub"
                className="w-8 h-8 flex items-center justify-center rounded-lg border border-neutral-800 bg-[#16181f] text-neutral-400 hover:text-white hover:border-neutral-700 transition-colors"
              >
                <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
                  <path fillRule="evenodd" clipRule="evenodd" d="M12 2C6.477 2 2 6.484 2 12.017c0 4.425 2.865 8.18 6.839 9.504.5.092.682-.217.682-.483 0-.237-.008-.868-.013-1.703-2.782.605-3.369-1.343-3.369-1.343-.454-1.158-1.11-1.466-1.11-1.466-.908-.62.069-.608.069-.608 1.003.07 1.53 1.032 1.53 1.032.892 1.53 2.341 1.088 2.91.832.092-.647.35-1.088.636-1.338-2.22-.253-4.555-1.113-4.555-4.951 0-1.093.39-1.988 1.029-2.688-.103-.253-.446-1.272.098-2.65 0 0 .84-.27 2.75 1.026A9.564 9.564 0 0112 6.844c.85.004 1.705.115 2.504.337 1.909-1.296 2.747-1.027 2.747-1.027.546 1.379.202 2.398.1 2.651.64.7 1.028 1.595 1.028 2.688 0 3.848-2.339 4.695-4.566 4.943.359.309.678.92.678 1.855 0 1.338-.012 2.419-.012 2.747 0 .268.18.58.688.482A10.019 10.019 0 0022 12.017C22 6.484 17.522 2 12 2z" />
                </svg>
              </a>

              <a
                href="#linkedin"
                aria-label="LinkedIn"
                className="w-8 h-8 flex items-center justify-center rounded-lg border border-neutral-800 bg-[#16181f] text-neutral-400 hover:text-white hover:border-neutral-700 transition-colors"
              >
                <svg className="w-3.5 h-3.5" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z" />
                </svg>
              </a>

              <a
                href="#contact"
                aria-label="Contact Email"
                className="w-8 h-8 flex items-center justify-center rounded-lg border border-neutral-800 bg-[#16181f] text-neutral-400 hover:text-white hover:border-neutral-700 transition-colors"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75" />
                </svg>
              </a>
            </div>
          </div>

          <div className="space-y-4">
            <h4 className="text-xs font-semibold tracking-wider text-white uppercase">
              SHOP
            </h4>
            <ul className="space-y-2.5 text-sm">
              {isLoading ? (
                <>
                  <li className="h-4 w-20 bg-neutral-800/60 rounded animate-pulse" />
                  <li className="h-4 w-28 bg-neutral-800/60 rounded animate-pulse" />
                  <li className="h-4 w-24 bg-neutral-800/60 rounded animate-pulse" />
                  <li className="h-4 w-16 bg-neutral-800/60 rounded animate-pulse" />
                </>
              ) : (
                categories.map((cat) => (
                  <li key={cat.categoryId}>
                    <Link
                      to={`/?categoryId=${cat.categoryId}`}
                      className="hover:text-white transition-colors"
                    >
                      {cat.categoryName}
                    </Link>
                  </li>
                ))
              )}
            </ul>
          </div>

          <div className="space-y-4">
            <h4 className="text-xs font-semibold tracking-wider text-white uppercase">
              CUSTOMER SERVICE
            </h4>
            <ul className="space-y-2.5 text-sm">
              <li>
                <a href="#track-order" className="hover:text-white transition-colors">
                  Track your order
                </a>
              </li>
              <li>
                <a href="#returns-exchanges" className="hover:text-white transition-colors">
                  Returns &amp; exchanges
                </a>
              </li>
              <li>
                <a href="#shipping-info" className="hover:text-white transition-colors">
                  Shipping info
                </a>
              </li>
              <li>
                <a href="#faqs" className="hover:text-white transition-colors">
                  FAQs
                </a>
              </li>
              <li>
                <a href="#contact-us" className="hover:text-white transition-colors">
                  Contact us
                </a>
              </li>
            </ul>
          </div>

          <div className="space-y-4">
            <h4 className="text-xs font-semibold tracking-wider text-white uppercase">
              COMPANY
            </h4>
            <ul className="space-y-2.5 text-sm">
              <li>
                <a href="#about-us" className="hover:text-white transition-colors">
                  About us
                </a>
              </li>
              <li>
                <a href="#project-case-study" className="hover:text-white transition-colors">
                  Project case study
                </a>
              </li>
              <li>
                <a href="#contact-us" className="hover:text-white transition-colors">
                  Contact us
                </a>
              </li>
            </ul>
          </div>
        </div>

        <div className="mt-14 pt-8 border-t border-neutral-800/80 flex flex-col md:flex-row items-center justify-between gap-4 text-xs text-neutral-500">
          <p>© 2026 CommerceHub. All rights reserved.</p>

          <div className="flex items-center gap-6">
            <a href="#privacy" className="hover:text-neutral-400 transition-colors">
              Privacy policy
            </a>
            <a href="#terms" className="hover:text-neutral-400 transition-colors">
              Terms of service
            </a>
            <a href="#sitemap" className="hover:text-neutral-400 transition-colors">
              Sitemap
            </a>
          </div>

          <div className="flex items-center gap-2">
            <span className="px-2.5 py-1 rounded bg-[#16181f] border border-neutral-800 text-[11px] font-medium text-neutral-400">
              Visa
            </span>
            <span className="px-2.5 py-1 rounded bg-[#16181f] border border-neutral-800 text-[11px] font-medium text-neutral-400">
              Mastercard
            </span>
            <span className="px-2.5 py-1 rounded bg-[#16181f] border border-neutral-800 text-[11px] font-medium text-neutral-400">
              UPI
            </span>
            <span className="px-2.5 py-1 rounded bg-[#16181f] border border-neutral-800 text-[11px] font-medium text-neutral-400">
              PayPal
            </span>
          </div>
        </div>
      </div>
    </footer>
  );
}

export default Footer;
