import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { Link } from 'react-router-dom';
import { Input } from '../common/Input';
import { Button } from '../common/Button';
import { EmailRequest } from '../../types/user.types';
import authService from '../../services/authService';

export function ForgotPassword() {
  const [countdown, setCountdown] = useState(0);
  const [isSent, setIsSent] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<EmailRequest>({
    defaultValues: {
      email: '',
    },
  });

  useEffect(() => {
    if (countdown <= 0) return;
    const interval = setInterval(() => {
      setCountdown((prev) => prev - 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [countdown]);

  const onSubmit = async (data: EmailRequest) => {
    if (countdown > 0) return;

    setSuccessMessage('');

    try {
      const response = await authService.forgotPassword(data);
      setSuccessMessage(
        response.message || 'If an account exists with this email, a reset link has been sent.'
      );
      setIsSent(true);
      setCountdown(30);
    } catch (err: unknown) {
      if (err && typeof err === 'object' && 'response' in err) {
        const axiosError = err as { response?: { status?: number; data?: { message?: string } } };
        if (axiosError.response?.status === 429) {
          setError('root', {
            message: 'Too many requests. Please wait a moment before trying again.',
          });
        } else {
          setError('root', {
            message: axiosError.response?.data?.message || 'Unable to send reset link. Please try again.',
          });
        }
      } else {
        setError('root', {
          message: 'Unable to connect to the server. Please try again.',
        });
      }
    }
  };

  return (
    <div className="max-w-[440px] w-full bg-white rounded-3xl border border-neutral-100 shadow-[0_8px_30px_rgb(0,0,0,0.04)] p-8 sm:p-10 transition-all">
      <Link
        to="/login"
        className="inline-flex items-center gap-2 text-sm text-neutral-600 hover:text-neutral-900 transition-colors mb-6 cursor-pointer font-medium"
      >
        <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
        </svg>
        <span>Back to sign in</span>
      </Link>

      <div className="text-left mb-7">
        <h1 className="text-2xl sm:text-[28px] font-bold text-neutral-900 tracking-tight">
          Forgot your password?
        </h1>
        <p className="text-neutral-500 text-sm mt-1.5 leading-relaxed">
          No worries — enter your email and we&apos;ll send a reset link.
        </p>
      </div>

      {errors.root?.message && (
        <div className="mb-5 p-3 rounded-xl bg-red-50 border border-red-200 text-xs text-red-600 font-medium">
          {errors.root.message}
        </div>
      )}

      {successMessage && (
        <div className="mb-5 p-3 rounded-xl bg-green-50 border border-green-200 text-xs text-green-700 font-medium">
          {successMessage}
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <Input
          label="Email"
          type="email"
          placeholder="you@example.com"
          error={errors.email?.message}
          {...register('email', {
            required: 'Email is required',
            pattern: {
              value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
              message: 'Please enter a valid email address',
            },
          })}
          leftIcon={
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75"
              />
            </svg>
          }
        />

        {isSent && countdown > 0 ? (
          <Button
            type="button"
            variant="none"
            disabled
            className="w-full bg-neutral-100 hover:bg-neutral-100 border border-neutral-200 text-neutral-600 font-semibold py-3 rounded-xl text-sm flex items-center justify-center gap-2 cursor-not-allowed select-none shadow-none"
          >
            <svg className="w-4 h-4 text-neutral-400 flex-shrink-0" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <span>Resend in {countdown}s</span>
          </Button>
        ) : (
          <Button
            type="submit"
            variant="none"
            isLoading={isSubmitting}
            className="w-full bg-[#16181f] hover:bg-black text-white font-semibold py-3 rounded-xl text-sm flex items-center justify-center gap-2 shadow-xs cursor-pointer"
          >
            <span>{isSent ? 'Resend reset link' : 'Send reset link'}</span>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
            </svg>
          </Button>
        )}
      </form>
    </div>
  );
}

export default ForgotPassword;
