/**
 * Module Service Interface
 */
import type {
  Module,
  CreateModuleRequest,
  UpdateModuleRequest,
  ModuleListResponse,
  ModuleFilters,
} from '../../types';

export interface IModuleService {
  getModules(projectId: string, filters?: ModuleFilters): Promise<ModuleListResponse>;
  getModule(moduleId: string): Promise<Module>;
  createModule(projectId: string, data: CreateModuleRequest): Promise<Module>;
  updateModule(moduleId: string, data: UpdateModuleRequest): Promise<Module>;
  updateModuleStatus(moduleId: string, status: 'Active' | 'Archived'): Promise<void>;
  deleteModule(moduleId: string): Promise<void>;
}

