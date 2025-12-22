/**
 * Course Detail Page
 * Kurs detay sayfası - roadmap özeti ve kayıt butonu
 * Route: /education/courses/:courseId
 */

import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import { EducationService } from '../services/educationService';
import { EnrollmentButton } from '../components/education/EnrollmentButton';
import { ProgressBar } from '../components/education/ProgressBar';
import { Button } from '../components/ui/button';
import { toast } from 'sonner';
import { ArrowLeft, Clock, BookOpen, Loader2, CheckCircle2, Users, Play, TrendingUp } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import type { CourseDetailResponse } from '../types/education';
import { ApiError } from '../services/api';

export function CourseDetailPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const [course, setCourse] = useState<CourseDetailResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [enrollmentLoading, setEnrollmentLoading] = useState(false);
  const [isEnrolledChecked, setIsEnrolledChecked] = useState(false);
  const [progress, setProgress] = useState<{ progressPercentage: number } | null>(null);

  useEffect(() => {
    if (courseId) {
      loadCourse();
      checkEnrollment();
    }
  }, [courseId]);

  const loadCourse = async () => {
    if (!courseId) return;

    try {
      setLoading(true);
      const data = await EducationService.getCourseById(courseId);
      setCourse(data);
    } catch (error) {
      console.error('Error loading course:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Kurs detayı yüklenemedi',
        });
      } else {
        toast.error('Hata', {
          description: 'Kurs detayı yüklenemedi',
        });
      }
      navigate('/education');
    } finally {
      setLoading(false);
    }
  };

  const checkEnrollment = async () => {
    if (!courseId) return;

    try {
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseDetailPage.tsx:58',message:'checkEnrollment ENTRY',data:{courseId},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
      // #endregion
      
      const enrollments = await EducationService.getUserEnrollments();
      const enrollment = enrollments.items.find(item => item.course.id === courseId);
      
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseDetailPage.tsx:64',message:'checkEnrollment RESULT',data:{hasEnrollment:!!enrollment,enrollmentId:enrollment?.enrollment?.id},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
      // #endregion
      
      const isEnrolled = !!enrollment;
      setIsEnrolledChecked(isEnrolled);
      
      // Eğer kayıtlıysa progress'i yükle
      if (isEnrolled) {
        loadProgress();
      }
    } catch (error) {
      console.error('Error checking enrollment:', error);
      setIsEnrolledChecked(false);
    }
  };

  const loadProgress = async () => {
    if (!courseId) return;

    try {
      const progressData = await EducationService.getUserProgress(courseId).catch(() => null);
      if (progressData) {
        setProgress({
          progressPercentage: progressData.progressPercentage,
        });
      } else {
        setProgress(null);
      }
    } catch (error) {
      console.error('Error loading progress:', error);
      setProgress(null);
    }
  };

  const handleEnrollmentChange = () => {
    // Kurs bilgilerini yeniden yükle
    loadCourse();
    
    // Global event dispatch - CoursesListPage'de dinlenecek
    window.dispatchEvent(new CustomEvent('course-enrollment-changed', {
      detail: { courseId }
    }));
  };

  const handleStartCourse = () => {
    if (courseId) {
      navigate(`/education/courses/${courseId}/roadmap`);
    }
  };

  const handleStepClick = (stepId: string) => {
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseDetailPage.tsx:74',message:'handleStepClick ENTRY',data:{courseId,stepId,isEnrolled},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
    // #endregion
    
    if (!courseId || !course?.roadmap || !isEnrolled) {
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseDetailPage.tsx:79',message:'handleStepClick BLOCKED',data:{hasCourseId:!!courseId,hasRoadmap:!!course?.roadmap,isEnrolled},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
      // #endregion
      return;
    }

    const step = course.roadmap.steps.find(s => s.id === stepId);
    if (!step || step.contents.length === 0) {
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseDetailPage.tsx:86',message:'handleStepClick NO CONTENT',data:{hasStep:!!step,contentsLength:step?.contents?.length||0},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
      // #endregion
      return;
    }

    // İlk içeriğe yönlendir
    const firstContent = step.contents[0];
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CourseDetailPage.tsx:93',message:'handleStepClick NAVIGATE',data:{firstContentId:firstContent.id,contentType:firstContent.type},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
    // #endregion
    navigate(`/education/courses/${courseId}/steps/${stepId}/contents/${firstContent.id}`);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-[#8B5CF6]" />
      </div>
    );
  }

  if (!course) {
    return null;
  }

  // API'den gelen userEnrollment bilgisi güvenilir değil, getUserEnrollments ile kontrol ediyoruz
  const isEnrolled = isEnrolledChecked || course.userEnrollment?.isEnrolled || false;
  const hasStarted = !!course.userEnrollment?.startedAt;
  const isCompleted = !!course.userEnrollment?.completedAt;
  // Progress bilgisini getUserProgress API'sinden alıyoruz
  const progressPercentage = progress?.progressPercentage || course.userEnrollment?.progressPercentage || 0;

  // Toplam adım sayısı
  const totalSteps = course.roadmap?.steps.length || 0;
  const estimatedDuration = course.roadmap?.estimatedDurationMinutes || 0;

  // Süreyi formatla
  const formatDuration = (minutes: number) => {
    if (minutes < 60) {
      return `${minutes} dk`;
    }
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return mins > 0 ? `${hours} saat ${mins} dk` : `${hours} saat`;
  };

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
              <BreadcrumbPage className="text-[#E5E7EB]">{course.title}</BreadcrumbPage>
            </BreadcrumbList>
          </Breadcrumb>

          {/* Back Button */}
          <Button
            variant="ghost"
            onClick={() => navigate('/education')}
            className="mb-6 text-[#9CA3AF] hover:text-[#E5E7EB]"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Geri Dön
          </Button>

          {/* Hero Section */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="relative mb-8"
          >
            {/* Thumbnail with Gradient Overlay */}
            {course.thumbnailUrl ? (
              <div className="relative w-full h-80 rounded-2xl overflow-hidden mb-6 group">
                <img
                  src={course.thumbnailUrl}
                  alt={course.title}
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-[#0D1117] via-[#0D1117]/50 to-transparent" />
                {isEnrolled && progressPercentage > 0 && (
                  <div className="absolute bottom-0 left-0 right-0 p-6">
                    <ProgressBar
                      percentage={progressPercentage}
                      label="Kurs İlerlemesi"
                      showPercentage={true}
                    />
                  </div>
                )}
              </div>
            ) : (
              <div className="relative w-full h-64 rounded-2xl overflow-hidden mb-6 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 border border-[#30363D]/50 flex items-center justify-center">
                <BookOpen className="w-24 h-24 text-[#8B5CF6]/30" />
                {isEnrolled && progressPercentage > 0 && (
                  <div className="absolute bottom-0 left-0 right-0 p-6 bg-[#161B22]/80 backdrop-blur-sm">
                    <ProgressBar
                      percentage={progressPercentage}
                      label="Kurs İlerlemesi"
                      showPercentage={true}
                    />
                  </div>
                )}
              </div>
            )}

            {/* Course Info Card */}
            <div className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8">
              <h1 className="text-3xl sm:text-4xl font-bold text-[#E5E7EB] mb-4">
                {course.title}
              </h1>

              <p className="text-[#9CA3AF] text-lg mb-6 leading-relaxed">
                {course.description}
              </p>

              {/* Course Stats */}
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-6">
                <motion.div
                  initial={{ opacity: 0, scale: 0.9 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ delay: 0.1 }}
                  className="flex items-center gap-3 p-3 bg-[#0D1117] rounded-lg border border-[#30363D]/50"
                >
                  <div className="p-2 bg-[#8B5CF6]/20 rounded-lg">
                    <BookOpen className="w-5 h-5 text-[#8B5CF6]" />
                  </div>
                  <div>
                    <p className="text-sm text-[#9CA3AF]">Adımlar</p>
                    <p className="text-lg font-semibold text-[#E5E7EB]">{totalSteps}</p>
                  </div>
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, scale: 0.9 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ delay: 0.2 }}
                  className="flex items-center gap-3 p-3 bg-[#0D1117] rounded-lg border border-[#30363D]/50"
                >
                  <div className="p-2 bg-[#2DD4BF]/20 rounded-lg">
                    <Clock className="w-5 h-5 text-[#2DD4BF]" />
                  </div>
                  <div>
                    <p className="text-sm text-[#9CA3AF]">Süre</p>
                    <p className="text-lg font-semibold text-[#E5E7EB]">{formatDuration(estimatedDuration)}</p>
                  </div>
                </motion.div>

                {isEnrolled && (
                  <motion.div
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    transition={{ delay: 0.3 }}
                    className="flex items-center gap-3 p-3 bg-[#0D1117] rounded-lg border border-[#30363D]/50"
                  >
                    <div className="p-2 bg-[#10B981]/20 rounded-lg">
                      <TrendingUp className="w-5 h-5 text-[#10B981]" />
                    </div>
                    <div>
                      <p className="text-sm text-[#9CA3AF]">İlerleme</p>
                      <p className="text-lg font-semibold text-[#E5E7EB]">{Math.round(progressPercentage)}%</p>
                    </div>
                  </motion.div>
                )}

                {isCompleted && (
                  <motion.div
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    transition={{ delay: 0.4 }}
                    className="flex items-center gap-3 p-3 bg-[#0D1117] rounded-lg border border-[#10B981]/50"
                  >
                    <div className="p-2 bg-[#10B981]/20 rounded-lg">
                      <CheckCircle2 className="w-5 h-5 text-[#10B981]" />
                    </div>
                    <div>
                      <p className="text-sm text-[#9CA3AF]">Durum</p>
                      <p className="text-lg font-semibold text-[#10B981]">Tamamlandı</p>
                    </div>
                  </motion.div>
                )}
              </div>

              {/* Action Buttons */}
              <div className="flex flex-col sm:flex-row gap-4">
                {isEnrolled ? (
                  <>
                    {!isCompleted && (
                      <Button
                        onClick={handleStartCourse}
                        className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white shadow-lg shadow-[#8B5CF6]/20"
                        size="lg"
                      >
                        <Play className="w-4 h-4 mr-2" />
                        {hasStarted ? 'Devam Et' : 'Kursa Başla'}
                      </Button>
                    )}
                    <Button
                      variant="outline"
                      onClick={handleStartCourse}
                      className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:border-[#8B5CF6]/50"
                      size="lg"
                    >
                      <BookOpen className="w-4 h-4 mr-2" />
                      Roadmap'i Görüntüle
                    </Button>
                    {isCompleted && (
                      <div className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-[#10B981]/20 to-[#10B981]/10 border border-[#10B981]/30 rounded-lg">
                        <CheckCircle2 className="w-5 h-5 text-[#10B981]" />
                        <span className="text-[#10B981] font-semibold">Kurs Tamamlandı!</span>
                      </div>
                    )}
                  </>
                ) : (
                  <EnrollmentButton
                    courseId={course.id}
                    isEnrolled={isEnrolled}
                    hasStarted={hasStarted}
                    onEnrollmentChange={handleEnrollmentChange}
                  />
                )}
              </div>
            </div>
          </motion.div>

          {/* Roadmap Preview */}
          {course.roadmap && (
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.2 }}
              className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8"
            >
              <h2 className="text-2xl font-bold text-[#E5E7EB] mb-4">
                {course.roadmap.title}
              </h2>
              {course.roadmap.description && (
                <p className="text-[#9CA3AF] mb-6">
                  {course.roadmap.description}
                </p>
              )}

              {/* Steps List */}
              <div className="space-y-3">
                {course.roadmap.steps.map((step, index) => (
                  <motion.div
                    key={step.id}
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: index * 0.1 }}
                    className={`bg-[#0D1117] border border-[#30363D] rounded-lg p-4 transition-all duration-300 group ${
                      isEnrolled ? 'hover:border-[#8B5CF6]/50 hover:bg-[#161B22]/50 cursor-pointer' : 'cursor-not-allowed opacity-60'
                    }`}
                    onClick={() => handleStepClick(step.id)}
                  >
                    <div className="flex items-start gap-4">
                      <div className="flex-shrink-0 w-10 h-10 rounded-lg bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] flex items-center justify-center text-white font-bold shadow-lg shadow-[#8B5CF6]/30 group-hover:scale-110 transition-transform">
                        {step.order}
                      </div>
                      <div className="flex-1">
                        <h3 className="text-lg font-semibold text-[#E5E7EB] mb-1 group-hover:text-[#8B5CF6] transition-colors">
                          {step.title}
                        </h3>
                        {step.description && (
                          <p className="text-[#9CA3AF] text-sm mb-3 line-clamp-2">
                            {step.description}
                          </p>
                        )}
                        <div className="flex items-center gap-4 text-sm">
                          <span className="text-[#9CA3AF] flex items-center gap-1">
                            <BookOpen className="w-4 h-4" />
                            {step.contents.length} İçerik
                          </span>
                          <span className="text-[#9CA3AF] flex items-center gap-1">
                            <Clock className="w-4 h-4" />
                            {formatDuration(step.estimatedDurationMinutes)}
                          </span>
                          {step.isRequired && (
                            <span className="px-2 py-1 bg-[#8B5CF6]/20 text-[#8B5CF6] rounded text-xs font-medium">
                              Zorunlu
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                  </motion.div>
                ))}
              </div>
            </motion.div>
          )}
        </div>
      </div>
    </div>
  );
}

