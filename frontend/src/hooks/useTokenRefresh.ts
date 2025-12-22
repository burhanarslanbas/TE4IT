/**
 * Token Refresh Hook
 * Access token'ı proaktif olarak yenilemek için kullanılır
 */

import { useEffect, useRef } from 'react';
import { 
  getToken, 
  getRefreshToken, 
  shouldRefreshToken, 
  getTimeUntilRefresh,
  isRefreshTokenExpired 
} from '../utils/tokenManager';
import { AuthService } from '../services/auth';
import { apiClient } from '../services/api';

interface UseTokenRefreshOptions {
  /**
   * Token yenileme başarısız olduğunda çağrılacak callback
   */
  onRefreshFailed?: () => void;
  
  /**
   * Token yenileme başarılı olduğunda çağrılacak callback
   */
  onRefreshSuccess?: () => void;
  
  /**
   * Otomatik yenileme aktif mi? (default: true)
   */
  enabled?: boolean;
}

/**
 * Token'ı proaktif olarak yenilemek için hook
 * Token'ın %80'i dolduğunda otomatik olarak yeniler
 */
export function useTokenRefresh(options: UseTokenRefreshOptions = {}) {
  const {
    onRefreshFailed,
    onRefreshSuccess,
    enabled = true,
  } = options;

  const intervalRef = useRef<NodeJS.Timeout | null>(null);
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);
  const isRefreshingRef = useRef(false);

  /**
   * Token'ı yenile
   */
  const refreshToken = async () => {
    // Zaten refresh işlemi devam ediyorsa, tekrar başlatma
    if (isRefreshingRef.current) {
      return;
    }

    // Token yoksa veya refresh token yoksa, işlem yapma
    const token = getToken();
    const refreshTokenValue = getRefreshToken();

    if (!token || !refreshTokenValue) {
      return;
    }

    // Refresh token expire olmuşsa, logout yap
    if (isRefreshTokenExpired(refreshTokenValue)) {
      console.warn('Refresh token süresi dolmuş, logout yapılıyor');
      if (onRefreshFailed) {
        onRefreshFailed();
      }
      return;
    }

    // Token yenilenmesi gerekiyor mu?
    if (!shouldRefreshToken(token)) {
      return;
    }

    try {
      isRefreshingRef.current = true;
      console.log('Token yenileniyor...');

      // Token'ı yenile
      const response = await AuthService.refreshAccessToken(refreshTokenValue);
      
      // Yeni token'ları kaydet
      apiClient.setToken(response.accessToken, response.expiresAt);

      console.log('Token başarıyla yenilendi');
      
      if (onRefreshSuccess) {
        onRefreshSuccess();
      }
    } catch (error) {
      console.error('Token yenileme hatası:', error);
      
      // Refresh token expire olmuşsa veya geçersizse, logout yap
      if (onRefreshFailed) {
        onRefreshFailed();
      }
    } finally {
      isRefreshingRef.current = false;
    }
  };

  /**
   * Token yenileme zamanını hesapla ve interval kur
   */
  const setupTokenRefresh = () => {
    // Önceki interval ve timeout'ları temizle
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }

    if (!enabled) {
      return;
    }

    const token = getToken();
    if (!token) {
      return;
    }

    // Token yenilenmesi gerekiyor mu?
    if (shouldRefreshToken(token)) {
      // Hemen yenile
      refreshToken();
      return;
    }

    // Token'ın ne kadar süre sonra yenilenmesi gerektiğini hesapla
    const timeUntilRefresh = getTimeUntilRefresh(token);
    
    if (timeUntilRefresh === null || timeUntilRefresh <= 0) {
      return;
    }

    // Token yenileme zamanı geldiğinde yenile
    timeoutRef.current = setTimeout(() => {
      refreshToken();
      // Yenileme sonrası interval'i tekrar kur
      setupTokenRefresh();
    }, timeUntilRefresh) as any;

    // Ayrıca her 30 saniyede bir kontrol et (güvenlik için)
    intervalRef.current = setInterval(() => {
      const currentToken = getToken();
      if (currentToken && shouldRefreshToken(currentToken)) {
        refreshToken();
        setupTokenRefresh(); // Interval'i yeniden kur
      }
    }, 30 * 1000) as any; // 30 saniye
  };

  useEffect(() => {
    if (!enabled) {
      return;
    }

    // İlk kontrol
    setupTokenRefresh();

    // Component unmount olduğunda interval ve timeout'ları temizle
    return () => {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
        timeoutRef.current = null;
      }
    };
  }, [enabled]);

  return {
    refreshToken,
  };
}

