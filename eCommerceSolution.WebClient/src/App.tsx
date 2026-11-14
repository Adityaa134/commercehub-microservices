import { useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { Navbar, Footer } from './components';
import { RootState } from './store';
import { setCategories } from './store/slices/categorySlice';
import { setUser } from './store/slices/authSlice';
import { setCart } from './store/slices/cartSlice';
import { setWishlistIds, clearWishlist } from './store/slices/wishlistSlice';
import categoryService from './services/categoryService';
import authService from './services/authService';
import cartService from './services/cartService';
import wishlistService from './services/wishlistService';
import { CartItemResponse } from './types/cart.types';

export function App() {
  const dispatch = useDispatch();
  const categories = useSelector((state: RootState) => state.categories.items);
  const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);

  useEffect(() => {
    const restoreSession = async () => {
      try {
        const user = await authService.getMe();
        dispatch(setUser(user));
      } catch {
      }
    };

    restoreSession();
  }, [dispatch]);

  useEffect(() => {
    const loadCategories = async () => {
      try {
        const data = await categoryService.getCategories();
        dispatch(setCategories(data));
      } catch {
      }
    };

    if (categories.length === 0) {
      loadCategories();
    }
  }, [categories.length, dispatch]);

  useEffect(() => {
    const loadCart = async () => {
      try {
        if (isAuthenticated) {
          const cartData = await cartService.getCart();
          dispatch(setCart(cartData));
        } else {
          const raw = localStorage.getItem('commercehub_guest_cart');
          if (raw) {
            const items: CartItemResponse[] = JSON.parse(raw);
            const totalItems = items.reduce((sum, item) => sum + item.quantity, 0);
            const cartTotal = items.reduce((sum, item) => sum + item.totalPrice, 0);
            dispatch(
              setCart({
                cartId: '',
                userId: '',
                cartItems: items,
                totalItems,
                cartTotal: Math.round(cartTotal * 100) / 100,
              })
            );
          } else {
            dispatch(
              setCart({
                cartId: '',
                userId: '',
                cartItems: [],
                totalItems: 0,
                cartTotal: 0,
              })
            );
          }
        }
      } catch {
        dispatch(
          setCart({
            cartId: '',
            userId: '',
            cartItems: [],
            totalItems: 0,
            cartTotal: 0,
          })
        );
      }
    };

    loadCart();
  }, [isAuthenticated, dispatch]);

  useEffect(() => {
    const loadWishlist = async () => {
      if (isAuthenticated) {
        try {
          const ids = await wishlistService.getWishlistProductIds();
          dispatch(setWishlistIds(ids));
        } catch {
        }
      } else {
        dispatch(clearWishlist());
      }
    };

    loadWishlist();
  }, [isAuthenticated, dispatch]);

  return (
    <div className="min-h-screen flex flex-col justify-between bg-[#f8f9fa] text-neutral-900">
      <Navbar />

      <main className="flex-1 w-full">
        <Outlet />
      </main>

      <Footer />
    </div>
  );
}

export default App;
