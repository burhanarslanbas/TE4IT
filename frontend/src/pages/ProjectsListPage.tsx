/**
 * Projects List Page
 * Route: /projects
 */
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ProjectService } from '../services/projectService';
import { hasPermission, PERMISSIONS } from '../utils/permissions';
import type { Project, ProjectFilters } from '../types';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Badge } from '../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from '../components/ui/dialog';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { Plus, Search, Eye } from 'lucide-react';
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from '../components/ui/pagination';

interface CreateProjectForm {
  title: string;
  description: string;
}

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
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  
  const { register, handleSubmit, reset, formState: { errors } } = useForm<CreateProjectForm>();

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

  const handleCreateProject = async (data: CreateProjectForm) => {
    try {
      await ProjectService.createProject({
        title: data.title,
        description: data.description || undefined,
      });
      toast.success('Proje oluşturuldu');
      setCreateDialogOpen(false);
      reset();
      loadProjects();
    } catch (error: any) {
      toast.error('Proje oluşturulamadı', {
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
      isActive: value === 'all' ? undefined : value === 'active',
      page: 1,
    });
  };

  const handlePageChange = (page: number) => {
    setFilters({ ...filters, page });
  };

  return (
    <div className="min-h-screen bg-[#0D1117] text-[#E5E7EB] p-6">
      <div className="max-w-7xl mx-auto">
        <div className="mb-6">
          <h1 className="text-3xl font-bold mb-4">Projects</h1>
          
          <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
            <div className="flex flex-1 gap-4 items-center">
              <div className="relative flex-1 max-w-md">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-[#9CA3AF] w-4 h-4" />
                <Input
                  placeholder="Search projects..."
                  value={filters.search || ''}
                  onChange={(e) => handleSearch(e.target.value)}
                  className="pl-10 bg-[#161B22] border-[#30363D] text-[#E5E7EB]"
                />
              </div>
              
              <Select value={filters.isActive === undefined ? 'all' : filters.isActive ? 'active' : 'archived'} onValueChange={handleFilterChange}>
                <SelectTrigger className="w-[180px] bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                  <SelectValue placeholder="Filter" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All</SelectItem>
                  <SelectItem value="active">Active</SelectItem>
                  <SelectItem value="archived">Archived</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {hasPermission(PERMISSIONS.PROJECT_CREATE) && (
              <Dialog open={createDialogOpen} onOpenChange={setCreateDialogOpen}>
                <DialogTrigger asChild>
                  <Button className="bg-[#8B5CF6] hover:bg-[#7C3AED] text-white">
                    <Plus className="w-4 h-4 mr-2" />
                    Create Project
                  </Button>
                </DialogTrigger>
                <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                  <DialogHeader>
                    <DialogTitle>Create Project</DialogTitle>
                  </DialogHeader>
                  <form onSubmit={handleSubmit(handleCreateProject)} className="space-y-4">
                    <div>
                      <Label htmlFor="title">Title *</Label>
                      <Input
                        id="title"
                        {...register('title', {
                          required: 'Title is required',
                          minLength: { value: 3, message: 'Title must be at least 3 characters' },
                          maxLength: { value: 120, message: 'Title must be at most 120 characters' },
                        })}
                        className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]"
                      />
                      {errors.title && (
                        <p className="text-red-500 text-sm mt-1">{errors.title.message}</p>
                      )}
                    </div>
                    <div>
                      <Label htmlFor="description">Description</Label>
                      <Textarea
                        id="description"
                        {...register('description', {
                          maxLength: { value: 1000, message: 'Description must be at most 1000 characters' },
                        })}
                        className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[100px]"
                      />
                      {errors.description && (
                        <p className="text-red-500 text-sm mt-1">{errors.description.message}</p>
                      )}
                    </div>
                    <DialogFooter>
                      <Button
                        type="button"
                        variant="outline"
                        onClick={() => setCreateDialogOpen(false)}
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

        {loading ? (
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
                    <TableHead className="text-[#E5E7EB]">Started Date</TableHead>
                    <TableHead className="text-[#E5E7EB]">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {projects.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={4} className="text-center py-8 text-[#9CA3AF]">
                        No projects found
                      </TableCell>
                    </TableRow>
                  ) : (
                    projects.map((project) => (
                      <TableRow key={project.id} className="border-[#30363D] hover:bg-[#21262D]">
                        <TableCell className="text-[#E5E7EB]">
                          <button
                            onClick={() => navigate(`/projects/${project.id}`)}
                            className="hover:text-[#8B5CF6] transition-colors"
                          >
                            {project.title}
                          </button>
                        </TableCell>
                        <TableCell>
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
                        </TableCell>
                        <TableCell className="text-[#9CA3AF]">
                          {new Date(project.startedDate).toLocaleDateString('tr-TR')}
                        </TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => navigate(`/projects/${project.id}`)}
                            className="text-[#8B5CF6] hover:text-[#7C3AED] hover:bg-[#21262D]"
                          >
                            <Eye className="w-4 h-4 mr-2" />
                            View
                          </Button>
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
  );
}

