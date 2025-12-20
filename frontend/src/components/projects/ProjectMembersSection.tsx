/**
 * Project Members Section
 * Proje üyelerini listeleme ve yönetim
 */

import { useState, useEffect } from 'react';
import { ProjectService } from '../../services/projectService';
import type { ProjectMember } from '../../types';
import { ProjectRole } from '../../types';
import { Button } from '../ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { toast } from 'sonner';
import { Users, UserX, Loader2, Crown, Eye, User, UserPlus } from 'lucide-react';
import { motion } from 'motion/react';
import { ConfirmDeleteDialog } from '../ConfirmDeleteDialog';

interface ProjectMembersSectionProps {
  projectId: string;
  canManage: boolean; // Üye yönetimi yapabilir mi?
  onInviteClick?: () => void; // Üye davet et butonu için callback
}

export function ProjectMembersSection({
  projectId,
  canManage,
  onInviteClick,
}: ProjectMembersSectionProps) {
  const [members, setMembers] = useState<ProjectMember[]>([]);
  const [loading, setLoading] = useState(false);
  const [updatingRole, setUpdatingRole] = useState<string | null>(null);
  const [removingMember, setRemovingMember] = useState<string | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [memberToRemove, setMemberToRemove] = useState<ProjectMember | null>(null);

  useEffect(() => {
    loadMembers();
  }, [projectId]);

  const loadMembers = async () => {
    try {
      setLoading(true);
      const membersList = await ProjectService.getProjectMembers(projectId);
      setMembers(membersList);
    } catch (error: any) {
      toast.error('Üyeler yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setLoading(false);
    }
  };

  const handleRoleChange = async (userId: string, newRole: ProjectRole) => {
    try {
      setUpdatingRole(userId);
      await ProjectService.updateMemberRole(projectId, userId, newRole);
      toast.success('Rol güncellendi');
      loadMembers();
    } catch (error: any) {
      toast.error('Rol güncellenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setUpdatingRole(null);
    }
  };

  const handleRemoveClick = (member: ProjectMember) => {
    setMemberToRemove(member);
    setDeleteDialogOpen(true);
  };

  const handleRemoveConfirm = async () => {
    if (!memberToRemove) return;

    try {
      setRemovingMember(memberToRemove.userId);
      await ProjectService.removeMember(projectId, memberToRemove.userId);
      toast.success('Üye kaldırıldı');
      loadMembers();
      setDeleteDialogOpen(false);
      setMemberToRemove(null);
    } catch (error: any) {
      toast.error('Üye kaldırılamadı', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setRemovingMember(null);
    }
  };

  const getRoleIcon = (role: ProjectRole) => {
    switch (role) {
      case ProjectRole.Owner:
        return <Crown className="w-4 h-4 text-yellow-500" />;
      case ProjectRole.Member:
        return <User className="w-4 h-4 text-[#2DD4BF]" />;
      case ProjectRole.Viewer:
        return <Eye className="w-4 h-4 text-[#6B7280]" />;
      default:
        return <User className="w-4 h-4" />;
    }
  };

  const getRoleLabel = (role: ProjectRole) => {
    switch (role) {
      case ProjectRole.Owner:
        return 'Sahip';
      case ProjectRole.Member:
        return 'Üye';
      case ProjectRole.Viewer:
        return 'Görüntüleyen';
      default:
        return role;
    }
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="bg-[#161B22]/50 backdrop-blur-md border border-[#2DD4BF]/20 rounded-xl p-6 space-y-4"
    >
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-[#E5E7EB] flex items-center gap-2">
          <Users className="w-5 h-5 text-[#2DD4BF]" />
          Proje Üyeleri ({members.length})
        </h2>
        {canManage && onInviteClick && (
          <Button
            onClick={onInviteClick}
            size="sm"
            className="bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] hover:from-[#14B8A6] hover:to-[#0D9488] text-white shadow-lg shadow-[#2DD4BF]/40"
          >
            <UserPlus className="w-4 h-4 mr-2" />
            Üye Ekle
          </Button>
        )}
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-8">
          <Loader2 className="w-6 h-6 animate-spin text-[#2DD4BF]" />
        </div>
      ) : members.length === 0 ? (
        <div className="text-center py-8 text-[#6B7280]">
          <p>Henüz üye yok</p>
        </div>
      ) : (
        <div className="space-y-3">
          {members.map((member) => (
            <motion.div
              key={member.userId}
              initial={{ opacity: 0, x: -10 }}
              animate={{ opacity: 1, x: 0 }}
              className="flex items-center justify-between p-4 bg-[#0D1117] border border-[#30363D] rounded-lg hover:border-[#2DD4BF]/50 transition-colors"
            >
              <div className="flex items-center gap-3 flex-1">
                <div className="w-10 h-10 rounded-full bg-gradient-to-br from-[#2DD4BF] to-[#14B8A6] flex items-center justify-center text-white text-sm font-medium">
                  {member.userName[0]?.toUpperCase() || 'U'}
                </div>
                <div className="flex-1">
                  <p className="font-medium text-[#E5E7EB]">{member.userName}</p>
                  <p className="text-xs text-[#6B7280]">{member.email}</p>
                </div>
              </div>

              <div className="flex items-center gap-3">
                {/* Rol */}
                {canManage && member.role !== ProjectRole.Owner ? (
                  <Select
                    value={member.role}
                    onValueChange={(value) =>
                      handleRoleChange(member.userId, value as ProjectRole)
                    }
                    disabled={updatingRole === member.userId}
                  >
                    <SelectTrigger className="w-[140px] bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                      <SelectValue>
                        <div className="flex items-center gap-2">
                          {getRoleIcon(member.role)}
                          <span>{getRoleLabel(member.role)}</span>
                        </div>
                      </SelectValue>
                    </SelectTrigger>
                    <SelectContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                      <SelectItem
                        value={ProjectRole.Viewer}
                        className="hover:bg-[#21262D]"
                      >
                        <div className="flex items-center gap-2">
                          <Eye className="w-4 h-4" />
                          <span>Görüntüleyen</span>
                        </div>
                      </SelectItem>
                      <SelectItem
                        value={ProjectRole.Member}
                        className="hover:bg-[#21262D]"
                      >
                        <div className="flex items-center gap-2">
                          <User className="w-4 h-4" />
                          <span>Üye</span>
                        </div>
                      </SelectItem>
                    </SelectContent>
                  </Select>
                ) : (
                  <div className="flex items-center gap-2 px-3 py-1.5 bg-[#161B22] border border-[#30363D] rounded-md">
                    {getRoleIcon(member.role)}
                    <span className="text-sm text-[#E5E7EB]">
                      {getRoleLabel(member.role)}
                    </span>
                  </div>
                )}

                {/* Kaldır Butonu */}
                {canManage && member.role !== ProjectRole.Owner && (
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => handleRemoveClick(member)}
                    disabled={removingMember === member.userId}
                    className="text-[#EF4444] hover:text-[#EF4444] hover:bg-[#EF4444]/10"
                  >
                    {removingMember === member.userId ? (
                      <Loader2 className="w-4 h-4 animate-spin" />
                    ) : (
                      <UserX className="w-4 h-4" />
                    )}
                  </Button>
                )}
              </div>
            </motion.div>
          ))}
        </div>
      )}

      <ConfirmDeleteDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        onConfirm={handleRemoveConfirm}
        entityType="Task"
        entityName={memberToRemove?.userName || ''}
      />
    </motion.div>
  );
}
