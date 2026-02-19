import React from 'react';

const TelegramIcon: React.FC<{ className?: string }> = ({ className }) => (
  <svg className={className} viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden>
    <path d="M21.81 3.73c-.18-.6-.63-1.06-1.23-1.25C19.62 2.2 12 4.8 12 4.8s-7.62-2.6-8.58-2.32c-.6.19-1.06.64-1.25 1.24C1.44 7.12 2 14.58 2 14.58s-.02.39.07.96c.11.71.47 1.6 1.55 1.77 1.86.3 7.46-2.96 8.38-3.74.1-.08.23-.12.36-.12.13 0 .26.04.36.12.92.78 6.52 4.04 8.38 3.74 1.08-.17 1.44-1.06 1.55-1.77.09-.57.07-.96.07-.96s.56-7.46-2.16-10.85z" fill="currentColor"/>
  </svg>
);

export default TelegramIcon;

