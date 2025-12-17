/**
 * Security Utilities
 * XSS protection, input sanitization, etc.
 */

/**
 * HTML karakterlerini escape et (XSS koruması)
 */
export function escapeHtml(text: string): string {
  const map: Record<string, string> = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#039;',
  };
  return text.replace(/[&<>"']/g, (m) => map[m]);
}

/**
 * Kullanıcı girdisini temizle ve sanitize et
 */
export function sanitizeInput(input: string): string {
  if (typeof input !== 'string') {
    return '';
  }
  
  // Trim whitespace
  let sanitized = input.trim();
  
  // Remove null bytes
  sanitized = sanitized.replace(/\0/g, '');
  
  // Remove control characters (except newline and tab)
  sanitized = sanitized.replace(/[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]/g, '');
  
  return sanitized;
}

/**
 * URL'yi validate et
 */
export function isValidUrl(url: string): boolean {
  try {
    const urlObj = new URL(url);
    return urlObj.protocol === 'http:' || urlObj.protocol === 'https:';
  } catch {
    return false;
  }
}

/**
 * Email formatını validate et
 */
export function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

/**
 * SQL injection pattern'lerini kontrol et (basit koruma)
 */
export function containsSqlInjection(input: string): boolean {
  const sqlPatterns = [
    /(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE)\b)/i,
    /(--|#|\/\*|\*\/|;)/,
    /(\b(OR|AND)\s+\d+\s*=\s*\d+)/i,
    /(\b(UNION|JOIN)\b)/i,
  ];
  
  return sqlPatterns.some((pattern) => pattern.test(input));
}

/**
 * XSS pattern'lerini kontrol et
 */
export function containsXss(input: string): boolean {
  const xssPatterns = [
    /<script[^>]*>.*?<\/script>/gi,
    /<iframe[^>]*>.*?<\/iframe>/gi,
    /javascript:/gi,
    /on\w+\s*=/gi,
    /<img[^>]+src[^>]*>/gi,
  ];
  
  return xssPatterns.some((pattern) => pattern.test(input));
}

/**
 * Güvenli string validation
 */
export function validateSafeString(input: string, maxLength?: number): {
  isValid: boolean;
  sanitized: string;
  errors: string[];
} {
  const errors: string[] = [];
  let sanitized = sanitizeInput(input);

  if (maxLength && sanitized.length > maxLength) {
    errors.push(`Maksimum ${maxLength} karakter olmalıdır`);
    sanitized = sanitized.substring(0, maxLength);
  }

  if (containsXss(sanitized)) {
    errors.push('Güvenlik: Geçersiz karakterler tespit edildi');
    sanitized = escapeHtml(sanitized);
  }

  if (containsSqlInjection(sanitized)) {
    errors.push('Güvenlik: Geçersiz karakterler tespit edildi');
  }

  return {
    isValid: errors.length === 0,
    sanitized,
    errors,
  };
}

