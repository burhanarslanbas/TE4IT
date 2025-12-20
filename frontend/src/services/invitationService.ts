/**
 * Invitation Service
 * Proje davetiyesi ile ilgili public API çağrıları
 * Token ile davetiye getirme ve kabul etme işlemleri
 */

import { apiClient } from './api';
import type { ProjectInvitationDetail } from '../types';

export class InvitationService {
  /**
   * Token ile davetiye detayını getir (Public endpoint)
   * @param token Davetiye token'ı
   * @returns Davetiye detayı
   */
  static async getInvitationByToken(token: string): Promise<ProjectInvitationDetail> {
    const response = await apiClient.get<ProjectInvitationDetail>(
      `/api/v1/projects/invitations/${token}`
    );
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Davetiye bulunamadı veya geçersiz');
  }

  /**
   * Davetiyeyi kabul et (Authenticated endpoint)
   * @param token Davetiye token'ı
   */
  static async acceptInvitation(token: string): Promise<void> {
    const response = await apiClient.post(
      `/api/v1/projects/invitations/${token}/accept`
    );
    
    if (!response.success) {
      throw new Error('Davetiye kabul edilemedi');
    }
  }
}
