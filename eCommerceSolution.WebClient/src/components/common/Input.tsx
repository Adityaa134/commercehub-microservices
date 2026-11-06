import { InputHTMLAttributes, forwardRef, ReactNode } from 'react';

export interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
  variant?: 'light' | 'dark';
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  (
    {
      label,
      error,
      helperText,
      id,
      className = '',
      disabled,
      leftIcon,
      rightIcon,
      variant = 'light',
      ...props
    },
    ref
  ) => {
    const inputId = id || (label ? label.toLowerCase().replace(/\s+/g, '-') : undefined);

    const isLight = variant === 'light';

    return (
      <div className="w-full space-y-1.5">
        {label && (
          <label
            htmlFor={inputId}
            className={`block text-xs font-semibold tracking-wide ${
              isLight ? 'text-neutral-700' : 'text-neutral-300'
            }`}
          >
            {label}
          </label>
        )}

        <div className="relative flex items-center">
          {leftIcon && (
            <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-neutral-400">
              {leftIcon}
            </div>
          )}

          <input
            ref={ref}
            id={inputId}
            disabled={disabled}
            className={`w-full rounded-xl px-3.5 py-2.5 text-sm transition-all focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed ${
              isLight
                ? 'bg-white text-neutral-900 placeholder-neutral-400 border border-neutral-200/90 focus:border-blue-500 focus:ring-2 focus:ring-blue-500/10'
                : 'bg-[#16181f] text-white placeholder-neutral-500 border border-neutral-700/70 focus:border-blue-500 focus:ring-1 focus:ring-blue-500'
            } ${leftIcon ? 'pl-10' : ''} ${rightIcon ? 'pr-10' : ''} ${
              error ? '!border-red-500 !focus:ring-red-500/10' : ''
            } ${className}`}
            {...props}
          />

          {rightIcon && (
            <div className="absolute inset-y-0 right-0 pr-3.5 flex items-center">
              {rightIcon}
            </div>
          )}
        </div>

        {error && <p className="text-xs text-red-500 font-medium">{error}</p>}
        {!error && helperText && (
          <p className="text-xs text-neutral-500">{helperText}</p>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input';

export default Input;
