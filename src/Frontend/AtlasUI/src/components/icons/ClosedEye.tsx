import React from 'react';

const ClosedEye: React.FC<React.SVGProps<SVGSVGElement>> = (props) => (
  <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" {...props}>
    <path d="M2 12s4-6 10-6 10 6 10 6-4 6-10 6S2 12 2 12z" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
    <path d="M6 10c1.5-1 3-1 6-1s4.5 0 6 1" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
    <g stroke="currentColor" strokeWidth="1.2" strokeLinecap="round">
      <path d="M8 7.2l-.8-1.6" />
      <path d="M11 6.6l-.6-1.4" />
      <path d="M13 6.6l.6-1.4" />
      <path d="M16 7.2l.8-1.6" />
    </g>
  </svg>
);

export default ClosedEye;
