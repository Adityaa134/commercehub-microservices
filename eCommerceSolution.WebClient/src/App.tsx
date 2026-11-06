import { Outlet } from 'react-router-dom';
import { Navbar, Footer } from './components';

export function App() {
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
