import * as React from "react";

const GoogleIcon = (props: any) => (
  <svg viewBox="0 0 24 24" width="20" height="20" fill="none" {...props}>
    <path d="M21.6 12.227c0-.72-.064-1.412-.182-2.075H12v3.934h5.464c-.236 1.28-.953 2.362-2.036 3.09v2.57h3.29c1.925-1.77 3.03-4.385 3.03-7.519z" fill="#4285F4" />
    <path d="M12 22c2.7 0 4.97-.9 6.63-2.45l-3.29-2.57c-.92.62-2.08.98-3.34.98-2.57 0-4.74-1.74-5.52-4.08H3.06v2.56C4.71 19.95 8.06 22 12 22z" fill="#34A853" />
    <path d="M6.48 13.86A6.997 6.997 0 0 1 6 12c0-.66.11-1.3.31-1.9V7.55H3.06A10.99 10.99 0 0 0 2 12c0 1.8.42 3.5 1.16 4.95l3.32-3.09z" fill="#FBBC05" />
    <path d="M12 6.5c1.47 0 2.8.5 3.84 1.48l2.88-2.88C16.97 3.5 14.7 2.5 12 2.5 8.06 2.5 4.71 4.55 3.06 7.45l3.52 2.67C7.26 8.25 9.43 6.5 12 6.5z" fill="#EA4335" />
  </svg>
);
const GitHubIcon = (props: any) => (
  <svg viewBox="0 0 24 24" width="20" height="20" fill="none" {...props}>
    <path fillRule="evenodd" clipRule="evenodd" d="M12 .5a12 12 0 00-3.79 23.39c.6.11.82-.26.82-.58 0-.29-.01-1.04-.02-2.04-3.34.73-4.04-1.61-4.04-1.61-.55-1.4-1.34-1.77-1.34-1.77-1.09-.75.08-.74.08-.74 1.2.09 1.83 1.24 1.83 1.24 1.07 1.83 2.8 1.3 3.48.99.11-.78.42-1.3.76-1.6-2.66-.3-5.47-1.33-5.47-5.93 0-1.31.47-2.38 1.24-3.22-.12-.3-.54-1.52.12-3.17 0 0 1.01-.32 3.3 1.23a11.5 11.5 0 016 0c2.28-1.55 3.29-1.23 3.29-1.23.66 1.65.24 2.87.12 3.17.77.84 1.24 1.91 1.24 3.22 0 4.61-2.81 5.62-5.49 5.92.43.37.82 1.1.82 2.22 0 1.6-.01 2.88-.01 3.27 0 .32.22.7.82.58A12 12 0 0012 .5z" fill="#000"/>
  </svg>
);

export default function SocialButtons({ onGoogle, onGithub }: { onGoogle?: () => void; onGithub?: () => void }) {
  return (
    <div className="flex gap-3">
      <button type="button" onClick={onGoogle} className="flex items-center gap-2 px-4 py-2 rounded-md border hover:bg-muted">
        <GoogleIcon />
        <span>Sign in with Google</span>
      </button>
      <button type="button" onClick={onGithub} className="flex items-center gap-2 px-4 py-2 rounded-md border hover:bg-muted">
        <GitHubIcon />
        <span>Sign in with GitHub</span>
      </button>
    </div>
  );
}
