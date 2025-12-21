/**
 * useAuth Hook
 * Authentication yönetimi için custom hook
 */
import { useState, useEffect, useCallback } from 'react';
import { AuthService } from '../services/auth';
import type { User } from '../services/auth';
import { ApiError } from '../core/errors/ApiError';
import { isTokenValid } from '../utils/tokenManager';

interface UseAuthReturn {
  isAuthenticated: boolean;
  isLoading: boolean;
  user: User | null;
  checkAuth: () => Promise<void>;
}

export function useAuth(): UseAuthReturn {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [user, setUser] = useState<User | null>(null);

  const checkAuth = useCallback(async () => {
    try {
      setIsLoading(true);
      
      // Önce token'ı kontrol et
      if (!isTokenValid()) {
        setIsAuthenticated(false);
        setUser(null);
        return;
      }

      // Token geçerliyse kullanıcı bilgilerini getir
      try {
        const userData = await AuthService.getCurrentUser();
        setIsAuthenticated(true);
        setUser(userData);
      } catch (error) {
        // Kullanıcı bilgileri alınamazsa token geçersiz demektir
        if (error instanceof ApiError && error.isAuthenticationError()) {
          setIsAuthenticated(false);
          setUser(null);
        }
      }
    } catch (error) {
      setIsAuthenticated(false);
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    checkAuth();
  }, [checkAuth]);

  return {
    isAuthenticated,
    isLoading,
    user,
    checkAuth,
  };
}

