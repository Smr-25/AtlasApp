import React from 'react';
import { toast } from '@/hooks/use-toast';

interface State {
  hasError: boolean;
  error: Error | null;
}

class ErrorBoundary extends React.Component<React.PropsWithChildren<{}>, State> {
  constructor(props: React.PropsWithChildren<{}>) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error) {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, info: any) {
    // Log to console and show a toast
    try {
      console.error('Unhandled error caught by ErrorBoundary:', error, info);
    } catch (e) {}
    try {
      toast({ title: 'Unexpected error', description: String(error.message || 'An unexpected error occurred.') });
    } catch (e) {}
  }

  handleRetry = () => {
    this.setState({ hasError: false, error: null });
    // reload the page as a simple retry fallback
    window.location.reload();
  };

  render() {
    if (this.state.hasError) {
      return (
        <div className="min-h-screen flex items-center justify-center bg-slate-50">
          <div className="max-w-lg p-6 bg-white rounded shadow">
            <h2 className="text-xl font-semibold mb-2">Bir xəta baş verdi</h2>
            <p className="mb-4">Təkrar cəhd edin və ya səhifəni yeniləyin. Əlavə məlumat üçün konsolu yoxlayın.</p>
            <div className="flex gap-2">
              <button onClick={this.handleRetry} className="btn btn-primary">Yenidən cəhd et</button>
              <button onClick={() => { window.location.href = '/'; }} className="btn">Əsas səhifəyə qayıt</button>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary;
