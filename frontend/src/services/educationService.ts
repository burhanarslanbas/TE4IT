/**
 * Education Service
 * Eğitim modülü ile ilgili tüm API çağrıları
 */

import { apiClient, ApiResponse } from './api';
import { ApiError } from './api';
import type {
  Course,
  CourseListItem,
  CourseListResponse,
  CourseDetailResponse,
  Enrollment,
  EnrollmentResponse,
  UserEnrollmentsResponse,
  EnrollmentWithCourse,
  CourseProgressResponse,
  ProgressDashboardResponse,
  GetCoursesRequest,
  EnrollInCourseRequest,
  CompleteContentRequest,
  UpdateVideoProgressRequest,
  GetUserEnrollmentsRequest,
  CreateCourseRequest,
  CreateCourseResponse,
  CreateRoadmapRequest,
} from '../types/education';

export class EducationService {
  /**
   * Tüm aktif kursları listele
   */
  static async getCourses(params?: GetCoursesRequest): Promise<CourseListResponse> {
    const queryParams = new URLSearchParams();
    
    if (params?.page) queryParams.append('page', params.page.toString());
    if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
    if (params?.search) queryParams.append('search', params.search);
    if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
    if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

    const queryString = queryParams.toString();
    const endpoint = `/api/v1/education/courses${queryString ? `?${queryString}` : ''}`;
    
    const response = await apiClient.get<CourseListResponse>(endpoint);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error(response.message || 'Kurslar yüklenemedi');
  }

  /**
   * Kurs detayını getir
   */
  static async getCourseById(courseId: string): Promise<CourseDetailResponse> {
    const endpoint = `/api/v1/education/courses/${courseId}`;
    
    const response = await apiClient.get<CourseDetailResponse>(endpoint);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error(response.message || 'Kurs detayı yüklenemedi');
  }

  /**
   * Kurs için roadmap getir
   */
  static async getRoadmapByCourseId(courseId: string): Promise<CourseDetailResponse['roadmap']> {
    const endpoint = `/api/v1/education/courses/${courseId}/roadmap`;
    
    const response = await apiClient.get<{ roadmap: CourseDetailResponse['roadmap'] }>(endpoint);
    
    if (response.success && response.data) {
      return response.data.roadmap;
    }
    
    throw new Error(response.message || 'Roadmap yüklenemedi');
  }

  /**
   * Kursa kayıt ol
   */
  static async enrollInCourse(courseId: string): Promise<EnrollmentResponse> {
    const endpoint = `/api/v1/education/courses/${courseId}/enroll`;
    
    try {
      const response = await apiClient.post<EnrollmentResponse>(
        endpoint,
        {}
      );
      
      if (response.success && response.data) {
        return response.data;
      }
      
      throw new Error(response.message || 'Kursa kayıt olunamadı');
    } catch (error: any) {
      // 400 Bad Request - Muhtemelen zaten kayıtlı
      if (error instanceof ApiError && error.status === 400) {
        // Zaten kayıtlı olabilir, kayıtlı kursları kontrol et
        try {
          const enrollments = await this.getUserEnrollments();
          const existingEnrollment = enrollments.items.find(
            item => item.course?.id === courseId
          );
          
          if (existingEnrollment) {
            // Zaten kayıtlı, mevcut enrollment'ı döndür
            return {
              id: existingEnrollment.enrollment.id,
              courseId: existingEnrollment.enrollment.courseId,
              userId: existingEnrollment.enrollment.userId,
              enrolledAt: existingEnrollment.enrollment.enrolledAt,
              startedAt: existingEnrollment.enrollment.startedAt,
              completedAt: existingEnrollment.enrollment.completedAt,
            };
          }
        } catch (checkError) {
          // Enrollment kontrolü başarısız, orijinal hatayı fırlat
        }
        
        // 400 hatası ama kayıt bulunamadı, orijinal hatayı fırlat
        throw new ApiError(
          error.message || 'Kursa kayıt olunamadı. Zaten kayıtlı olabilirsiniz.',
          400
        );
      }
      
      // Diğer hatalar
      throw error;
    }
  }

  /**
   * Kullanıcının kayıtlarını getir
   */
  static async getUserEnrollments(params?: GetUserEnrollmentsRequest): Promise<UserEnrollmentsResponse> {
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:141',message:'getUserEnrollments ENTRY',data:{params},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'A'})}).catch(()=>{});
    // #endregion
    
    const queryParams = new URLSearchParams();
    
    if (params?.status) queryParams.append('status', params.status);

    const queryString = queryParams.toString();
    const endpoint = `/api/v1/education/enrollments${queryString ? `?${queryString}` : ''}`;
    
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:148',message:'getUserEnrollments BEFORE API call',data:{endpoint},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'D'})}).catch(()=>{});
    // #endregion
    
    try {
      const response = await apiClient.get<any>(endpoint);
      
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:152',message:'getUserEnrollments AFTER API call',data:{success:response.success,hasData:!!response.data,dataType:typeof response.data,dataIsArray:Array.isArray(response.data),dataKeys:response.data?Object.keys(response.data):null,dataValue:JSON.stringify(response.data).substring(0,500)},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'A,B,C,D'})}).catch(()=>{});
      // #endregion
      
      if (response.success && response.data) {
        // #region agent log
        fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:156',message:'getUserEnrollments BEFORE parsing',data:{hasItems:!!response.data.items,itemsType:Array.isArray(response.data.items)?'array':typeof response.data.items,hasEnrollments:!!response.data.enrollments,enrollmentsType:Array.isArray(response.data.enrollments)?'array':typeof response.data.enrollments},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'A,B'})}).catch(()=>{});
        // #endregion
        
        let items: any[] = [];
        let totalCount = 0;
        
        // Backend direkt array döndürüyorsa (Hipotez A)
        if (Array.isArray(response.data)) {
          // #region agent log
          fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:163',message:'getUserEnrollments BRANCH: Array format',data:{arrayLength:response.data.length,firstItemKeys:response.data[0]?Object.keys(response.data[0]):null},timestamp:Date.now(),sessionId:'debug-session',runId:'post-fix',hypothesisId:'A'})}).catch(()=>{});
          // #endregion
          
          // Backend'in döndürdüğü formatı EnrollmentWithCourse formatına dönüştür
          items = response.data.map((item: any) => {
            // Backend direkt enrollment objesi döndürüyor, EnrollmentWithCourse formatına çevir
            return {
              enrollment: {
                id: item.id || item.enrollmentId,
                userId: item.userId || '',
                courseId: item.courseId,
                enrolledAt: item.enrolledAt || item.enrolledAt,
                startedAt: item.startedAt || null,
                completedAt: item.completedAt || null,
                isActive: item.isActive !== undefined ? item.isActive : true,
              },
              course: {
                id: item.courseId,
                title: item.courseTitle || item.title || '',
                description: item.courseDescription || item.description || '',
                thumbnailUrl: item.thumbnailUrl || null,
                estimatedDurationMinutes: item.estimatedDurationMinutes || 0,
                stepCount: item.stepCount || 0,
                enrollmentCount: item.enrollmentCount || 0,
                createdAt: item.createdAt || item.enrolledAt || new Date().toISOString(),
              },
              progress: item.progressPercentage !== undefined ? {
                progressPercentage: item.progressPercentage || 0,
                completedSteps: item.completedSteps || 0,
                totalSteps: item.totalSteps || 0,
                timeSpentMinutes: item.timeSpentMinutes || 0,
                lastAccessedAt: item.lastAccessedAt || null,
              } : undefined,
            };
          });
          totalCount = items.length;
        }
        // Backend { items: [...] } formatında döndürüyorsa
        else if (response.data.items && Array.isArray(response.data.items)) {
          // #region agent log
          fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:200',message:'getUserEnrollments BRANCH: items format',data:{itemsLength:response.data.items.length},timestamp:Date.now(),sessionId:'debug-session',runId:'post-fix',hypothesisId:'B'})}).catch(()=>{});
          // #endregion
          // Backend'in döndürdüğü formatı EnrollmentWithCourse formatına dönüştür
          items = response.data.items.map((item: any) => ({
            enrollment: {
              id: item.id || item.enrollmentId || item.enrollment?.id,
              userId: item.userId || item.enrollment?.userId || '',
              courseId: item.courseId || item.enrollment?.courseId,
              enrolledAt: item.enrolledAt || item.enrollment?.enrolledAt,
              startedAt: item.startedAt || item.enrollment?.startedAt || null,
              completedAt: item.completedAt || item.enrollment?.completedAt || null,
              isActive: item.isActive !== undefined ? item.isActive : (item.enrollment?.isActive !== undefined ? item.enrollment.isActive : true),
            },
            course: item.course || {
              id: item.courseId,
              title: item.courseTitle || item.title || '',
              description: item.courseDescription || item.description || '',
              thumbnailUrl: item.thumbnailUrl || null,
              estimatedDurationMinutes: item.estimatedDurationMinutes || 0,
              stepCount: item.stepCount || 0,
              enrollmentCount: item.enrollmentCount || 0,
              createdAt: item.createdAt || item.enrolledAt || new Date().toISOString(),
            },
            progress: item.progress || (item.progressPercentage !== undefined ? {
              progressPercentage: item.progressPercentage || 0,
              completedSteps: item.completedSteps || 0,
              totalSteps: item.totalSteps || 0,
              timeSpentMinutes: item.timeSpentMinutes || 0,
              lastAccessedAt: item.lastAccessedAt || null,
            } : undefined),
          }));
          totalCount = response.data.totalCount ?? response.data.count ?? items.length;
        }
        // Backend { enrollments: [...] } formatında döndürüyorsa (Hipotez B - Dashboard'dan görüldüğü gibi)
        else if (response.data.enrollments && Array.isArray(response.data.enrollments)) {
          // #region agent log
          fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:230',message:'getUserEnrollments BRANCH: enrollments format',data:{enrollmentsLength:response.data.enrollments.length},timestamp:Date.now(),sessionId:'debug-session',runId:'post-fix',hypothesisId:'B'})}).catch(()=>{});
          // #endregion
          // Backend'in döndürdüğü formatı EnrollmentWithCourse formatına dönüştür
          items = response.data.enrollments.map((item: any) => ({
            enrollment: {
              id: item.id || item.enrollmentId || item.enrollment?.id,
              userId: item.userId || item.enrollment?.userId || '',
              courseId: item.courseId || item.enrollment?.courseId,
              enrolledAt: item.enrolledAt || item.enrollment?.enrolledAt,
              startedAt: item.startedAt || item.enrollment?.startedAt || null,
              completedAt: item.completedAt || item.enrollment?.completedAt || null,
              isActive: item.isActive !== undefined ? item.isActive : (item.enrollment?.isActive !== undefined ? item.enrollment.isActive : true),
            },
            course: item.course || {
              id: item.courseId,
              title: item.courseTitle || item.title || '',
              description: item.courseDescription || item.description || '',
              thumbnailUrl: item.thumbnailUrl || null,
              estimatedDurationMinutes: item.estimatedDurationMinutes || 0,
              stepCount: item.stepCount || 0,
              enrollmentCount: item.enrollmentCount || 0,
              createdAt: item.createdAt || item.enrolledAt || new Date().toISOString(),
            },
            progress: item.progress || (item.progressPercentage !== undefined ? {
              progressPercentage: item.progressPercentage || 0,
              completedSteps: item.completedSteps || 0,
              totalSteps: item.totalSteps || 0,
              timeSpentMinutes: item.timeSpentMinutes || 0,
              lastAccessedAt: item.lastAccessedAt || null,
            } : undefined),
          }));
          totalCount = response.data.totalCount ?? response.data.count ?? items.length;
        }
        
        // #region agent log
        fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:181',message:'getUserEnrollments AFTER parsing',data:{itemsCount:items.length,totalCount,firstItem:items[0]?JSON.stringify(items[0]).substring(0,200):null},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'A,B,C'})}).catch(()=>{});
        // #endregion
        
        const result = {
          items,
          totalCount,
        };
        
        // #region agent log
        fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:190',message:'getUserEnrollments RETURN',data:{itemsCount:result.items.length,totalCount:result.totalCount},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'A,B,C'})}).catch(()=>{});
        // #endregion
        
        return result;
      }
      
      // Response başarılı ama data yok
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:195',message:'getUserEnrollments NO DATA',data:{success:response.success,hasData:!!response.data},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'D'})}).catch(()=>{});
      // #endregion
      return {
        items: [],
        totalCount: 0,
      };
    } catch (error) {
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'educationService.ts:201',message:'getUserEnrollments ERROR',data:{errorMessage:error instanceof Error?error.message:'unknown',errorType:error instanceof ApiError?'ApiError':'other',status:error instanceof ApiError?error.status:null},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'D'})}).catch(()=>{});
      // #endregion
      
      if (error instanceof ApiError && error.status === 404) {
        return {
          items: [],
          totalCount: 0,
        };
      }
      
      return {
        items: [],
        totalCount: 0,
      };
    }
  }

  /**
   * Kurs bazında kullanıcı ilerlemesini getir
   */
  static async getUserProgress(courseId: string): Promise<CourseProgressResponse> {
    const endpoint = `/api/v1/education/courses/${courseId}/progress`;
    
    const response = await apiClient.get<CourseProgressResponse>(endpoint);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error(response.message || 'İlerleme bilgisi yüklenemedi');
  }

  /**
   * Kullanıcının genel ilerleme dashboard'unu getir
   */
  static async getProgressDashboard(): Promise<ProgressDashboardResponse> {
    const endpoint = `/api/v1/education/progress/dashboard`;
    
    const response = await apiClient.get<ProgressDashboardResponse>(endpoint);
    
    if (response.success && response.data) {
      // Güvenli dönüş: courses varsayılan değerle
      return {
        ...response.data,
        totalCourses: response.data.totalCourses ?? 0,
        completedCourses: response.data.completedCourses ?? 0,
        activeCourses: response.data.activeCourses ?? 0,
        totalTimeSpentMinutes: response.data.totalTimeSpentMinutes ?? 0,
        courses: Array.isArray(response.data.courses) ? response.data.courses : [],
      };
    }
    
    // Hata durumunda boş response döndür
    return {
      totalCourses: 0,
      completedCourses: 0,
      activeCourses: 0,
      totalTimeSpentMinutes: 0,
      courses: [],
    };
  }

  /**
   * İçeriği tamamlandı olarak işaretle
   */
  static async completeContent(
    contentId: string,
    courseId: string,
    data?: CompleteContentRequest
  ): Promise<ApiResponse<void>> {
    const endpoint = `/api/v1/education/contents/${contentId}/complete`;
    
    const response = await apiClient.post<void>(
      endpoint,
      {
        ...(data || {}),
        courseId, // API'ye courseId gönderilmesi gerekiyor
      }
    );
    
    if (response.success) {
      return response;
    }
    
    throw new Error(response.message || 'İçerik tamamlanamadı');
  }

  /**
   * Video içeriği için ilerleme güncelle
   */
  static async updateVideoProgress(
    contentId: string,
    courseId: string,
    data: UpdateVideoProgressRequest
  ): Promise<ApiResponse<void>> {
    const endpoint = `/api/v1/education/contents/${contentId}/video-progress`;
    
    const response = await apiClient.post<void>(
      endpoint,
      {
        ...data,
        courseId, // API'ye courseId gönderilmesi gerekiyor
      }
    );
    
    if (response.success) {
      return response;
    }
    
    throw new Error(response.message || 'Video ilerlemesi güncellenemedi');
  }

  /**
   * Yeni kurs oluştur
   */
  static async createCourse(data: CreateCourseRequest): Promise<CreateCourseResponse> {
    const endpoint = `/api/v1/education/courses`;
    
    try {
      const response = await apiClient.post<{ id: string } | CreateCourseResponse>(
        endpoint,
        data
      );
      
      if (response.success && response.data) {
        // Backend sadece { id: string } döndürüyorsa, frontend formatına çevir
        if ('id' in response.data && !('title' in response.data)) {
          return {
            id: response.data.id,
            title: data.title,
            description: data.description,
            thumbnailUrl: data.thumbnailUrl,
            createdAt: new Date().toISOString(),
          };
        }
        return response.data as CreateCourseResponse;
      }
      
      throw new Error(response.message || 'Kurs oluşturulamadı');
    } catch (error: any) {
      // 404 hatası - Backend endpoint henüz hazır değil
      if (error instanceof ApiError && error.status === 404) {
        throw new ApiError(
          'Kurs oluşturma özelliği henüz aktif değil. Backend API endpoint\'i hazır olmadığı için şu an kurs oluşturulamıyor.',
          404
        );
      }
      // 403 hatası - Yetki yok
      if (error instanceof ApiError && error.status === 403) {
        throw new ApiError(
          'Kurs oluşturmak için yeterli yetkiniz yok. Admin veya OrganizationManager rolüne sahip olmalısınız.',
          403
        );
      }
      // Diğer hatalar
      throw error;
    }
  }

  /**
   * Kurs için roadmap oluştur
   */
  static async createRoadmap(
    courseId: string,
    data: CreateRoadmapRequest
  ): Promise<ApiResponse<void>> {
    const endpoint = `/api/v1/education/courses/${courseId}/roadmap`;
    
    const response = await apiClient.post<void>(
      endpoint,
      data
    );
    
    if (response.success) {
      return response;
    }
    
    throw new Error(response.message || 'Roadmap oluşturulamadı');
  }
}

