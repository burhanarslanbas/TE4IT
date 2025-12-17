/**
 * Permission Utilities
 * JWT token'dan permission'ları okuma ve kontrol etme fonksiyonları
 */

/**
 * JWT token'dan permission'ları al
 */
export const getPermissionsFromToken = (): string[] => {
  try {
    const token = localStorage.getItem('te4it_token');
    if (!token) return [];

    // JWT token'ı decode et (basit decode, production'da jwt-decode kütüphanesi kullanılabilir)
    const base64Url = token.split('.')[1];
    if (!base64Url) return [];

    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );

    const decoded = JSON.parse(jsonPayload);
    return decoded.permission || decoded.permissions || [];
  } catch (error) {
    console.error('Error decoding token:', error);
    return [];
  }
};

/**
 * Belirli bir permission'a sahip olup olmadığını kontrol et
 */
export const hasPermission = (permission: string): boolean => {
  const permissions = getPermissionsFromToken();
  return permissions.includes(permission);
};

/**
 * Birden fazla permission'dan en az birine sahip olup olmadığını kontrol et
 */
export const hasAnyPermission = (permissions: string[]): boolean => {
  const userPermissions = getPermissionsFromToken();
  return permissions.some((perm) => userPermissions.includes(perm));
};

/**
 * Tüm permission'lara sahip olup olmadığını kontrol et
 */
export const hasAllPermissions = (permissions: string[]): boolean => {
  const userPermissions = getPermissionsFromToken();
  return permissions.every((perm) => userPermissions.includes(perm));
};

