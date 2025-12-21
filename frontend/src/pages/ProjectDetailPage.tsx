/**
 * Project Detail Page
 * Proje detayları ve modül listesi
 */

import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import {
  ArrowLeft,
  Edit,
  Trash2,
  Archive,
  ArchiveRestore,
  Plus,
  Search,
  Filter,
  Eye,
  Calendar,
  Folder,
  Package,
} from 'lucide-react';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '../components/ui/dialog';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '../components/ui/alert-dialog';
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
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbSeparator,
} from '../components/ui/breadcrumb';
import { projectService } from '../services/projectService';
import { moduleService } from '../services/moduleService';
import { useCaseService } from '../services/useCaseService';
import { hasPermission } from '../utils/permissions';
import { Project, Module, UseCase, Task, CreateModuleRequest, UpdateProjectRequest } from '../types';
import { toast } from 'sonner';
import { ApiError } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';
import { FileText, CheckSquare, ChevronRight } from 'lucide-react';

interface ProjectStage {
  module: Module;
  useCases: Array<{
    useCase: UseCase;
    tasks: Task[];
  }>;
}

export const ProjectDetailPage: React.FC = () => {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const [project, setProject] = useState<Project | null>(null);
  const [modules, setModules] = useState<Module[]>([]);
  const [projectStages, setProjectStages] = useState<ProjectStage[]>([]);
  const [loading, setLoading] = useState(true);
  const [modulesLoading, setModulesLoading] = useState(true);
  const [stagesLoading, setStagesLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<boolean | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isCreateModuleModalOpen, setIsCreateModuleModalOpen] = useState(false);
  const [editFormData, setEditFormData] = useState<UpdateProjectRequest>({
    title: '',
    description: '',
  });
  const [createModuleFormData, setCreateModuleFormData] =
    useState<CreateModuleRequest>({
      title: '',
      description: '',
    });
  const [isSaving, setIsSaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isCreatingModule, setIsCreatingModule] = useState(false);

  const pageSize = 20;

  // Proje detayını yükle
  const loadProject = async () => {
    if (!projectId) return;

    try {
      setLoading(true);
      const response = await projectService.getById(projectId);

      if (response.success && response.data) {
        setProject(response.data);
        setEditFormData({
          title: response.data.title,
          description: response.data.description || '',
        });
      }
    } catch (error) {
      console.error('Error loading project:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Proje yüklenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Proje yüklenirken bir hata oluştu.',
        });
      }
      navigate('/projects');
    } finally {
      setLoading(false);
    }
  };

  // Modülleri yükle
  const loadModules = async () => {
    if (!projectId) return;

    try {
      setModulesLoading(true);
      const response = await projectService.getModules(projectId, {
        page,
        pageSize,
        isActive: statusFilter,
        search: searchTerm || undefined,
      });

      if (response.success && response.data) {
        setModules(response.data.items);
        setTotalPages(response.data.totalPages);
      }
    } catch (error) {
      console.error('Error loading modules:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Modüller yüklenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Modüller yüklenirken bir hata oluştu.',
        });
      }
    } finally {
      setModulesLoading(false);
    }
  };

  useEffect(() => {
    loadProject();
  }, [projectId]);

  useEffect(() => {
    if (project) {
      loadModules();
      loadProjectStages();
    }
  }, [project, page, statusFilter, searchTerm]);

  // Proje aşamalarını yükle (tüm modüller, use case'ler ve task'lar)
  const loadProjectStages = async () => {
    if (!projectId) return;

    try {
      setStagesLoading(true);
      
      // Tüm modülleri yükle (pagination olmadan)
      const modulesResponse = await projectService.getModules(projectId, {
        page: 1,
        pageSize: 100, // Tüm modülleri al
        isActive: null,
        search: undefined,
      });

      if (!modulesResponse.success || !modulesResponse.data) {
        return;
      }

      const allModules = modulesResponse.data.items;
      const stages: ProjectStage[] = [];

      // Her modül için use case'leri ve task'ları yükle
      for (const module of allModules) {
        const useCasesResponse = await moduleService.getUseCases(module.id, {
          page: 1,
          pageSize: 100, // Tüm use case'leri al
          isActive: null,
          search: undefined,
        });

        const useCasesWithTasks: Array<{ useCase: UseCase; tasks: Task[] }> = [];

        if (useCasesResponse.success && useCasesResponse.data) {
          for (const useCase of useCasesResponse.data.items) {
            const tasksResponse = await useCaseService.getTasks(useCase.id, {
              page: 1,
              pageSize: 100, // Tüm task'ları al
              state: null,
              type: null,
              assigneeId: null,
              dueDate: null,
              search: undefined,
            });

            useCasesWithTasks.push({
              useCase,
              tasks: tasksResponse.success && tasksResponse.data ? tasksResponse.data.items : [],
            });
          }
        }

        stages.push({
          module,
          useCases: useCasesWithTasks,
        });
      }

      setProjectStages(stages);
    } catch (error) {
      console.error('Error loading project stages:', error);
      // Hata durumunda sessizce devam et
    } finally {
      setStagesLoading(false);
    }
  };

  // Proje güncelle
  const handleUpdateProject = async () => {
    if (!projectId || !editFormData.title?.trim()) {
      toast.error('Hata', {
        description: 'Proje başlığı zorunludur.',
      });
      return;
    }

    try {
      setIsSaving(true);
      const response = await projectService.update(projectId, editFormData);

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: 'Proje başarıyla güncellendi.',
        });
        setIsEditModalOpen(false);
        loadProject();
      }
    } catch (error) {
      console.error('Error updating project:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Proje güncellenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Proje güncellenirken bir hata oluştu.',
        });
      }
    } finally {
      setIsSaving(false);
    }
  };

  // Proje durumunu değiştir
  const handleChangeStatus = async () => {
    if (!projectId || !project) return;

    try {
      const response = await projectService.changeStatus(projectId, {
        isActive: !project.isActive,
      });

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: `Proje ${project.isActive ? 'arşivlendi' : 'aktifleştirildi'}.`,
        });
        loadProject();
      }
    } catch (error) {
      console.error('Error changing status:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Durum değiştirilirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Durum değiştirilirken bir hata oluştu.',
        });
      }
    }
  };

  // Proje sil
  const handleDeleteProject = async () => {
    if (!projectId) return;

    try {
      setIsDeleting(true);
      const response = await projectService.delete(projectId);

      if (response.success) {
        toast.success('Başarılı', {
          description: 'Proje başarıyla silindi.',
        });
        navigate('/projects');
      }
    } catch (error) {
      console.error('Error deleting project:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Proje silinirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Proje silinirken bir hata oluştu.',
        });
      }
    } finally {
      setIsDeleting(false);
      setIsDeleteDialogOpen(false);
    }
  };

  // Modül oluştur
  const handleCreateModule = async () => {
    if (!projectId || !createModuleFormData.title.trim()) {
      toast.error('Hata', {
        description: 'Modül başlığı zorunludur.',
      });
      return;
    }

    try {
      setIsCreatingModule(true);
      const response = await projectService.createModule(
        projectId,
        createModuleFormData
      );

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: 'Modül başarıyla oluşturuldu.',
        });
        setIsCreateModuleModalOpen(false);
        setCreateModuleFormData({ title: '', description: '' });
        loadModules();
      }
    } catch (error) {
      console.error('Error creating module:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Modül oluşturulurken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Modül oluşturulurken bir hata oluştu.',
        });
      }
    } finally {
      setIsCreatingModule(false);
    }
  };

  const canUpdate = hasPermission('ProjectUpdate');
  const canDelete = hasPermission('ProjectDelete');
  const canCreateModule = hasPermission('ModuleCreate');

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#161B22] to-[#0D1117] flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-2 border-[#8B5CF6] border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-[#9CA3AF]">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (!project) {
    return null;
  }

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
      </div>

      <div className="container mx-auto px-6 py-24 relative z-10">
        {/* Breadcrumb */}
        <motion.div
          initial={{ y: -20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5 }}
          className="mb-6"
        >
          <Breadcrumb>
            <BreadcrumbList className="text-[#9CA3AF]">
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link to="/projects" className="hover:text-[#8B5CF6]">
                    Projects
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem className="text-[#E5E7EB]">
                {project.title}
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </motion.div>

        {/* Back Button */}
        <motion.div
          initial={{ y: -20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.1 }}
          className="mb-6"
        >
          <Button
            variant="ghost"
            onClick={() => navigate('/projects')}
            className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Back to Projects
          </Button>
        </motion.div>

        {/* Project Info Card */}
        <motion.div
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.2 }}
          className="bg-[#161B22] border border-[#30363D] rounded-xl p-6 mb-8"
        >
          <div className="flex items-start justify-between mb-4">
            <div className="flex-1">
              <div className="flex items-center gap-3 mb-2">
                <Folder className="w-8 h-8 text-[#8B5CF6]" />
                <h1 className="text-3xl font-bold text-[#E5E7EB]">
                  {project.title}
                </h1>
                <Badge
                  variant={project.isActive ? 'default' : 'secondary'}
                  className={
                    project.isActive
                      ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                      : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                  }
                >
                  {project.isActive ? 'Active' : 'Archived'}
                </Badge>
              </div>
              {project.description && (
                <p className="text-[#9CA3AF] mb-4">{project.description}</p>
              )}
              <div className="flex items-center gap-4 text-sm text-[#9CA3AF]">
                <div className="flex items-center">
                  <Calendar className="w-4 h-4 mr-2" />
                  Started: {new Date(project.startedDate).toLocaleDateString('tr-TR')}
                </div>
              </div>
            </div>
            <div className="flex gap-2">
              {canUpdate && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setIsEditModalOpen(true)}
                  className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
                >
                  <Edit className="w-4 h-4 mr-2" />
                  Edit
                </Button>
              )}
              <Button
                variant="outline"
                size="sm"
                onClick={handleChangeStatus}
                className={
                  project.isActive
                    ? 'bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]'
                    : 'bg-[#10B981]/10 border-[#10B981]/30 text-[#10B981] hover:bg-[#10B981]/20'
                }
              >
                {project.isActive ? (
                  <>
                    <Archive className="w-4 h-4 mr-2" />
                    Archive
                  </>
                ) : (
                  <>
                    <ArchiveRestore className="w-4 h-4 mr-2" />
                    Activate
                  </>
                )}
              </Button>
              {canDelete && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setIsDeleteDialogOpen(true)}
                  className="bg-[#EF4444]/10 border-[#EF4444]/30 text-[#EF4444] hover:bg-[#EF4444]/20"
                >
                  <Trash2 className="w-4 h-4 mr-2" />
                  Delete
                </Button>
              )}
            </div>
          </div>
        </motion.div>

        {/* Project Stages Section */}
        {projectStages.length > 0 && (
          <motion.div
            initial={{ y: 20, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ duration: 0.5, delay: 0.25 }}
            className="mb-8"
          >
            <div className="mb-6">
              <h2 className="text-2xl font-bold text-[#E5E7EB] mb-2 flex items-center gap-2">
                <Folder className="w-6 h-6 text-[#8B5CF6]" />
                {t('project.stages.title')}
              </h2>
              <p className="text-[#9CA3AF]">
                {t('project.stages.subtitle')}
              </p>
            </div>

            {stagesLoading ? (
              <div className="bg-[#161B22] border border-[#30363D] rounded-xl p-8">
                <div className="text-center">
                  <div className="w-8 h-8 border-2 border-[#8B5CF6] border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
                  <p className="text-[#9CA3AF]">{t('common.loading')}</p>
                </div>
              </div>
            ) : (
              <div className="space-y-4">
                {projectStages.map((stage, stageIndex) => (
                  <motion.div
                    key={stage.module.id}
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.4, delay: stageIndex * 0.1 }}
                    className="bg-[#161B22] border border-[#30363D] rounded-xl overflow-hidden"
                  >
                    {/* Module Header - Mor Renk */}
                    <div className="bg-gradient-to-r from-[#8B5CF6]/20 to-[#8B5CF6]/10 border-b border-[#8B5CF6]/30 p-4">
                      <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3">
                          <div className="w-10 h-10 bg-[#8B5CF6] rounded-lg flex items-center justify-center">
                            <Package className="w-5 h-5 text-white" />
                          </div>
                          <div>
                            <h3 className="text-lg font-semibold text-[#E5E7EB]">
                              {stage.module.title}
                            </h3>
                            {stage.module.description && (
                              <p className="text-sm text-[#9CA3AF] mt-1">
                                {stage.module.description}
                              </p>
                            )}
                          </div>
                        </div>
                        <Badge
                          variant={stage.module.isActive ? 'default' : 'secondary'}
                          className={
                            stage.module.isActive
                              ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                              : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                          }
                        >
                          {stage.module.isActive ? t('projects.status.active') : t('projects.status.archived')}
                        </Badge>
                      </div>
                    </div>

                    {/* Use Cases - Turkuaz Renk */}
                    {stage.useCases.length > 0 ? (
                      <div className="p-4 space-y-3">
                        {stage.useCases.map((useCaseData, useCaseIndex) => (
                          <motion.div
                            key={useCaseData.useCase.id}
                            initial={{ opacity: 0, x: -20 }}
                            animate={{ opacity: 1, x: 0 }}
                            transition={{ duration: 0.3, delay: useCaseIndex * 0.05 }}
                            className="bg-gradient-to-r from-[#2DD4BF]/10 to-[#2DD4BF]/5 border border-[#2DD4BF]/30 rounded-lg p-4 ml-6"
                          >
                            <div className="flex items-center justify-between mb-3">
                              <div className="flex items-center gap-2">
                                <div className="w-8 h-8 bg-[#2DD4BF] rounded-lg flex items-center justify-center">
                                  <FileText className="w-4 h-4 text-white" />
                                </div>
                                <div>
                                  <h4 className="text-base font-medium text-[#E5E7EB]">
                                    {useCaseData.useCase.title}
                                  </h4>
                                  {useCaseData.useCase.description && (
                                    <p className="text-xs text-[#9CA3AF] mt-1">
                                      {useCaseData.useCase.description}
                                    </p>
                                  )}
                                </div>
                              </div>
                              <Badge
                                variant={useCaseData.useCase.isActive ? 'default' : 'secondary'}
                                className={
                                  useCaseData.useCase.isActive
                                    ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                                    : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                                }
                              >
                                {useCaseData.useCase.isActive ? t('projects.status.active') : t('projects.status.archived')}
                              </Badge>
                            </div>

                            {/* Tasks - Pembe Renk */}
                            {useCaseData.tasks.length > 0 ? (
                              <div className="ml-6 space-y-2 mt-3">
                                {useCaseData.tasks.map((task, taskIndex) => (
                                  <motion.div
                                    key={task.id}
                                    initial={{ opacity: 0, x: -20 }}
                                    animate={{ opacity: 1, x: 0 }}
                                    transition={{ duration: 0.2, delay: taskIndex * 0.03 }}
                                    className="bg-gradient-to-r from-[#EC4899]/10 to-[#EC4899]/5 border border-[#EC4899]/30 rounded-lg p-3 cursor-pointer hover:border-[#EC4899]/50 hover:shadow-lg hover:shadow-[#EC4899]/10 transition-all"
                                    onClick={() =>
                                      navigate(
                                        `/projects/${projectId}/modules/${stage.module.id}/usecases/${useCaseData.useCase.id}/tasks/${task.id}`
                                      )
                                    }
                                  >
                                    <div className="flex items-center justify-between">
                                      <div className="flex items-center gap-2 flex-1">
                                        <div className="w-6 h-6 bg-[#EC4899] rounded flex items-center justify-center">
                                          <CheckSquare className="w-3 h-3 text-white" />
                                        </div>
                                        <span className="text-sm font-medium text-[#E5E7EB]">
                                          {task.title}
                                        </span>
                                      </div>
                                      <div className="flex items-center gap-2">
                                        <Badge
                                          className={
                                            task.state === 'Completed'
                                              ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                                              : task.state === 'InProgress'
                                              ? 'bg-[#3B82F6]/10 text-[#3B82F6] border-[#3B82F6]/30'
                                              : task.state === 'Cancelled'
                                              ? 'bg-[#EF4444]/10 text-[#EF4444] border-[#EF4444]/30'
                                              : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                                          }
                                        >
                                          {task.state}
                                        </Badge>
                                        <ChevronRight className="w-4 h-4 text-[#9CA3AF]" />
                                      </div>
                                    </div>
                                  </motion.div>
                                ))}
                              </div>
                            ) : (
                              <div className="ml-6 mt-2 text-sm text-[#6B7280] italic">
                                {t('project.stages.noTasks')}
                              </div>
                            )}
                          </motion.div>
                        ))}
                      </div>
                    ) : (
                      <div className="p-4 ml-6 text-sm text-[#6B7280] italic">
                        {t('project.stages.noUseCases')}
                      </div>
                    )}
                  </motion.div>
                ))}
              </div>
            )}
          </motion.div>
        )}

        {/* Modules Section */}
        <motion.div
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.3 }}
        >
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="text-2xl font-bold text-[#E5E7EB] mb-2 flex items-center gap-2">
                <Package className="w-6 h-6 text-[#2DD4BF]" />
                Modules
              </h2>
              <p className="text-[#9CA3AF]">
                Manage modules for this project
              </p>
            </div>
            {canCreateModule && (
              <Button
                onClick={() => setIsCreateModuleModalOpen(true)}
                className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90 shadow-lg shadow-[#8B5CF6]/25"
              >
                <Plus className="w-4 h-4 mr-2" />
                Create Module
              </Button>
            )}
          </div>

          {/* Filters */}
          <div className="mb-6 flex flex-col sm:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-[#9CA3AF]" />
              <Input
                placeholder="Search modules..."
                value={searchTerm}
                onChange={(e) => {
                  setSearchTerm(e.target.value);
                  setPage(1);
                }}
                className="pl-10 bg-[#161B22] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
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
                <SelectValue placeholder="Filter by status" />
              </SelectTrigger>
              <SelectContent className="bg-[#161B22] border-[#30363D]">
                <SelectItem value="all">All Modules</SelectItem>
                <SelectItem value="active">Active</SelectItem>
                <SelectItem value="archived">Archived</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Modules List */}
          {modulesLoading ? (
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
          ) : modules.length === 0 ? (
            <div className="text-center py-16 bg-[#161B22] border border-[#30363D] rounded-xl">
              <Package className="w-16 h-16 text-[#6B7280] mx-auto mb-4" />
              <p className="text-[#9CA3AF] text-lg">
                {searchTerm || statusFilter !== null
                  ? 'No modules found matching your filters.'
                  : 'No modules yet. Create your first module!'}
              </p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {modules.map((module, index) => (
                <motion.div
                  key={module.id}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.4, delay: index * 0.1 }}
                  whileHover={{ scale: 1.02, y: -4 }}
                  className="bg-[#161B22] border border-[#30363D] rounded-xl p-6 hover:border-[#2DD4BF]/50 hover:shadow-lg hover:shadow-[#2DD4BF]/10 transition-all cursor-pointer group"
                  onClick={() =>
                    navigate(`/projects/${projectId}/modules/${module.id}`)
                  }
                >
                  <div className="flex items-start justify-between mb-4">
                    <h3 className="text-xl font-semibold text-[#E5E7EB] group-hover:text-[#2DD4BF] transition-colors">
                      {module.title}
                    </h3>
                    <Badge
                      variant={module.isActive ? 'default' : 'secondary'}
                      className={
                        module.isActive
                          ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                          : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                      }
                    >
                      {module.isActive ? 'Active' : 'Archived'}
                    </Badge>
                  </div>
                  {module.description && (
                    <p className="text-[#9CA3AF] text-sm mb-4 line-clamp-2">
                      {module.description}
                    </p>
                  )}
                  <div className="flex items-center justify-between">
                    <div className="text-[#9CA3AF] text-sm">
                      {module.useCaseCount || 0} Use Cases
                    </div>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="text-[#2DD4BF] hover:text-[#2DD4BF] hover:bg-[#2DD4BF]/10"
                      onClick={(e) => {
                        e.stopPropagation();
                        navigate(`/projects/${projectId}/modules/${module.id}`);
                      }}
                    >
                      <Eye className="w-4 h-4 mr-2" />
                      View
                    </Button>
                  </div>
                </motion.div>
              ))}
            </div>
          )}

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-8 flex justify-center gap-2">
              <Button
                variant="outline"
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
                className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
              >
                Previous
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
                Next
              </Button>
            </div>
          )}
        </motion.div>
      </div>

      {/* Edit Project Modal */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
              Edit Project
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div>
              <Label htmlFor="edit-title" className="text-[#E5E7EB]">
                Title <span className="text-[#EF4444]">*</span>
              </Label>
              <Input
                id="edit-title"
                value={editFormData.title}
                onChange={(e) =>
                  setEditFormData({ ...editFormData, title: e.target.value })
                }
                placeholder="Enter project title"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                maxLength={120}
              />
            </div>
            <div>
              <Label htmlFor="edit-description" className="text-[#E5E7EB]">
                Description
              </Label>
              <Textarea
                id="edit-description"
                value={editFormData.description}
                onChange={(e) =>
                  setEditFormData({
                    ...editFormData,
                    description: e.target.value,
                  })
                }
                placeholder="Enter project description"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={4}
                maxLength={1000}
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsEditModalOpen(false)}
              className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
            >
              Cancel
            </Button>
            <Button
              onClick={handleUpdateProject}
              disabled={isSaving || !editFormData.title?.trim()}
              className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
            >
              {isSaving ? 'Saving...' : 'Save'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Project</AlertDialogTitle>
            <AlertDialogDescription className="text-[#9CA3AF]">
              Are you sure you want to delete this project? This action cannot be
              undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]">
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteProject}
              disabled={isDeleting}
              className="bg-[#EF4444] text-white hover:bg-[#EF4444]/90"
            >
              {isDeleting ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Create Module Modal */}
      <Dialog
        open={isCreateModuleModalOpen}
        onOpenChange={setIsCreateModuleModalOpen}
      >
        <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
              Create New Module
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div>
              <Label htmlFor="module-title" className="text-[#E5E7EB]">
                Title <span className="text-[#EF4444]">*</span>
              </Label>
              <Input
                id="module-title"
                value={createModuleFormData.title}
                onChange={(e) =>
                  setCreateModuleFormData({
                    ...createModuleFormData,
                    title: e.target.value,
                  })
                }
                placeholder="Enter module title"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                maxLength={100}
              />
            </div>
            <div>
              <Label htmlFor="module-description" className="text-[#E5E7EB]">
                Description
              </Label>
              <Textarea
                id="module-description"
                value={createModuleFormData.description}
                onChange={(e) =>
                  setCreateModuleFormData({
                    ...createModuleFormData,
                    description: e.target.value,
                  })
                }
                placeholder="Enter module description"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={4}
                maxLength={1000}
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsCreateModuleModalOpen(false)}
              className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
            >
              Cancel
            </Button>
            <Button
              onClick={handleCreateModule}
              disabled={isCreatingModule || !createModuleFormData.title.trim()}
              className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
            >
              {isCreatingModule ? 'Creating...' : 'Create'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

