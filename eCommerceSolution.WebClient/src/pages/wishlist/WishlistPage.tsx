import { useSelector } from 'react-redux';
import { Navigate } from 'react-router-dom';
import { RootState } from '../../store';
import { Wishlist } from '../../components/wishlist/Wishlist';

export function WishlistPage() {
  const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);

  if (!isAuthenticated) {
    return <Navigate to="/login?redirect=/wishlist" replace />;
  }

  return <Wishlist />;
}

export default WishlistPage;

