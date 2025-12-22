/**
 * Course Roadmap Page
 * Kurs roadmap sayfası - adımlar listesi ve ilerleme takibi
 * Route: /education/courses/:courseId/roadmap
 */

import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import { EducationService } from '../services/educationService';
import { RoadmapView } from '../components/education/RoadmapView';
import { ProgressBar } from '../components/education/ProgressBar';
import { Button } from '../components/ui/button';
import { toast } from 'sonner';
import { ArrowLeft, Loader2 } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import type { CourseDetailResponse, CourseProgressResponse } from '../types/education';
import { ApiError } from '../services/api';

export function CourseRoadmapPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const [course, setCourse] = useState<CourseDetailResponse | null>(null);
  const [progress, setProgress] = useState<CourseProgressResponse | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (courseId) {
      loadData();
    }
  }, [courseId]);

  const loadData = async () => {
    if (!courseId) return;

    try {
      setLoading(true);
      const [courseData, progressData] = await Promise.all([
        EducationService.getCourseById(courseId),
        EducationService.getUserProgress(courseId).catch(() => null), // Progress yoksa null döner
      ]);

      setCourse(courseData);
      setProgress(progressData);
    } catch (error) {
      console.error('Error loading roadmap:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Roadmap yüklenemedi',
        });
      } else {
        toast.error('Hata', {
          description: 'Roadmap yüklenemedi',
        });
      }
      navigate(`/education/courses/${courseId}`);
    } finally {
      setLoading(false);
    }
  };

  const handleStepClick = (stepId: string) => {
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseRoadmapPage.tsx:62',message:'handleStepClick ENTRY',data:{courseId,stepId,hasCourse:!!course,hasRoadmap:!!course?.roadmap},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
    // #endregion
    
    if (!courseId || !course?.roadmap) {
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseRoadmapPage.tsx:68',message:'handleStepClick BLOCKED - no course/roadmap',data:{hasCourseId:!!courseId,hasCourse:!!course,hasRoadmap:!!course?.roadmap},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
      // #endregion
      return;
    }

    const step = course.roadmap.steps.find(s => s.id === stepId);
    if (!step || step.contents.length === 0) {
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseRoadmapPage.tsx:75',message:'handleStepClick BLOCKED - no step/content',data:{hasStep:!!step,contentsLength:step?.contents?.length||0},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
      // #endregion
      return;
    }

    // İlk içeriğe yönlendir
    const firstContent = step.contents[0];
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseRoadmapPage.tsx:82',message:'handleStepClick NAVIGATE',data:{firstContentId:firstContent.id,contentType:firstContent.type,contentTitle:firstContent.title},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
    // #endregion
    navigate(`/education/courses/${courseId}/steps/${stepId}/contents/${firstContent.id}`);
  };

  const handleContentClick = (stepId: string, contentId: string) => {
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseRoadmapPage.tsx:73',message:'handleContentClick',data:{courseId,stepId,contentId},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'E'})}).catch(()=>{});
    // #endregion
    
    if (!courseId) return;
    navigate(`/education/courses/${courseId}/steps/${stepId}/contents/${contentId}`);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-[#8B5CF6]" />
      </div>
    );
  }

  if (!course || !course.roadmap) {
    return null;
  }

  const progressPercentage = progress?.progressPercentage || 0;

  return (
    <div className="min-h-screen bg-[#0D1117] text-[#E5E7EB] relative overflow-hidden">
      {/* Animated Background Blobs */}
      <div className="fixed inset-0 pointer-events-none overflow-hidden">
        <motion.div
          className="absolute -top-40 -right-40 w-[600px] h-[600px] bg-[#8B5CF6] opacity-[0.08] blur-[120px] rounded-full"
          animate={{
            scale: [1, 1.2, 1],
            x: [0, 40, 0],
            y: [0, -30, 0],
          }}
          transition={{
            duration: 20,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        />
        <motion.div
          className="absolute -bottom-40 -left-40 w-[600px] h-[600px] bg-[#2DD4BF] opacity-[0.08] blur-[120px] rounded-full"
          animate={{
            scale: [1, 1.3, 1],
            x: [0, -40, 0],
            y: [0, 30, 0],
          }}
          transition={{
            duration: 25,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        />
      </div>

      {/* Main Content */}
      <div className="relative z-10 px-4 sm:px-6 pb-12" style={{ paddingTop: 'calc(var(--navbar-height) + 2rem)' }}>
        <div className="max-w-5xl mx-auto">
          {/* Breadcrumb */}
          <Breadcrumb className="mb-6">
            <BreadcrumbList>
              <BreadcrumbItem>
                <BreadcrumbLink
                  href="/education"
                  onClick={(e) => {
                    e.preventDefault();
                    navigate('/education');
                  }}
                  className="text-[#9CA3AF] hover:text-[#E5E7EB]"
                >
                  Eğitimler
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator className="text-[#6B7280]" />
              <BreadcrumbItem>
                <BreadcrumbLink
                  href={`/education/courses/${courseId}`}
                  onClick={(e) => {
                    e.preventDefault();
                    navigate(`/education/courses/${courseId}`);
                  }}
                  className="text-[#9CA3AF] hover:text-[#E5E7EB]"
                >
                  {course.title}
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator className="text-[#6B7280]" />
              <BreadcrumbPage className="text-[#E5E7EB]">Roadmap</BreadcrumbPage>
            </BreadcrumbList>
          </Breadcrumb>

          {/* Back Button */}
          <Button
            variant="ghost"
            onClick={() => navigate(`/education/courses/${courseId}`)}
            className="mb-6 text-[#9CA3AF] hover:text-[#E5E7EB]"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Kursa Dön
          </Button>

          {/* Header */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8 mb-6"
          >
            <h1 className="text-3xl font-bold text-[#E5E7EB] mb-2">
              {course.roadmap.title}
            </h1>
            {course.roadmap.description && (
              <p className="text-[#9CA3AF] mb-6">
                {course.roadmap.description}
              </p>
            )}

            {/* Progress Bar */}
            {progress && (
              <ProgressBar
                percentage={progressPercentage}
                label="Kurs İlerlemesi"
                showPercentage={true}
              />
            )}

            {/* Stats */}
            {progress && (
              <div className="grid grid-cols-2 sm:grid-cols-3 gap-4 mt-6">
                <motion.div
                  initial={{ opacity: 0, scale: 0.9 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ delay: 0.1 }}
                  className="p-4 bg-[#0D1117] rounded-lg border border-[#30363D]/50"
                >
                  <p className="text-sm text-[#9CA3AF] mb-1">Tamamlanan Adımlar</p>
                  <p className="text-2xl font-bold text-[#E5E7EB]">
                    {progress.completedSteps}
                    <span className="text-lg text-[#9CA3AF]">/{progress.totalSteps}</span>
                  </p>
                </motion.div>
                {progress.timeSpentMinutes > 0 && (
                  <motion.div
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    transition={{ delay: 0.2 }}
                    className="p-4 bg-[#0D1117] rounded-lg border border-[#30363D]/50"
                  >
                    <p className="text-sm text-[#9CA3AF] mb-1">Geçirilen Süre</p>
                    <p className="text-2xl font-bold text-[#E5E7EB]">
                      {progress.timeSpentMinutes}
                      <span className="text-lg text-[#9CA3AF]"> dk</span>
                    </p>
                  </motion.div>
                )}
                <motion.div
                  initial={{ opacity: 0, scale: 0.9 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ delay: 0.3 }}
                  className="p-4 bg-[#0D1117] rounded-lg border border-[#30363D]/50"
                >
                  <p className="text-sm text-[#9CA3AF] mb-1">İlerleme</p>
                  <p className="text-2xl font-bold text-[#8B5CF6]">
                    {Math.round(progressPercentage)}%
                  </p>
                </motion.div>
              </div>
            )}
          </motion.div>

          {/* Roadmap Steps */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
          >
            <RoadmapView
              roadmap={course.roadmap}
              stepProgresses={progress?.steps || []}
              onStepClick={handleStepClick}
              courseId={courseId}
              onContentClick={handleContentClick}
            />
          </motion.div>
        </div>
      </div>
    </div>
  );
}

