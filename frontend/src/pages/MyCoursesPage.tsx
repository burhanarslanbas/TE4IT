/**
 * My Courses Page
 * Kullanıcının kayıtlı kursları dashboard'u
 * Route: /education/my-courses
 */

import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import { EducationService } from '../services/educationService';
import { CourseCard } from '../components/education/CourseCard';
import { ProgressBar } from '../components/education/ProgressBar';
import { Button } from '../components/ui/button';
import { toast } from 'sonner';
import { BookOpen, Loader2, TrendingUp, Clock, Award } from 'lucide-react';
import type { EnrollmentWithCourse, ProgressDashboardResponse } from '../types/education';
import { ApiError } from '../services/api';

export function MyCoursesPage() {
  const navigate = useNavigate();
  const [enrollments, setEnrollments] = useState<EnrollmentWithCourse[]>([]);
  const [dashboard, setDashboard] = useState<ProgressDashboardResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<'all' | 'active' | 'completed'>('all');

  useEffect(() => {
    loadData();
  }, [filter]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [enrollmentsResponse, dashboardResponse] = await Promise.all([
        EducationService.getUserEnrollments({
          status: filter === 'all' ? undefined : filter,
        }),
        EducationService.getProgressDashboard().catch(() => null),
      ]);

      setEnrollments(enrollmentsResponse.items);
      setDashboard(dashboardResponse);
    } catch (error) {
      console.error('Error loading my courses:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Kurslar yüklenemedi',
        });
      } else {
        toast.error('Hata', {
          description: 'Kurslar yüklenemedi',
        });
      }
    } finally {
      setLoading(false);
    }
  };

  const filteredEnrollments = enrollments.filter(item => {
    if (filter === 'active') {
      return !item.enrollment.completedAt;
    }
    if (filter === 'completed') {
      return !!item.enrollment.completedAt;
    }
    return true;
  });

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
        <div className="max-w-7xl mx-auto">
          {/* Header */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="mb-8"
          >
            <div className="flex items-center gap-3 mb-6">
              <div className="w-12 h-12 bg-gradient-to-br from-[#8B5CF6] to-[#2DD4BF] rounded-xl flex items-center justify-center shadow-lg shadow-[#8B5CF6]/30">
                <BookOpen className="w-6 h-6 text-white" />
              </div>
              <h1 className="text-3xl sm:text-4xl font-bold text-[#E5E7EB]">
                Eğitimlerim
              </h1>
            </div>

            {/* Statistics */}
            {dashboard && (
              <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.1 }}
                  className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6"
                >
                  <div className="flex items-center gap-3 mb-2">
                    <BookOpen className="w-5 h-5 text-[#8B5CF6]" />
                    <span className="text-sm text-[#9CA3AF]">Toplam Kurs</span>
                  </div>
                  <p className="text-2xl font-bold text-[#E5E7EB]">
                    {dashboard.totalCourses}
                  </p>
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.2 }}
                  className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6"
                >
                  <div className="flex items-center gap-3 mb-2">
                    <TrendingUp className="w-5 h-5 text-[#2DD4BF]" />
                    <span className="text-sm text-[#9CA3AF]">Aktif Kurs</span>
                  </div>
                  <p className="text-2xl font-bold text-[#E5E7EB]">
                    {dashboard.activeCourses}
                  </p>
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.3 }}
                  className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6"
                >
                  <div className="flex items-center gap-3 mb-2">
                    <Award className="w-5 h-5 text-[#10B981]" />
                    <span className="text-sm text-[#9CA3AF]">Tamamlanan</span>
                  </div>
                  <p className="text-2xl font-bold text-[#E5E7EB]">
                    {dashboard.completedCourses}
                  </p>
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.4 }}
                  className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6"
                >
                  <div className="flex items-center gap-3 mb-2">
                    <Clock className="w-5 h-5 text-[#8B5CF6]" />
                    <span className="text-sm text-[#9CA3AF]">Toplam Süre</span>
                  </div>
                  <p className="text-2xl font-bold text-[#E5E7EB]">
                    {formatDuration(dashboard.totalTimeSpentMinutes)}
                  </p>
                </motion.div>
              </div>
            )}
          </motion.div>

          {/* Filters */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.5 }}
            className="flex gap-2 mb-6"
          >
            <Button
              variant={filter === 'all' ? 'default' : 'outline'}
              onClick={() => setFilter('all')}
              className={filter === 'all' 
                ? 'bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] text-white' 
                : 'border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]'
              }
            >
              Tümü
            </Button>
            <Button
              variant={filter === 'active' ? 'default' : 'outline'}
              onClick={() => setFilter('active')}
              className={filter === 'active' 
                ? 'bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] text-white' 
                : 'border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]'
              }
            >
              Aktif
            </Button>
            <Button
              variant={filter === 'completed' ? 'default' : 'outline'}
              onClick={() => setFilter('completed')}
              className={filter === 'completed' 
                ? 'bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] text-white' 
                : 'border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]'
              }
            >
              Tamamlanan
            </Button>
          </motion.div>

          {/* Courses Grid */}
          {loading ? (
            <div className="flex items-center justify-center py-20">
              <Loader2 className="w-8 h-8 animate-spin text-[#8B5CF6]" />
            </div>
          ) : filteredEnrollments.length === 0 ? (
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-12 text-center"
            >
              <BookOpen className="w-16 h-16 text-[#6B7280] mx-auto mb-4" />
              <h3 className="text-xl font-semibold text-[#E5E7EB] mb-2">
                {filter === 'active' 
                  ? 'Aktif kursunuz bulunmuyor'
                  : filter === 'completed'
                  ? 'Tamamlanan kursunuz bulunmuyor'
                  : 'Henüz kursa kayıt olmadınız'}
              </h3>
              <p className="text-[#9CA3AF] mb-6">
                {filter === 'all' && 'Eğitimler sayfasından kurslara kayıt olabilirsiniz.'}
              </p>
              {filter === 'all' && (
                <Button
                  onClick={() => navigate('/education')}
                  className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white"
                >
                  Kursları Keşfet
                </Button>
              )}
            </motion.div>
          ) : (
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ delay: 0.6 }}
              className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
            >
              {filteredEnrollments.map((item) => {
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
        </div>
      </div>
    </div>
  );
}

