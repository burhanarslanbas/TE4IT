/**
 * Education Module Type Definitions
 * Eğitim modülü için tüm TypeScript type tanımlamaları
 */

// ==================== Content Type Enum ====================

export enum ContentType {
  Text = 1,           // Medium tarzı zengin metin
  VideoLink = 2,      // YouTube, Vimeo vb. video linki
  DocumentLink = 3,   // PDF, DOCX vb. doküman linki
  ExternalLink = 4    // Genel dış link
}

// ==================== Course Types ====================

export interface Course {
  id: string;
  title: string;
  description: string;
  thumbnailUrl?: string;
  createdBy: string; // UserId (Guid)
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  roadmap?: CourseRoadmap;
}

export interface CourseRoadmap {
  title: string;
  description?: string;
  estimatedDurationMinutes: number;
  steps: RoadmapStep[];
}

export interface RoadmapStep {
  id: string;
  title: string;
  description?: string;
  order: number;
  isRequired: boolean;
  estimatedDurationMinutes: number;
  contents: CourseContent[];
}

export interface CourseContent {
  id: string;
  type: ContentType;
  title: string;
  content?: string; // Text için HTML/Markdown içerik
  linkUrl?: string; // Video/Document/External için URL
  embedUrl?: string; // Frontend için embed URL (backend'den gelecek)
  platform?: string; // youtube, vimeo, vb.
  order: number;
  isRequired: boolean;
}

// ==================== Enrollment Types ====================

export interface Enrollment {
  id: string;
  userId: string; // UserId (Guid)
  courseId: string; // MongoDB ObjectId
  enrolledAt: string;
  startedAt?: string; // İlk içerik erişimi
  completedAt?: string; // Kurs tamamlandığında
  isActive: boolean;
}

// ==================== Progress Types ====================

export interface Progress {
  id: string;
  userId: string; // UserId (Guid)
  enrollmentId: string; // MongoDB ObjectId
  courseId: string; // MongoDB ObjectId
  stepId: string; // RoadmapStep Id
  contentId: string; // CourseContent Id
  isCompleted: boolean;
  completedAt?: string;
  timeSpentMinutes?: number;
  lastAccessedAt?: string;
  watchedPercentage?: number; // Video için izlenme yüzdesi
}

// ==================== API Request/Response Types ====================

// Course List Response
export interface CourseListResponse {
  items: CourseListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CourseListItem {
  id: string;
  title: string;
  description: string;
  thumbnailUrl?: string;
  estimatedDurationMinutes: number;
  stepCount: number;
  enrollmentCount: number;
  createdAt: string;
}

// Course Detail Response
export interface CourseDetailResponse {
  id: string;
  title: string;
  description: string;
  thumbnailUrl?: string;
  roadmap?: CourseRoadmap;
  userEnrollment?: UserEnrollment;
}

export interface UserEnrollment {
  isEnrolled: boolean;
  enrolledAt?: string;
  startedAt?: string;
  completedAt?: string;
  progressPercentage: number;
}

// Enrollment Response
export interface EnrollmentResponse {
  id: string;
  courseId: string;
  userId: string;
  enrolledAt: string;
  startedAt?: string;
  completedAt?: string;
}

// User Enrollments Response
export interface UserEnrollmentsResponse {
  items: EnrollmentWithCourse[];
  totalCount: number;
}

export interface EnrollmentWithCourse {
  enrollment: Enrollment;
  course: CourseListItem;
  progress?: CourseProgress;
}

// Course Progress Response
export interface CourseProgressResponse {
  courseId: string;
  enrollmentId: string;
  progressPercentage: number;
  completedSteps: number;
  totalSteps: number;
  timeSpentMinutes: number;
  steps: StepProgress[];
}

export interface StepProgress {
  stepId: string;
  title: string;
  order: number;
  isCompleted: boolean;
  completedAt?: string;
  contents: ContentProgress[];
}

export interface ContentProgress {
  contentId: string;
  title: string;
  isCompleted: boolean;
  completedAt?: string;
}

// Progress Dashboard Response
export interface ProgressDashboardResponse {
  totalCourses: number;
  completedCourses: number;
  activeCourses: number;
  totalTimeSpentMinutes: number;
  courses: CourseProgressSummary[];
}

export interface CourseProgressSummary {
  courseId: string;
  title: string;
  progressPercentage: number;
  status: 'active' | 'completed';
  lastAccessedAt?: string;
}

// ==================== API Request Types ====================

export interface GetCoursesRequest {
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: 'title' | 'createdAt';
  sortOrder?: 'asc' | 'desc';
}

export interface EnrollInCourseRequest {
  // Empty body - courseId URL'de
}

export interface CompleteContentRequest {
  timeSpentMinutes?: number;
}

export interface UpdateVideoProgressRequest {
  watchedPercentage: number; // 0-100
  timeSpentSeconds: number;
  isCompleted: boolean;
}

export interface GetUserEnrollmentsRequest {
  status?: 'active' | 'completed' | 'all';
}

// ==================== Create Course Types ====================

export interface CreateCourseRequest {
  title: string;
  description: string;
  thumbnailUrl?: string;
}

export interface CreateCourseResponse {
  id: string;
  title: string;
  description: string;
  thumbnailUrl?: string;
  createdAt: string;
}

export interface CreateRoadmapRequest {
  title: string;
  description?: string;
  estimatedDurationMinutes: number;
  steps: CreateRoadmapStepRequest[];
}

export interface CreateRoadmapStepRequest {
  title: string;
  description?: string;
  order: number;
  isRequired: boolean;
  estimatedDurationMinutes: number;
  contents: CreateContentRequest[];
}

export interface CreateContentRequest {
  type: ContentType;
  title: string;
  content?: string; // Text için HTML/Markdown içerik
  linkUrl?: string; // Video/Document/External için URL
  order: number;
  isRequired: boolean;
}

// ==================== Course Filters ====================

export interface CourseFilters {
  status?: 'all' | 'active' | 'completed';
  search?: string;
}

// ==================== Course Status Types ====================

export type CourseStatus = 'not-enrolled' | 'enrolled' | 'in-progress' | 'completed';

export interface CourseWithProgress extends CourseListItem {
  enrollment?: Enrollment;
  progress?: CourseProgress;
  status: CourseStatus;
}

export interface CourseProgress {
  progressPercentage: number;
  completedSteps: number;
  totalSteps: number;
  timeSpentMinutes: number;
  lastAccessedAt?: string;
}

