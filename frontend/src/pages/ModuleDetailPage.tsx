/**
 * Module Detail Page
 * Modül detayları ve use case listesi
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
  Folder,
  Package,
  FileText,
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
import { moduleService } from '../services/moduleService';
import { projectService } from '../services/projectService';
import { hasPermission } from '../utils/permissions';
import {
  Module,
  Project,
  UseCase,
  CreateUseCaseRequest,
  UpdateModuleRequest,
} from '../types';
import { toast } from 'sonner';
import { ApiError } from '../services/api';

export const ModuleDetailPage: React.FC = () => {
  const { projectId, moduleId } = useParams<{
    projectId: string;
    moduleId: string;
  }>();
  const navigate = useNavigate();
  const [project, setProject] = useState<Project | null>(null);
  const [module, setModule] = useState<Module | null>(null);
  const [useCases, setUseCases] = useState<UseCase[]>([]);
  const [loading, setLoading] = useState(true);
  const [useCasesLoading, setUseCasesLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<boolean | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isCreateUseCaseModalOpen, setIsCreateUseCaseModalOpen] =
    useState(false);
  const [editFormData, setEditFormData] = useState<UpdateModuleRequest>({
    title: '',
    description: '',
  });
  const [createUseCaseFormData, setCreateUseCaseFormData] =
    useState<CreateUseCaseRequest>({
      title: '',
      description: '',
      importantNotes: '',
    });
  const [isSaving, setIsSaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isCreatingUseCase, setIsCreatingUseCase] = useState(false);

  const pageSize = 20;

  // Proje ve modül detaylarını yükle
  const loadData = async () => {
    if (!projectId || !moduleId) return;

    try {
      setLoading(true);
      const [projectResponse, moduleResponse] = await Promise.all([
        projectService.getById(projectId),
        moduleService.getById(moduleId),
      ]);

      if (projectResponse.success && projectResponse.data) {
        setProject(projectResponse.data);
      }

      if (moduleResponse.success && moduleResponse.data) {
        setModule(moduleResponse.data);
        setEditFormData({
          title: moduleResponse.data.title,
          description: moduleResponse.data.description || '',
        });
      }
    } catch (error) {
      console.error('Error loading data:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Veri yüklenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Veri yüklenirken bir hata oluştu.',
        });
      }
      navigate(`/projects/${projectId}`);
    } finally {
      setLoading(false);
    }
  };

  // Use case'leri yükle
  const loadUseCases = async () => {
    if (!moduleId) return;

    try {
      setUseCasesLoading(true);
      const response = await moduleService.getUseCases(moduleId, {
        page,
        pageSize,
        isActive: statusFilter,
        search: searchTerm || undefined,
      });

      if (response.success && response.data) {
        setUseCases(response.data.items);
        setTotalPages(response.data.totalPages);
      }
    } catch (error) {
      console.error('Error loading use cases:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Use case\'ler yüklenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Use case\'ler yüklenirken bir hata oluştu.',
        });
      }
    } finally {
      setUseCasesLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [projectId, moduleId]);

  useEffect(() => {
    if (module) {
      loadUseCases();
    }
  }, [module, page, statusFilter, searchTerm]);

  // Modül güncelle
  const handleUpdateModule = async () => {
    if (!moduleId || !editFormData.title?.trim()) {
      toast.error('Hata', {
        description: 'Modül başlığı zorunludur.',
      });
      return;
    }

    try {
      setIsSaving(true);
      const response = await moduleService.update(moduleId, editFormData);

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: 'Modül başarıyla güncellendi.',
        });
        setIsEditModalOpen(false);
        loadData();
      }
    } catch (error) {
      console.error('Error updating module:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Modül güncellenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Modül güncellenirken bir hata oluştu.',
        });
      }
    } finally {
      setIsSaving(false);
    }
  };

  // Modül durumunu değiştir
  const handleChangeStatus = async () => {
    if (!moduleId || !module) return;

    try {
      const response = await moduleService.changeStatus(moduleId, {
        isActive: !module.isActive,
      });

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: `Modül ${module.isActive ? 'arşivlendi' : 'aktifleştirildi'}.`,
        });
        loadData();
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

  // Modül sil
  const handleDeleteModule = async () => {
    if (!moduleId) return;

    try {
      setIsDeleting(true);
      const response = await moduleService.delete(moduleId);

      if (response.success) {
        toast.success('Başarılı', {
          description: 'Modül başarıyla silindi.',
        });
        navigate(`/projects/${projectId}`);
      }
    } catch (error) {
      console.error('Error deleting module:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Modül silinirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Modül silinirken bir hata oluştu.',
        });
      }
    } finally {
      setIsDeleting(false);
      setIsDeleteDialogOpen(false);
    }
  };

  // Use case oluştur
  const handleCreateUseCase = async () => {
    if (!moduleId || !createUseCaseFormData.title.trim()) {
      toast.error('Hata', {
        description: 'Use case başlığı zorunludur.',
      });
      return;
    }

    try {
      setIsCreatingUseCase(true);
      const response = await moduleService.createUseCase(
        moduleId,
        createUseCaseFormData
      );

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: 'Use case başarıyla oluşturuldu.',
        });
        setIsCreateUseCaseModalOpen(false);
        setCreateUseCaseFormData({ title: '', description: '', importantNotes: '' });
        loadUseCases();
      }
    } catch (error) {
      console.error('Error creating use case:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Use case oluşturulurken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Use case oluşturulurken bir hata oluştu.',
        });
      }
    } finally {
      setIsCreatingUseCase(false);
    }
  };

  const canUpdate = hasPermission('ModuleUpdate');
  const canDelete = hasPermission('ModuleDelete');
  const canCreateUseCase = hasPermission('UseCaseCreate');

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

  if (!module || !project) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#161B22] to-[#0D1117] relative overflow-hidden">
      {/* Animated Background Gradients */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <motion.div
          className="absolute top-0 left-1/4 w-96 h-96 bg-[#2DD4BF]/20 rounded-full blur-3xl"
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
          className="absolute bottom-0 right-1/4 w-96 h-96 bg-[#EC4899]/20 rounded-full blur-3xl"
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
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link
                    to={`/projects/${projectId}`}
                    className="hover:text-[#8B5CF6]"
                  >
                    {project.title}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem className="text-[#E5E7EB]">
                {module.title}
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
            onClick={() => navigate(`/projects/${projectId}`)}
            className="text-[#E5E7EB] hover:bg-[#2DD4BF]/10 border border-[#30363D]/50"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Back to Project
          </Button>
        </motion.div>

        {/* Module Info Card */}
        <motion.div
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.2 }}
          className="bg-[#161B22] border border-[#30363D] rounded-xl p-6 mb-8"
        >
          <div className="flex items-start justify-between mb-4">
            <div className="flex-1">
              <div className="flex items-center gap-3 mb-2">
                <Package className="w-8 h-8 text-[#2DD4BF]" />
                <h1 className="text-3xl font-bold text-[#E5E7EB]">
                  {module.title}
                </h1>
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
                <p className="text-[#9CA3AF] mb-4">{module.description}</p>
              )}
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
                  module.isActive
                    ? 'bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]'
                    : 'bg-[#10B981]/10 border-[#10B981]/30 text-[#10B981] hover:bg-[#10B981]/20'
                }
              >
                {module.isActive ? (
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

        {/* Use Cases Section */}
        <motion.div
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.3 }}
        >
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="text-2xl font-bold text-[#E5E7EB] mb-2 flex items-center gap-2">
                <FileText className="w-6 h-6 text-[#EC4899]" />
                Use Cases
              </h2>
              <p className="text-[#9CA3AF]">
                Manage use cases for this module
              </p>
            </div>
            {canCreateUseCase && (
              <Button
                onClick={() => setIsCreateUseCaseModalOpen(true)}
                className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90 shadow-lg shadow-[#8B5CF6]/25"
              >
                <Plus className="w-4 h-4 mr-2" />
                Create Use Case
              </Button>
            )}
          </div>

          {/* Filters */}
          <div className="mb-6 flex flex-col sm:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-[#9CA3AF]" />
              <Input
                placeholder="Search use cases..."
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
                <SelectItem value="all">All Use Cases</SelectItem>
                <SelectItem value="active">Active</SelectItem>
                <SelectItem value="archived">Archived</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Use Cases List */}
          {useCasesLoading ? (
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
          ) : useCases.length === 0 ? (
            <div className="text-center py-16 bg-[#161B22] border border-[#30363D] rounded-xl">
              <FileText className="w-16 h-16 text-[#6B7280] mx-auto mb-4" />
              <p className="text-[#9CA3AF] text-lg">
                {searchTerm || statusFilter !== null
                  ? 'No use cases found matching your filters.'
                  : 'No use cases yet. Create your first use case!'}
              </p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {useCases.map((useCase, index) => (
                <motion.div
                  key={useCase.id}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.4, delay: index * 0.1 }}
                  whileHover={{ scale: 1.02, y: -4 }}
                  className="bg-[#161B22] border border-[#30363D] rounded-xl p-6 hover:border-[#EC4899]/50 hover:shadow-lg hover:shadow-[#EC4899]/10 transition-all cursor-pointer group"
                  onClick={() =>
                    navigate(
                      `/projects/${projectId}/modules/${moduleId}/usecases/${useCase.id}`
                    )
                  }
                >
                  <div className="flex items-start justify-between mb-4">
                    <h3 className="text-xl font-semibold text-[#E5E7EB] group-hover:text-[#EC4899] transition-colors">
                      {useCase.title}
                    </h3>
                    <Badge
                      variant={useCase.isActive ? 'default' : 'secondary'}
                      className={
                        useCase.isActive
                          ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                          : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                      }
                    >
                      {useCase.isActive ? 'Active' : 'Archived'}
                    </Badge>
                  </div>
                  {useCase.description && (
                    <p className="text-[#9CA3AF] text-sm mb-4 line-clamp-2">
                      {useCase.description}
                    </p>
                  )}
                  <div className="flex items-center justify-between">
                    <div className="text-[#9CA3AF] text-sm">
                      {useCase.taskCount || 0} Tasks
                    </div>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="text-[#EC4899] hover:text-[#EC4899] hover:bg-[#EC4899]/10"
                      onClick={(e) => {
                        e.stopPropagation();
                        navigate(
                          `/projects/${projectId}/modules/${moduleId}/usecases/${useCase.id}`
                        );
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

      {/* Edit Module Modal */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
              Edit Module
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
                placeholder="Enter module title"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                maxLength={100}
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
              onClick={() => setIsEditModalOpen(false)}
              className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
            >
              Cancel
            </Button>
            <Button
              onClick={handleUpdateModule}
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
            <AlertDialogTitle>Delete Module</AlertDialogTitle>
            <AlertDialogDescription className="text-[#9CA3AF]">
              Are you sure you want to delete this module? This action cannot be
              undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]">
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteModule}
              disabled={isDeleting}
              className="bg-[#EF4444] text-white hover:bg-[#EF4444]/90"
            >
              {isDeleting ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Create Use Case Modal */}
      <Dialog
        open={isCreateUseCaseModalOpen}
        onOpenChange={setIsCreateUseCaseModalOpen}
      >
        <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
              Create New Use Case
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div>
              <Label htmlFor="usecase-title" className="text-[#E5E7EB]">
                Title <span className="text-[#EF4444]">*</span>
              </Label>
              <Input
                id="usecase-title"
                value={createUseCaseFormData.title}
                onChange={(e) =>
                  setCreateUseCaseFormData({
                    ...createUseCaseFormData,
                    title: e.target.value,
                  })
                }
                placeholder="Enter use case title"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                maxLength={100}
              />
            </div>
            <div>
              <Label htmlFor="usecase-description" className="text-[#E5E7EB]">
                Description
              </Label>
              <Textarea
                id="usecase-description"
                value={createUseCaseFormData.description}
                onChange={(e) =>
                  setCreateUseCaseFormData({
                    ...createUseCaseFormData,
                    description: e.target.value,
                  })
                }
                placeholder="Enter use case description"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={4}
                maxLength={1000}
              />
            </div>
            <div>
              <Label htmlFor="usecase-notes" className="text-[#E5E7EB]">
                Important Notes
              </Label>
              <Textarea
                id="usecase-notes"
                value={createUseCaseFormData.importantNotes}
                onChange={(e) =>
                  setCreateUseCaseFormData({
                    ...createUseCaseFormData,
                    importantNotes: e.target.value,
                  })
                }
                placeholder="Enter important notes"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={3}
                maxLength={500}
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsCreateUseCaseModalOpen(false)}
              className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
            >
              Cancel
            </Button>
            <Button
              onClick={handleCreateUseCase}
              disabled={isCreatingUseCase || !createUseCaseFormData.title.trim()}
              className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
            >
              {isCreatingUseCase ? 'Creating...' : 'Create'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

