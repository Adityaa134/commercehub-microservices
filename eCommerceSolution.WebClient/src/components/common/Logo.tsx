interface LogoProps {
  showText?: boolean;
  textColor?: string;
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

export function Logo({
  showText = true,
  textColor = 'text-neutral-900',
  size = 'md',
  className = '',
}: LogoProps) {
  const iconSizes = {
    sm: 'w-7 h-7 text-sm rounded-md',
    md: 'w-8 h-8 text-base rounded-lg',
    lg: 'w-10 h-10 text-lg rounded-xl',
  };

  const textSizes = {
    sm: 'text-lg',
    md: 'text-xl',
    lg: 'text-2xl',
  };

  return (
    <div className={`flex items-center gap-2.5 ${className}`}>
      <span
        className={`flex items-center justify-center bg-blue-600 text-white font-bold shadow-sm flex-shrink-0 ${iconSizes[size]}`}
      >
        C
      </span>
      {showText && (
        <span className={`font-bold tracking-tight ${textSizes[size]} ${textColor}`}>
          CommerceHub
        </span>
      )}
    </div>
  );
}

export default Logo;
