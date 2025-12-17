/**
 * Protected Route Component
 * Kullanıcının authenticated olması gereken sayfalar için
 */

import React, { useState, useEffect } from 'react';
import { Navigate } from 'react-router-dom';
import { AuthService } from '../services/auth';
import { toast } from 'sonner';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
}) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const checkAuthStatus = () => {
      try {
        if (AuthService.isAuthenticated()) {
          setIsAuthenticated(true);
        } else {
          setIsAuthenticated(false);
        }
      } catch (error) {
        console.error('Auth check error:', error);
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };

    checkAuthStatus();
  }, []);

  if (isLoading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-2 border-[#8B5CF6] border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-[#9CA3AF]">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    toast.error('Giriş yapmalısınız', {
      description: 'Bu sayfaya erişmek için lütfen giriş yapın.',
      duration: 3000,
    });
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

