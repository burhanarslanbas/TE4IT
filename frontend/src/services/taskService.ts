/**
 * Task API Servisleri
 *
 * Amaç:
 * - Backend enum'ları bazen numeric (1-based) bazen string dönebiliyor.
 * - Frontend ise TaskType / TaskState değerlerini string union olarak kullanıyor.
 *
 * Bu yüzden:
 * 1) Backend -> Frontend map: numeric/string fark etmez, her zaman string'e çevir.
 * 2) Frontend -> Backend map: backend numeric bekliyorsa numeric gönder.
 */

import { apiClient } from './api';
import type {
  Task,
  CreateTaskRequest,
  UpdateTaskRequest,
  TaskListResponse,
  TaskState,
  TaskType,
} from '../types';

/* -------------------------------------------------------
   ENUM MAPPING - TaskType
-------------------------------------------------------- */

/**
 * Backend tarafında TaskType enum'u numeric (1-based).
 * Frontend ise TaskType'ı string union olarak tutuyor.
 */
function toBackendTaskType(taskType: TaskType): number {
  switch (taskType) {
    case 'Documentation':
      return 1;
    case 'Feature':
      return 2;
    case 'Test':
      return 3;
    case 'Bug':
      return 4;
    default:
      // Runtime fallback
      return 2;
  }
}

/**
 * Backend'den gelen TaskType:
 * - "Documentation" gibi string gelebilir
 * - 1,2,3,4 gibi numeric gelebilir
 * Biz bunu her zaman frontend string union'a çeviriyoruz.
 */
function fromBackendTaskType(taskType: unknown): TaskType {
  // Backend string enum olarak dönüyorsa
  if (
    taskType === 'Documentation' ||
    taskType === 'Feature' ||
    taskType === 'Test' ||
    taskType === 'Bug'
  ) {
    return taskType;
  }

  // Backend numeric enum olarak dönüyorsa
  if (typeof taskType === 'number') {
    switch (taskType) {
      case 1:
        return 'Documentation';
      case 2:
        return 'Feature';
      case 3:
        return 'Test';
      case 4:
        return 'Bug';
      default:
        return 'Feature';
    }
  }

  return 'Feature';
}

/* -------------------------------------------------------
   ENUM MAPPING - TaskState  ✅ (BURASI SENDE EKSİKTİ)
-------------------------------------------------------- */

/**
 * Backend TaskState enum numeric (1-based):
 * NotStarted=1, InProgress=2, Completed=3, Cancelled=4
 *
 * Frontend TaskState ise string union.
 */
function toBackendTaskState(state: TaskState): number {
  switch (state) {
    case 'NotStarted':
      return 1;
    case 'InProgress':
      return 2;
    case 'Completed':
      return 3;
    case 'Cancelled':
      return 4;
    default:
      return 1;
  }
}

/**
 * Backend'den gelen TaskState:
 * - "InProgress" gibi string gelebilir
 * - 1,2,3,4 gibi numeric gelebilir
 * Biz bunu her zaman frontend string union'a çeviriyoruz.
 */
function fromBackendTaskState(state: unknown): TaskState {
  // Backend string enum olarak dönüyorsa
  if (
    state === 'NotStarted' ||
    state === 'InProgress' ||
    state === 'Completed' ||
    state === 'Cancelled'
  ) {
    return state;
  }

  // Backend numeric enum olarak dönüyorsa
  if (typeof state === 'number') {
    switch (state) {
      case 1:
        return 'NotStarted';
      case 2:
        return 'InProgress';
      case 3:
        return 'Completed';
      case 4:
        return 'Cancelled';
      default:
        return 'NotStarted';
    }
  }

  return 'NotStarted';
}

/* -------------------------------------------------------
   BACKEND DTO SHAPES (Swagger ile uyumlu)
-------------------------------------------------------- */

// Backend create/update request alan adları (Swagger DTO ile uyumlu)
// Backend, TaskType alanını bekliyor; frontend'de bu alan `type` olarak tutuluyor.
type BackendCreateTaskRequest = {
  title: string;
  description?: string;
  importantNotes?: string;
  taskType: number;
  dueDate?: string;
};

type BackendUpdateTaskRequest = {
  title?: string;
  description?: string;
  importantNotes?: string;
  taskType?: number;
  dueDate?: string;
};

// Backend response type
// Önemli: state/taskState numeric gelebilir! O yüzden unknown yazıyoruz.
interface BackendTask {
  id: string;
  useCaseId: string;
  title: string;
  description?: string;
  importantNotes?: string;

  // Backend farklı shape dönebilir: type/taskType ve state/taskState
  type?: unknown;
  taskType?: unknown;
  state?: unknown;
  taskState?: unknown;

  assigneeId?: string;
  assigneeName?: string;
  startedDate?: string;
  dueDate?: string;
  createdDate?: string;
  updatedDate?: string;
}

interface BackendTaskListResponse {
  items: BackendTask[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/* -------------------------------------------------------
   MAPPING: BackendTask -> Frontend Task
-------------------------------------------------------- */

/**
 * Backend Task response'unu frontend Task tipine çevirir.
 * - TaskType ve TaskState mutlaka normalize edilir (string union).
 * - Böylece UI tarafında config lookup (taskTypeConfig/taskStateConfig) asla undefined olmaz.
 */
function mapBackendTaskToFrontend(backendTask: BackendTask): Task {
  const rawType = backendTask.type ?? backendTask.taskType;
  const rawState = backendTask.state ?? backendTask.taskState;

  const convertedType = fromBackendTaskType(rawType);
  const convertedState = fromBackendTaskState(rawState);

  console.log('[MAP TASK] Converting backend task:', {
    id: backendTask.id,
    rawType,
    rawTypeType: typeof rawType,
    convertedType,
    rawState,
    rawStateType: typeof rawState,
    convertedState,
  });

  return {
    id: backendTask.id,
    useCaseId: backendTask.useCaseId,
    title: backendTask.title,
    description: backendTask.description,
    importantNotes: backendTask.importantNotes,

    // ✅ UI'nin beklediği alanlar:
    type: convertedType,
    state: convertedState,

    assigneeId: backendTask.assigneeId,
    assigneeName: backendTask.assigneeName,
    startedDate: backendTask.startedDate,
    dueDate: backendTask.dueDate,

    // createdAt / updatedAt fallback
    createdAt: backendTask.createdDate || backendTask.startedDate || new Date().toISOString(),
    updatedAt: backendTask.updatedDate || backendTask.startedDate || new Date().toISOString(),
  };
}

export interface TaskFilters {
  page?: number;
  pageSize?: number;
  state?: TaskState | 'All';
  type?: 'Documentation' | 'Feature' | 'Test' | 'Bug' | 'All';
  search?: string;
}

export class TaskService {
  /**
   * Use case task'larını getir
   */
  static async getTasks(useCaseId: string, filters?: TaskFilters): Promise<TaskListResponse> {
    const params = new URLSearchParams();

    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());

    // Not: Backend state filter string mi numeric mi bekliyor bilmiyoruz.
    // Şu an mevcut davranışı bozmamak için string gönderiyoruz.
    if (filters?.state && filters.state !== 'All') {
      params.append('state', filters.state);
    }

    if (filters?.type && filters.type !== 'All') {
      params.append('type', filters.type);
    }

    if (filters?.search) params.append('search', filters.search);

    const queryString = params.toString();

    // Swagger: GET /api/v1/Tasks/usecases/{useCaseId}
    const endpoint = `/api/v1/Tasks/usecases/${useCaseId}${queryString ? `?${queryString}` : ''}`;

    const response = await apiClient.get<BackendTaskListResponse>(endpoint);

    if (response.success && response.data) {
      return {
        items: response.data.items.map(mapBackendTaskToFrontend),
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
      };
    }

    throw new Error("Task'lar yüklenemedi");
  }

  /**
   * Task detayını getir
   */
  static async getTask(taskId: string): Promise<Task> {
    const response = await apiClient.get<BackendTask>(`/api/v1/Tasks/${taskId}`);

    if (response.success && response.data) {
      return mapBackendTaskToFrontend(response.data);
    }

    throw new Error('Task bulunamadı');
  }

  /**
   * Yeni task oluştur
   */
  static async createTask(useCaseId: string, data: CreateTaskRequest): Promise<Task> {
    try {
      // HTML date input'u YYYY-MM-DD formatında gelir, backend ISO 8601 bekler
      let formattedDueDate: string | undefined = undefined;

      if (data.dueDate) {
        // Eğer sadece tarih varsa (YYYY-MM-DD), UTC gece yarısına çevir
        formattedDueDate = data.dueDate.length === 10 ? `${data.dueDate}T00:00:00Z` : data.dueDate;
      }

      const payload: BackendCreateTaskRequest = {
        title: data.title,
        description: data.description,
        importantNotes: data.importantNotes,
        taskType: toBackendTaskType(data.type),
        dueDate: formattedDueDate,
      };

      const endpoint = `/api/v1/Tasks/usecases/${useCaseId}`;

      console.log('[CREATE TASK] Request:', {
        method: 'POST',
        endpoint,
        useCaseId,
        payload,
        taskType: payload.taskType,
      });

      // Swagger: POST /api/v1/Tasks/usecases/{useCaseId}
      const response = await apiClient.post<BackendTask>(endpoint, payload);

      if (response.success && response.data) {
        console.log('[CREATE TASK] Success:', response.data);
        return mapBackendTaskToFrontend(response.data);
      }

      throw new Error(response.message || 'Task oluşturulamadı');
    } catch (error: any) {
      console.error('[CREATE TASK] Error:', {
        message: error.message,
        statusCode: error.statusCode,
        status: error.status,
        errors: error.errors,
        fullError: error,
      });
      throw error;
    }
  }

  /**
   * Task güncelle
   */
  static async updateTask(taskId: string, data: UpdateTaskRequest): Promise<Task> {
    // HTML date input'u YYYY-MM-DD formatında gelir, backend ISO 8601 bekler
    let formattedDueDate: string | undefined = undefined;

    if (data.dueDate) {
      formattedDueDate = data.dueDate.length === 10 ? `${data.dueDate}T00:00:00Z` : data.dueDate;
    }

    const payload: BackendUpdateTaskRequest = {
      title: data.title,
      description: data.description,
      importantNotes: data.importantNotes,
      taskType: data.type ? toBackendTaskType(data.type) : undefined,
      dueDate: formattedDueDate,
    };

    const response = await apiClient.put<BackendTask>(`/api/v1/Tasks/${taskId}`, payload);

    if (response.success && response.data) {
      return mapBackendTaskToFrontend(response.data);
    }

    throw new Error('Task güncellenemedi');
  }

  /**
   * Task state değiştir (NotStarted/InProgress/Completed/Cancelled)
   *
   * Önemli:
   * Backend enum numeric bekliyorsa string göndermek sorun çıkarır.
   * Bu yüzden state'i numeric'e çevirerek gönderiyoruz.
   * Backend ChangeTaskStateRequest record'u NewState property'si bekliyor.
   */
  static async updateTaskState(taskId: string, state: TaskState): Promise<void> {
    const payload = { NewState: toBackendTaskState(state) };

    const response = await apiClient.patch(`/api/v1/Tasks/${taskId}/state`, payload);

    if (!response.success) {
      throw new Error(response.message || 'Task durumu güncellenemedi');
    }
  }

  /**
   * Task assign et
   */
  static async assignTask(taskId: string, assigneeId: string): Promise<void> {
    const response = await apiClient.post(`/api/v1/Tasks/${taskId}/assign`, { assigneeId });

    if (!response.success) {
      throw new Error('Task ataması yapılamadı');
    }
  }

  /**
   * Task sil
   * @throws {Error} 409 - Task ilişkileri var
   * @throws {Error} 404 - Task bulunamadı
   * @throws {Error} 403 - Yetki yok
   */
  static async deleteTask(taskId: string): Promise<void> {
    try {
      console.log('[DELETE TASK] Starting delete for task:', taskId);
      const response = await apiClient.delete(`/api/v1/Tasks/${taskId}`);

      if (!response.success) {
        throw new Error(response.message || 'Task silinemedi');
      }

      console.log('[DELETE TASK] Successfully deleted task:', taskId);
    } catch (error: any) {
      console.error('[DELETE TASK] Error:', error);

      // 409 Conflict - İlişkiler var
      if (error.statusCode === 409 || error.status === 409) {
        throw new Error("Bu task'ı silebilmek için önce ilişkili öğeleri kaldırmanız gerekiyor.");
      }

      // 404 Not Found - Zaten silinmiş
      if (error.statusCode === 404 || error.status === 404) {
        throw new Error('Task bulunamadı. Zaten silinmiş olabilir.');
      }

      // 403 Forbidden - Yetki yok
      if (error.statusCode === 403 || error.status === 403) {
        throw new Error("Bu task'ı silmek için yetkiniz bulunmamaktadır.");
      }

      // Backend'den gelen hata mesajını koru
      if (error.message) {
        throw error;
      }

      throw new Error('Task silinemedi');
    }
  }
}
