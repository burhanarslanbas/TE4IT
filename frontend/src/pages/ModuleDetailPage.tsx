/**
 * Module Detail Page
 * Modül detayları ve use case listesi
 */

import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import {
  ArrowLeft,
  Trash2,
  Archive,
  ArchiveRestore,
  Plus,
  Search,
  Eye,
  Layers,
} from 'lucide-react';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Badge } from '../components/ui/badge';
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbSeparator,
} from '../components/ui/breadcrumb';
import { ModuleService } from '../services/moduleService';
import { ProjectService } from '../services/projectService';
import { UseCaseService } from '../services/useCaseService';
import { hasPermission } from '../utils/permissions';
import { ApiError } from '../services/api';
import {
  Module,
  Project,
  UseCase,
} from '../types';
import { toast } from 'sonner';
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from '../components/ui/pagination';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '../components/ui/tooltip';
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog';

export function ModuleDetailPage() {
  const { projectId, moduleId } = useParams<{ projectId: string; moduleId: string }>();
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
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

  useEffect(() => {
    if (projectId && moduleId) {
      loadProject();
      loadModule();
    }
  }, [projectId, moduleId]);

  useEffect(() => {
    if (moduleId) {
      loadUseCases();
    }
  }, [moduleId, page, statusFilter, searchTerm]);

  const loadProject = async () => {
    if (!projectId) return;
    try {
      const data = await ProjectService.getProject(projectId);
      setProject(data);
    } catch (error: any) {
      toast.error('Proje yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    }
  };

  const loadModule = async () => {
    if (!moduleId) return;
    try {
      setLoading(true);
      const [projectData, moduleData] = await Promise.all([
        ProjectService.getProject(projectId!),
        ModuleService.getModule(moduleId!),
      ]);

      setProject(projectData);
      setModule(moduleData);
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
      const response = await UseCaseService.getUseCases(moduleId, {
        page,
        pageSize: 10,
        status: statusFilter === null ? 'All' : (statusFilter ? 'Active' : 'Archived'),
        search: searchTerm || undefined,
      });

      setUseCases(response.items);
      setTotalPages(response.totalPages);
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

  const handleDeleteModule = async () => {
    if (!moduleId || !projectId) return;
    
    console.log('[MODULE DELETE] Starting delete for module:', moduleId);
    
    await ModuleService.deleteModule(moduleId);
    
    console.log('[MODULE DELETE] Successfully deleted, navigating to project');
    toast.success('Modül silindi');
    
    // Navigate to project detail page
    navigate(`/projects/${projectId}`);
  };

  const handleStatusChange = async (newIsActive: boolean) => {
    if (!moduleId || !module) return;
    
    // Eğer zaten istenen durumdaysa işlem yapma
    const currentIsActive = module.status === 'Active';
    if (currentIsActive === newIsActive) {
      return;
    }
    
    try {
      // PATCH isteği at
      const response = await ModuleService.updateModuleStatus(moduleId, newIsActive);
      
      // Response'daki isActive'i kontrol et
      if (response.isActive !== newIsActive) {
        toast.error('Modül durumu güncellenemedi', {
          description: 'Backend beklenen durumu döndürmedi',
        });
        // Refetch ile server state'i doğrula
        await loadModule();
        return;
      }
      
      // Başarılı - toast ve refetch
      toast.success(`Modül ${newIsActive ? 'aktifleştirildi' : 'arşivlendi'}`);
      await loadModule();
    } catch (error: any) {
      toast.error('Modül durumu güncellenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
      // Hata durumunda da refetch - UI ile backend senkronize olsun
      await loadModule();
    }
  };

  const handleSearch = (value: string) => {
    setSearchTerm(value);
    setPage(1);
  };

  const handleFilterChange = (value: string) => {
    if (value === 'All') {
      setStatusFilter(null);
    } else if (value === 'Active') {
      setStatusFilter(true);
    } else {
      setStatusFilter(false);
    }
    setPage(1);
  };

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const canDelete = hasPermission('ModuleDelete');

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
    <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#0D1117] to-[#161B22] text-[#E5E7EB]">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pb-12" style={{ paddingTop: 'calc(var(--navbar-height) + 2rem)' }}>
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
                    Projeler
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
            Projeye Dön
          </Button>
        </motion.div>

        {/* Module Info Section */}
        <div className="bg-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-6 sm:p-8 mb-6 shadow-xl">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-4">
            <div>
              <h1 className="text-3xl font-bold mb-2">Modül: {module.title}</h1>
              <div className="flex items-center gap-3">
                <Badge
                  variant={module.status === 'Active' ? 'default' : 'secondary'}
                  className={
                    module.status === 'Active'
                      ? 'bg-[#10B981] text-white border-[#10B981] border-2 shadow-lg shadow-[#10B981]/30 px-3 py-1.5 flex items-center justify-center'
                      : 'bg-[#6B7280] text-white px-3 py-1.5 flex items-center justify-center'
                  }
                >
                  {module.status === 'Active' ? (
                    <span className="flex items-center gap-1.5 whitespace-nowrap">
                      <span className="w-2 h-2 bg-white rounded-full animate-pulse"></span>
                      <span>Aktif</span>
                    </span>
                  ) : (
                    <span className="whitespace-nowrap">Arşivlendi</span>
                  )}
                </Badge>
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleStatusChange(true)}
                    disabled={module.status === 'Active'}
                    className={`border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] transition-all ${
                      module.status === 'Active'
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
                    disabled={module.status === 'Archived'}
                    className={`border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] transition-all ${
                      module.status === 'Archived'
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

        {/* Use Cases Section */}
        <div className="bg-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-6 sm:p-8 mb-6 shadow-xl">
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
                    variant={useCase.status === 'Active' ? 'default' : 'secondary'}
                    className={
                      useCase.status === 'Active'
                        ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30 border px-3 py-1.5 flex items-center justify-center'
                        : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30 border px-3 py-1.5 flex items-center justify-center'
                    }
                  >
                    <span className="whitespace-nowrap">{useCase.status === 'Active' ? 'Aktif' : 'Arşivlenmiş'}</span>
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
                    Görüntüle
                  </Button>
                </div>
              </motion.div>
            ))}
          </div>
        </div>

        {/* Delete Module Button */}
        {canDelete && (
          <div className="mb-6">
            <Button
              variant="destructive"
              onClick={() => setDeleteDialogOpen(true)}
              className="bg-[#EF4444] hover:bg-[#DC2626] text-white"
            >
              <Trash2 className="w-4 h-4 mr-2" />
              Modülü Sil
            </Button>
            <ConfirmDeleteDialog
              open={deleteDialogOpen}
              onOpenChange={setDeleteDialogOpen}
              entityType="Module"
              entityName={module.title}
              onConfirm={handleDeleteModule}
              children={module.useCaseCount ? [{ count: module.useCaseCount, type: 'use case' }] : undefined}
            />
          </div>
        )}

        {/* Use Cases Section */}
        <div className="bg-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-6 sm:p-8 mb-6 shadow-xl">
          <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between mb-4">
            <h2 className="text-2xl font-bold">Use Cases</h2>
            <div className="flex flex-1 gap-3 items-center">
              {/* Search Icon Button */}
              <div className="flex items-center justify-center w-10 h-10 bg-[#161B22] border border-[#30363D] rounded-xl">
                <Search className="text-[#9CA3AF] w-5 h-5" />
              </div>
              
              {/* Search Input */}
              <div className="relative flex-1 max-w-md">
                <Input
                  placeholder="Use case ara..."
                  value={searchTerm}
                  onChange={(e) => handleSearch(e.target.value)}
                  className="h-10 rounded-xl pl-4 pr-4 bg-[#161B22] border-[#30363D] text-[#E5E7EB]"
                />
              </div>
              
              <Select value={statusFilter === null ? 'All' : (statusFilter ? 'Active' : 'Archived')} onValueChange={handleFilterChange}>
                <SelectTrigger className="w-[180px] bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                  <SelectValue placeholder="Filtre" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="All">Tümü</SelectItem>
                  <SelectItem value="Active">Aktif</SelectItem>
                  <SelectItem value="Archived">Arşivlenmiş</SelectItem>
                </SelectContent>
              </Select>

              {true && ( // hasPermission(PERMISSIONS.USECASE_CREATE) - temporarily bypassed for development
                <TooltipProvider>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <span>
                        <Button 
                          onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/new`)}
                          disabled={module.status !== 'Active'}
                          className="bg-[#8B5CF6] hover:bg-[#7C3AED] text-white disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                          <Plus className="w-4 h-4 mr-2" />
                          Use Case Oluştur
                        </Button>
                      </span>
                    </TooltipTrigger>
                    {module.status !== 'Active' && (
                      <TooltipContent>
                        <p>Use case oluşturmak için modülü aktifleştirin</p>
                      </TooltipContent>
                    )}
                  </Tooltip>
                </TooltipProvider>
              )}
            </div>
          </div>

          {useCasesLoading ? (
            <div className="bg-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-2xl overflow-hidden shadow-xl">
              <Table>
                <TableHeader>
                  <TableRow className="border-[#30363D] hover:bg-[#21262D]/50">
                    <TableHead className="text-[#E5E7EB] text-left">Başlık</TableHead>
                    <TableHead className="text-[#E5E7EB] text-center">Durum</TableHead>
                    <TableHead className="text-[#E5E7EB] text-center">Tasks</TableHead>
                    <TableHead className="text-[#E5E7EB] text-center">İşlemler</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {[1, 2, 3, 4, 5, 6].map((i) => (
                    <TableRow key={i} className="border-[#30363D]/50 hover:bg-[#21262D]/30">
                      <TableCell>
                        <div className="h-5 bg-[#21262D] rounded animate-pulse w-3/4" />
                      </TableCell>
                      <TableCell>
                        <div className="h-6 bg-[#21262D] rounded-full animate-pulse w-20" />
                      </TableCell>
                      <TableCell>
                        <div className="h-5 bg-[#21262D] rounded animate-pulse w-8" />
                      </TableCell>
                      <TableCell>
                        <div className="h-8 bg-[#21262D] rounded animate-pulse w-20" />
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          ) : (
            <>
              <div className="bg-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-2xl overflow-hidden shadow-xl">
                <Table>
                  <TableHeader>
                    <TableRow className="border-[#30363D] hover:bg-[#21262D]">
                      <TableHead className="text-[#E5E7EB] text-left">Başlık</TableHead>
                      <TableHead className="text-[#E5E7EB] text-center">Durum</TableHead>
                      <TableHead className="text-[#E5E7EB] text-center">Tasks</TableHead>
                      <TableHead className="text-[#E5E7EB] text-center">İşlemler</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {useCases.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={4} className="text-center py-16">
                          <div className="flex flex-col items-center justify-center">
                            <div className="w-16 h-16 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-2xl flex items-center justify-center mb-4">
                              <Layers className="w-8 h-8 text-[#8B5CF6]" />
                            </div>
                            <h3 className="text-lg font-semibold text-[#E5E7EB] mb-2">
                              Henüz use case yok
                            </h3>
                            <p className="text-[#9CA3AF] mb-6 max-w-md">
                              İş yapısını oluşturmaya başlamak için ilk use case'inizi oluşturun.
                            </p>
                            {true && ( // hasPermission(PERMISSIONS.USECASE_CREATE) - temporarily bypassed for development
                              <TooltipProvider>
                                <Tooltip>
                                  <TooltipTrigger asChild>
                                    <span>
                                      <Button
                                        onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/new`)}
                                        disabled={module.status !== 'Active'}
                                        className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                                      >
                                        <Plus className="w-4 h-4 mr-2" />
                                        Use Case Oluştur
                                      </Button>
                                    </span>
                                  </TooltipTrigger>
                                  {module.status !== 'Active' && (
                                    <TooltipContent>
                                      <p>Use case oluşturmak için modülü aktifleştirin</p>
                                    </TooltipContent>
                                  )}
                                </Tooltip>
                              </TooltipProvider>
                            )}
                          </div>
                        </TableCell>
                      </TableRow>
                    ) : (
                      useCases.map((useCase) => (
                        <TableRow key={useCase.id} className="border-[#30363D] hover:bg-[#21262D]">
                          <TableCell className="text-[#E5E7EB] text-left">
                            <button
                              onClick={() => {
                                const targetPath = `/projects/${projectId}/modules/${moduleId}/usecases/${useCase.id}`;
                                console.log('🔵 Title clicked - Navigating to:', targetPath);
                                console.log('📍 UseCase:', { id: useCase.id, title: useCase.title });
                                navigate(targetPath);
                              }}
                              className="hover:text-[#8B5CF6] transition-colors cursor-pointer text-left"
                            >
                              {useCase.title}
                            </button>
                          </TableCell>
                          <TableCell className="text-center">
                            <div className="flex justify-center">
                              <Badge
                                variant={useCase.status === 'Active' ? 'default' : 'secondary'}
                                className={
                                  useCase.status === 'Active'
                                    ? 'bg-[#10B981] text-white px-3 py-1.5 flex items-center justify-center'
                                    : 'bg-[#6B7280] text-white px-3 py-1.5 flex items-center justify-center'
                                }
                              >
                                <span className="whitespace-nowrap">{useCase.status === 'Active' ? 'Aktif' : 'Arşivlenmiş'}</span>
                              </Badge>
                            </div>
                          </TableCell>
                          <TableCell className="text-[#9CA3AF] text-center">
                            <span className="text-sm">
                              {useCase.taskCount || 0}
                            </span>
                          </TableCell>
                          <TableCell className="text-center">
                            <div className="flex justify-center gap-2">
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => {
                                  const targetPath = `/projects/${projectId}/modules/${moduleId}/usecases/${useCase.id}`;
                                  console.log('👁️ View button clicked - Navigating to:', targetPath);
                                  console.log('📍 Route params:', { projectId, moduleId, useCaseId: useCase.id });
                                  navigate(targetPath);
                                }}
                                className="text-[#8B5CF6] hover:text-[#7C3AED] hover:bg-[#21262D]"
                              >
                                <Eye className="w-4 h-4 mr-2" />
                                Görüntüle
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
                          onClick={() => page > 1 && handlePageChange(page - 1)}
                          className={page === 1 ? 'pointer-events-none opacity-50' : 'cursor-pointer'}
                        />
                      </PaginationItem>
                      {Array.from({ length: totalPages }, (_, i) => i + 1).map((pageNum) => (
                        <PaginationItem key={pageNum}>
                          <PaginationLink
                            onClick={() => handlePageChange(pageNum)}
                            isActive={page === pageNum}
                            className="cursor-pointer"
                          >
                            {pageNum}
                          </PaginationLink>
                        </PaginationItem>
                      ))}
                      <PaginationItem>
                        <PaginationNext
                          onClick={() => page < totalPages && handlePageChange(page + 1)}
                          className={page === totalPages ? 'pointer-events-none opacity-50' : 'cursor-pointer'}
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
};

