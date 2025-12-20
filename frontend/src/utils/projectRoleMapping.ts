/**
 * Project Role Mapping Utilities
 * Backend numeric role değerleri ile frontend string enum arasında dönüşüm
 * Backend: Viewer=1, Member=2, Owner=5
 */

import { ProjectRole } from '../types';

/**
 * ProjectRole enum'u numeric değere çevir (backend API için)
 */
export function projectRoleToNumeric(role: ProjectRole): number {
  switch (role) {
    case ProjectRole.Viewer:
      return 1;
    case ProjectRole.Member:
      return 2;
    case ProjectRole.Owner:
      return 5;
    default:
      return 2; // Varsayılan olarak Member (normal üye)
  }
}

/**
 * Numeric değeri ProjectRole enum'una çevir (backend'den gelen response için)
 */
export function numericToProjectRole(numericRole: number): ProjectRole {
  switch (numericRole) {
    case 1:
      return ProjectRole.Viewer;
    case 2:
      return ProjectRole.Member;
    case 5:
      return ProjectRole.Owner;
    default:
      return ProjectRole.Member;
  }
}
