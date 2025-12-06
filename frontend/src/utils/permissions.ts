/**
 * Permission Helper Fonksiyonları
 * JWT token'dan permission'ları alıp kontrol eder
 */

/**
 * JWT token'dan permission'ları çıkar
 */
function getPermissionsFromToken(): string[] {
  try {
    const token = localStorage.getItem('te4it_token');
    if (!token) return [];

    // JWT token'ı decode et (basit decode, production'da library kullanılmalı)
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
    return decoded.permission || [];
  } catch (error) {
    console.error('Token decode hatası:', error);
    return [];
  }
}

/**
 * Kullanıcının belirtilen permission'a sahip olup olmadığını kontrol et
 */
export function hasPermission(permission: string): boolean {
  const permissions = getPermissionsFromToken();
  return permissions.includes(permission);
}

/**
 * Kullanıcının belirtilen permission'lardan en az birine sahip olup olmadığını kontrol et
 */
export function hasAnyPermission(permissions: string[]): boolean {
  const userPermissions = getPermissionsFromToken();
  return permissions.some((perm) => userPermissions.includes(perm));
}

/**
 * Kullanıcının belirtilen tüm permission'lara sahip olup olmadığını kontrol et
 */
export function hasAllPermissions(permissions: string[]): boolean {
  const userPermissions = getPermissionsFromToken();
  return permissions.every((perm) => userPermissions.includes(perm));
}

/**
 * Permission constant'ları
 */
export const PERMISSIONS = {
  // Project permissions
  PROJECT_CREATE: 'ProjectCreate',
  PROJECT_READ: 'ProjectRead',
  PROJECT_UPDATE: 'ProjectUpdate',
  PROJECT_DELETE: 'ProjectDelete',
  
  // Module permissions
  MODULE_CREATE: 'ModuleCreate',
  MODULE_READ: 'ModuleRead',
  MODULE_UPDATE: 'ModuleUpdate',
  MODULE_DELETE: 'ModuleDelete',
  
  // UseCase permissions
  USECASE_CREATE: 'UseCaseCreate',
  USECASE_READ: 'UseCaseRead',
  USECASE_UPDATE: 'UseCaseUpdate',
  USECASE_DELETE: 'UseCaseDelete',
  
  // Task permissions
  TASK_CREATE: 'TaskCreate',
  TASK_READ: 'TaskRead',
  TASK_UPDATE: 'TaskUpdate',
  TASK_ASSIGN: 'TaskAssign',
  TASK_STATE_CHANGE: 'TaskStateChange',
  TASK_DELETE: 'TaskDelete',
  
  // TaskRelation permissions
  TASK_RELATION_CREATE: 'TaskRelationCreate',
  TASK_RELATION_DELETE: 'TaskRelationDelete',
} as const;

