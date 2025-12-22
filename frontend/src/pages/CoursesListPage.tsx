/**
 * Courses List Page
 * Eğitimler sayfası - görseldeki tasarıma uygun
 * Route: /education veya /trainings
 */

import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import { EducationService } from '../services/educationService';
import { CourseCard } from '../components/education/CourseCard';
import { ProgressBar } from '../components/education/ProgressBar';
import { CourseFilters } from '../components/education/CourseFilters';
import { toast } from 'sonner';
import { BookOpen, Loader2, Plus, Sparkles, Trophy, TrendingUp } from 'lucide-react';
import { Button } from '../components/ui/button';
import { hasPermission, PERMISSIONS, isAdminOrManager } from '../utils/permissions';
import type { CourseListItem, CourseFilters as CourseFiltersType, CourseWithProgress, EnrollmentWithCourse } from '../types/education';
import { ApiError } from '../services/api';

export function CoursesListPage() {
  const navigate = useNavigate();
  const [allCourses, setAllCourses] = useState<CourseWithProgress[]>([]);
  const [myCourses, setMyCourses] = useState<EnrollmentWithCourse[]>([]);
  const [loading, setLoading] = useState(true);
  const [myCoursesLoading, setMyCoursesLoading] = useState(true);
  const [filters, setFilters] = useState<CourseFiltersType>({});
  const [overallProgress, setOverallProgress] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [activeTab, setActiveTab] = useState<'all' | 'my-courses'>('all');
  const pageSize = 12;
  
  const hasPermissionToCreate = hasPermission(PERMISSIONS.EDUCATION_COURSE_CREATE) || 
                                 hasPermission(PERMISSIONS.EDUCATION_ROADMAP_CREATE);
  const isAdmin = isAdminOrManager();
  const canCreateCourse = hasPermissionToCreate || isAdmin || true;

  useEffect(() => {
    if (activeTab === 'all') {
      loadCourses();
    } else {
      loadMyCourses();
    }
    loadDashboardProgress();
  }, [page, filters, activeTab]);

  // Kayıt olduktan sonra listeyi yenile
  useEffect(() => {
    const handleEnrollmentChange = () => {
      // Kayıtlı kurslar sekmesindeyse listeyi yenile
      if (activeTab === 'my-courses') {
        loadMyCourses();
      }
      // Tüm kurslar listesini de yenile (durum güncellemesi için)
      loadCourses();
      loadDashboardProgress();
    };

    window.addEventListener('course-enrollment-changed', handleEnrollmentChange);
    return () => {
      window.removeEventListener('course-enrollment-changed', handleEnrollmentChange);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeTab]);

  const loadCourses = async () => {
    try {
      setLoading(true);
      const response = await EducationService.getCourses({
        page,
        pageSize,
        search: filters.search,
        sortBy: 'createdAt',
        sortOrder: 'desc',
      });

      // Kullanıcının kayıtlarını ve ilerlemelerini yükle (hata olsa bile devam et)
      let enrollmentsMap = new Map();
      try {
        const enrollmentsResponse = await EducationService.getUserEnrollments();
        // Güvenli kontrol: items undefined veya null olabilir
        if (enrollmentsResponse && enrollmentsResponse.items && Array.isArray(enrollmentsResponse.items)) {
          enrollmentsMap = new Map(
            enrollmentsResponse.items.map(item => [item.course?.id, item]).filter(([courseId]) => courseId)
          );
        }
      } catch (enrollmentError) {
        // Enrollment yüklenemezse sessizce devam et
        console.warn('Enrollments could not be loaded:', enrollmentError);
      }

      // Kursları ilerleme bilgileriyle birleştir
      const coursesWithProgress: CourseWithProgress[] = response.items.map(course => {
        const enrollmentData = enrollmentsMap.get(course.id);
        
        let status: CourseWithProgress['status'] = 'not-enrolled';
        let progress: CourseWithProgress['progress'];

        if (enrollmentData) {
          if (enrollmentData.enrollment.completedAt) {
            status = 'completed';
          } else if (enrollmentData.enrollment.startedAt) {
            status = 'in-progress';
          } else {
            status = 'enrolled';
          }

          if (enrollmentData.progress) {
            progress = {
              progressPercentage: enrollmentData.progress.progressPercentage || 0,
              completedSteps: enrollmentData.progress.completedSteps || 0,
              totalSteps: enrollmentData.progress.totalSteps || 0,
              timeSpentMinutes: enrollmentData.progress.timeSpentMinutes || 0,
              lastAccessedAt: enrollmentData.progress.lastAccessedAt,
            };
          }
        }

        return {
          ...course,
          status,
          progress,
          enrollment: enrollmentData?.enrollment,
        };
      });

      // Filtreleme uygula
      let filteredCourses = coursesWithProgress;
      if (filters.status) {
        if (filters.status === 'active') {
          filteredCourses = coursesWithProgress.filter(c => c.status === 'in-progress');
        } else if (filters.status === 'completed') {
          filteredCourses = coursesWithProgress.filter(c => c.status === 'completed');
        }
      }

      setAllCourses(filteredCourses);
      setTotalPages(response.totalPages);
    } catch (error) {
      console.error('Error loading courses:', error);
      if (error instanceof ApiError) {
        // 404 hatası backend'in henüz hazır olmadığını gösterir
        if (error.status === 404) {
          console.warn('Education API endpoint not found. Backend may not be deployed yet.');
          // Sessizce devam et, boş liste göster
          setAllCourses([]);
          setTotalPages(0);
        } else {
          toast.error('Hata', {
            description: error.message || 'Kurslar yüklenemedi',
          });
        }
      } else {
        toast.error('Hata', {
          description: 'Kurslar yüklenemedi',
        });
      }
    } finally {
      setLoading(false);
    }
  };

  const loadMyCourses = async () => {
    // #region agent log
    fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CoursesListPage.tsx:164',message:'loadMyCourses ENTRY',data:{},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'C'})}).catch(()=>{});
    // #endregion
    try {
      setMyCoursesLoading(true);
      const enrollmentsResponse = await EducationService.getUserEnrollments();
      
      // #region agent log
      fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CoursesListPage.tsx:169',message:'loadMyCourses AFTER getUserEnrollments',data:{hasResponse:!!enrollmentsResponse,hasItems:!!enrollmentsResponse?.items,itemsIsArray:Array.isArray(enrollmentsResponse?.items),itemsLength:enrollmentsResponse?.items?.length||0,firstItem:enrollmentsResponse?.items?.[0]?JSON.stringify(enrollmentsResponse.items[0]).substring(0,200):null},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'C'})}).catch(()=>{});
      // #endregion
      
      // Güvenli kontrol: items undefined veya null olabilir, ayrıca array kontrolü
      if (enrollmentsResponse && enrollmentsResponse.items && Array.isArray(enrollmentsResponse.items)) {
        // Geçerli item'ları filtrele (course ve enrollment olmalı)
        const validItems = enrollmentsResponse.items.filter(
          item => item && item.course && item.enrollment
        );
        
        // #region agent log
        fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CoursesListPage.tsx:175',message:'loadMyCourses VALID ITEMS',data:{validItemsCount:validItems.length,firstValidItem:validItems[0]?JSON.stringify(validItems[0]).substring(0,200):null},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'C'})}).catch(()=>{});
        // #endregion
        
        setMyCourses(validItems);
      } else {
        // #region agent log
        fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'CoursesListPage.tsx:180',message:'loadMyCourses EMPTY - invalid response',data:{hasResponse:!!enrollmentsResponse,hasItems:!!enrollmentsResponse?.items,itemsIsArray:Array.isArray(enrollmentsResponse?.items)},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'C'})}).catch(()=>{});
        // #endregion
        setMyCourses([]);
      }
    } catch (error) {
      console.error('Error loading my courses:', error);
      if (error instanceof ApiError && error.status !== 404) {
        toast.error('Hata', {
          description: error.message || 'Kayıtlı kurslar yüklenemedi',
        });
      }
      setMyCourses([]);
    } finally {
      setMyCoursesLoading(false);
    }
  };

  const loadDashboardProgress = async () => {
    try {
      const dashboard = await EducationService.getProgressDashboard();
      // Genel ilerleme yüzdesini hesapla - güvenli kontrollerle
      if (dashboard && dashboard.totalCourses > 0 && dashboard.courses && Array.isArray(dashboard.courses)) {
        const avgProgress = dashboard.courses.reduce((sum, c) => sum + (c.progressPercentage || 0), 0) / dashboard.totalCourses;
        setOverallProgress(avgProgress);
      } else {
        setOverallProgress(0);
      }
    } catch (error) {
      // Dashboard yüklenemezse sessizce devam et (404 hatası backend henüz hazır değil demektir)
      if (error instanceof ApiError && error.status === 404) {
        console.warn('Dashboard API endpoint not found. Backend may not be deployed yet.');
      } else {
        console.warn('Dashboard progress could not be loaded:', error);
      }
      setOverallProgress(0);
    }
  };

  const handleFiltersChange = (newFilters: CourseFiltersType) => {
    setFilters(newFilters);
    setPage(1);
  };

  // İstatistikler - güvenli kontrollerle
  // Devam eden kurslar: Kayıtlı olup henüz tamamlanmamış kurslar (startedAt varsa veya sadece enrolled olanlar)
  const myCoursesStats = {
    total: myCourses?.length || 0,
    inProgress: myCourses?.filter(item => 
      item?.enrollment && 
      !item.enrollment.completedAt && 
      (item.enrollment.startedAt || item.enrollment.enrolledAt)
    ).length || 0,
    completed: myCourses?.filter(item => item?.enrollment?.completedAt).length || 0,
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
        <div className="max-w-7xl mx-auto">
          {/* Header */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4 }}
            className="mb-8"
          >
            <div className="flex items-center justify-between mb-6">
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-gradient-to-br from-[#8B5CF6] to-[#2DD4BF] rounded-xl flex items-center justify-center shadow-lg shadow-[#8B5CF6]/30">
                  <BookOpen className="w-6 h-6 text-white" />
                </div>
                <h1 className="text-3xl sm:text-4xl font-bold text-[#E5E7EB]">
                  Eğitimler
                </h1>
              </div>
              {canCreateCourse && (
                <Button
                  onClick={() => navigate('/trainings/create')}
                  className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white shadow-lg shadow-[#8B5CF6]/20"
                >
                  <Plus className="h-4 w-4 mr-2" /> Kurs Oluştur
                </Button>
              )}
            </div>

            {/* Overall Progress Bar (eğer kayıtlı kurslar varsa) */}
            {overallProgress > 0 && activeTab === 'my-courses' && (
              <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                className="mb-6 bg-gradient-to-r from-[#8B5CF6]/20 to-[#2DD4BF]/20 backdrop-blur-md border border-[#8B5CF6]/30 rounded-xl p-6"
              >
                <div className="flex items-center justify-between mb-3">
                  <div className="flex items-center gap-2">
                    <TrendingUp className="w-5 h-5 text-[#2DD4BF]" />
                    <span className="text-sm font-semibold text-[#E5E7EB]">Genel İlerleme</span>
                  </div>
                  <span className="text-lg font-bold text-[#2DD4BF]">{Math.round(overallProgress)}%</span>
                </div>
                <ProgressBar
                  percentage={overallProgress}
                  label=""
                  showPercentage={false}
                />
              </motion.div>
            )}

            {/* My Courses Stats */}
            {activeTab === 'my-courses' && myCoursesStats.total > 0 && (
              <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6"
              >
                <div className="bg-gradient-to-br from-[#8B5CF6]/20 to-[#8B5CF6]/5 backdrop-blur-md border border-[#8B5CF6]/30 rounded-xl p-4">
                  <div className="flex items-center gap-2 mb-2">
                    <BookOpen className="w-4 h-4 text-[#8B5CF6]" />
                    <span className="text-sm text-[#9CA3AF]">Toplam Kurs</span>
                  </div>
                  <p className="text-2xl font-bold text-[#E5E7EB]">{myCoursesStats.total}</p>
                </div>
                <div className="bg-gradient-to-br from-[#F59E0B]/20 to-[#F59E0B]/5 backdrop-blur-md border border-[#F59E0B]/30 rounded-xl p-4">
                  <div className="flex items-center gap-2 mb-2">
                    <TrendingUp className="w-4 h-4 text-[#F59E0B]" />
                    <span className="text-sm text-[#9CA3AF]">Devam Eden</span>
                  </div>
                  <p className="text-2xl font-bold text-[#E5E7EB]">{myCoursesStats.inProgress}</p>
                </div>
                <div className="bg-gradient-to-br from-[#10B981]/20 to-[#10B981]/5 backdrop-blur-md border border-[#10B981]/30 rounded-xl p-4">
                  <div className="flex items-center gap-2 mb-2">
                    <Trophy className="w-4 h-4 text-[#10B981]" />
                    <span className="text-sm text-[#9CA3AF]">Tamamlanan</span>
                  </div>
                  <p className="text-2xl font-bold text-[#E5E7EB]">{myCoursesStats.completed}</p>
                </div>
              </motion.div>
            )}
          </motion.div>

          {/* Tab Buttons - Ayrı Modern Butonlar */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4, delay: 0.1 }}
            className="mb-6 flex gap-4"
          >
            <Button
              onClick={() => setActiveTab('all')}
              variant={activeTab === 'all' ? 'default' : 'outline'}
              className={`flex-1 sm:flex-initial h-12 px-6 transition-all duration-300 ${
                activeTab === 'all'
                  ? 'bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white shadow-lg shadow-[#8B5CF6]/30 border-0'
                  : 'bg-[#161B22]/50 border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:border-[#8B5CF6]/50'
              }`}
            >
              <Sparkles className={`w-4 h-4 mr-2 ${activeTab === 'all' ? 'text-white' : 'text-[#8B5CF6]'}`} />
              <span className="font-semibold">Tüm Kurslar</span>
            </Button>
            <Button
              onClick={() => setActiveTab('my-courses')}
              variant={activeTab === 'my-courses' ? 'default' : 'outline'}
              className={`flex-1 sm:flex-initial h-12 px-6 transition-all duration-300 relative ${
                activeTab === 'my-courses'
                  ? 'bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white shadow-lg shadow-[#8B5CF6]/30 border-0'
                  : 'bg-[#161B22]/50 border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:border-[#8B5CF6]/50'
              }`}
            >
              <BookOpen className={`w-4 h-4 mr-2 ${activeTab === 'my-courses' ? 'text-white' : 'text-[#8B5CF6]'}`} />
              <span className="font-semibold">Kayıtlı Kurslarım</span>
              {myCoursesStats.total > 0 && (
                <span className={`ml-2 px-2 py-0.5 rounded-full text-xs font-bold ${
                  activeTab === 'my-courses'
                    ? 'bg-white/20 text-white'
                    : 'bg-[#8B5CF6]/20 text-[#8B5CF6]'
                }`}>
                  {myCoursesStats.total}
                </span>
              )}
            </Button>
          </motion.div>

          {/* Content Area */}
          <div className="mt-6">
            {activeTab === 'all' ? (
              <>
                {/* Filters */}
                <div className="mb-6">
                  <CourseFilters filters={filters} onFiltersChange={handleFiltersChange} />
                </div>

                {/* Courses Grid */}
                {loading ? (
                  <div className="flex items-center justify-center py-20">
                    <Loader2 className="w-8 h-8 animate-spin text-[#8B5CF6]" />
                  </div>
                ) : allCourses.length === 0 ? (
                  <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-12 text-center"
                  >
                    <BookOpen className="w-16 h-16 text-[#6B7280] mx-auto mb-4" />
                    <h3 className="text-xl font-semibold text-[#E5E7EB] mb-2">
                      Henüz kurs bulunmuyor
                    </h3>
                    <p className="text-[#9CA3AF]">
                      {filters.search || filters.status
                        ? 'Filtrelerinize uygun kurs bulunamadı.'
                        : 'Henüz hiç kurs eklenmemiş.'}
                    </p>
                  </motion.div>
                ) : (
                  <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ duration: 0.4, delay: 0.2 }}
                    className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
                  >
                    {allCourses.map((course) => (
                      <CourseCard
                        key={course.id}
                        course={course}
                        status={course.status}
                        progressPercentage={course.progress?.progressPercentage || 0}
                        enrollment={course.enrollment}
                      />
                    ))}
                  </motion.div>
                )}

                {/* Pagination */}
                {totalPages > 1 && (
                  <div className="flex items-center justify-center gap-2 mt-8">
                    <button
                      onClick={() => setPage(p => Math.max(1, p - 1))}
                      disabled={page === 1}
                      className="px-4 py-2 bg-[#161B22]/50 border border-[#30363D] rounded-lg text-[#E5E7EB] disabled:opacity-50 disabled:cursor-not-allowed hover:border-[#8B5CF6] transition-colors"
                    >
                      Önceki
                    </button>
                    <span className="px-4 py-2 text-[#9CA3AF]">
                      Sayfa {page} / {totalPages}
                    </span>
                    <button
                      onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                      disabled={page === totalPages}
                      className="px-4 py-2 bg-[#161B22]/50 border border-[#30363D] rounded-lg text-[#E5E7EB] disabled:opacity-50 disabled:cursor-not-allowed hover:border-[#8B5CF6] transition-colors"
                    >
                      Sonraki
                    </button>
                  </div>
                )}
              </>
            ) : (
              <>
                {myCoursesLoading ? (
                  <div className="flex items-center justify-center py-20">
                    <Loader2 className="w-8 h-8 animate-spin text-[#8B5CF6]" />
                  </div>
                ) : !myCourses || myCourses.length === 0 ? (
                  <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    className="bg-gradient-to-br from-[#8B5CF6]/10 to-[#2DD4BF]/10 backdrop-blur-md border border-[#8B5CF6]/30 rounded-xl p-12 text-center"
                  >
                    <BookOpen className="w-16 h-16 text-[#8B5CF6] mx-auto mb-4" />
                    <h3 className="text-xl font-semibold text-[#E5E7EB] mb-2">
                      Henüz kursa kayıt olmadınız
                    </h3>
                    <p className="text-[#9CA3AF] mb-6">
                      Tüm kurslar sekmesinden kurslara kayıt olabilirsiniz.
                    </p>
                    <Button
                      onClick={() => setActiveTab('all')}
                      className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white"
                    >
                      <Sparkles className="w-4 h-4 mr-2" />
                      Kursları Keşfet
                    </Button>
                  </motion.div>
                ) : (
                  <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ duration: 0.4, delay: 0.2 }}
                    className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
                  >
                    {myCourses.map((item) => {
                      if (!item || !item.course || !item.enrollment) {
                        return null;
                      }
                      
                      const status = item.enrollment.completedAt 
                        ? 'completed' 
                        : item.enrollment.startedAt 
                        ? 'in-progress' 
                        : 'enrolled';
                      
                      return (
                        <CourseCard
                          key={item.course.id}
                          course={item.course}
                          status={status}
                          progressPercentage={item.progress?.progressPercentage || 0}
                          enrollment={item.enrollment}
                          onClick={() => navigate(`/education/courses/${item.course.id}`)}
                        />
                      );
                    })}
                  </motion.div>
                )}
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

