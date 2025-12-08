/**
 * Projects List Page
 * Tüm projelerin listelendiği sayfa
 */

import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import { Plus, Search, Filter, Eye, Calendar, Folder, Sparkles, FileText, AlertCircle } from 'lucide-react';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from '../components/ui/dialog';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../components/ui/select';
import { Badge } from '../components/ui/badge';
import { projectService } from '../services/projectService';
import { hasPermission } from '../utils/permissions';
import { Project, CreateProjectRequest } from '../types';
import { toast } from 'sonner';
import { ApiError } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';
import { cn } from '../components/ui/utils';

export const ProjectsListPage: React.FC = () => {
  const navigate = useNavigate();
  const { t } = useLanguage();
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<boolean | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [createFormData, setCreateFormData] = useState<CreateProjectRequest>({
    title: '',
    description: '',
  });
  const [isCreating, setIsCreating] = useState(false);
  const [titleError, setTitleError] = useState<string>('');
  const [descriptionError, setDescriptionError] = useState<string>('');

  const pageSize = 20;

  // Real-time validasyon
  const validateTitle = (value: string) => {
    if (!value.trim()) {
      setTitleError(t('projects.titleRequired'));
      return false;
    }
    if (value.length < 3) {
      setTitleError(t('projects.titleMinLength'));
      return false;
    }
    if (value.length > 120) {
      setTitleError(t('projects.titleMaxLength'));
      return false;
    }
    setTitleError('');
    return true;
  };

  const validateDescription = (value: string) => {
    if (value && value.length > 1000) {
      setDescriptionError(t('projects.descriptionMaxLength'));
      return false;
    }
    setDescriptionError('');
    return true;
  };

  // Projeleri yükle
  const loadProjects = async () => {
    try {
      setLoading(true);
      const response = await projectService.list({
        page,
        pageSize,
        isActive: statusFilter,
        search: searchTerm || undefined,
      });

      if (response.success && response.data) {
        setProjects(response.data.items);
        setTotalPages(response.data.totalPages);
      }
    } catch (error) {
      console.error('Error loading projects:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('projects.loadError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('projects.loadError'),
        });
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProjects();
  }, [page, statusFilter, searchTerm]);

  // Yeni proje oluştur
  const handleCreateProject = async () => {
    const isTitleValid = validateTitle(createFormData.title);
    const isDescriptionValid = validateDescription(createFormData.description || '');

    if (!isTitleValid || !isDescriptionValid) {
      return;
    }

    try {
      setIsCreating(true);
      const response = await projectService.create(createFormData);

      if (response.success && response.data) {
        toast.success(t('common.success'), {
          description: t('projects.createSuccess'),
        });
        setIsCreateModalOpen(false);
        setCreateFormData({ title: '', description: '' });
        loadProjects();
      }
    } catch (error) {
      console.error('Error creating project:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('projects.createError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('projects.createError'),
        });
      }
    } finally {
      setIsCreating(false);
    }
  };

  // Yetkilendirme şimdilik devre dışı - herkes proje oluşturabilir
  const canCreate = true; // hasPermission('ProjectCreate');

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#161B22] to-[#0D1117] relative overflow-hidden">
      {/* Animated Background Gradients */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <motion.div
          className="absolute top-0 left-1/4 w-96 h-96 bg-[#8B5CF6]/20 rounded-full blur-3xl"
          animate={{
            x: [0, 100, 0],
            y: [0, -50, 0],
            scale: [1, 1.2, 1],
          }}
          transition={{
            duration: 20,
            repeat: Infinity,
            ease: 'easeInOut',
          }}
        />
        <motion.div
          className="absolute bottom-0 right-1/4 w-96 h-96 bg-[#2DD4BF]/20 rounded-full blur-3xl"
          animate={{
            x: [0, -100, 0],
            y: [0, 50, 0],
            scale: [1, 1.2, 1],
          }}
          transition={{
            duration: 25,
            repeat: Infinity,
            ease: 'easeInOut',
          }}
        />
        <motion.div
          className="absolute top-1/2 left-1/2 w-96 h-96 bg-[#EC4899]/10 rounded-full blur-3xl"
          animate={{
            scale: [1, 1.3, 1],
            opacity: [0.3, 0.5, 0.3],
          }}
          transition={{
            duration: 15,
            repeat: Infinity,
            ease: 'easeInOut',
          }}
        />
      </div>

      <div className="container mx-auto px-6 py-24 relative z-10">
        {/* Header */}
        <motion.div
          initial={{ y: -20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5 }}
          className="mb-8"
        >
          <div className="flex items-center justify-between flex-wrap gap-4">
            <div>
              <h1 className="text-4xl font-bold text-[#E5E7EB] mb-2 flex items-center gap-3">
                <Folder className="w-10 h-10 text-[#8B5CF6]" />
                {t('projects.title')}
              </h1>
              <p className="text-[#9CA3AF]">
                {t('projects.subtitle')}
              </p>
            </div>
            <motion.div
              initial={{ opacity: 0, x: 20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.5, delay: 0.2 }}
            >
              <Button
                onClick={() => {
                  if (!canCreate) {
                    toast.error(t('common.error'), {
                      description: t('projects.noPermission'),
                    });
                    return;
                  }
                  setIsCreateModalOpen(true);
                }}
                disabled={!canCreate}
                className={cn(
                  "relative text-white shadow-lg transition-all duration-300 overflow-hidden group",
                  canCreate
                    ? "bg-gradient-to-r from-[#8B5CF6] to-[#EC4899] hover:from-[#8B5CF6]/90 hover:to-[#EC4899]/90 shadow-[#8B5CF6]/25 hover:shadow-[#8B5CF6]/40 hover:scale-105"
                    : "bg-[#6B7280] cursor-not-allowed opacity-60"
                )}
              >
                <motion.div
                  className="absolute inset-0 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF] opacity-0 group-hover:opacity-20 transition-opacity duration-300"
                  animate={canCreate ? {
                    x: ['-100%', '100%'],
                  } : {}}
                  transition={canCreate ? {
                    duration: 2,
                    repeat: Infinity,
                    ease: 'linear'
                  } : {}}
                />
                <motion.div
                  animate={canCreate ? {
                    rotate: [0, 90, 0],
                  } : {}}
                  transition={canCreate ? {
                    duration: 0.3,
                    ease: 'easeInOut'
                  } : {}}
                >
                  <Plus className="w-4 h-4 mr-2 relative z-10" />
                </motion.div>
                <span className="relative z-10">{t('projects.create')}</span>
                {!canCreate && (
                  <span className="ml-2 text-xs opacity-75 relative z-10">
                    ({t('projects.noPermission')})
                  </span>
                )}
              </Button>
            </motion.div>
          </div>
        </motion.div>

        {/* Filters and Search */}
        <motion.div
          initial={{ y: -20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.1 }}
          className="mb-6 flex flex-col sm:flex-row gap-4"
        >
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-[#9CA3AF]" />
            <Input
              placeholder={t('projects.search')}
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setPage(1);
              }}
              className="pl-10 bg-[#161B22] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6] focus:ring-[#8B5CF6]/20"
            />
          </div>
          <Select
            value={
              statusFilter === null
                ? 'all'
                : statusFilter
                  ? 'active'
                  : 'archived'
            }
            onValueChange={(value) => {
              setStatusFilter(
                value === 'all' ? null : value === 'active' ? true : false
              );
              setPage(1);
            }}
          >
            <SelectTrigger className="w-full sm:w-[180px] bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
              <Filter className="w-4 h-4 mr-2" />
              <SelectValue placeholder={t('common.filter')} />
            </SelectTrigger>
            <SelectContent className="bg-[#161B22] border-[#30363D]">
              <SelectItem value="all">{t('projects.filter.all')}</SelectItem>
              <SelectItem value="active">{t('projects.filter.active')}</SelectItem>
              <SelectItem value="archived">{t('projects.filter.archived')}</SelectItem>
            </SelectContent>
          </Select>
        </motion.div>

        {/* Projects List */}
        {loading ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[...Array(6)].map((_, i) => (
              <div
                key={i}
                className="bg-[#161B22] border border-[#30363D] rounded-xl p-6 animate-pulse"
              >
                <div className="h-6 bg-[#21262D] rounded mb-4"></div>
                <div className="h-4 bg-[#21262D] rounded mb-2"></div>
                <div className="h-4 bg-[#21262D] rounded w-2/3"></div>
              </div>
            ))}
          </div>
        ) : projects.length === 0 ? (
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="text-center py-16"
          >
            <Folder className="w-16 h-16 text-[#6B7280] mx-auto mb-4" />
            <p className="text-[#9CA3AF] text-lg">
              {searchTerm || statusFilter !== null
                ? t('projects.noResults')
                : t('projects.noProjects')}
            </p>
          </motion.div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {projects.map((project, index) => (
              <motion.div
                key={project.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4, delay: index * 0.1 }}
                whileHover={{ scale: 1.02, y: -4 }}
                className="bg-[#161B22] border border-[#30363D] rounded-xl p-6 hover:border-[#8B5CF6]/50 hover:shadow-lg hover:shadow-[#8B5CF6]/10 transition-all cursor-pointer group"
                onClick={() => navigate(`/projects/${project.id}`)}
              >
                <div className="flex items-start justify-between mb-4">
                  <h3 className="text-xl font-semibold text-[#E5E7EB] group-hover:text-[#8B5CF6] transition-colors">
                    {project.title}
                  </h3>
                  <Badge
                    variant={project.isActive ? 'default' : 'secondary'}
                    className={
                      project.isActive
                        ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                        : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                    }
                  >
                    {project.isActive ? t('projects.status.active') : t('projects.status.archived')}
                  </Badge>
                </div>
                {project.description && (
                  <p className="text-[#9CA3AF] text-sm mb-4 line-clamp-2">
                    {project.description}
                  </p>
                )}
                <div className="flex items-center justify-between">
                  <div className="flex items-center text-[#9CA3AF] text-sm">
                    <Calendar className="w-4 h-4 mr-2" />
                    {new Date(project.startedDate).toLocaleDateString('tr-TR')}
                  </div>
                    <Button
                    variant="ghost"
                    size="sm"
                    className="text-[#8B5CF6] hover:text-[#8B5CF6] hover:bg-[#8B5CF6]/10"
                    onClick={(e) => {
                      e.stopPropagation();
                      navigate(`/projects/${project.id}`);
                    }}
                  >
                    <Eye className="w-4 h-4 mr-2" />
                    {t('projects.view')}
                  </Button>
                </div>
              </motion.div>
            ))}
          </div>
        )}

        {/* Pagination */}
        {totalPages > 1 && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.3 }}
            className="mt-8 flex justify-center gap-2"
          >
            <Button
              variant="outline"
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={page === 1}
              className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
            >
              {t('common.previous')}
            </Button>
            <div className="flex items-center gap-2">
              {[...Array(totalPages)].map((_, i) => {
                const pageNum = i + 1;
                if (
                  pageNum === 1 ||
                  pageNum === totalPages ||
                  (pageNum >= page - 1 && pageNum <= page + 1)
                ) {
                  return (
                    <Button
                      key={pageNum}
                      variant={page === pageNum ? 'default' : 'outline'}
                      onClick={() => setPage(pageNum)}
                      className={
                        page === pageNum
                          ? 'bg-[#8B5CF6] text-white'
                          : 'bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]'
                      }
                    >
                      {pageNum}
                    </Button>
                  );
                } else if (pageNum === page - 2 || pageNum === page + 2) {
                  return <span key={pageNum} className="text-[#9CA3AF]">...</span>;
                }
                return null;
              })}
            </div>
            <Button
              variant="outline"
              onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
            >
              {t('common.next')}
            </Button>
          </motion.div>
        )}
      </div>

      {/* Create Project Modal */}
      <Dialog open={isCreateModalOpen} onOpenChange={setIsCreateModalOpen}>
        <DialogContent className="bg-gradient-to-br from-[#161B22] via-[#0D1117] to-[#161B22] border-2 border-transparent bg-clip-padding !z-[9999] max-w-3xl p-0 overflow-hidden">
          {/* Gradient Border Effect */}
          <div className="absolute inset-0 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF] opacity-20 blur-xl -z-10" />
          <div className="absolute top-0 left-0 right-0 h-1 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF]" />
          
          {/* Animated Background */}
          <div className="absolute inset-0 overflow-hidden pointer-events-none">
            <motion.div
              className="absolute top-0 right-0 w-64 h-64 bg-[#8B5CF6]/10 rounded-full blur-3xl"
              animate={{
                scale: [1, 1.2, 1],
                opacity: [0.3, 0.5, 0.3],
              }}
              transition={{
                duration: 4,
                repeat: Infinity,
                ease: 'easeInOut',
              }}
            />
            <motion.div
              className="absolute bottom-0 left-0 w-64 h-64 bg-[#EC4899]/10 rounded-full blur-3xl"
              animate={{
                scale: [1, 1.3, 1],
                opacity: [0.3, 0.5, 0.3],
              }}
              transition={{
                duration: 5,
                repeat: Infinity,
                ease: 'easeInOut',
              }}
            />
          </div>

          <div className="relative z-10 p-8">
            <DialogHeader className="mb-6">
              <motion.div
                initial={{ opacity: 0, y: -20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3 }}
                className="flex items-center gap-4 mb-4"
              >
                <div className="w-14 h-14 bg-gradient-to-br from-[#8B5CF6] to-[#EC4899] rounded-xl flex items-center justify-center shadow-lg shadow-[#8B5CF6]/30">
                  <Sparkles className="w-7 h-7 text-white" />
                </div>
                <div>
                  <DialogTitle className="text-3xl font-bold bg-gradient-to-r from-[#8B5CF6] to-[#EC4899] bg-clip-text text-transparent">
                    {t('projects.createModal.title')}
                  </DialogTitle>
                  <DialogDescription className="text-[#9CA3AF] mt-1 text-base">
                    {t('projects.createModal.description')}
                  </DialogDescription>
                </div>
              </motion.div>
            </DialogHeader>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.4, delay: 0.1 }}
              className="space-y-6"
            >
              {/* Title Field */}
              <div className="space-y-2">
                <Label htmlFor="title" className="text-[#E5E7EB] text-base font-semibold flex items-center gap-2">
                  <FileText className="w-4 h-4 text-[#8B5CF6]" />
                  {t('projects.createModal.titleLabel')}
                  <span className="text-[#EF4444]">*</span>
                </Label>
                <Input
                  id="title"
                  value={createFormData.title}
                  onChange={(e) =>
                    setCreateFormData({ ...createFormData, title: e.target.value })
                  }
                  placeholder={t('projects.createModal.titlePlaceholder')}
                  className="h-12 bg-[#21262D] border-2 border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 focus:shadow-[0_0_20px_rgba(139,92,246,0.3)] transition-all text-base"
                  maxLength={120}
                />
                <div className="flex items-center justify-between">
                  <p className="text-xs text-[#9CA3AF]">
                    {createFormData.title.length < 3 ? (
                      <span className="text-[#EF4444]">
                        {t('projects.createModal.minLength')} (3-120 {t('common.characters')})
                      </span>
                    ) : (
                      <span className="text-[#10B981]">
                        {t('projects.createModal.valid')}
                      </span>
                    )}
                  </p>
                  <p className="text-xs text-[#9CA3AF]">
                    {createFormData.title.length}/120 {t('common.characters')}
                  </p>
                </div>
              </div>

              {/* Description Field */}
              <div className="space-y-2">
                <Label htmlFor="description" className="text-[#E5E7EB] text-base font-semibold flex items-center gap-2">
                  <FileText className="w-4 h-4 text-[#2DD4BF]" />
                  {t('projects.createModal.descriptionLabel')}
                </Label>
                <Textarea
                  id="description"
                  value={createFormData.description}
                  onChange={(e) =>
                    setCreateFormData({
                      ...createFormData,
                      description: e.target.value,
                    })
                  }
                  placeholder={t('projects.createModal.descriptionPlaceholder')}
                  className="min-h-[120px] bg-[#21262D] border-2 border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#2DD4BF] focus:ring-2 focus:ring-[#2DD4BF]/20 focus:shadow-[0_0_20px_rgba(45,212,191,0.3)] transition-all resize-none text-base"
                  rows={5}
                  maxLength={1000}
                />
                <div className="flex items-center justify-end">
                  <p className="text-xs text-[#9CA3AF]">
                    {createFormData.description?.length || 0}/1000 {t('common.characters')}
                  </p>
                </div>
              </div>

              {/* Info Box */}
              <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ delay: 0.3 }}
                className="bg-gradient-to-r from-[#8B5CF6]/10 to-[#2DD4BF]/10 border border-[#8B5CF6]/30 rounded-xl p-4"
              >
                <div className="flex items-start gap-3">
                  <div className="w-8 h-8 bg-[#8B5CF6]/20 rounded-lg flex items-center justify-center flex-shrink-0 mt-0.5">
                    <Sparkles className="w-4 h-4 text-[#8B5CF6]" />
                  </div>
                  <div>
                    <p className="text-sm font-medium text-[#E5E7EB] mb-1">
                      {t('projects.createModal.infoTitle')}
                    </p>
                    <p className="text-xs text-[#9CA3AF]">
                      {t('projects.createModal.infoDescription')}
                    </p>
                  </div>
                </div>
              </motion.div>
            </motion.div>

            <DialogFooter className="mt-8 gap-3">
              <Button
                variant="outline"
                onClick={() => {
                  setIsCreateModalOpen(false);
                  setCreateFormData({ title: '', description: '' });
                }}
                className="bg-[#21262D] border-2 border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D] hover:border-[#6B7280] transition-all px-6"
              >
                {t('common.cancel')}
              </Button>
              <Button
                onClick={handleCreateProject}
                disabled={isCreating || !createFormData.title.trim() || createFormData.title.length < 3}
                className={cn(
                  "relative overflow-hidden px-8 transition-all duration-300",
                  isCreating || !createFormData.title.trim() || createFormData.title.length < 3
                    ? "bg-[#6B7280] text-white cursor-not-allowed opacity-60"
                    : "bg-gradient-to-r from-[#8B5CF6] to-[#EC4899] text-white hover:from-[#8B5CF6]/90 hover:to-[#EC4899]/90 shadow-lg shadow-[#8B5CF6]/25 hover:shadow-[#8B5CF6]/40 hover:scale-105"
                )}
              >
                {isCreating ? (
                  <>
                    <motion.div
                      className="absolute inset-0 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF] opacity-20"
                      animate={{
                        x: ['-100%', '100%'],
                      }}
                      transition={{
                        duration: 1.5,
                        repeat: Infinity,
                        ease: 'linear'
                      }}
                    />
                    <span className="relative z-10 flex items-center gap-2">
                      <motion.div
                        className="w-4 h-4 border-2 border-white border-t-transparent rounded-full"
                        animate={{ rotate: 360 }}
                        transition={{ duration: 1, repeat: Infinity, ease: 'linear' }}
                      />
                      {t('projects.createModal.creating')}
                    </span>
                  </>
                ) : (
                  <span className="relative z-10 flex items-center gap-2">
                    <Plus className="w-4 h-4" />
                    {t('projects.createModal.create')}
                  </span>
                )}
              </Button>
            </DialogFooter>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};

