/**
 * Permission Utilities
 * JWT token'dan permission'ları okuma ve kontrol etme fonksiyonları
 */

/**
 * Permission constant'ları
 */
export const PERMISSIONS = {
  PROJECT_CREATE: 'ProjectCreate',
  PROJECT_UPDATE: 'ProjectUpdate',
  PROJECT_DELETE: 'ProjectDelete',
  MODULE_CREATE: 'ModuleCreate',
  MODULE_UPDATE: 'ModuleUpdate',
  MODULE_DELETE: 'ModuleDelete',
  USECASE_CREATE: 'UseCaseCreate',
  USECASE_UPDATE: 'UseCaseUpdate',
  USECASE_DELETE: 'UseCaseDelete',
  TASK_CREATE: 'TaskCreate',
  TASK_UPDATE: 'TaskUpdate',
  TASK_DELETE: 'TaskDelete',
  // Education permissions
  EDUCATION_COURSE_CREATE: 'Education.Course.Create',
  EDUCATION_COURSE_UPDATE: 'Education.Course.Update',
  EDUCATION_COURSE_DELETE: 'Education.Course.Delete',
  EDUCATION_ROADMAP_CREATE: 'Education.Roadmap.Create',
  EDUCATION_ROADMAP_UPDATE: 'Education.Roadmap.Update',
} as const;

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
 * Token'dan kullanıcı rolünü al
 */
export const getUserRoleFromToken = (): string | null => {
  try {
    const token = localStorage.getItem('te4it_token');
    if (!token) return null;

    const base64Url = token.split('.')[1];
    if (!base64Url) return null;

    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );

    const decoded = JSON.parse(jsonPayload);
    // Role farklı field'larda olabilir
    return decoded.role || decoded.roles || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
  } catch (error) {
    console.error('Error decoding token for role:', error);
    return null;
  }
};

/**
 * Admin veya OrganizationManager rolüne sahip olup olmadığını kontrol et
 */
export const isAdminOrManager = (): boolean => {
  const role = getUserRoleFromToken();
  if (!role) return false;
  
  const roleLower = role.toLowerCase();
  return roleLower === 'administrator' || 
         roleLower === 'admin' || 
         roleLower === 'organizationmanager' ||
         roleLower === 'organization manager';
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

