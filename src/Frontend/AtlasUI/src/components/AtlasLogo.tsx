import React from 'react';

interface AtlasLogoProps {
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

const AtlasLogo: React.FC<AtlasLogoProps> = ({ size = 'md', className = '' }) => {
  const sizes = {
    sm: 'text-xl',
    md: 'text-2xl',
    lg: 'text-4xl',
  };

  return (
    <span className={`atlas-logo inline-flex items-baseline ${sizes[size]} ${className}`}>
      <span className="text-foreground">Atla</span>
      <span className="relative text-foreground">
        s
        <svg
          className="absolute bottom-0 left-0"
          viewBox="0 0 20 30"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
          style={{
            width: size === 'lg' ? '18px' : size === 'md' ? '13px' : '10px',
            height: size === 'lg' ? '28px' : size === 'md' ? '20px' : '15px',
            transform: `translateY(${size === 'lg' ? '14px' : size === 'md' ? '10px' : '7px'}) translateX(-2px)`,
          }}
        >
          <path
            d="M10 0 C10 8, 4 14, 2 22 C1 26, 0 28, 0 30"
            stroke="currentColor"
            strokeWidth="2.5"
            strokeLinecap="round"
            fill="none"
          />
        </svg>
      </span>
    </span>
  );
};

export default AtlasLogo;
