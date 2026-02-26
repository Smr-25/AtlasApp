import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const Welcome: React.FC = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const t = setTimeout(() => {
      navigate('/login');
    }, 2000);
    return () => clearTimeout(t);
  }, [navigate]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-sky-500 to-indigo-700 text-white">
      <div className="text-center p-8">
        <div className="mb-6">
          {/* Simple animated logo: rotating globe */}
          <svg width="120" height="120" viewBox="0 0 24 24" className="mx-auto animate-spin-slow">
            <circle cx="12" cy="12" r="10" stroke="white" strokeWidth="0.5" fill="none" opacity="0.4" />
            <path d="M2 12a10 10 0 0020 0 10 10 0 00-20 0z" stroke="white" strokeWidth="0.8" fill="none" />
            <path d="M4 6c3 1.5 6 1.5 8 0s5-1.5 8 0" stroke="white" strokeWidth="0.9" fill="none" opacity="0.85" />
          </svg>
        </div>
        <h1 className="text-4xl font-bold mb-2">Welcome Atlas</h1>
        <p className="opacity-90 mb-4">Gözəl idarəetmə üçün güclü görünüş və zəka.</p>
        <div className="text-sm opacity-80">Girişə yönləndirilirsiniz...</div>
      </div>
    </div>
  );
};

export default Welcome;

