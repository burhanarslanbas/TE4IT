/**
 * Project Service Interface
 */
import type {
  Project,
  CreateProjectRequest,
  UpdateProjectRequest,
  ProjectListResponse,
  ProjectFilters,
} from '../../types';

export interface IProjectService {
  getProjects(filters?: ProjectFilters): Promise<ProjectListResponse>;
  getProject(id: string): Promise<Project>;
  createProject(data: CreateProjectRequest): Promise<Project>;
  updateProject(id: string, data: UpdateProjectRequest): Promise<Project>;
  updateProjectStatus(id: string, status: 'Active' | 'Archived'): Promise<void>;
  deleteProject(id: string): Promise<void>;
}

