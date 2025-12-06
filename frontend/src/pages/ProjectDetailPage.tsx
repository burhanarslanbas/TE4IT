/**
 * Project Detail Page
 * Route: /projects/:projectId
 */
import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { ProjectService } from '../services/projectService';
import { ModuleService } from '../services/moduleService';
import { hasPermission, PERMISSIONS } from '../utils/permissions';
import type { Project, Module, ModuleFilters } from '../types';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Badge } from '../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from '../components/ui/dialog';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from '../components/ui/alert-dialog';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { ArrowLeft, Plus, Search, Edit, Trash2, Eye, Archive, ArchiveRestore } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from '../components/ui/pagination';

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
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center">
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
    <div className="min-h-screen bg-[#0D1117] text-[#E5E7EB] p-6">
      <div className="max-w-7xl mx-auto">
        {/* Breadcrumb */}
        <Breadcrumb className="mb-6">
          <BreadcrumbList>
            <BreadcrumbItem>
              <BreadcrumbLink asChild>
                <Link to="/projects" className="hover:text-[#8B5CF6]">Projects</Link>
              </BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <BreadcrumbPage>{project.title}</BreadcrumbPage>
            </BreadcrumbItem>
          </BreadcrumbList>
        </Breadcrumb>

        {/* Back Button */}
        <Button
          variant="ghost"
          onClick={() => navigate('/projects')}
          className="mb-6 text-[#E5E7EB] hover:text-[#8B5CF6] hover:bg-[#21262D]"
        >
          <ArrowLeft className="w-4 h-4 mr-2" />
          Back to Projects
        </Button>

        {/* Project Info Section */}
        <div className="bg-[#161B22] border border-[#30363D] rounded-lg p-6 mb-6">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-4">
            <div>
              <h1 className="text-3xl font-bold mb-2">Project: {project.title}</h1>
              <div className="flex items-center gap-3">
                <Badge
                  variant={project.status === 'Active' ? 'default' : 'secondary'}
                  className={
                    project.status === 'Active'
                      ? 'bg-[#10B981] text-white'
                      : 'bg-[#6B7280] text-white'
                  }
                >
                  {project.status}
                </Badge>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handleStatusChange}
                  className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                >
                  {project.status === 'Active' ? (
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
              </div>
            </div>
            <div className="flex gap-2">
              {hasPermission(PERMISSIONS.PROJECT_UPDATE) && (
                <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
                  <DialogTrigger asChild>
                    <Button variant="outline" className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]">
                      <Edit className="w-4 h-4 mr-2" />
                      Edit Project
                    </Button>
                  </DialogTrigger>
                  <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                    <DialogHeader>
                      <DialogTitle>Edit Project</DialogTitle>
                    </DialogHeader>
                    <form onSubmit={handleSubmitProject(handleUpdateProject)} className="space-y-4">
                      <div>
                        <Label htmlFor="edit-title">Title *</Label>
                        <Input
                          id="edit-title"
                          {...registerProject('title', {
                            required: 'Title is required',
                            minLength: { value: 3, message: 'Title must be at least 3 characters' },
                            maxLength: { value: 120, message: 'Title must be at most 120 characters' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]"
                        />
                        {projectErrors.title && (
                          <p className="text-red-500 text-sm mt-1">{projectErrors.title.message}</p>
                        )}
                      </div>
                      <div>
                        <Label htmlFor="edit-description">Description</Label>
                        <Textarea
                          id="edit-description"
                          {...registerProject('description', {
                            maxLength: { value: 1000, message: 'Description must be at most 1000 characters' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[100px]"
                        />
                        {projectErrors.description && (
                          <p className="text-red-500 text-sm mt-1">{projectErrors.description.message}</p>
                        )}
                      </div>
                      <DialogFooter>
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() => setEditDialogOpen(false)}
                          className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                        >
                          Cancel
                        </Button>
                        <Button type="submit" className="bg-[#8B5CF6] hover:bg-[#7C3AED] text-white">
                          Save
                        </Button>
                      </DialogFooter>
                    </form>
                  </DialogContent>
                </Dialog>
              )}
              {hasPermission(PERMISSIONS.PROJECT_DELETE) && (
                <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
                  <AlertDialogTrigger asChild>
                    <Button variant="destructive" className="bg-[#EF4444] hover:bg-[#DC2626] text-white">
                      <Trash2 className="w-4 h-4 mr-2" />
                      Delete Project
                    </Button>
                  </AlertDialogTrigger>
                  <AlertDialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                    <AlertDialogHeader>
                      <AlertDialogTitle>Delete Project</AlertDialogTitle>
                      <AlertDialogDescription>
                        Are you sure you want to delete this project? This action cannot be undone.
                      </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                      <AlertDialogCancel className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]">
                        Cancel
                      </AlertDialogCancel>
                      <AlertDialogAction
                        onClick={handleDeleteProject}
                        className="bg-[#EF4444] hover:bg-[#DC2626] text-white"
                      >
                        Delete
                      </AlertDialogAction>
                    </AlertDialogFooter>
                  </AlertDialogContent>
                </AlertDialog>
              )}
            </div>
          </div>

          {project.description && (
            <div className="mb-4">
              <h3 className="text-sm font-semibold text-[#9CA3AF] mb-2">Description:</h3>
              <p className="text-[#E5E7EB]">{project.description}</p>
            </div>
          )}

          <div className="text-sm text-[#9CA3AF]">
            Started: {new Date(project.startedDate).toLocaleDateString('tr-TR')}
          </div>
        </div>

        {/* Modules Section */}
        <div>
          <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between mb-4">
            <h2 className="text-2xl font-bold">Modules</h2>
            <div className="flex flex-1 gap-4 items-center">
              <div className="relative flex-1 max-w-md">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-[#9CA3AF] w-4 h-4" />
                <Input
                  placeholder="Search modules..."
                  value={filters.search || ''}
                  onChange={(e) => handleSearch(e.target.value)}
                  className="pl-10 bg-[#161B22] border-[#30363D] text-[#E5E7EB]"
                />
              </div>
              
              <Select value={filters.status || 'All'} onValueChange={handleFilterChange}>
                <SelectTrigger className="w-[180px] bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                  <SelectValue placeholder="Filter" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="All">All</SelectItem>
                  <SelectItem value="Active">Active</SelectItem>
                  <SelectItem value="Archived">Archived</SelectItem>
                </SelectContent>
              </Select>

              {hasPermission(PERMISSIONS.MODULE_CREATE) && (
                <Dialog open={createModuleDialogOpen} onOpenChange={setCreateModuleDialogOpen}>
                  <DialogTrigger asChild>
                    <Button className="bg-[#8B5CF6] hover:bg-[#7C3AED] text-white">
                      <Plus className="w-4 h-4 mr-2" />
                      Create Module
                    </Button>
                  </DialogTrigger>
                  <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                    <DialogHeader>
                      <DialogTitle>Create Module</DialogTitle>
                    </DialogHeader>
                    <form onSubmit={handleSubmitModule(handleCreateModule)} className="space-y-4">
                      <div>
                        <Label htmlFor="module-title">Title *</Label>
                        <Input
                          id="module-title"
                          {...registerModule('title', {
                            required: 'Title is required',
                            minLength: { value: 3, message: 'Title must be at least 3 characters' },
                            maxLength: { value: 100, message: 'Title must be at most 100 characters' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]"
                        />
                        {moduleErrors.title && (
                          <p className="text-red-500 text-sm mt-1">{moduleErrors.title.message}</p>
                        )}
                      </div>
                      <div>
                        <Label htmlFor="module-description">Description</Label>
                        <Textarea
                          id="module-description"
                          {...registerModule('description', {
                            maxLength: { value: 1000, message: 'Description must be at most 1000 characters' },
                          })}
                          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[100px]"
                        />
                        {moduleErrors.description && (
                          <p className="text-red-500 text-sm mt-1">{moduleErrors.description.message}</p>
                        )}
                      </div>
                      <DialogFooter>
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() => setCreateModuleDialogOpen(false)}
                          className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                        >
                          Cancel
                        </Button>
                        <Button type="submit" className="bg-[#8B5CF6] hover:bg-[#7C3AED] text-white">
                          Create
                        </Button>
                      </DialogFooter>
                    </form>
                  </DialogContent>
                </Dialog>
              )}
            </div>
          </div>

          {modulesLoading ? (
            <div className="flex items-center justify-center py-12">
              <div className="w-8 h-8 border-2 border-[#8B5CF6] border-t-transparent rounded-full animate-spin"></div>
            </div>
          ) : (
            <>
              <div className="bg-[#161B22] border border-[#30363D] rounded-lg overflow-hidden">
                <Table>
                  <TableHeader>
                    <TableRow className="border-[#30363D] hover:bg-[#21262D]">
                      <TableHead className="text-[#E5E7EB]">Title</TableHead>
                      <TableHead className="text-[#E5E7EB]">Status</TableHead>
                      <TableHead className="text-[#E5E7EB]">Use Cases</TableHead>
                      <TableHead className="text-[#E5E7EB]">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {modules.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={4} className="text-center py-8 text-[#9CA3AF]">
                          No modules found
                        </TableCell>
                      </TableRow>
                    ) : (
                      modules.map((module) => (
                        <TableRow key={module.id} className="border-[#30363D] hover:bg-[#21262D]">
                          <TableCell className="text-[#E5E7EB]">
                            <button
                              onClick={() => navigate(`/projects/${projectId}/modules/${module.id}`)}
                              className="hover:text-[#8B5CF6] transition-colors"
                            >
                              {module.title}
                            </button>
                          </TableCell>
                          <TableCell>
                            <Badge
                              variant={module.status === 'Active' ? 'default' : 'secondary'}
                              className={
                                module.status === 'Active'
                                  ? 'bg-[#10B981] text-white'
                                  : 'bg-[#6B7280] text-white'
                              }
                            >
                              {module.status}
                            </Badge>
                          </TableCell>
                          <TableCell className="text-[#9CA3AF]">
                            <button
                              onClick={() => navigate(`/projects/${projectId}/modules/${module.id}/usecases`)}
                              className="hover:text-[#8B5CF6] transition-colors"
                            >
                              {module.useCaseCount || 0}
                            </button>
                          </TableCell>
                          <TableCell>
                            <div className="flex gap-2">
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => navigate(`/projects/${projectId}/modules/${module.id}`)}
                                className="text-[#8B5CF6] hover:text-[#7C3AED] hover:bg-[#21262D]"
                              >
                                <Eye className="w-4 h-4 mr-2" />
                                View
                              </Button>
                            </div>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              </div>

              {totalPages > 1 && (
                <div className="mt-6">
                  <Pagination>
                    <PaginationContent>
                      <PaginationItem>
                        <PaginationPrevious
                          onClick={() => filters.page && filters.page > 1 && handlePageChange(filters.page - 1)}
                          className={filters.page === 1 ? 'pointer-events-none opacity-50' : 'cursor-pointer'}
                        />
                      </PaginationItem>
                      {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
                        <PaginationItem key={page}>
                          <PaginationLink
                            onClick={() => handlePageChange(page)}
                            isActive={filters.page === page}
                            className="cursor-pointer"
                          >
                            {page}
                          </PaginationLink>
                        </PaginationItem>
                      ))}
                      <PaginationItem>
                        <PaginationNext
                          onClick={() => filters.page && filters.page < totalPages && handlePageChange(filters.page + 1)}
                          className={filters.page === totalPages ? 'pointer-events-none opacity-50' : 'cursor-pointer'}
                        />
                      </PaginationItem>
                    </PaginationContent>
                  </Pagination>
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}

