/**
 * Invitations List
 * Bekleyen davetiyeleri listeleme
 */

import { useState, useEffect } from 'react';
import { ProjectService } from '../../services/projectService';
import type { ProjectInvitation } from '../../types';
import { Button } from '../ui/button';
import { toast } from 'sonner';
import { Mail, X, Clock, CheckCircle, XCircle, Loader2 } from 'lucide-react';
import { motion } from 'motion/react';
import { ConfirmDeleteDialog } from '../ConfirmDeleteDialog';

interface InvitationsListProps {
  projectId: string;
  canManage: boolean; // Davetiye yönetimi yapabilir mi?
  refreshKey?: number; // Force refresh için
}

export function InvitationsList({ projectId, canManage, refreshKey }: InvitationsListProps) {
  const [invitations, setInvitations] = useState<ProjectInvitation[]>([]);
  const [loading, setLoading] = useState(false);
  const [cancelling, setCancelling] = useState<string | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [invitationToCancel, setInvitationToCancel] = useState<ProjectInvitation | null>(null);

  useEffect(() => {
    loadInvitations();
  }, [projectId, refreshKey]);

  const loadInvitations = async () => {
    try {
      setLoading(true);
      const invitationsList = await ProjectService.getProjectInvitations(projectId);
      setInvitations(invitationsList);
    } catch (error: any) {
      toast.error('Davetiyeler yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setLoading(false);
    }
  };

  const handleCancelClick = (invitation: ProjectInvitation) => {
    setInvitationToCancel(invitation);
    setDeleteDialogOpen(true);
  };

  const handleCancelConfirm = async () => {
    if (!invitationToCancel) return;

    try {
      setCancelling(invitationToCancel.invitationId);
      await ProjectService.cancelInvitation(
        projectId,
        invitationToCancel.invitationId
      );
      toast.success('Davetiye iptal edildi');
      loadInvitations();
      setDeleteDialogOpen(false);
      setInvitationToCancel(null);
    } catch (error: any) {
      toast.error('Davetiye iptal edilemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setCancelling(null);
    }
  };

  const getStatusIcon = (status: ProjectInvitation['status']) => {
    switch (status) {
      case 'Pending':
        return <Clock className="w-4 h-4 text-yellow-500" />;
      case 'Accepted':
        return <CheckCircle className="w-4 h-4 text-green-500" />;
      case 'Cancelled':
        return <XCircle className="w-4 h-4 text-[#6B7280]" />;
      case 'Expired':
        return <XCircle className="w-4 h-4 text-red-500" />;
      default:
        return <Clock className="w-4 h-4" />;
    }
  };

  const getStatusLabel = (status: ProjectInvitation['status']) => {
    switch (status) {
      case 'Pending':
        return 'Bekliyor';
      case 'Accepted':
        return 'Kabul Edildi';
      case 'Cancelled':
        return 'İptal Edildi';
      case 'Expired':
        return 'Süresi Doldu';
      default:
        return status;
    }
  };

  const getRoleLabel = (role: string) => {
    switch (role) {
      case 'Owner':
        return 'Sahip';
      case 'Member':
        return 'Üye';
      case 'Viewer':
        return 'Görüntüleyen';
      default:
        return role;
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  // Sadece bekleyen davetiyeleri göster
  const pendingInvitations = invitations.filter((inv) => inv.status === 'Pending');

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="bg-[#161B22]/50 backdrop-blur-md border border-[#2DD4BF]/20 rounded-xl p-6 space-y-4"
    >
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-[#E5E7EB] flex items-center gap-2">
          <Mail className="w-5 h-5 text-[#2DD4BF]" />
          Bekleyen Davetiyeler ({pendingInvitations.length})
        </h2>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-8">
          <Loader2 className="w-6 h-6 animate-spin text-[#2DD4BF]" />
        </div>
      ) : pendingInvitations.length === 0 ? (
        <div className="text-center py-8 text-[#6B7280]">
          <p>Bekleyen davetiye yok</p>
        </div>
      ) : (
        <div className="space-y-3">
          {pendingInvitations.map((invitation) => (
            <motion.div
              key={invitation.invitationId}
              initial={{ opacity: 0, x: -10 }}
              animate={{ opacity: 1, x: 0 }}
              className="flex items-center justify-between p-4 bg-[#0D1117] border border-[#30363D] rounded-lg hover:border-[#2DD4BF]/50 transition-colors"
            >
              <div className="flex items-center gap-3 flex-1">
                <div className="w-10 h-10 rounded-full bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] flex items-center justify-center text-white text-sm font-medium">
                  <Mail className="w-5 h-5" />
                </div>
                <div className="flex-1">
                  <p className="font-medium text-[#E5E7EB]">{invitation.email}</p>
                  <div className="flex items-center gap-3 text-xs text-[#6B7280] mt-1">
                    <span>Rol: {getRoleLabel(invitation.role)}</span>
                    <span>•</span>
                    <span>Davet eden: {invitation.invitedByUserName}</span>
                    <span>•</span>
                    <span>Bitiş: {formatDate(invitation.expiresAt)}</span>
                  </div>
                </div>
              </div>

              <div className="flex items-center gap-3">
                {/* Status */}
                <div className="flex items-center gap-2 px-3 py-1.5 bg-[#161B22] border border-[#30363D] rounded-md">
                  {getStatusIcon(invitation.status)}
                  <span className="text-sm text-[#E5E7EB]">
                    {getStatusLabel(invitation.status)}
                  </span>
                </div>

                {/* İptal Butonu */}
                {canManage && (
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => handleCancelClick(invitation)}
                    disabled={cancelling === invitation.invitationId}
                    className="text-[#EF4444] hover:text-[#EF4444] hover:bg-[#EF4444]/10"
                  >
                    {cancelling === invitation.invitationId ? (
                      <Loader2 className="w-4 h-4 animate-spin" />
                    ) : (
                      <X className="w-4 h-4" />
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
        onConfirm={handleCancelConfirm}
        entityType="Task"
        entityName={invitationToCancel?.email || ''}
      />
    </motion.div>
  );
}
