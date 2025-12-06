/**
 * TE4IT Frontend - Type Definitions
 * Tüm entity'ler için TypeScript tipleri
 */

// ==================== Project Types ====================
export interface Project {
  id: string;
  title: string;
  description?: string;
  status: 'Active' | 'Archived';
  startedDate: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProjectRequest {
  title: string;
  description?: string;
}

export interface UpdateProjectRequest {
  title?: string;
  description?: string;
}

export interface ProjectListResponse {
  items: Project[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ==================== Module Types ====================
export interface Module {
  id: string;
  projectId: string;
  title: string;
  description?: string;
  status: 'Active' | 'Archived';
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

export interface ModuleListResponse {
  items: Module[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ==================== UseCase Types ====================
export interface UseCase {
  id: string;
  moduleId: string;
  title: string;
  description?: string;
  importantNotes?: string;
  status: 'Active' | 'Archived';
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

export interface UseCaseListResponse {
  items: UseCase[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ==================== Task Types ====================
export type TaskType = 'Documentation' | 'Feature' | 'Test' | 'Bug';
export type TaskState = 'NotStarted' | 'InProgress' | 'Completed' | 'Cancelled';

export interface Task {
  id: string;
  useCaseId: string;
  title: string;
  description?: string;
  importantNotes?: string;
  type: TaskType;
  state: TaskState;
  assigneeId?: string;
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

export interface TaskListResponse {
  items: Task[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ==================== Task Relation Types ====================
export type TaskRelationType = 'Blocks' | 'RelatesTo' | 'Fixes' | 'Duplicates';

export interface TaskRelation {
  id: string;
  sourceTaskId: string;
  targetTaskId: string;
  relationType: TaskRelationType;
  targetTask?: Task;
}

export interface CreateTaskRelationRequest {
  targetTaskId: string;
  relationType: TaskRelationType;
}

// ==================== User Types ====================
export interface User {
  id: string;
  userName: string;
  email: string;
  firstName?: string;
  lastName?: string;
}

// ==================== Filter & Pagination Types ====================
export interface PaginationParams {
  page?: number;
  pageSize?: number;
}

export interface ProjectFilters extends PaginationParams {
  isActive?: boolean;
  search?: string;
}

export interface ModuleFilters extends PaginationParams {
  status?: 'Active' | 'Archived' | 'All';
  search?: string;
}

export interface UseCaseFilters extends PaginationParams {
  status?: 'Active' | 'Archived' | 'All';
  search?: string;
}

export interface TaskFilters extends PaginationParams {
  state?: TaskState | 'All';
  type?: TaskType | 'All';
  assigneeId?: string;
  dueDate?: 'Overdue' | 'Today' | 'ThisWeek' | 'ThisMonth' | 'All';
  search?: string;
}

// ==================== Statistics Types ====================
export interface ProjectStatistics {
  moduleCount: number;
  useCaseCount: number;
  taskCount: number;
  completedTaskCount: number;
  taskStatusDistribution: {
    notStarted: number;
    inProgress: number;
    completed: number;
    cancelled: number;
  };
  overdueTasks: Task[];
  upcomingDeadlines: Task[];
}

