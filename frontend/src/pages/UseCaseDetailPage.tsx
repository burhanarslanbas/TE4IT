/**
 * UseCase Detail Page
 * Route: /projects/:projectId/modules/:moduleId/usecases/:useCaseId
 */
import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { UseCaseService } from '../services/useCaseService';
import { ModuleService } from '../services/moduleService';
import { ProjectService } from '../services/projectService';
import { hasPermission, PERMISSIONS } from '../utils/permissions';
import type { UseCase, Module, Project, Task } from '../types';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Badge } from '../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogTrigger, DialogFooter } from '../components/ui/dialog';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '../components/ui/tooltip';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { ArrowLeft, Edit, Trash2, Archive, ArchiveRestore, AlertTriangle, Plus, Layers, FileText, Bug, Wrench, TestTube, Search, Eye } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import { motion, AnimatePresence } from 'motion/react';
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog';
import { useTasks } from '../hooks/useTasks';
import { taskTypeConfig, taskStateConfig } from '../utils/taskHelpers';

interface EditUseCaseForm {
  title: string;
  description: string;
  importantNotes: string;
}

export function UseCaseDetailPage() {
  const { projectId, moduleId, useCaseId } = useParams<{ projectId: string; moduleId: string; useCaseId: string }>();
  const navigate = useNavigate();
  const [project, setProject] = useState<Project | null>(null);
  const [module, setModule] = useState<Module | null>(null);
  const [useCase, setUseCase] = useState<UseCase | null>(null);
  const [loading, setLoading] = useState(true);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [taskFilters, setTaskFilters] = useState({ page: 1, pageSize: 20, state: 'All' as const, type: 'All' as const, search: '' });
  
  const { register, handleSubmit, reset, formState: { errors }, watch } = useForm<EditUseCaseForm>();
  
  const descriptionLength = watch('description')?.length || 0;
  const notesLength = watch('importantNotes')?.length || 0;
  
  // Tasks hook
  const { tasks, loading: tasksLoading, refetch: refetchTasks } = useTasks(useCaseId, taskFilters);

  useEffect(() => {
    console.log('🎯 UseCaseDetailPage mounted');
    console.log('📍 Route params:', { projectId, moduleId, useCaseId });
    
    if (projectId && moduleId && useCaseId) {
      console.log('✅ All params present, loading data...');
      loadData();
    } else {
      console.error('❌ Missing route params:', { projectId, moduleId, useCaseId });
    }
  }, [projectId, moduleId, useCaseId]);

  useEffect(() => {
    if (useCase) {
      reset({
        title: useCase.title,
        description: useCase.description || '',
        importantNotes: useCase.importantNotes || '',
      });
    }
  }, [useCase, reset]);

  const loadData = async () => {
    try {
      setLoading(true);
      console.log('🔄 Loading use case data for:', { projectId, moduleId, useCaseId });
      
      const [projectData, moduleData, useCaseData] = await Promise.all([
        projectId ? ProjectService.getProject(projectId) : Promise.resolve(null),
        moduleId ? ModuleService.getModule(moduleId) : Promise.resolve(null),
        useCaseId ? UseCaseService.getUseCase(useCaseId) : Promise.resolve(null),
      ]);
      
      console.log('✅ Data loaded:', { 
        project: projectData?.title, 
        module: moduleData?.title, 
        useCase: useCaseData?.title 
      });
      
      setProject(projectData);
      setModule(moduleData);
      setUseCase(useCaseData);
    } catch (error: any) {
      console.error('❌ Error loading use case:', error);
      toast.error('Use case yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
      if (projectId && moduleId) {
        navigate(`/projects/${projectId}/modules/${moduleId}`);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleUpdate = async (data: EditUseCaseForm) => {
    if (!useCaseId) return;
    try {
      console.log('[UPDATE USECASE] Starting update for use case:', useCaseId);
      await UseCaseService.updateUseCase(useCaseId, {
        title: data.title,
        description: data.description || undefined,
        importantNotes: data.importantNotes || undefined,
      });
      console.log('[UPDATE USECASE] Successfully updated use case');
      toast.success('Use case güncellendi');
      setEditDialogOpen(false);
      await loadData();
    } catch (error: any) {
      console.error('[UPDATE USECASE] Error:', error);
      toast.error('Use case güncellenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
      // Dialog açık kalsın, kullanıcı tekrar deneyebilir
    }
  };

  const handleDelete = async () => {
    if (!useCaseId || !projectId || !moduleId) {
      throw new Error('Eksik parametreler');
    }
    
    try {
      console.log('[USECASE DELETE] Starting delete for use case:', useCaseId);
      
      await UseCaseService.deleteUseCase(useCaseId);
      
      console.log('[USECASE DELETE] Successfully deleted, navigating to module');
      toast.success('Use case silindi');
      
      // Navigate to module detail page
      navigate(`/projects/${projectId}/modules/${moduleId}`);
    } catch (error: any) {
      console.error('[USECASE DELETE] Error:', error);
      // Hata mesajı ConfirmDeleteDialog tarafından gösterilecek
      throw error;
    }
  };

  const handleStatusChange = async (newIsActive: boolean) => {
    if (!useCaseId || !useCase) return;
    
    // Eğer zaten istenen durumdaysa işlem yapma
    const currentIsActive = useCase.status === 'Active';
    if (currentIsActive === newIsActive) {
      return;
    }
    
    try {
      // PATCH isteği at
      const response = await UseCaseService.updateUseCaseStatus(useCaseId, newIsActive);
      
      // Response'daki isActive'i kontrol et
      if (response.isActive !== newIsActive) {
        toast.error('Use case durumu güncellenemedi', {
          description: 'Backend beklenen durumu döndürmedi',
        });
        // Refetch ile server state'i doğrula
        await loadData();
        return;
      }
      
      // Başarılı - toast ve refetch
      toast.success(`Use case ${newIsActive ? 'aktifleştirildi' : 'arşivlendi'}`);
      await loadData();
    } catch (error: any) {
      toast.error('Use case durumu güncellenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
      // Hata durumunda da refetch - UI ile backend senkronize olsun
      await loadData();
    }
  };

  const handleTaskSearch = (value: string) => {
    setTaskFilters({ ...taskFilters, search: value, page: 1 });
  };

  const handleTaskStateFilter = (value: string) => {
    setTaskFilters({ ...taskFilters, state: value as any, page: 1 });
  };

  const handleTaskTypeFilter = (value: string) => {
    setTaskFilters({ ...taskFilters, type: value as any, page: 1 });
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#0D1117] to-[#161B22] flex items-center justify-center">
        <div className="text-center">
          <motion.div
            className="w-16 h-16 border-4 border-[#8B5CF6] border-t-transparent rounded-full mx-auto mb-4"
            animate={{ rotate: 360 }}
            transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
          />
          <p className="text-[#9CA3AF] text-lg">Use case yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (!useCase || !module || !project) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#0D1117] to-[#161B22] text-[#E5E7EB]">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pb-12" style={{ paddingTop: 'calc(var(--navbar-height) + 2rem)' }}>
        {/* Breadcrumb */}
        <motion.div
          initial={{ opacity: 0, y: -10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.3 }}
        >
          <Breadcrumb className="mb-6">
            <BreadcrumbList>
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link to="/projects" className="hover:text-[#8B5CF6] transition-colors">Projeler</Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link to={`/projects/${projectId}`} className="hover:text-[#8B5CF6] transition-colors">{project.title}</Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link to={`/projects/${projectId}/modules/${moduleId}`} className="hover:text-[#8B5CF6] transition-colors">{module.title}</Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-[#E5E7EB]">{useCase.title}</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </motion.div>

        {/* Back Button */}
        <motion.div
          initial={{ opacity: 0, x: -10 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.3, delay: 0.1 }}
        >
          <Button
            variant="ghost"
            onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}`)}
            className="mb-6 text-[#E5E7EB] hover:text-[#8B5CF6] hover:bg-[#21262D]/50 transition-all"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Modüle Dön
          </Button>
        </motion.div>

        {/* UseCase Info Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.2 }}
          className="bg-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-6 sm:p-8 mb-6 shadow-xl"
        >
          <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-6 mb-6">
            <div className="flex-1">
              <div className="flex items-start gap-4 mb-4">
                <div className="w-12 h-12 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-xl flex items-center justify-center flex-shrink-0">
                  <Layers className="w-6 h-6 text-[#8B5CF6]" />
                </div>
                <div className="flex-1">
                  <h1 className="text-3xl sm:text-4xl font-bold mb-3 text-[#E5E7EB]">
                    {useCase?.title || 'Use Case Başlığı Yükleniyor...'}
                  </h1>
                  <div className="flex items-center gap-3 flex-wrap">
                    <Badge
                      variant={useCase.status === 'Active' ? 'default' : 'secondary'}
                      className={`${
                        useCase.status === 'Active'
                          ? 'bg-[#10B981] text-white border-[#10B981] border-2 shadow-lg shadow-[#10B981]/30'
                          : 'bg-[#6B7280] text-white'
                      } px-3 py-1.5 text-sm font-medium flex items-center justify-center`}
                    >
                      {useCase.status === 'Active' ? (
                        <span className="flex items-center gap-1.5">
                          <span className="w-2 h-2 bg-white rounded-full animate-pulse"></span>
                          <span>Aktif</span>
                        </span>
                      ) : (
                        'Arşivlendi'
                      )}
                    </Badge>
                    <div className="flex gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleStatusChange(true)}
                        disabled={useCase.status === 'Active'}
                        className={`border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] transition-all ${
                          useCase.status === 'Active'
                            ? 'opacity-50 cursor-not-allowed'
                            : 'hover:border-[#10B981] hover:text-[#10B981]'
                        }`}
                      >
                        <ArchiveRestore className="w-4 h-4 mr-2" />
                        Aktifleştir
                      </Button>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleStatusChange(false)}
                        disabled={useCase.status === 'Archived'}
                        className={`border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] transition-all ${
                          useCase.status === 'Archived'
                            ? 'opacity-50 cursor-not-allowed'
                            : 'hover:border-[#F59E0B] hover:text-[#F59E0B]'
                        }`}
                      >
                        <Archive className="w-4 h-4 mr-2" />
                        Arşivle
                      </Button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            
            <div className="flex gap-2 flex-wrap">
              {true && ( // hasPermission(PERMISSIONS.USECASE_UPDATE) - temporarily bypassed for development
                <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
                  <DialogTrigger asChild>
                    <Button 
                      variant="outline" 
                      className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:border-[#8B5CF6]/50 transition-all"
                    >
                      <Edit className="w-4 h-4 mr-2" />
                      Düzenle
                    </Button>
                  </DialogTrigger>
                  <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] max-w-2xl">
                    <DialogHeader>
                      <DialogTitle className="text-xl">Use Case Düzenle</DialogTitle>
                      <DialogDescription className="text-[#9CA3AF]">
                        Use case bilgilerini güncelleyin. Değişiklikler hemen kaydedilecektir.
                      </DialogDescription>
                    </DialogHeader>
                    <form onSubmit={handleSubmit(handleUpdate)} className="space-y-5">
                      <div>
                        <Label htmlFor="edit-title" className="text-[#E5E7EB] mb-2">Başlık *</Label>
                        <Input
                          id="edit-title"
                          {...register('title', {
                            required: 'Başlık gereklidir',
                            minLength: { value: 3, message: 'Başlık en az 3 karakter olmalıdır' },
                            maxLength: { value: 100, message: 'Başlık en fazla 100 karakter olabilir' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] transition-all"
                        />
                        {errors.title && (
                          <p className="text-red-400 text-sm mt-1">{errors.title.message}</p>
                        )}
                      </div>
                      <div>
                        <div className="flex items-center justify-between mb-2">
                          <Label htmlFor="edit-description" className="text-[#E5E7EB]">Açıklama</Label>
                          <span className="text-xs text-[#6B7280]">{descriptionLength}/1000</span>
                        </div>
                        <Textarea
                          id="edit-description"
                          {...register('description', {
                            maxLength: { value: 1000, message: 'Açıklama en fazla 1000 karakter olabilir' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[120px] focus:ring-2 focus:ring-[#8B5CF6] transition-all"
                        />
                        {errors.description && (
                          <p className="text-red-400 text-sm mt-1">{errors.description.message}</p>
                        )}
                      </div>
                      <div>
                        <div className="flex items-center justify-between mb-2">
                          <Label htmlFor="edit-notes" className="text-[#E5E7EB]">Önemli Notlar</Label>
                          <span className="text-xs text-[#6B7280]">{notesLength}/500</span>
                        </div>
                        <Textarea
                          id="edit-notes"
                          {...register('importantNotes', {
                            maxLength: { value: 500, message: 'Önemli notlar en fazla 500 karakter olabilir' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[100px] focus:ring-2 focus:ring-[#8B5CF6] transition-all"
                          placeholder="⚠️ Önemli notlar..."
                        />
                        {errors.importantNotes && (
                          <p className="text-red-400 text-sm mt-1">{errors.importantNotes.message}</p>
                        )}
                      </div>
                      <DialogFooter>
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() => setEditDialogOpen(false)}
                          className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                        >
                          İptal
                        </Button>
                        <Button type="submit" className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white transition-all">
                          Değişiklikleri Kaydet
                        </Button>
                      </DialogFooter>
                    </form>
                  </DialogContent>
                </Dialog>
              )}
              
              {true && ( // hasPermission(PERMISSIONS.USECASE_DELETE) - temporarily bypassed for development
                <>
                  <Button
                    variant="destructive"
                    onClick={() => setDeleteDialogOpen(true)}
                    className="bg-[#EF4444] hover:bg-[#DC2626] text-white transition-all"
                  >
                    <Trash2 className="w-4 h-4 mr-2" />
                    Sil
                  </Button>
                  <ConfirmDeleteDialog
                    open={deleteDialogOpen}
                    onOpenChange={setDeleteDialogOpen}
                    entityType="UseCase"
                    entityName={useCase.title}
                    onConfirm={handleDelete}
                    children={useCase.taskCount ? [{ count: useCase.taskCount, type: 'task' }] : undefined}
                  />
                </>
              )}
            </div>
          </div>

          {/* Description Card */}
          {useCase.description && (
            <motion.div
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.3, delay: 0.3 }}
              className="bg-[#0D1117]/50 border border-[#30363D]/50 rounded-xl p-6 mb-4"
            >
              <h3 className="text-sm font-semibold text-[#9CA3AF] mb-4 uppercase tracking-wide">Açıklama</h3>
              <p className="text-[#E5E7EB] leading-relaxed whitespace-pre-wrap px-2">{useCase.description}</p>
            </motion.div>
          )}

          {/* Important Notes Card */}
          {useCase.importantNotes && (
            <motion.div
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.3, delay: 0.4 }}
              className="bg-gradient-to-br from-[#F59E0B]/10 via-[#0D1117]/50 to-[#0D1117]/50 border border-[#F59E0B]/30 rounded-xl p-6"
            >
              <div className="flex items-start gap-3">
                <AlertTriangle className="w-5 h-5 text-[#F59E0B] flex-shrink-0 mt-0.5" />
                <div className="flex-1">
                  <h3 className="text-sm font-semibold text-[#F59E0B] mb-4 uppercase tracking-wide">Önemli Notlar</h3>
                  <p className="text-[#E5E7EB] leading-relaxed whitespace-pre-wrap px-2">{useCase.importantNotes}</p>
                </div>
              </div>
            </motion.div>
          )}
        </motion.div>

        {/* Tasks Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.5 }}
          className="bg-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-6 sm:p-8 shadow-xl"
        >
          {/* Header */}
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-6">
            <h2 className="text-2xl font-bold bg-gradient-to-r from-[#E5E7EB] to-[#9CA3AF] bg-clip-text text-transparent">
              Tasks
            </h2>
            <div className="flex items-center gap-3 flex-wrap">
              {/* State Filter */}
              <Select value={taskFilters.state} onValueChange={handleTaskStateFilter}>
                <SelectTrigger className="w-[140px] bg-[#0D1117] border-[#30363D] text-[#E5E7EB] text-sm">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                  <SelectItem value="All">Tüm Durumlar</SelectItem>
                  <SelectItem value="NotStarted">Başlanmadı</SelectItem>
                  <SelectItem value="InProgress">Devam Ediyor</SelectItem>
                  <SelectItem value="Completed">Tamamlandı</SelectItem>
                  <SelectItem value="Cancelled">İptal Edildi</SelectItem>
                </SelectContent>
              </Select>

              {/* Type Filter */}
              <Select value={taskFilters.type} onValueChange={handleTaskTypeFilter}>
                <SelectTrigger className="w-[140px] bg-[#0D1117] border-[#30363D] text-[#E5E7EB] text-sm">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                  <SelectItem value="All">Tüm Tipler</SelectItem>
                  <SelectItem value="Feature">Feature</SelectItem>
                  <SelectItem value="Bug">Bug</SelectItem>
                  <SelectItem value="Test">Test</SelectItem>
                  <SelectItem value="Documentation">Documentation</SelectItem>
                </SelectContent>
              </Select>

              {/* Create Task Button */}
              <TooltipProvider>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <span>
                      <Button
                        onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/new`)}
                        disabled={useCase.status !== 'Active'}
                        className="bg-[#8B5CF6] hover:bg-[#7C3AED] text-white disabled:opacity-50 disabled:cursor-not-allowed"
                      >
                        <Plus className="w-4 h-4 mr-2" />
                        Task Oluştur
                      </Button>
                    </span>
                  </TooltipTrigger>
                  {useCase.status !== 'Active' && (
                    <TooltipContent>
                      <p>Task oluşturmak için use case'i aktifleştirin</p>
                    </TooltipContent>
                  )}
                </Tooltip>
              </TooltipProvider>
            </div>
          </div>

          {/* Search */}
          <div className="mb-6">
            <div className="flex gap-3 items-center">
              {/* Search Icon Button */}
              <div className="flex items-center justify-center w-10 h-10 bg-[#0D1117] border border-[#30363D] rounded-xl">
                <Search className="text-[#6B7280] w-5 h-5" />
              </div>
              
              {/* Search Input */}
              <div className="relative flex-1">
                <Input
                  placeholder="Task ara..."
                  value={taskFilters.search}
                  onChange={(e) => handleTaskSearch(e.target.value)}
                  className="h-10 rounded-xl pl-4 pr-4 bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:ring-2 focus:ring-[#8B5CF6]"
                />
              </div>
            </div>
          </div>

          {/* Tasks Table */}
          {tasksLoading ? (
            <div className="text-center py-16">
              <motion.div
                animate={{ rotate: 360 }}
                transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
                className="w-12 h-12 border-4 border-[#8B5CF6] border-t-transparent rounded-full mx-auto mb-4"
              />
              <p className="text-[#9CA3AF]">Yükleniyor...</p>
            </div>
          ) : tasks.length === 0 ? (
            <div className="text-center py-16">
              <motion.div
                className="w-20 h-20 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-2xl flex items-center justify-center mx-auto mb-6"
                animate={{ 
                  scale: [1, 1.05, 1],
                }}
                transition={{ 
                  duration: 2, 
                  repeat: Infinity, 
                  ease: "easeInOut" 
                }}
              >
                <Layers className="w-10 h-10 text-[#8B5CF6]" />
              </motion.div>
              <h3 className="text-xl font-semibold text-[#E5E7EB] mb-3">
                Henüz task yok
              </h3>
              <p className="text-[#9CA3AF] mb-6">
                Bu use case için ilk task'ı oluşturarak başlayın
              </p>
              {useCase.status === 'Active' && (
                <Button
                  onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/new`)}
                  className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white"
                >
                  <Plus className="w-4 h-4 mr-2" />
                  İlk Task'ı Oluştur
                </Button>
              )}
            </div>
          ) : (
            <div className="bg-[#0D1117]/50 border border-[#30363D]/50 rounded-xl overflow-hidden">
              <Table>
                <TableHeader>
                  <TableRow className="border-[#30363D] hover:bg-[#21262D]/50">
                    <TableHead className="text-[#E5E7EB] text-left">Başlık</TableHead>
                    <TableHead className="text-[#E5E7EB] text-center">Tip</TableHead>
                    <TableHead className="text-[#E5E7EB] text-center">Durum</TableHead>
                    <TableHead className="text-[#E5E7EB] text-center">Bitiş Tarihi</TableHead>
                    <TableHead className="text-[#E5E7EB] text-center">Aksiyonlar</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {tasks.map((task) => {
                    // Debug: Task objesini kontrol et
                    if (!taskTypeConfig[task.type]) {
                      console.error('[TASK RENDER] Invalid task type:', {
                        taskId: task.id,
                        taskTitle: task.title,
                        taskType: task.type,
                        taskTypeType: typeof task.type,
                        fullTask: task,
                        availableTypes: Object.keys(taskTypeConfig)
                      });
                    }

                    // Fallback: Eğer type mapping yoksa Feature kullan
                    const typeConfig = taskTypeConfig[task.type] || taskTypeConfig.Feature;
                    const stateConfig = taskStateConfig[task.state] || taskStateConfig.NotStarted;

                    return (
                      <TableRow key={task.id} className="border-[#30363D] hover:bg-[#21262D]/50 cursor-pointer">
                        <TableCell className="text-[#E5E7EB] font-medium text-left">{task.title}</TableCell>
                        <TableCell className="text-center">
                          <div className="flex justify-center">
                            <Badge className={`${typeConfig.color} border-0`}>
                              <span className="flex items-center gap-1.5">
                                {typeConfig.icon}
                                <span>{typeConfig.label}</span>
                              </span>
                            </Badge>
                          </div>
                        </TableCell>
                        <TableCell className="text-center">
                          <div className="flex justify-center">
                            <Badge className={`${stateConfig.color} border-0`}>
                              {stateConfig.label}
                            </Badge>
                          </div>
                        </TableCell>
                        <TableCell className="text-[#9CA3AF] text-center">
                          {task.dueDate ? new Date(task.dueDate).toLocaleDateString('tr-TR') : '-'}
                        </TableCell>
                        <TableCell className="text-center">
                          <div className="flex justify-center items-center gap-2">
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${task.id}`)}
                              className="text-[#8B5CF6] hover:text-[#7C3AED] hover:bg-[#8B5CF6]/10"
                              title="Görüntüle"
                            >
                              <Eye className="w-4 h-4" />
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>
          )}
        </motion.div>

      </div>
    </div>
  );
}
