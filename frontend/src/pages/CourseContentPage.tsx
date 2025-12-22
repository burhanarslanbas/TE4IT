/**
 * Course Content Page
 * İçerik görüntüleme sayfası - içerik tipine göre render eder
 * Route: /education/courses/:courseId/steps/:stepId/contents/:contentId
 */

import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import { EducationService } from '../services/educationService';
import { ContentViewer } from '../components/education/ContentViewer';
import { Button } from '../components/ui/button';
import { toast } from 'sonner';
import { ArrowLeft, ArrowRight, Loader2, ChevronLeft, ChevronRight } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import type { CourseDetailResponse, CourseProgressResponse } from '../types/education';
import { ApiError } from '../services/api';

export function CourseContentPage() {
  const { courseId, stepId, contentId } = useParams<{
    courseId: string;
    stepId: string;
    contentId: string;
  }>();
  const navigate = useNavigate();
  const [course, setCourse] = useState<CourseDetailResponse | null>(null);
  const [progress, setProgress] = useState<CourseProgressResponse | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (courseId && stepId && contentId) {
      loadData();
    }
  }, [courseId, stepId, contentId]);

  const loadData = async () => {
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseContentPage.tsx:36',message:'loadData ENTRY',data:{courseId,stepId,contentId},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'E'})}).catch(()=>{});
    // #endregion
    
    if (!courseId || !stepId || !contentId) return;

    try {
      setLoading(true);
      const [courseData, progressData] = await Promise.all([
        EducationService.getCourseById(courseId),
        EducationService.getUserProgress(courseId).catch(() => null),
      ]);

      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseContentPage.tsx:48',message:'loadData AFTER API calls',data:{hasCourse:!!courseData,hasRoadmap:!!courseData?.roadmap,stepsCount:courseData?.roadmap?.steps?.length||0,hasProgress:!!progressData},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'E'})}).catch(()=>{});
      // #endregion

      setCourse(courseData);
      setProgress(progressData);
      
      // #region agent log
      const currentContent = courseData?.roadmap?.steps
        ?.find(s => s.id === stepId)
        ?.contents?.find(c => c.id === contentId);
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseContentPage.tsx:55',message:'loadData CONTENT CHECK',data:{hasCurrentContent:!!currentContent,contentType:currentContent?.type,contentTitle:currentContent?.title},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'E'})}).catch(()=>{});
      // #endregion
    } catch (error) {
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseContentPage.tsx:60',message:'loadData ERROR',data:{errorMessage:error instanceof Error?error.message:'unknown',errorType:error instanceof ApiError?'ApiError':'other'},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'E'})}).catch(()=>{});
      // #endregion
      
      console.error('Error loading content:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'İçerik yüklenemedi',
        });
      } else {
        toast.error('Hata', {
          description: 'İçerik yüklenemedi',
        });
      }
      navigate(`/education/courses/${courseId}/roadmap`);
    } finally {
      setLoading(false);
    }
  };

  const getCurrentContent = () => {
    if (!course?.roadmap || !stepId || !contentId) return null;

    const step = course.roadmap.steps.find(s => s.id === stepId);
    if (!step) return null;

    return step.contents.find(c => c.id === contentId) || null;
  };

  const getCurrentStep = () => {
    if (!course?.roadmap || !stepId) return null;
    return course.roadmap.steps.find(s => s.id === stepId) || null;
  };

  const getNavigationContent = () => {
    if (!course?.roadmap || !stepId || !contentId) return { prev: null, next: null };

    const step = course.roadmap.steps.find(s => s.id === stepId);
    if (!step) return { prev: null, next: null };

    const currentIndex = step.contents.findIndex(c => c.id === contentId);
    if (currentIndex === -1) return { prev: null, next: null };

    // Önceki içerik (aynı adımda)
    const prevContent = currentIndex > 0 ? step.contents[currentIndex - 1] : null;
    
    // Sonraki içerik (aynı adımda veya sonraki adımda)
    let nextContent = currentIndex < step.contents.length - 1 
      ? step.contents[currentIndex + 1] 
      : null;

    // Eğer adımın son içeriğindeyse, sonraki adımın ilk içeriğine bak
    if (!nextContent) {
      const currentStepIndex = course.roadmap.steps.findIndex(s => s.id === stepId);
      if (currentStepIndex < course.roadmap.steps.length - 1) {
        const nextStep = course.roadmap.steps[currentStepIndex + 1];
        if (nextStep && nextStep.contents.length > 0) {
          nextContent = nextStep.contents[0];
        }
      }
    }

    return {
      prev: prevContent ? { stepId, contentId: prevContent.id } : null,
      next: nextContent ? {
        stepId: nextContent === step.contents.find(c => c.id === nextContent?.id) 
          ? stepId 
          : course.roadmap.steps.find(s => s.contents.some(c => c.id === nextContent?.id))?.id || stepId,
        contentId: nextContent.id,
      } : null,
    };
  };

  const handleContentComplete = () => {
    // İçerik tamamlandığında progress'i yeniden yükle
    if (courseId) {
      EducationService.getUserProgress(courseId)
        .then(setProgress)
        .catch(console.error);
    }
  };

  const handleNavigate = (targetStepId: string, targetContentId: string) => {
    navigate(`/education/courses/${courseId}/steps/${targetStepId}/contents/${targetContentId}`);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-[#8B5CF6]" />
      </div>
    );
  }

  const currentContent = getCurrentContent();
  const currentStep = getCurrentStep();
  const navigation = getNavigationContent();

  if (!course || !currentContent || !currentStep) {
    return null;
  }

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
              <BreadcrumbItem>
                <BreadcrumbLink
                  href={`/education/courses/${courseId}/roadmap`}
                  onClick={(e) => {
                    e.preventDefault();
                    navigate(`/education/courses/${courseId}/roadmap`);
                  }}
                  className="text-[#9CA3AF] hover:text-[#E5E7EB]"
                >
                  Roadmap
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator className="text-[#6B7280]" />
              <BreadcrumbPage className="text-[#E5E7EB]">{currentContent.title}</BreadcrumbPage>
            </BreadcrumbList>
          </Breadcrumb>

          {/* Navigation Bar */}
          <div className="flex items-center justify-between mb-6">
            <Button
              variant="ghost"
              onClick={() => navigate(`/education/courses/${courseId}/roadmap`)}
              className="text-[#9CA3AF] hover:text-[#E5E7EB]"
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
              Roadmap'e Dön
            </Button>

            {/* Prev/Next Navigation */}
            <div className="flex gap-2">
              {navigation.prev && (
                <Button
                  variant="outline"
                  onClick={() => handleNavigate(navigation.prev!.stepId, navigation.prev!.contentId)}
                  className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                >
                  <ChevronLeft className="w-4 h-4 mr-1" />
                  Önceki
                </Button>
              )}
              {navigation.next && (
                <Button
                  variant="outline"
                  onClick={() => handleNavigate(navigation.next!.stepId, navigation.next!.contentId)}
                  className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                >
                  Sonraki
                  <ChevronRight className="w-4 h-4 ml-1" />
                </Button>
              )}
            </div>
          </div>

          {/* Step Info */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-4 mb-6"
          >
            <div className="flex items-center gap-3">
              <div className="w-8 h-8 rounded-full bg-[#8B5CF6]/20 border border-[#8B5CF6]/30 flex items-center justify-center text-[#8B5CF6] font-semibold text-sm">
                {currentStep.order}
              </div>
              <div>
                <h3 className="text-lg font-semibold text-[#E5E7EB]">
                  {currentStep.title}
                </h3>
                <p className="text-sm text-[#9CA3AF]">
                  {currentStep.contents.findIndex(c => c.id === contentId) + 1} / {currentStep.contents.length} İçerik
                </p>
              </div>
            </div>
          </motion.div>

          {/* Content Viewer */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 }}
          >
            {courseId && (
              <ContentViewer
                content={currentContent}
                courseId={courseId}
                onComplete={handleContentComplete}
              />
            )}
          </motion.div>
        </div>
      </div>
    </div>
  );
}

