/**
 * User Service
 * Kullanıcı listesi ve arama işlemleri
 */

import { apiClient } from './api';
import type { UserResponse, PaginationResponse } from '../types';

interface BackendUserListResponse {
  items: UserResponse[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export class UserService {
  /**
   * Tüm kullanıcıları sayfalı olarak getir
   * @param page Sayfa numarası (varsayılan: 1)
   * @param pageSize Sayfa boyutu (varsayılan: 50)
   * @returns Kullanıcı listesi
   */
  static async getUsers(
    page: number = 1,
    pageSize: number = 50
  ): Promise<PaginationResponse<UserResponse>> {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());

    const response = await apiClient.get<BackendUserListResponse>(
      `/api/v1/users?${params.toString()}`
    );

    if (response.success && response.data) {
      return {
        items: response.data.items,
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
      };
    }

    throw new Error('Kullanıcılar yüklenemedi');
  }

  /**
   * Kullanıcıları email veya userName'e göre filtrele (client-side)
   * @param users Kullanıcı listesi
   * @param searchTerm Arama terimi
   * @returns Filtrelenmiş kullanıcı listesi
   */
  static filterUsers(users: UserResponse[], searchTerm: string): UserResponse[] {
    if (!searchTerm.trim()) {
      return users;
    }

    const term = searchTerm.toLowerCase().trim();
    return users.filter(
      (user) =>
        user.email.toLowerCase().includes(term) ||
        user.userName.toLowerCase().includes(term)
    );
  }
}
