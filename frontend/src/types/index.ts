/**
 * TE4IT Frontend Type Definitions
 * Tüm entity'ler ve API response tipleri için TypeScript tanımlamaları
 */

// ==================== Base Types ====================

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles?: string[];
  permissions?: string[];
}

/**
 * Backend'den gelen kullanıcı bilgisi (UserResponse)
 */
export interface UserResponse {
  id: string;
  userName: string;
  email: string;
  emailConfirmed: boolean;
  roles: string[];
}

// ==================== Project Types ====================

export interface Project {
  id: string;
  title: string;
  description?: string;
  isActive: boolean;
  status?: 'Active' | 'Archived'; // Frontend'de kullanılan status (isActive'den türetilir)
  startedDate: string;
  createdAt: string;
  updatedAt: string;
  ownerId?: string; // Projeyi oluşturan kullanıcının ID'si
  ownerEmail?: string; // Projeyi oluşturan kullanıcının email'i
  ownerName?: string; // Projeyi oluşturan kullanıcının adı
}

export interface CreateProjectRequest {
  title: string;
  description?: string;
}

export interface UpdateProjectRequest {
  title?: string;
  description?: string;
}

// ==================== Module Types ====================

export interface Module {
  id: string;
  projectId: string;
  title: string;
  description?: string;
  isActive: boolean;
  useCaseCount?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateModuleRequest {
  title: string;
  description?: string;
}

export interface UpdateModuleRequest {
  title?: string;
  description?: string;
}

// ==================== UseCase Types ====================

export interface UseCase {
  id: string;
  moduleId: string;
  title: string;
  description?: string;
  importantNotes?: string;
  isActive: boolean;
  taskCount?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUseCaseRequest {
  title: string;
  description?: string;
  importantNotes?: string;
}

export interface UpdateUseCaseRequest {
  title?: string;
  description?: string;
  importantNotes?: string;
}

// ==================== Task Types ====================

export enum TaskType {
  Documentation = 'Documentation',
  Feature = 'Feature',
  Test = 'Test',
  Bug = 'Bug',
}

export enum TaskState {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
}

export interface Task {
  id: string;
  useCaseId: string;
  title: string;
  description?: string;
  importantNotes?: string;
  type: TaskType;
  state: TaskState;
  assigneeId?: string;
  assignee?: User;
  assigneeName?: string;
  startedDate?: string;
  dueDate?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  importantNotes?: string;
  type: TaskType;
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  importantNotes?: string;
  type?: TaskType;
  dueDate?: string;
}

// ==================== Task Relation Types ====================

export enum TaskRelationType {
  Blocks = 'Blocks',
  RelatesTo = 'RelatesTo',
  Fixes = 'Fixes',
  Duplicates = 'Duplicates',
}

export interface TaskRelation {
  id: string;
  sourceTaskId: string;
  targetTaskId: string;
  relationType: TaskRelationType;
  targetTask?: Task;
  createdAt: string;
}

export interface CreateTaskRelationRequest {
  targetTaskId: string;
  relationType: TaskRelationType;
}

// ==================== Filter & Pagination Types ====================

export interface PaginationParams {
  page: number;
  pageSize: number;
}

export interface PaginationResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ProjectFilters {
  isActive?: boolean | null; // null = All
  search?: string;
}

export interface ModuleFilters {
  isActive?: boolean | null; // null = All
  search?: string;
}

export interface UseCaseFilters {
  page?: number;
  pageSize?: number;
  status?: 'All' | 'Active' | 'Archived';
  search?: string;
}

export interface TaskFilters {
  state?: TaskState | null; // null = All
  type?: TaskType | null; // null = All
  assigneeId?: string | null; // null = All, "me" = current user
  dueDate?: string | null; // "overdue", "today", "thisWeek", "thisMonth", null = All
  search?: string;
}

// ==================== Status Change Types ====================

export interface ChangeStatusRequest {
  isActive: boolean;
}

export interface ChangeTaskStateRequest {
  state: TaskState;
}

export interface AssignTaskRequest {
  assigneeId: string;
}

// ==================== Project Member & Invitation Types ====================

/**
 * Proje rolleri - Backend ProjectRole enum'u ile uyumlu
 */
export enum ProjectRole {
  Viewer = 'Viewer',   // 1 - Sadece görüntüleme yetkisi
  Member = 'Member',   // 2 - Düzenleme yetkisi
  Owner = 'Owner',     // 5 - Tam yetki (proje sahibi)
}

/**
 * Proje üyesi bilgileri
 */
export interface ProjectMember {
  userId: string;
  userName: string;
  email: string;
  role: ProjectRole;
  joinedDate: string;
}

/**
 * Proje davetiye bilgileri (liste için)
 */
export interface ProjectInvitation {
  invitationId: string;
  email: string;
  role: ProjectRole;
  status: 'Pending' | 'Accepted' | 'Cancelled' | 'Expired';
  createdDate: string;
  expiresAt: string;
  acceptedAt?: string;
  invitedByUserName: string;
}

/**
 * Proje davetiye detayı (token ile getirilen)
 */
export interface ProjectInvitationDetail {
  invitationId: string;
  projectId: string;
  projectTitle: string;
  email: string;
  role: ProjectRole;
  expiresAt: string;
  invitedByUserName: string;
}

/**
 * Davetiye gönderme request'i
 * Backend numeric role bekliyor (1=Viewer, 2=Member, 5=Owner)
 */
export interface InviteProjectMemberRequest {
  email: string;
  role: number; // Backend numeric role değeri
}

/**
 * Davetiye gönderme response'u
 */
export interface InviteProjectMemberResponse {
  invitationId: string;
  email: string;
  expiresAt: string;
}

/**
 * Üye rolü güncelleme request'i
 */
export interface UpdateProjectMemberRoleRequest {
  role: ProjectRole;
}

