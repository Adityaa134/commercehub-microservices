import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, Link } from 'react-router-dom';
import { Input } from '../common/Input';
import { Button } from '../common/Button';
import { RegisterRequest } from '../../types/user.types';
import authService from '../../services/authService';

export function Register() {
  const navigate = useNavigate();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [agreedToTerms, setAgreedToTerms] = useState(false);
  const [termsError, setTermsError] = useState('');

  const {
    register,
    handleSubmit,
    watch,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<RegisterRequest>({
    defaultValues: {
      fullName: '',
      email: '',
      phoneNumber: '',
      password: '',
      confirmPassword: '',
    },
  });

  const password = watch('password') || '';

  const getPasswordStrength = (pwd: string) => {
    if (!pwd) {
      return {
        score: 0,
        label: 'Enter a password',
        textColor: 'text-neutral-400',
        barColor: 'bg-neutral-200',
      };
    }

    let score = 0;
    if (pwd.length >= 8) score++;
    if (/[a-z]/.test(pwd) && /[A-Z]/.test(pwd)) score++;
    if (/[0-9]/.test(pwd)) score++;
    if (/[^A-Za-z0-9]/.test(pwd)) score++;

    if (score <= 1) {
      return {
        score: 1,
        label: 'Weak',
        textColor: 'text-red-500',
        barColor: 'bg-red-500',
      };
    }
    if (score === 2) {
      return {
        score: 2,
        label: 'Fair',
        textColor: 'text-orange-500',
        barColor: 'bg-orange-500',
      };
    }
    if (score === 3) {
      return {
        score: 3,
        label: 'Good',
        textColor: 'text-amber-500',
        barColor: 'bg-amber-500',
      };
    }
    return {
      score: 4,
      label: 'Strong',
      textColor: 'text-emerald-500',
      barColor: 'bg-emerald-500',
    };
  };

  const strength = getPasswordStrength(password);

  const onSubmit = async (data: RegisterRequest) => {
    setTermsError('');

    if (!agreedToTerms) {
      setTermsError('Please agree to the Terms of service and Privacy policy.');
      return;
    }

    if (data.password !== data.confirmPassword) {
      setError('confirmPassword', {
        message: 'Passwords do not match.',
      });
      return;
    }

    try {
      await authService.register(data);
      navigate('/verify-email', { state: { email: data.email } });
    } catch (err: unknown) {
      if (err && typeof err === 'object' && 'response' in err) {
        const axiosError = err as { response?: { data?: { message?: string } } };
        setError('root', {
          message: axiosError.response?.data?.message || 'Registration failed. Please check your details.',
        });
      } else {
        setError('root', {
          message: 'Unable to connect to the server. Please try again.',
        });
      }
    }
  };

  return (
    <div className="max-w-[460px] w-full bg-white rounded-3xl border border-neutral-100 shadow-[0_8px_30px_rgb(0,0,0,0.04)] p-6 sm:p-10 transition-all mx-auto">
      <div className="text-left mb-7">
        <h1 className="text-2xl sm:text-[28px] font-bold text-neutral-900 tracking-tight">
          Create your account
        </h1>
        <p className="text-neutral-500 text-sm mt-1.5 leading-relaxed">
          Join CommerceHub for faster checkout and order tracking.
        </p>
      </div>

      <Button
        variant="none"
        type="button"
        className="w-full flex items-center justify-center gap-3 py-3 px-4 border border-neutral-200/90 rounded-xl bg-white hover:bg-neutral-50 text-neutral-700 text-sm font-semibold transition-colors shadow-xs"
      >
        <svg className="w-4 h-4 flex-shrink-0" viewBox="0 0 24 24">
          <path
            fill="#4285F4"
            d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
          />
          <path
            fill="#34A853"
            d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
          />
          <path
            fill="#FBBC05"
            d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.06H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.94l2.85-2.22.81-.63z"
          />
          <path
            fill="#EA4335"
            d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.06l3.66 2.84c.87-2.6 3.3-4.52 6.16-4.52z"
          />
        </svg>
        <span>Continue with Google</span>
      </Button>

      <div className="relative my-6 flex items-center justify-center">
        <div className="absolute inset-0 flex items-center">
          <div className="w-full border-t border-neutral-200/80" />
        </div>
        <span className="relative bg-white px-3 text-xs font-semibold text-neutral-400 uppercase tracking-wider">
          OR
        </span>
      </div>

      {errors.root?.message && (
        <div className="mb-5 p-3 rounded-xl bg-red-50 border border-red-200 text-xs text-red-600 font-medium">
          {errors.root.message}
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <Input
          label="Full name"
          type="text"
          placeholder="Aditya Gupta"
          error={errors.fullName?.message}
          {...register('fullName', {
            required: 'Full Name is required',
            minLength: {
              value: 2,
              message: 'Full Name must be between 2 and 50 characters.',
            },
            maxLength: {
              value: 50,
              message: 'Full Name must be between 2 and 50 characters.',
            },
          })}
          leftIcon={
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z"
              />
            </svg>
          }
        />

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

        <Input
          label="Phone number"
          type="tel"
          placeholder="9876543210"
          error={errors.phoneNumber?.message}
          {...register('phoneNumber', {
            required: 'Phone Number is required',
            minLength: {
              value: 10,
              message: 'Phone number must be at least 10 digits',
            },
            maxLength: {
              value: 10,
              message: 'Phone number must be at most 10 digits',
            },
            pattern: {
              value: /^[0-9]+$/,
              message: 'Phone Number should contains only digits',
            },
          })}
          leftIcon={
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M2.25 6.75c0 8.284 6.716 15 15 15h2.25a2.25 2.25 0 002.25-2.25v-1.372c0-.516-.351-.966-.852-1.091l-4.423-1.106c-.44-.11-.902.055-1.173.417l-.97 1.293c-.282.376-.769.542-1.21.38a12.035 12.035 0 01-7.143-7.143c-.162-.441.004-.928.38-1.21l1.293-.97c.363-.271.527-.734.417-1.173L6.963 3.102a1.125 1.125 0 00-1.091-.852H4.5A2.25 2.25 0 002.25 4.5v2.25z"
              />
            </svg>
          }
        />

        <div>
          <Input
            label="Password"
            type={showPassword ? 'text' : 'password'}
            placeholder="Create a password"
            error={errors.password?.message}
            {...register('password', {
              required: 'Password is required',
              validate: {
                minLength: (v) => v.length >= 8 || 'Password must be at least 8 characters',
                hasUppercase: (v) => /[A-Z]/.test(v) || 'Password must contain at least one uppercase letter',
                hasLowercase: (v) => /[a-z]/.test(v) || 'Password must contain at least one lowercase letter',
                hasDigit: (v) => /[0-9]/.test(v) || 'Password must contain at least one digit',
                hasSpecialChar: (v) => /[^A-Za-z0-9]/.test(v) || 'Password must contain at least one special character',
                hasUniqueChars: (v) => new Set(v).size >= 3 || 'Password must contain at least 3 unique characters',
              },
            })}
            leftIcon={
              <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z"
                />
              </svg>
            }
            rightIcon={
              <Button
                variant="none"
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="text-neutral-400 hover:text-neutral-600 transition-colors p-0"
                aria-label={showPassword ? 'Hide password' : 'Show password'}
              >
                {showPassword ? (
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88"
                    />
                  </svg>
                ) : (
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z"
                    />
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                    />
                  </svg>
                )}
              </Button>
            }
          />

          <div className="grid grid-cols-4 gap-2 mt-2">
            {[1, 2, 3, 4].map((level) => (
              <div
                key={level}
                className={`h-1 rounded-full transition-all duration-300 ${strength.score >= level ? strength.barColor : 'bg-neutral-200'
                  }`}
              />
            ))}
          </div>
          <p className={`text-xs font-medium mt-1.5 transition-colors ${strength.textColor}`}>
            Password strength: {strength.label}
          </p>
        </div>

        <Input
          label="Confirm password"
          type={showConfirmPassword ? 'text' : 'password'}
          placeholder="Re-enter your password"
          error={errors.confirmPassword?.message}
          {...register('confirmPassword', {
            required: 'Please confirm your password',
            validate: {
              matchesPassword: (value) => value === watch('password') || 'Passwords do not match',
              minLength: (v) => v.length >= 8 || 'Password must be at least 8 characters',
              hasUppercase: (v) => /[A-Z]/.test(v) || 'Password must contain at least one uppercase letter',
              hasLowercase: (v) => /[a-z]/.test(v) || 'Password must contain at least one lowercase letter',
              hasDigit: (v) => /[0-9]/.test(v) || 'Password must contain at least one digit',
              hasSpecialChar: (v) => /[^A-Za-z0-9]/.test(v) || 'Password must contain at least one special character',
              hasUniqueChars: (v) => new Set(v).size >= 3 || 'Password must contain at least 3 unique characters',
            },
          })}
          leftIcon={
            <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z"
              />
            </svg>
          }
          rightIcon={
            <Button
              variant="none"
              type="button"
              onClick={() => setShowConfirmPassword(!showConfirmPassword)}
              className="text-neutral-400 hover:text-neutral-600 transition-colors p-0"
              aria-label={showConfirmPassword ? 'Hide confirm password' : 'Show confirm password'}
            >
              {showConfirmPassword ? (
                <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88"
                  />
                </svg>
              ) : (
                <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="1.8" viewBox="0 0 24 24">
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z"
                  />
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                  />
                </svg>
              )}
            </Button>
          }
        />

        <div className="pt-1">
          <label className="flex items-start gap-2.5 cursor-pointer select-none text-xs text-neutral-600 leading-normal">
            <input
              type="checkbox"
              checked={agreedToTerms}
              onChange={(e) => {
                setAgreedToTerms(e.target.checked);
                if (e.target.checked) {
                  setTermsError('');
                }
              }}
              className="w-4 h-4 mt-0.5 rounded border-neutral-300 text-blue-600 focus:ring-blue-500 cursor-pointer flex-shrink-0"
            />
            <span>
              I agree to the{' '}
              <a href="#terms" className="font-semibold text-blue-600 hover:text-blue-500">
                Terms of service
              </a>{' '}
              and{' '}
              <a href="#privacy" className="font-semibold text-blue-600 hover:text-blue-500">
                Privacy policy
              </a>
            </span>
          </label>
          {termsError && (
            <p className="text-xs text-red-500 mt-1 font-medium">{termsError}</p>
          )}
        </div>

        <Button
          type="submit"
          variant="none"
          isLoading={isSubmitting}
          className="w-full bg-[#16181f] hover:bg-black text-white font-semibold py-3 rounded-xl text-sm flex items-center justify-center gap-2 mt-5 shadow-xs cursor-pointer"
        >
          <span>Create account</span>
          <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
          </svg>
        </Button>
      </form>

      <p className="mt-8 text-center text-sm text-neutral-500 font-medium">
        Already have an account?{' '}
        <Link
          to="/login"
          className="font-semibold text-blue-600 hover:text-blue-500 transition-colors cursor-pointer"
        >
          Sign in
        </Link>
      </p>
    </div>
  );
}

export default Register;
