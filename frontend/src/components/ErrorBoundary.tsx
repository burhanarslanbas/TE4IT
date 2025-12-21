/**
 * Global Error Boundary Component
 * Uygulama genelinde hataları yakalar ve kullanıcıya gösterir
 */
import React, { Component, ErrorInfo, ReactNode } from 'react';
import { Button } from './ui/button';
import { AlertTriangle, RefreshCw, Home } from 'lucide-react';
import { ApiError } from '../core/errors/ApiError';

interface Props {
  children: ReactNode;
  fallback?: ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
  errorInfo: ErrorInfo | null;
}

export class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = {
      hasError: false,
      error: null,
      errorInfo: null,
    };
  }

  static getDerivedStateFromError(error: Error): State {
    return {
      hasError: true,
      error,
      errorInfo: null,
    };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    // Log error to console in development
    if (import.meta.env.DEV) {
      console.error('ErrorBoundary caught an error:', error, errorInfo);
    }

    // TODO: Log to error tracking service (Sentry, etc.)
    this.setState({
      error,
      errorInfo,
    });
  }

  handleReset = () => {
    this.setState({
      hasError: false,
      error: null,
      errorInfo: null,
    });
  };

  handleGoHome = () => {
    window.location.href = '/';
  };

  render() {
    if (this.state.hasError) {
      // Custom fallback UI
      if (this.props.fallback) {
        return this.props.fallback;
      }

      const { error } = this.state;
      const isApiError = error instanceof ApiError;

      return (
        <div className="min-h-screen bg-[#0D1117] flex items-center justify-center px-4">
          <div className="max-w-2xl w-full">
            <div className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8 shadow-lg">
              {/* Error Icon */}
              <div className="flex justify-center mb-6">
                <div className="w-20 h-20 bg-[#EF4444]/10 rounded-full flex items-center justify-center">
                  <AlertTriangle className="w-10 h-10 text-[#EF4444]" />
                </div>
              </div>

              {/* Error Title */}
              <h1 className="text-2xl font-bold text-[#E5E7EB] text-center mb-4">
                Bir Hata Oluştu
              </h1>

              {/* Error Message */}
              <div className="bg-[#0D1117]/60 rounded-xl p-4 mb-6 border border-[#30363D]/30">
                <p className="text-[#E5E7EB] text-center mb-2">
                  {isApiError && error.status === 401
                    ? 'Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.'
                    : isApiError && error.status === 403
                    ? 'Bu işlem için yetkiniz bulunmamaktadır.'
                    : error?.message || 'Beklenmeyen bir hata oluştu'}
                </p>
                
                {isApiError && error.errors && error.errors.length > 0 && (
                  <ul className="mt-3 space-y-1">
                    {error.errors.map((err, index) => (
                      <li key={index} className="text-sm text-[#9CA3AF]">
                        • {err}
                      </li>
                    ))}
                  </ul>
                )}

                {/* Development: Show full error */}
                {import.meta.env.DEV && error && (
                  <details className="mt-4">
                    <summary className="text-sm text-[#6B7280] cursor-pointer hover:text-[#9CA3AF]">
                      Teknik Detaylar (Development)
                    </summary>
                    <pre className="mt-2 text-xs text-[#9CA3AF] bg-[#0D1117] p-3 rounded overflow-auto max-h-40">
                      {error.stack}
                    </pre>
                  </details>
                )}
              </div>

              {/* Action Buttons */}
              <div className="flex flex-col sm:flex-row gap-3 justify-center">
                {isApiError && error.status === 401 ? (
                  <Button
                    onClick={() => {
                      window.location.href = '/login';
                    }}
                    className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9]"
                  >
                    Giriş Sayfasına Git
                  </Button>
                ) : (
                  <>
                    <Button
                      onClick={this.handleReset}
                      variant="outline"
                      className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                    >
                      <RefreshCw className="w-4 h-4 mr-2" />
                      Tekrar Dene
                    </Button>
                    <Button
                      onClick={this.handleGoHome}
                      className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9]"
                    >
                      <Home className="w-4 h-4 mr-2" />
                      Ana Sayfaya Dön
                    </Button>
                  </>
                )}
              </div>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}

