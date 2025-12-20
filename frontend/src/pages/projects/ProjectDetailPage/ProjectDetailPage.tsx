/**
 * Project Detail Page - Refactored
 * SOLID prensiplerine uygun, component bazlı yapı
 * Route: /projects/:projectId
 */
import { useState, useEffect, useMemo } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import { ProjectService } from '../../../services/projectService';
import { useProject } from '../../../hooks/useProject';
import { useModules } from '../../../hooks/useModules';
import { useAuth } from '../../../hooks/useAuth';
import { hasPermission, PERMISSIONS } from '../../../utils/permissions';
import type { ModuleFilters, ProjectMember } from '../../../types';
import { ProjectRole } from '../../../types';
import { numericToProjectRole } from '../../../utils/projectRoleMapping';
import { getToken } from '../../../utils/tokenManager';
import { Button } from '../../../components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from '../../../components/ui/dialog';
import { toast } from 'sonner';
import { ArrowLeft, Plus } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../../../components/ui/breadcrumb';
import { ProjectHeader } from './components/ProjectHeader';
import { ProjectInfo } from './components/ProjectInfo';
import { ModuleFiltersComponent } from './components/ModuleFilters';
import { ModuleList } from './components/ModuleList';
import { EditProjectForm } from './components/EditProjectForm';
import { ProjectMembersSection } from '../../../components/projects/ProjectMembersSection';
import { InvitationsList } from '../../../components/projects/InvitationsList';
import { InviteMemberModal } from '../../../components/projects/InviteMemberModal';

export function ProjectDetailPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [filters, setFilters] = useState<ModuleFilters>({
    page: 1,
    pageSize: 20,
    status: 'All',
    search: '',
  });
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [inviteModalOpen, setInviteModalOpen] = useState(false);
  const [membersKey, setMembersKey] = useState(0); // Force re-render için
  const [projectMembers, setProjectMembers] = useState<ProjectMember[]>([]);

  // Custom hooks kullan
  const { project, loading, error, refetch: refetchProject } = useProject(projectId);
  const { modules, loading: modulesLoading, refetch: refetchModules } = useModules(projectId, filters);
  const { user: currentUser, isLoading: authLoading } = useAuth();

  // Proje üyelerini yükle (Owner rolünü bulmak için)
  useEffect(() => {
    if (projectId) {
      ProjectService.getProjectMembers(projectId)
        .then((members) => {
          console.log('[DEBUG] Project members loaded:', members);
          setProjectMembers(members);
        })
        .catch((error) => {
          console.error('[DEBUG] Error loading project members:', error);
          toast.error('Üyeler yüklenemedi', {
            description: error.message || 'Bir hata oluştu',
          });
        });
    }
  }, [projectId, membersKey]);

  // Token'dan email al (fallback olarak)
  const getEmailFromToken = (): string | null => {
    try {
      const token = getToken();
      if (!token) return null;
      
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      
      const payload = JSON.parse(atob(parts[1]));
      return payload.email || payload.Email || payload.emailAddress || null;
    } catch (error) {
      console.error('[DEBUG] Error getting email from token:', error);
      return null;
    }
  };

  // Sadece projeyi oluşturan kullanıcı üye yönetimi yapabilir
  // Backend'den ownerId gelmiyorsa, proje üyelerinden Owner rolüne sahip olanı bul
  const canManageMembers = useMemo(() => {
    const tokenEmail = getEmailFromToken();
    
    console.log('[DEBUG] canManageMembers check:', {
      authLoading,
      currentUser: currentUser ? { id: currentUser.id, email: currentUser.email } : null,
      tokenEmail,
      project: project ? { id: project.id, ownerId: project.ownerId, ownerEmail: project.ownerEmail } : null,
      projectMembersCount: projectMembers.length,
      projectMembers: projectMembers,
    });

    // Auth henüz yükleniyorsa, false dön (henüz bilinmiyor)
    if (authLoading) {
      console.log('[DEBUG] canManageMembers: false (auth still loading)');
      return false;
    }

    if (!project) {
      console.log('[DEBUG] canManageMembers: false (no project)');
      return false;
    }

    // currentUser yoksa ama token'dan email alabiliyorsak, onu kullan
    const userEmail = currentUser?.email || tokenEmail;
    const userId = currentUser?.id;

    if (!userEmail && !userId) {
      console.log('[DEBUG] canManageMembers: false (no user email or id)');
      return false;
    }

    // Önce backend'den gelen ownerId/ownerEmail ile kontrol et
    if (project.ownerId && userId && userId === project.ownerId) {
      console.log('[DEBUG] canManageMembers: true (ownerId match)');
      return true;
    }
    if (project.ownerEmail && userEmail && userEmail === project.ownerEmail) {
      console.log('[DEBUG] canManageMembers: true (ownerEmail match)');
      return true;
    }

    // Backend'den owner bilgisi gelmiyorsa, proje üyelerinden Owner rolüne sahip olanı bul
    // Role artık normalize edilmiş olmalı (string enum)
    const ownerMember = projectMembers.find(
      (member) => member.role === ProjectRole.Owner
    );

    console.log('[DEBUG] Owner member found:', ownerMember);

    if (ownerMember) {
      // Owner rolüne sahip üyenin ID'si veya email'i ile karşılaştır
      const isOwner = 
        (userId && ownerMember.userId === userId) ||
        (userEmail && ownerMember.email === userEmail);
      console.log('[DEBUG] canManageMembers:', isOwner, '(owner member match)', {
        userId,
        ownerMemberUserId: ownerMember.userId,
        userEmail,
        ownerMemberEmail: ownerMember.email,
      });
      return isOwner;
    }

    // Eğer hiç owner bulunamazsa, mevcut kullanıcının proje üyeleri arasında Owner rolüne sahip olup olmadığını kontrol et
    const currentUserMember = projectMembers.find(
      (member) =>
        ((userId && member.userId === userId) || (userEmail && member.email === userEmail)) &&
        member.role === ProjectRole.Owner
    );

    console.log('[DEBUG] Current user member:', currentUserMember);
    const result = !!currentUserMember;
    console.log('[DEBUG] canManageMembers final result:', result);
    return result;
  }, [currentUser, project, projectMembers, authLoading]);

  // Project actions
  const handleUpdateProject = async (data: { title: string; description: string }) => {
    if (!projectId) return;
    try {
      await ProjectService.updateProject(projectId, {
        title: data.title,
        description: data.description || undefined,
      });
      toast.success('Proje güncellendi');
      setEditDialogOpen(false);
      refetchProject();
    } catch (error: any) {
      toast.error('Proje güncellenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    }
  };

  const handleDeleteProject = async () => {
    if (!projectId) return;
    
    console.log('[PROJECT DELETE] Starting delete for project:', projectId);
    
    await ProjectService.deleteProject(projectId);
    
    console.log('[PROJECT DELETE] Successfully deleted, navigating to projects list');
    toast.success('Proje silindi');
    
    // Navigate to projects list
    navigate('/projects');
  };

  const handleStatusChange = async () => {
    if (!projectId || !project) return;
    try {
      const newStatus = project.status === 'Active' ? 'Archived' : 'Active';
      await ProjectService.updateProjectStatus(projectId, newStatus);
      toast.success(`Proje ${newStatus === 'Active' ? 'aktifleştirildi' : 'arşivlendi'}`);
      refetchProject();
    } catch (error: any) {
      toast.error('Proje durumu güncellenemedi', {
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

  const handleModuleClick = (moduleId: string) => {
    navigate(`/projects/${projectId}/modules/${moduleId}`);
  };

  // Loading state
  if (loading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center relative overflow-hidden">
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

  // Error state
  if (error || !project) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center">
        <div className="text-center">
          <p className="text-[#EF4444] mb-4">Proje yüklenemedi</p>
          <Button onClick={() => navigate('/projects')} variant="outline">
            Projelere Dön
          </Button>
        </div>
      </div>
    );
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
      <div className="relative z-10 px-4 sm:px-6 pb-12" style={{ paddingTop: 'calc(var(--navbar-height) + 2rem)' }}>
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
                    <Link to="/projects" className="hover:text-[#8B5CF6] transition-colors">
                      Projeler
                    </Link>
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

          {/* Project Info Section */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4, delay: 0.2 }}
            className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-6 sm:p-8 mb-8 shadow-lg relative overflow-hidden"
          >
            {/* Top Gradient Line */}
            <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-[#8B5CF6]/50 to-transparent" />

            {/* Edit Project Dialog */}
            <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
              <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                <DialogHeader>
                  <DialogTitle>Projeyi Düzenle</DialogTitle>
                </DialogHeader>
                <EditProjectForm
                  project={project}
                  onSubmit={handleUpdateProject}
                  onCancel={() => setEditDialogOpen(false)}
                />
              </DialogContent>
            </Dialog>

            {/* Delete Project Dialog */}
            <ProjectHeader
              project={project}
              onStatusChange={handleStatusChange}
              onEdit={() => {}}
              onDelete={handleDeleteProject}
              editDialogOpen={editDialogOpen}
              setEditDialogOpen={setEditDialogOpen}
              deleteDialogOpen={deleteDialogOpen}
              setDeleteDialogOpen={setDeleteDialogOpen}
              moduleCount={modules.length}
            />

            <ProjectInfo 
              project={project} 
              onInviteMember={() => setInviteModalOpen(true)}
              canManage={hasPermission(PERMISSIONS.PROJECT_UPDATE)}
            />
          </motion.div>

          {/* Members & Invitations Section */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4, delay: 0.25 }}
            className="mb-8 space-y-6"
          >
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl sm:text-2xl font-semibold text-[#E5E7EB]">
                Üye Yönetimi
              </h2>
              {canManageMembers && (
                <Button
                  onClick={() => setInviteModalOpen(true)}
                  className="bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] hover:from-[#14B8A6] hover:to-[#0D9488] text-white shadow-lg shadow-[#2DD4BF]/40"
                >
                  <Plus className="w-4 h-4 mr-2" />
                  Üye Davet Et
                </Button>
              )}
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <ProjectMembersSection
                key={membersKey}
                projectId={projectId!}
                canManage={canManageMembers || false}
                onInviteClick={() => setInviteModalOpen(true)}
              />
              <InvitationsList
                key={membersKey}
                projectId={projectId!}
                canManage={canManageMembers || false}
              />
            </div>

            <InviteMemberModal
              projectId={projectId!}
              open={inviteModalOpen}
              onClose={() => setInviteModalOpen(false)}
              onSuccess={() => {
                setMembersKey((prev) => prev + 1); // Force re-render for both members and invitations
              }}
            />
          </motion.div>

          {/* Modules Section */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4, delay: 0.3 }}
          >
            {/* Control Panel */}
            <div className="mb-6 flex flex-col lg:flex-row items-start lg:items-center justify-between gap-4 bg-[#161B22]/40 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-4 sm:p-6">
              <div className="flex items-center gap-3">
                {projectId && (
                  <motion.button
                    whileHover={{ scale: 1.1 }}
                    whileTap={{ scale: 0.9 }}
                    onClick={() => navigate(`/projects/${projectId}/modules/new`)}
                    className="w-10 h-10 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-lg flex items-center justify-center hover:from-[#8B5CF6]/30 hover:to-[#2DD4BF]/30 transition-all cursor-pointer group"
                    title="Yeni Modül Oluştur"
                  >
                    <Plus className="w-5 h-5 text-[#8B5CF6] group-hover:text-[#7C3AED] group-hover:rotate-90 transition-all duration-300" />
                  </motion.button>
                )}
                <h2 className="text-xl sm:text-2xl font-semibold text-[#E5E7EB]">
                  Modüller
                </h2>
              </div>

              <div className="flex flex-col sm:flex-row gap-3 w-full lg:w-auto">
                <ModuleFiltersComponent
                  filters={filters}
                  onSearchChange={handleSearch}
                  onStatusChange={handleFilterChange}
                />

                {/* Create Module Button - Yeni Sayfaya Yönlendir */}
                {projectId && (
                  <motion.div
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                  >
                    <Button 
                      size="lg"
                      onClick={() => navigate(`/projects/${projectId}/modules/new`)}
                      className="bg-gradient-to-r from-[#8B5CF6] via-[#7C3AED] to-[#6D28D9] text-white hover:from-[#7C3AED] hover:via-[#6D28D9] hover:to-[#5B21B6] shadow-lg shadow-[#8B5CF6]/40 hover:shadow-[#8B5CF6]/60 transition-all duration-300 h-11 px-6 rounded-lg font-semibold whitespace-nowrap group relative overflow-hidden"
                    >
                      <span className="relative z-10 flex items-center gap-2">
                        <Plus className="w-5 h-5 group-hover:rotate-90 transition-transform duration-300" />
                        <span>Yeni Modül Oluştur</span>
                      </span>
                      <motion.div
                        className="absolute inset-0 bg-gradient-to-r from-transparent via-white/10 to-transparent"
                        initial={{ x: '-100%' }}
                        whileHover={{ x: '100%' }}
                        transition={{ duration: 0.6 }}
                      />
                    </Button>
                  </motion.div>
                )}
              </div>
            </div>

            {/* Modules List */}
            <ModuleList
              modules={modules}
              loading={modulesLoading}
              projectId={projectId!}
              onCreateModule={() => navigate(`/projects/${projectId}/modules/new`)}
              onModuleClick={handleModuleClick}
            />
          </motion.div>

          {/* Floating Action Button - Mobil için */}
          {projectId && (
            <motion.button
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              whileHover={{ scale: 1.1 }}
              whileTap={{ scale: 0.9 }}
              onClick={() => navigate(`/projects/${projectId}/modules/new`)}
              className="fixed bottom-6 right-6 lg:hidden z-50 w-16 h-16 bg-gradient-to-r from-[#8B5CF6] via-[#7C3AED] to-[#6D28D9] text-white rounded-full shadow-2xl shadow-[#8B5CF6]/50 hover:shadow-[#8B5CF6]/70 flex items-center justify-center transition-all duration-300 group"
            >
              <Plus className="w-7 h-7 group-hover:rotate-90 transition-transform duration-300" />
              <span className="absolute -top-2 -right-2 w-4 h-4 bg-[#2DD4BF] rounded-full animate-pulse" />
            </motion.button>
          )}
        </div>
      </div>
    </div>
  );
}

export default ProjectDetailPage;

