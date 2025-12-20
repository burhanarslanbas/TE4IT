/**
 * Roadmap Service
 * AI Roadmap API entegrasyonu
 */

import type { RoadmapResponse, RoadmapGenerationRequest } from '../types/roadmap';

const AI_API_BASE_URL = 'https://multi-source-search-api.onrender.com';

/**
 * Timeout ile fetch wrapper
 */
async function fetchWithTimeout(
  url: string,
  options: RequestInit,
  timeout: number = 60000
): Promise<Response> {
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), timeout);

  try {
    const response = await fetch(url, {
      ...options,
      signal: controller.signal,
    });
    clearTimeout(timeoutId);
    return response;
  } catch (error: any) {
    clearTimeout(timeoutId);
    if (error.name === 'AbortError') {
      throw new Error('İstek zaman aşımına uğradı. Lütfen tekrar deneyin.');
    }
    throw error;
  }
}

export class RoadmapService {
  /**
   * AI ile roadmap oluştur
   * @param data Proje bilgileri ve repo sayısı
   * @returns Roadmap verisi
   */
  static async generateRoadmap(data: RoadmapGenerationRequest): Promise<RoadmapResponse> {
    try {
      console.log('[ROADMAP SERVICE] Generating roadmap with data:', data);

      const response = await fetchWithTimeout(
        `${AI_API_BASE_URL}/plan/from-search`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            title: data.title,
            description: data.description,
            max_similar_projects: data.max_similar_projects,
          }),
        },
        60000 // 60 saniye timeout
      );

      if (!response.ok) {
        // HTTP hata durumları
        const status = response.status;
        let errorMessage = 'Roadmap oluşturulamadı';

        try {
          const errorData = await response.json();
          errorMessage = errorData?.message || errorData?.error || errorMessage;
        } catch {
          // JSON parse edilemezse response text'ini kullan
          const text = await response.text();
          errorMessage = text || errorMessage;
        }

        if (status === 400) {
          throw new Error(`Geçersiz istek: ${errorMessage}`);
        } else if (status === 500) {
          throw new Error('Sunucu hatası. Lütfen daha sonra tekrar deneyin.');
        } else {
          throw new Error(errorMessage);
        }
      }

      const result: RoadmapResponse = await response.json();

      console.log('[ROADMAP SERVICE] Roadmap generated successfully:', result);

      return result;
    } catch (error: any) {
      console.error('[ROADMAP SERVICE] Error generating roadmap:', error);

      // Zaten Error instance'ı ise direkt fırlat
      if (error instanceof Error) {
        throw error;
      }

      // Network hatası
      if (error.message?.includes('fetch') || error.message?.includes('network')) {
        throw new Error('Sunucuya bağlanılamadı. İnternet bağlantınızı kontrol edin.');
      }

      // Diğer hatalar
      throw new Error(error.message || 'Roadmap oluşturulurken bir hata oluştu.');
    }
  }
}
