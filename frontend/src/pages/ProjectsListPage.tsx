/**
 * Projects List Page - Modern Glassmorphism Design
 * Route: /projects
 */
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'motion/react';
import { ProjectService } from '../services/projectService';
import type { Project, ProjectFilters } from '../types';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Badge } from '../components/ui/badge';
import { toast } from 'sonner';
import { Plus, Search, Eye, Calendar, Archive, Filter } from 'lucide-react';

export function ProjectsListPage() {
  const navigate = useNavigate();
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState<ProjectFilters>({
    page: 1,
    pageSize: 20,
    isActive: undefined,
    search: '',
  });
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    loadProjects();
  }, [filters]);

  const loadProjects = async () => {
    try {
      setLoading(true);
      const response = await ProjectService.getProjects(filters);
      setProjects(response.items);
      setTotalPages(response.totalPages);
    } catch (error: any) {
      toast.error('Projeler yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setLoading(false);
    }
  };


  const handleSearch = (value: string) => {
    setFilters({ ...filters, search: value, page: 1 });
  };

  const handleFilterChange = (value: string) => {
    setFilters({
      ...filters,
      isActive: value === 'all' ? undefined : value === 'active',
      page: 1,
    });
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

      {/* Main Content Container - pt-24 ile üst bar için padding */}
      <div className="relative z-10 pt-24 px-6 pb-12">
        <div className="max-w-7xl mx-auto">
          {/* Control Panel - Title Row with Create Project Card */}
          <div className="mb-8">
            {/* Title and Create Project Card Row */}
            <div className="flex items-center gap-4 mb-4">
              {/* Title */}
              <h1 className="text-2xl font-semibold text-[#E5E7EB] flex-shrink-0">
                Projelerim
              </h1>

              {/* Create New Project Card - Başlık Hizasında, Sol Tarafa Yakın */}
              <motion.div
                initial={{ opacity: 0, scale: 0.9 }}
                animate={{ opacity: 1, scale: 1 }}
                transition={{ duration: 0.4 }}
                whileHover={{ scale: 1.02, y: -2 }}
                onClick={() => navigate('/projects/new')}
                className="group relative bg-gradient-to-br from-[#161B22]/40 to-[#161B22]/60 backdrop-blur-sm border-2 border-dashed border-[#8B5CF6]/30 rounded-xl px-6 py-3 cursor-pointer overflow-hidden transition-all duration-300 hover:border-[#8B5CF6] hover:shadow-[0_0_30px_rgba(139,92,246,0.2)]"
              >
                {/* Hover Gradient Overlay */}
                <div className="absolute inset-0 bg-gradient-to-br from-[#8B5CF6]/10 to-[#2DD4BF]/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300" />

                <div className="relative z-10 flex items-center gap-3">
                  {/* Animated Plus Icon */}
                  <motion.div
                    whileHover={{ rotate: 90, scale: 1.1 }}
                    transition={{ duration: 0.3 }}
                    className="w-10 h-10 bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] rounded-lg flex items-center justify-center shadow-lg shadow-[#8B5CF6]/30 flex-shrink-0"
                  >
                    <Plus className="w-5 h-5 text-white" />
                  </motion.div>

                  <div className="text-left">
                    <h3 className="text-base font-semibold text-[#E5E7EB] group-hover:text-white transition-colors">
                      Yeni Proje Oluştur
                    </h3>
                    <p className="text-[#9CA3AF] text-xs">
                      Hemen başlayın
                    </p>
                  </div>
                </div>
              </motion.div>
            </div>

            {/* Search and Filter Row */}
            <motion.div
              initial={{ opacity: 0, y: -10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.3, delay: 0.1 }}
              className="flex flex-col sm:flex-row gap-3 bg-[#161B22]/40 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-4"
            >
              {/* Search Input */}
              <div className="relative flex-1 lg:flex-initial lg:w-80">
                <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-[#6B7280] w-4 h-4 pointer-events-none" />
                <Input
                  placeholder="Proje ara..."
                  value={filters.search || ''}
                  onChange={(e) => handleSearch(e.target.value)}
                  className="pl-11 pr-4 bg-[#0D1117]/60 backdrop-blur-sm border-[#30363D]/80 text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#8B5CF6] focus:ring-1 focus:ring-[#8B5CF6]/30 transition-all h-10 rounded-lg"
                />
              </div>

              {/* Filter Select */}
              <Select
                value={filters.isActive === undefined ? 'all' : filters.isActive ? 'active' : 'archived'}
                onValueChange={handleFilterChange}
              >
                <SelectTrigger className="w-full sm:w-[140px] bg-[#0D1117]/60 backdrop-blur-sm border-[#30363D]/80 text-[#E5E7EB] h-10 rounded-lg">
                  <Filter className="w-4 h-4 mr-2 text-[#6B7280]" />
                  <SelectValue placeholder="Filtre" />
                </SelectTrigger>
                <SelectContent className="bg-[#161B22] border-[#30363D]">
                  <SelectItem value="all">Tümü</SelectItem>
                  <SelectItem value="active">Aktif</SelectItem>
                  <SelectItem value="archived">Arşivlenmiş</SelectItem>
                </SelectContent>
              </Select>
            </motion.div>
          </div>

          {/* Projects Grid */}
          {loading ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {[1, 2, 3, 4, 5, 6].map((i) => (
                <motion.div
                  key={i}
                  initial={{ opacity: 0, scale: 0.9 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ duration: 0.3, delay: i * 0.05 }}
                  className="bg-[#161B22]/60 backdrop-blur-sm border border-[#30363D] rounded-2xl p-6 h-48 animate-pulse"
                />
              ))}
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {/* Project Cards */}
              <AnimatePresence>
                {projects.map((project, index) => (
                  <motion.div
                    key={project.id}
                    initial={{ opacity: 0, scale: 0.95, y: 20 }}
                    animate={{ opacity: 1, scale: 1, y: 0 }}
                    exit={{ opacity: 0, scale: 0.95 }}
                    transition={{ duration: 0.3, delay: index * 0.03 }}
                    whileHover={{ scale: 1.01, y: -4 }}
                    onClick={() => navigate(`/projects/${project.id}`)}
                    className="group relative bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6 cursor-pointer overflow-hidden transition-all duration-300 hover:border-[#8B5CF6]/40 hover:shadow-[0_8px_30px_rgba(139,92,246,0.12)] hover:bg-[#161B22]/70"
                  >
                    {/* Top Gradient Line */}
                    <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-[#8B5CF6]/50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500" />

                    {/* Hover Gradient Overlay */}
                    <div className="absolute inset-0 bg-gradient-to-br from-[#8B5CF6]/[0.03] via-transparent to-[#2DD4BF]/[0.03] opacity-0 group-hover:opacity-100 transition-opacity duration-500" />

                    <div className="relative z-10 space-y-4">
                      {/* Header Row */}
                      <div className="flex items-start justify-between gap-3">
                        <div className="flex-1 min-w-0">
                          <h3 className="text-lg font-semibold text-[#E5E7EB] group-hover:text-white transition-colors truncate mb-1.5">
                            {project.title}
                          </h3>
                          {project.description && (
                            <p className="text-[#9CA3AF] text-sm line-clamp-2 leading-relaxed">
                              {project.description}
                            </p>
                          )}
                        </div>

                        {/* Status Badge */}
                        <Badge
                          className={`${
                            project.status === 'Active'
                              ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                              : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                          } border px-2.5 py-1 flex-shrink-0`}
                        >
                          {project.status === 'Active' ? (
                            <span className="flex items-center gap-1.5">
                              <span className="w-1.5 h-1.5 bg-[#10B981] rounded-full animate-pulse" />
                              <span className="text-xs font-medium">Aktif</span>
                            </span>
                          ) : (
                            <span className="flex items-center gap-1.5">
                              <Archive className="w-3 h-3" />
                              <span className="text-xs font-medium">Arşiv</span>
                            </span>
                          )}
                        </Badge>
                      </div>

                      {/* Footer Info */}
                      <div className="flex items-center justify-between pt-2 border-t border-[#30363D]/30">
                        <div className="flex items-center gap-2 text-xs text-[#6B7280]">
                          <Calendar className="w-3.5 h-3.5" />
                          <span>{new Date(project.createdAt).toLocaleDateString('tr-TR', {
                            day: 'numeric',
                            month: 'short',
                            year: 'numeric'
                          })}</span>
                        </div>

                        {/* View Action */}
                        <div className="flex items-center gap-1.5 text-[#8B5CF6] opacity-0 group-hover:opacity-100 transition-opacity">
                          <span className="text-xs font-medium">Görüntüle</span>
                          <Eye className="w-3.5 h-3.5" />
                        </div>
                      </div>
                    </div>

                    {/* Subtle Corner Accent */}
                    <div className="absolute top-0 right-0 w-24 h-24 bg-gradient-to-br from-[#8B5CF6]/5 to-transparent rounded-bl-full opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
                  </motion.div>
                ))}
              </AnimatePresence>
            </div>
          )}

          {/* Empty State */}
          {!loading && projects.length === 0 && (
            <motion.div
              initial={{ opacity: 0, scale: 0.9 }}
              animate={{ opacity: 1, scale: 1 }}
              transition={{ duration: 0.5 }}
              className="text-center py-20"
            >
              <div className="bg-[#161B22]/40 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-16 max-w-lg mx-auto">
                <motion.div
                  className="w-24 h-24 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-2xl flex items-center justify-center mx-auto mb-6 shadow-lg shadow-[#8B5CF6]/10"
                  animate={{
                    scale: [1, 1.05, 1],
                  }}
                  transition={{
                    duration: 2,
                    repeat: Infinity,
                    ease: "easeInOut",
                  }}
                >
                  <Plus className="w-12 h-12 text-[#8B5CF6]" />
                </motion.div>

                <h3 className="text-2xl font-semibold text-[#E5E7EB] mb-3">
                  Henüz proje yok
                </h3>
                <p className="text-[#9CA3AF] mb-8 text-base">
                  İlk projenizi oluşturarak yolculuğunuza başlayın
                </p>

                <Button
                  onClick={() => navigate('/projects/new')}
                  className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 hover:shadow-[#8B5CF6]/50 transition-all"
                >
                  <Plus className="w-4 h-4 mr-2" />
                  İlk Projeyi Oluştur
                </Button>
              </div>
            </motion.div>
          )}
        </div>
      </div>
    </div>
  );
}
