/**
 * Roadmap Types
 * AI API'den gelen roadmap verileri için type definitions
 */

export interface RoadmapTask {
  task: string;
  priority: 'high' | 'medium' | 'low';
  estimated_hours: number;
  description: string;
}

export interface RoadmapUseCase {
  name: string;
  description: string;
  tasks: RoadmapTask[];
}

export interface RoadmapModule {
  name: string;
  description: string;
  use_cases: RoadmapUseCase[];
}

export interface RoadmapResponse {
  project_summary: string;
  tech_stack: string[];
  roadmap: RoadmapModule[];
  similar_projects_found: number;
  key_insights: string[];
}

export interface RoadmapGenerationRequest {
  title: string;
  description: string;
  max_similar_projects: number; // 1-10 arası
}

export interface SelectedRoadmapItems {
  modules: Set<string>; // Module name'leri
  useCases: Set<string>; // "moduleName|useCaseName" formatında
  tasks: Set<string>; // "moduleName|useCaseName|taskIndex" formatında
}
