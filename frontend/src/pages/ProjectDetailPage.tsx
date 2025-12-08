/**
 * Project Detail Page
 * Route: /projects/:projectId
 */
import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion, AnimatePresence } from 'motion/react';
import { ProjectService } from '../services/projectService';
import { ModuleService } from '../services/moduleService';
import { hasPermission, PERMISSIONS } from '../utils/permissions';
import type { Project, Module, ModuleFilters } from '../types';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Badge } from '../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from '../components/ui/dialog';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from '../components/ui/alert-dialog';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { ArrowLeft, Plus, Search, Edit, Trash2, Eye, Archive, ArchiveRestore, Calendar, Filter, Layers } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';

interface EditProjectForm {
  title: string;
  description: string;
}

interface CreateModuleForm {
  title: string;
  description: string;
}

export function ProjectDetailPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [project, setProject] = useState<Project | null>(null);
  const [modules, setModules] = useState<Module[]>([]);
  const [loading, setLoading] = useState(true);
  const [modulesLoading, setModulesLoading] = useState(true);
  const [filters, setFilters] = useState<ModuleFilters>({
    page: 1,
    pageSize: 20,
    status: 'All',
    search: '',
  });
  const [totalPages, setTotalPages] = useState(1);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [createModuleDialogOpen, setCreateModuleDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  
  const { register: registerProject, handleSubmit: handleSubmitProject, reset: resetProject, formState: { errors: projectErrors } } = useForm<EditProjectForm>();
  const { register: registerModule, handleSubmit: handleSubmitModule, reset: resetModule, formState: { errors: moduleErrors } } = useForm<CreateModuleForm>();

  useEffect(() => {
    if (projectId) {
      loadProject();
    }
  }, [projectId]);

  useEffect(() => {
    if (projectId) {
      loadModules();
    }
  }, [projectId, filters]);

  useEffect(() => {
    if (project) {
      resetProject({
        title: project.title,
        description: project.description || '',
      });
    }
  }, [project, resetProject]);

  const loadProject = async () => {
    if (!projectId) return;
    try {
      setLoading(true);
      const data = await ProjectService.getProject(projectId);
      setProject(data);
    } catch (error: any) {
      toast.error('Proje yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
      navigate('/projects');
    } finally {
      setLoading(false);
    }
  };

  const loadModules = async () => {
    if (!projectId) return;
    try {
      setModulesLoading(true);
      const response = await ModuleService.getModules(projectId, filters);
      setModules(response.items);
      setTotalPages(response.totalPages);
    } catch (error: any) {
      toast.error('Modüller yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setModulesLoading(false);
    }
  };

  const handleUpdateProject = async (data: EditProjectForm) => {
    if (!projectId) return;
    try {
      await ProjectService.updateProject(projectId, {
        title: data.title,
        description: data.description || undefined,
      });
      toast.success('Proje güncellendi');
      setEditDialogOpen(false);
      loadProject();
    } catch (error: any) {
      toast.error('Proje güncellenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    }
  };

  const handleDeleteProject = async () => {
    if (!projectId) return;
    try {
      await ProjectService.deleteProject(projectId);
      toast.success('Proje silindi');
      navigate('/projects');
    } catch (error: any) {
      toast.error('Proje silinemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    }
  };

  const handleStatusChange = async () => {
    if (!projectId || !project) return;
    try {
      const newStatus = project.status === 'Active' ? 'Archived' : 'Active';
      await ProjectService.updateProjectStatus(projectId, newStatus);
      toast.success(`Proje ${newStatus === 'Active' ? 'aktifleştirildi' : 'arşivlendi'}`);
      loadProject();
    } catch (error: any) {
      toast.error('Proje durumu güncellenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    }
  };

  const handleCreateModule = async (data: CreateModuleForm) => {
    if (!projectId) return;
    try {
      await ModuleService.createModule(projectId, {
        title: data.title,
        description: data.description || undefined,
      });
      toast.success('Modül oluşturuldu');
      setCreateModuleDialogOpen(false);
      resetModule();
      loadModules();
    } catch (error: any) {
      toast.error('Modül oluşturulamadı', {
        description: error.message || 'Bir hata oluştu',
      });
    }
  };

  const handleSearch = (value: string) => {
    setFilters({ ...filters, search: value, page: 1 });
  };

  const handleFilterChange = (value: string) => {
    setFilters({
      ...filters,
      status: value as 'All' | 'Active' | 'Archived',
      page: 1,
    });
  };

  const handlePageChange = (page: number) => {
    setFilters({ ...filters, page });
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center relative overflow-hidden">
        {/* Animated Background */}
        <div className="fixed inset-0 pointer-events-none overflow-hidden">
          <motion.div
            className="absolute -top-40 -right-40 w-[600px] h-[600px] bg-[#8B5CF6] opacity-[0.08] blur-[120px] rounded-full"
            animate={{ scale: [1, 1.2, 1], rotate: [0, 90, 0] }}
            transition={{ duration: 20, repeat: Infinity }}
          />
        </div>
        <div className="text-center relative z-10">
          <motion.div
            animate={{ rotate: 360 }}
            transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
            className="w-12 h-12 border-4 border-[#8B5CF6] border-t-transparent rounded-full mx-auto mb-4"
          />
          <p className="text-[#9CA3AF] text-lg">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (!project) {
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
      <div className="relative z-10 pt-24 px-4 sm:px-6 pb-12">
        <div className="max-w-7xl mx-auto">
          {/* Breadcrumb */}
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3 }}
            className="mb-6"
          >
            <Breadcrumb>
              <BreadcrumbList>
                <BreadcrumbItem>
                  <BreadcrumbLink asChild>
                    <Link to="/projects" className="hover:text-[#8B5CF6] transition-colors">Projeler</Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                  <BreadcrumbPage className="text-[#E5E7EB]">{project.title}</BreadcrumbPage>
                </BreadcrumbItem>
              </BreadcrumbList>
            </Breadcrumb>
          </motion.div>

          {/* Back Button */}
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.3, delay: 0.1 }}
          >
            <Button
              variant="ghost"
              onClick={() => navigate('/projects')}
              className="mb-6 text-[#E5E7EB] hover:text-[#8B5CF6] hover:bg-[#21262D]/50"
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
              <span className="hidden sm:inline">Projelere Dön</span>
              <span className="sm:hidden">Geri</span>
            </Button>
          </motion.div>

        {/* Project Info Section - Glassmorphism Card */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.2 }}
          className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-6 sm:p-8 mb-8 shadow-lg relative overflow-hidden"
        >
          {/* Top Gradient Line */}
          <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-[#8B5CF6]/50 to-transparent" />
          
          <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-6 mb-6">
            <div className="flex-1 min-w-0">
              <h1 className="text-2xl sm:text-3xl font-bold mb-3 text-[#E5E7EB] break-words">
                {project.title}
              </h1>
              <div className="flex flex-wrap items-center gap-3 mb-4">
                <Badge
                  className={
                    project.status === 'Active'
                      ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30 border px-3 py-1'
                      : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30 border px-3 py-1'
                  }
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
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handleStatusChange}
                  className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]/80 h-8 text-xs"
                >
                  {project.status === 'Active' ? (
                    <>
                      <Archive className="w-3 h-3 mr-1.5" />
                      Arşivle
                    </>
                  ) : (
                    <>
                      <ArchiveRestore className="w-3 h-3 mr-1.5" />
                      Aktifleştir
                    </>
                  )}
                </Button>
              </div>
            </div>
            <div className="flex flex-wrap gap-2">
              {hasPermission(PERMISSIONS.PROJECT_UPDATE) && (
                <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
                  <DialogTrigger asChild>
                    <Button variant="outline" className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]/80 h-9 text-sm">
                      <Edit className="w-4 h-4 mr-2" />
                      <span className="hidden sm:inline">Düzenle</span>
                      <span className="sm:hidden">Düzenle</span>
                    </Button>
                  </DialogTrigger>
                  <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                    <DialogHeader>
                      <DialogTitle>Projeyi Düzenle</DialogTitle>
                    </DialogHeader>
                    <form onSubmit={handleSubmitProject(handleUpdateProject)} className="space-y-4">
                      <div>
                        <Label htmlFor="edit-title">Proje Başlığı *</Label>
                        <Input
                          id="edit-title"
                          {...registerProject('title', {
                            required: 'Proje başlığı zorunludur',
                            minLength: { value: 3, message: 'En az 3 karakter olmalıdır' },
                            maxLength: { value: 120, message: 'En fazla 120 karakter olabilir' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]"
                        />
                        {projectErrors.title && (
                          <p className="text-[#EF4444] text-xs mt-2">{projectErrors.title.message}</p>
                        )}
                      </div>
                      <div>
                        <Label htmlFor="edit-description">Açıklama</Label>
                        <Textarea
                          id="edit-description"
                          {...registerProject('description', {
                            maxLength: { value: 1000, message: 'En fazla 1000 karakter olabilir' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[100px]"
                        />
                        {projectErrors.description && (
                          <p className="text-[#EF4444] text-xs mt-2">{projectErrors.description.message}</p>
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
                        <Button type="submit" className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white">
                          Kaydet
                        </Button>
                      </DialogFooter>
                    </form>
                  </DialogContent>
                </Dialog>
              )}
              {hasPermission(PERMISSIONS.PROJECT_DELETE) && (
                <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
                  <AlertDialogTrigger asChild>
                    <Button variant="destructive" className="bg-[#EF4444] hover:bg-[#DC2626] text-white h-9 text-sm">
                      <Trash2 className="w-4 h-4 mr-2" />
                      <span className="hidden sm:inline">Sil</span>
                      <span className="sm:hidden">Sil</span>
                    </Button>
                  </AlertDialogTrigger>
                  <AlertDialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                    <AlertDialogHeader>
                      <AlertDialogTitle>Projeyi Sil</AlertDialogTitle>
                      <AlertDialogDescription className="text-[#9CA3AF]">
                        Bu projeyi silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.
                      </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                      <AlertDialogCancel className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]">
                        İptal
                      </AlertDialogCancel>
                      <AlertDialogAction
                        onClick={handleDeleteProject}
                        className="bg-[#EF4444] hover:bg-[#DC2626] text-white"
                      >
                        Sil
                      </AlertDialogAction>
                    </AlertDialogFooter>
                  </AlertDialogContent>
                </AlertDialog>
              )}
            </div>
          </div>

          {project.description && (
            <div className="mb-6 p-4 bg-[#0D1117]/40 rounded-xl border border-[#30363D]/30">
              <h3 className="text-sm font-semibold text-[#9CA3AF] mb-2 flex items-center gap-2">
                <Layers className="w-4 h-4" />
                Açıklama:
              </h3>
              <p className="text-[#E5E7EB] leading-relaxed">{project.description}</p>
            </div>
          )}

          <div className="flex items-center gap-2 text-sm text-[#9CA3AF]">
            <Calendar className="w-4 h-4" />
            <span>Başlangıç:</span>
            <span className="text-[#E5E7EB] font-medium">
              {new Date(project.startedDate).toLocaleDateString('tr-TR', {
                day: 'numeric',
                month: 'long',
                year: 'numeric'
              })}
            </span>
          </div>
        </motion.div>

        {/* Modules Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.3 }}
        >
          {/* Control Panel */}
          <div className="mb-6 flex flex-col lg:flex-row items-start lg:items-center justify-between gap-4 bg-[#161B22]/40 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-4">
            {/* Left: Title */}
            <h2 className="text-xl sm:text-2xl font-semibold text-[#E5E7EB] flex-shrink-0">Modüller</h2>
            
            {/* Right: Search, Filter, Create */}
            <div className="flex flex-col sm:flex-row gap-3 w-full lg:w-auto">
              {/* Search Input */}
              <div className="relative flex-1 lg:flex-initial lg:w-80">
                <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-[#6B7280] w-4 h-4 pointer-events-none" />
                <Input
                  placeholder="Modül ara..."
                  value={filters.search || ''}
                  onChange={(e) => handleSearch(e.target.value)}
                  className="pl-11 pr-4 bg-[#0D1117]/60 backdrop-blur-sm border-[#30363D]/80 text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#8B5CF6] focus:ring-1 focus:ring-[#8B5CF6]/30 transition-all h-10 rounded-lg"
                />
              </div>
              
              {/* Filter Select */}
              <Select value={filters.status || 'All'} onValueChange={handleFilterChange}>
                <SelectTrigger className="w-full sm:w-[140px] bg-[#0D1117]/60 backdrop-blur-sm border-[#30363D]/80 text-[#E5E7EB] h-10 rounded-lg">
                  <Filter className="w-4 h-4 mr-2 text-[#6B7280]" />
                  <SelectValue placeholder="Filtre" />
                </SelectTrigger>
                <SelectContent className="bg-[#161B22] border-[#30363D]">
                  <SelectItem value="All">Tümü</SelectItem>
                  <SelectItem value="Active">Aktif</SelectItem>
                  <SelectItem value="Archived">Arşivlenmiş</SelectItem>
                </SelectContent>
              </Select>

              {/* Create Module Button */}
              {hasPermission(PERMISSIONS.MODULE_CREATE) && (
                <Dialog open={createModuleDialogOpen} onOpenChange={setCreateModuleDialogOpen}>
                  <DialogTrigger asChild>
                    <Button className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 hover:shadow-[#8B5CF6]/50 h-10 rounded-lg whitespace-nowrap">
                      <Plus className="w-4 h-4 mr-2" />
                      Yeni Modül
                    </Button>
                  </DialogTrigger>
                  <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                    <DialogHeader>
                      <DialogTitle>Yeni Modül Oluştur</DialogTitle>
                    </DialogHeader>
                    <form onSubmit={handleSubmitModule(handleCreateModule)} className="space-y-4">
                      <div>
                        <Label htmlFor="module-title">Modül Başlığı *</Label>
                        <Input
                          id="module-title"
                          {...registerModule('title', {
                            required: 'Modül başlığı zorunludur',
                            minLength: { value: 3, message: 'En az 3 karakter olmalıdır' },
                            maxLength: { value: 100, message: 'En fazla 100 karakter olabilir' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]"
                          placeholder="Modül adını girin..."
                        />
                        {moduleErrors.title && (
                          <p className="text-[#EF4444] text-xs mt-2">{moduleErrors.title.message}</p>
                        )}
                      </div>
                      <div>
                        <Label htmlFor="module-description">Açıklama</Label>
                        <Textarea
                          id="module-description"
                          {...registerModule('description', {
                            maxLength: { value: 1000, message: 'En fazla 1000 karakter olabilir' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[100px]"
                          placeholder="Modül açıklaması (opsiyonel)..."
                        />
                        {moduleErrors.description && (
                          <p className="text-[#EF4444] text-xs mt-2">{moduleErrors.description.message}</p>
                        )}
                      </div>
                      <DialogFooter>
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() => setCreateModuleDialogOpen(false)}
                          className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                        >
                          İptal
                        </Button>
                        <Button type="submit" className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white">
                          Oluştur
                        </Button>
                      </DialogFooter>
                    </form>
                  </DialogContent>
                </Dialog>
              )}
            </div>
          </div>

          {/* Modules Grid/List */}
          {modulesLoading ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {[1, 2, 3, 4, 5, 6].map((i) => (
                <motion.div
                  key={i}
                  initial={{ opacity: 0, scale: 0.9 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ duration: 0.3, delay: i * 0.05 }}
                  className="bg-[#161B22]/60 backdrop-blur-sm border border-[#30363D] rounded-2xl p-6 h-40 animate-pulse"
                />
              ))}
            </div>
          ) : modules.length === 0 ? (
            <motion.div
              initial={{ opacity: 0, scale: 0.9 }}
              animate={{ opacity: 1, scale: 1 }}
              transition={{ duration: 0.5 }}
              className="text-center py-20"
            >
              <div className="bg-[#161B22]/40 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-16 max-w-lg mx-auto">
                <motion.div
                  className="w-24 h-24 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-2xl flex items-center justify-center mx-auto mb-6 shadow-lg shadow-[#8B5CF6]/10"
                  animate={{ scale: [1, 1.05, 1] }}
                  transition={{ duration: 2, repeat: Infinity, ease: "easeInOut" }}
                >
                  <Layers className="w-12 h-12 text-[#8B5CF6]" />
                </motion.div>
                <h3 className="text-2xl font-semibold text-[#E5E7EB] mb-3">
                  Henüz modül yok
                </h3>
                <p className="text-[#9CA3AF] mb-8 text-base">
                  İlk modülünüzü oluşturarak başlayın
                </p>
                {hasPermission(PERMISSIONS.MODULE_CREATE) && (
                  <Button
                    onClick={() => setCreateModuleDialogOpen(true)}
                    className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 hover:shadow-[#8B5CF6]/50 transition-all"
                  >
                    <Plus className="w-4 h-4 mr-2" />
                    İlk Modülü Oluştur
                  </Button>
                )}
              </div>
            </motion.div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              <AnimatePresence>
                {modules.map((module, index) => (
                  <motion.div
                    key={module.id}
                    initial={{ opacity: 0, scale: 0.95, y: 20 }}
                    animate={{ opacity: 1, scale: 1, y: 0 }}
                    exit={{ opacity: 0, scale: 0.95 }}
                    transition={{ duration: 0.3, delay: index * 0.03 }}
                    whileHover={{ scale: 1.01, y: -4 }}
                    onClick={() => navigate(`/projects/${projectId}/modules/${module.id}`)}
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
                            {module.title}
                          </h3>
                          {module.description && (
                            <p className="text-[#9CA3AF] text-sm line-clamp-2 leading-relaxed">
                              {module.description}
                            </p>
                          )}
                        </div>

                        {/* Status Badge */}
                        <Badge
                          className={`${
                            module.status === 'Active'
                              ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                              : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                          } border px-2.5 py-1 flex-shrink-0`}
                        >
                          {module.status === 'Active' ? (
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
                          <Layers className="w-3.5 h-3.5" />
                          <span>{module.useCaseCount || 0} Use Case</span>
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
        </motion.div>
      </div>
      </div>
    </div>
  );
}

export default ProjectDetailPage;

