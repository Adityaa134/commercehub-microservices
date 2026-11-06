import { useState, useEffect, useRef } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { Logo } from '../common/Logo';
import { Spinner } from '../common/Spinner';
import authService from '../../services/authService';

type ConfirmationStatus = 'loading' | 'success' | 'error';

export function ConfirmEmail() {
  const [searchParams] = useSearchParams();
  const [status, setStatus] = useState<ConfirmationStatus>('loading');
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const hasCalledRef = useRef(false);

  useEffect(() => {
    if (hasCalledRef.current) return;
    hasCalledRef.current = true;

    const userId = searchParams.get('uid') || searchParams.get('userId');
    const token = searchParams.get('token');

    if (!userId || !token) {
      setStatus('error');
      setErrorMessage('Invalid or missing confirmation link. Please check the link from your email.');
      return;
    }

    const confirmAccount = async () => {
      try {
        const response = await authService.confirmEmail({ userId, token });
        setSuccessMessage(response.message || 'Your account is verified and ready to go. Please sign in to continue.');
        setStatus('success');
      } catch (err: unknown) {
        setStatus('error');
        if (err && typeof err === 'object' && 'response' in err) {
          const axiosError = err as { response?: { data?: { message?: string } } };
          setErrorMessage(
            axiosError.response?.data?.message || 'The confirmation link is invalid or has expired.'
          );
        } else {
          setErrorMessage('Unable to connect to the server. Please try again later.');
        }
      }
    };

    confirmAccount();
  }, [searchParams]);

  return (
    <div className="max-w-[440px] w-full bg-white rounded-3xl border border-neutral-100 shadow-[0_8px_30px_rgb(0,0,0,0.04)] p-8 sm:p-10 text-center transition-all mx-auto">
      <div className="flex justify-center mb-6">
        <Logo size="md" />
      </div>

      {status === 'loading' && (
        <div>
          <div className="w-16 h-16 mx-auto mb-6 rounded-full bg-neutral-100 flex items-center justify-center text-neutral-800">
            <Spinner size="md" />
          </div>

          <h1 className="text-2xl sm:text-[28px] font-bold text-neutral-900 tracking-tight mb-2">
            Confirming your email...
          </h1>

          <p className="text-neutral-500 text-sm leading-relaxed">
            This&apos;ll just take a second.
          </p>
        </div>
      )}

      {status === 'success' && (
        <div>
          <div className="w-16 h-16 mx-auto mb-6 rounded-full bg-emerald-50 flex items-center justify-center text-emerald-600">
            <svg className="w-8 h-8" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
              <circle cx="12" cy="12" r="9" />
              <path strokeLinecap="round" strokeLinejoin="round" d="M8.5 12.5l2.5 2.5 4.5-5" />
            </svg>
          </div>

          <h1 className="text-2xl sm:text-[28px] font-bold text-neutral-900 tracking-tight mb-2">
            Email confirmed
          </h1>

          <p className="text-neutral-500 text-sm leading-relaxed mb-8">
            {successMessage || 'Your account is verified and ready to go. Please sign in to continue.'}
          </p>

          <Link
            to="/login"
            className="w-full bg-[#16181f] hover:bg-black text-white font-semibold py-3.5 px-6 rounded-2xl text-sm flex items-center justify-center gap-2 shadow-xs cursor-pointer transition-colors"
          >
            <span>Continue to sign in</span>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
            </svg>
          </Link>
        </div>
      )}

      {status === 'error' && (
        <div>
          <div className="w-16 h-16 mx-auto mb-6 rounded-full bg-red-50 flex items-center justify-center text-red-600">
            <svg className="w-8 h-8" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
              <circle cx="12" cy="12" r="9" />
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 8v4m0 4h.01" />
            </svg>
          </div>

          <h1 className="text-2xl sm:text-[28px] font-bold text-neutral-900 tracking-tight mb-2">
            Verification failed
          </h1>

          <p className="text-neutral-500 text-sm leading-relaxed mb-8">
            {errorMessage || 'The confirmation link is invalid or has expired.'}
          </p>

          <Link
            to="/login"
            className="w-full bg-[#16181f] hover:bg-black text-white font-semibold py-3.5 px-6 rounded-2xl text-sm flex items-center justify-center gap-2 shadow-xs cursor-pointer transition-colors"
          >
            <span>Back to sign in</span>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
            </svg>
          </Link>

          <div className="mt-4">
            <Link
              to="/verify-email"
              className="text-xs text-neutral-500 hover:text-neutral-900 transition-colors font-medium cursor-pointer"
            >
              Need a new verification link? Resend here
            </Link>
          </div>
        </div>
      )}
    </div>
  );
}

export default ConfirmEmail;
