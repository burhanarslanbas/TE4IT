/**
 * Token Manager
 * Güvenli token yönetimi için utility
 */

const TOKEN_KEY = 'te4it_token';
const REFRESH_TOKEN_KEY = 'te4it_refresh_token';
const TOKEN_EXPIRY_KEY = 'te4it_token_expiry';

/**
 * Token'ı sessionStorage'a kaydet (localStorage'dan daha güvenli)
 */
export function saveToken(token: string, expiresAt?: string): void {
  try {
    // sessionStorage kullan (XSS'e karşı daha güvenli)
    sessionStorage.setItem(TOKEN_KEY, token);
    
    if (expiresAt) {
      sessionStorage.setItem(TOKEN_EXPIRY_KEY, expiresAt);
    } else {
      // JWT token'dan expiry'yi çıkar
      const expiry = getTokenExpiry(token);
      if (expiry) {
        sessionStorage.setItem(TOKEN_EXPIRY_KEY, expiry.toISOString());
      }
    }
  } catch (error) {
    console.error('Token kaydedilemedi:', error);
  }
}

/**
 * Refresh token'ı kaydet
 */
export function saveRefreshToken(refreshToken: string): void {
  try {
    sessionStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  } catch (error) {
    console.error('Refresh token kaydedilemedi:', error);
  }
}

/**
 * Token'ı oku
 */
export function getToken(): string | null {
  try {
    return sessionStorage.getItem(TOKEN_KEY);
  } catch (error) {
    console.error('Token okunamadı:', error);
    return null;
  }
}

/**
 * Refresh token'ı oku
 */
export function getRefreshToken(): string | null {
  try {
    return sessionStorage.getItem(REFRESH_TOKEN_KEY);
  } catch (error) {
    console.error('Refresh token okunamadı:', error);
    return null;
  }
}

/**
 * Token'ları temizle
 */
export function clearTokens(): void {
  try {
    sessionStorage.removeItem(TOKEN_KEY);
    sessionStorage.removeItem(REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(TOKEN_EXPIRY_KEY);
  } catch (error) {
    console.error('Token temizlenemedi:', error);
  }
}

/**
 * JWT token'dan expiry date'i çıkar
 */
export function getTokenExpiry(token: string): Date | null {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) {
      return null;
    }

    const payload = JSON.parse(atob(parts[1]));
    if (payload.exp) {
      return new Date(payload.exp * 1000);
    }
    return null;
  } catch (error) {
    console.error('Token expiry çıkarılamadı:', error);
    return null;
  }
}

/**
 * Token'ın süresi dolmuş mu?
 */
export function isTokenExpired(token?: string | null): boolean {
  const tokenToCheck = token || getToken();
  
  if (!tokenToCheck) {
    return true;
  }

  try {
    // Önce sessionStorage'dan expiry'yi kontrol et
    const storedExpiry = sessionStorage.getItem(TOKEN_EXPIRY_KEY);
    if (storedExpiry) {
      const expiryDate = new Date(storedExpiry);
      if (expiryDate < new Date()) {
        return true;
      }
    }

    // Token'dan expiry'yi çıkar
    const expiry = getTokenExpiry(tokenToCheck);
    if (!expiry) {
      return true;
    }

    // 5 dakika buffer ekle (token yenileme için)
    const bufferTime = 5 * 60 * 1000; // 5 dakika
    return expiry.getTime() - bufferTime < Date.now();
  } catch (error) {
    console.error('Token expiry kontrolü hatası:', error);
    return true;
  }
}

/**
 * Token'ın ne kadar süre sonra dolacağını hesapla (milisaniye)
 */
export function getTokenExpiryTime(token?: string | null): number | null {
  const tokenToCheck = token || getToken();
  
  if (!tokenToCheck) {
    return null;
  }

  const expiry = getTokenExpiry(tokenToCheck);
  if (!expiry) {
    return null;
  }

  return expiry.getTime() - Date.now();
}

/**
 * Token geçerli mi? (var mı ve süresi dolmamış mı?)
 */
export function isTokenValid(): boolean {
  const token = getToken();
  return !!token && !isTokenExpired(token);
}

