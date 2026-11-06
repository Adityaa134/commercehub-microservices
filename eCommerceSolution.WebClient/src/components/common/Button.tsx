import { ButtonHTMLAttributes } from 'react';
import Spinner from './Spinner';

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'dark' | 'outline' | 'danger' | 'ghost' | 'none';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
}

export function Button({
  children,
  variant = 'primary',
  size = 'md',
  isLoading = false,
  disabled,
  className = '',
  ...props
}: ButtonProps) {
  const baseStyles =
    'inline-flex items-center justify-center font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:cursor-not-allowed cursor-pointer';

  const variantStyles: Record<'primary' | 'secondary' | 'dark' | 'outline' | 'danger' | 'ghost' | 'none', string> = {
    primary:
      'bg-blue-600 hover:bg-blue-500 text-white focus:ring-blue-500 shadow-sm focus:ring-offset-[#0e1015] disabled:opacity-50',
    secondary:
      'bg-neutral-800 hover:bg-neutral-700 text-white focus:ring-neutral-600 focus:ring-offset-[#0e1015] disabled:opacity-50',
    dark:
      'bg-[#16181f] hover:bg-black text-white focus:ring-neutral-900 shadow-sm disabled:opacity-50',
    outline:
      'border border-neutral-300 hover:bg-neutral-100 hover:text-neutral-900 text-neutral-800 focus:ring-neutral-400 bg-white disabled:opacity-50',
    danger:
      'bg-red-600 hover:bg-red-500 text-white focus:ring-red-500 focus:ring-offset-[#0e1015] disabled:opacity-50',
    ghost:
      'text-neutral-400 hover:text-white hover:bg-neutral-800/60 focus:ring-neutral-600 focus:ring-offset-[#0e1015] disabled:opacity-50',
    none: '',
  };

  const sizeStyles = {
    sm: 'px-3 py-1.5 text-xs rounded-md gap-1.5',
    md: 'px-4 py-2 text-sm rounded-lg gap-2',
    lg: 'px-5 py-2.5 text-base rounded-lg gap-2.5',
  };

  const spinnerSizes: Record<'sm' | 'md' | 'lg', 'sm' | 'md' | 'lg'> = {
    sm: 'sm',
    md: 'sm',
    lg: 'md',
  };

  return (
    <button
      disabled={disabled || isLoading}
      className={`${baseStyles} ${variantStyles[variant]} ${variant !== 'none' ? sizeStyles[size] : ''} ${className}`}
      {...props}
    >
      {isLoading && <Spinner size={spinnerSizes[size]} />}
      {children}
    </button>
  );
}

export default Button;
