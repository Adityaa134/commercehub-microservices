import { useState, useEffect } from 'react';
import { useLocation, useSearchParams, Link } from 'react-router-dom';
import { Logo } from '../common/Logo';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import authService from '../../services/authService';

export function VerifyEmail() {
  const location = useLocation();
  const [searchParams] = useSearchParams();
  const initialEmail = (location.state as { email?: string })?.email || searchParams.get('email') || '';

  const [email, setEmail] = useState(initialEmail);
  const [countdown, setCountdown] = useState(30);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    if (countdown <= 0) return;
    const interval = setInterval(() => {
      setCountdown((prev) => prev - 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [countdown]);

  const handleResend = async () => {
    if (countdown > 0 || isSubmitting) return;

    if (!email || !email.includes('@')) {
      setErrorMessage('Please provide a valid email address to resend verification.');
      return;
    }

    setErrorMessage('');
    setSuccessMessage('');
    setIsSubmitting(true);

    try {
      const response = await authService.resendConfirmationEmail({ email });
      setSuccessMessage(
        response.message || 'Verification link sent successfully. Please check your inbox.'
      );
      setCountdown(30);
    } catch (err: unknown) {
      if (err && typeof err === 'object' && 'response' in err) {
        const axiosError = err as { response?: { status?: number; data?: { message?: string } } };
        if (axiosError.response?.status === 429) {
          setErrorMessage('Too many requests. Please wait before requesting another email.');
        } else {
          setErrorMessage(
            axiosError.response?.data?.message || 'Unable to resend verification link. Please try again.'
          );
        }
      } else {
        setErrorMessage('Unable to connect to the server. Please try again.');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="max-w-[440px] w-full bg-white rounded-3xl border border-neutral-100 shadow-[0_8px_30px_rgb(0,0,0,0.04)] p-8 sm:p-10 text-center transition-all mx-auto">
      <div className="flex justify-center mb-6">
        <Logo size="md" />
      </div>

      <div className="w-16 h-16 mx-auto mb-6 rounded-full bg-blue-50 flex items-center justify-center text-blue-600">
        <svg
          className="w-8 h-8"
          fill="none"
          stroke="currentColor"
          strokeWidth="1.8"
          viewBox="0 0 24 24"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75"
          />
        </svg>
      </div>

      <h1 className="text-2xl sm:text-[28px] font-bold text-neutral-900 tracking-tight mb-2">
        Verify your email
      </h1>

      <p className="text-neutral-500 text-sm leading-relaxed">
        Your account&apos;s almost ready. We&apos;ve sent a verification link to
      </p>
      {email ? (
        <p className="text-sm font-semibold text-neutral-900 mb-6 mt-1 break-all">
          {email}
        </p>
      ) : (
        <div className="my-4 text-left">
          <Input
            label="Email address"
            type="email"
            placeholder="you@example.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>
      )}

      <div className="bg-neutral-50/80 rounded-2xl p-4 sm:p-5 border border-neutral-100 text-left text-xs sm:text-sm text-neutral-600 leading-relaxed mb-6">
        Click the link in that email to activate and verify your account. If you don&apos;t see it within a minute or two, please check your spam or junk folder.
      </div>

      {errorMessage && (
        <div className="mb-5 p-3 rounded-xl bg-red-50 border border-red-200 text-xs text-red-600 font-medium text-left">
          {errorMessage}
        </div>
      )}

      {successMessage && (
        <div className="mb-5 p-3 rounded-xl bg-green-50 border border-green-200 text-xs text-green-700 font-medium text-left">
          {successMessage}
        </div>
      )}

      {countdown > 0 ? (
        <Button
          type="button"
          variant="none"
          disabled
          className="w-full bg-neutral-100 hover:bg-neutral-100 border border-neutral-200 text-neutral-400 font-medium py-3 rounded-xl text-sm flex items-center justify-center gap-2 cursor-not-allowed select-none shadow-none"
        >
          <svg
            className="w-4 h-4 text-neutral-400 flex-shrink-0"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0l3.181 3.183a8.25 8.25 0 0013.803-3.7M4.031 9.865a8.25 8.25 0 0113.803-3.7l3.181 3.182m0-4.991v4.99"
            />
          </svg>
          <span>Resend available in {countdown}s</span>
        </Button>
      ) : (
        <Button
          type="button"
          variant="none"
          isLoading={isSubmitting}
          onClick={handleResend}
          className="w-full bg-[#16181f] hover:bg-black text-white font-semibold py-3 rounded-xl text-sm flex items-center justify-center gap-2 shadow-xs cursor-pointer transition-colors"
        >
          <svg
            className="w-4 h-4 flex-shrink-0"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0l3.181 3.183a8.25 8.25 0 0013.803-3.7M4.031 9.865a8.25 8.25 0 0113.803-3.7l3.181 3.182m0-4.991v4.99"
            />
          </svg>
          <span>Resend verification email</span>
        </Button>
      )}

      <div className="mt-6 flex justify-center">
        <Link
          to="/login"
          className="inline-flex items-center gap-2 text-sm text-neutral-600 hover:text-neutral-900 transition-colors font-medium cursor-pointer"
        >
          <svg
            className="w-4 h-4"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18"
            />
          </svg>
          <span>Back to sign in</span>
        </Link>
      </div>
    </div>
  );
}

export default VerifyEmail;
