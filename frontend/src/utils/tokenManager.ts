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

/**
 * Refresh token'ın expiry date'ini çıkar
 */
export function getRefreshTokenExpiry(refreshToken?: string | null): Date | null {
  const tokenToCheck = refreshToken || getRefreshToken();
  
  if (!tokenToCheck) {
    return null;
  }

  return getTokenExpiry(tokenToCheck);
}

/**
 * Refresh token'ın süresi dolmuş mu?
 */
export function isRefreshTokenExpired(refreshToken?: string | null): boolean {
  const expiry = getRefreshTokenExpiry(refreshToken);
  
  if (!expiry) {
    return true;
  }

  return expiry < new Date();
}

/**
 * Token'ın yenilenmesi gerekiyor mu? (Token'ın %80'i dolmuş mu?)
 * Bu, token expire olmadan önce proaktif olarak yenilemek için kullanılır
 */
export function shouldRefreshToken(token?: string | null): boolean {
  const tokenToCheck = token || getToken();
  
  if (!tokenToCheck) {
    return false;
  }

  try {
    const expiry = getTokenExpiry(tokenToCheck);
    if (!expiry) {
      return false;
    }

    const now = Date.now();
    const expiryTime = expiry.getTime();
    const timeUntilExpiry = expiryTime - now;
    
    // Token zaten expire olmuşsa yenile
    if (timeUntilExpiry <= 0) {
      return true;
    }
    
    // Token'ın kalan süresinin %20'sinden az kaldıysa yenile
    // Örnek: Token 1 saat geçerliyse (3600 saniye), 12 dakika (720 saniye) kaldığında yenile
    // Bu, token'ın %80'i dolmuş demektir
    const refreshThreshold = 0.2; // %20 kaldığında yenile
    
    // Token'ın toplam ömrünü tahmin etmek için, token'ın ne zaman oluşturulduğunu bilmemiz gerekir
    // JWT token'da genellikle 'iat' (issued at) claim'i vardır
    const parts = tokenToCheck.split('.');
    if (parts.length !== 3) {
      return false;
    }

    try {
      const payload = JSON.parse(atob(parts[1]));
      const issuedAt = payload.iat ? payload.iat * 1000 : null;
      
      if (issuedAt) {
        // Token'ın toplam ömrü
        const totalLifetime = expiryTime - issuedAt;
        // Kalan süre
        const remainingTime = timeUntilExpiry;
        
        // Kalan süre, toplam ömrün %20'sinden azsa yenile
        return remainingTime < (totalLifetime * refreshThreshold);
      } else {
        // iat yoksa, basit bir yaklaşım: 5 dakikadan az kaldıysa yenile
        const fiveMinutes = 5 * 60 * 1000;
        return timeUntilExpiry < fiveMinutes;
      }
    } catch (parseError) {
      // Parse hatası, basit kontrol yap
      const fiveMinutes = 5 * 60 * 1000;
      return timeUntilExpiry < fiveMinutes;
    }
  } catch (error) {
    console.error('Token refresh kontrolü hatası:', error);
    return false;
  }
}

/**
 * Token'ın ne kadar süre sonra yenilenmesi gerektiğini hesapla (milisaniye)
 * Token'ın %80'i dolduğunda yenileme zamanı gelir
 */
export function getTimeUntilRefresh(token?: string | null): number | null {
  const tokenToCheck = token || getToken();
  
  if (!tokenToCheck) {
    return null;
  }

  try {
    const expiry = getTokenExpiry(tokenToCheck);
    if (!expiry) {
      return null;
    }

    const now = Date.now();
    const expiryTime = expiry.getTime();
    const timeUntilExpiry = expiryTime - now;
    
    if (timeUntilExpiry <= 0) {
      return 0; // Hemen yenile
    }
    
    // Token'ın toplam ömrünü bulmak için iat claim'ini kontrol et
    const parts = tokenToCheck.split('.');
    if (parts.length !== 3) {
      return null;
    }

    try {
      const payload = JSON.parse(atob(parts[1]));
      const issuedAt = payload.iat ? payload.iat * 1000 : null;
      
      if (issuedAt) {
        // Token'ın toplam ömrü
        const totalLifetime = expiryTime - issuedAt;
        // Token'ın %80'i dolduğunda yenile (kalan sürenin %20'si kaldığında)
        const refreshThreshold = 0.2;
        const refreshTime = totalLifetime * refreshThreshold;
        
        // Şu anki zamandan itibaren ne kadar süre sonra yenileme zamanı gelir?
        const timeSinceIssued = now - issuedAt;
        const timeUntilRefresh = (totalLifetime * (1 - refreshThreshold)) - timeSinceIssued;
        
        return Math.max(0, timeUntilRefresh);
      } else {
        // iat yoksa, basit yaklaşım: Kalan sürenin %20'si kaldığında yenile
        const refreshThreshold = 0.2;
        return Math.max(0, timeUntilExpiry * refreshThreshold);
      }
    } catch (parseError) {
      // Parse hatası, basit kontrol yap
      const refreshThreshold = 0.2;
      return Math.max(0, timeUntilExpiry * refreshThreshold);
    }
  } catch (error) {
    console.error('Token refresh zamanı hesaplama hatası:', error);
    return null;
  }
}

