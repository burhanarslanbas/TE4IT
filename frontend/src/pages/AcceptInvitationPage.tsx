/**
 * Accept Invitation Page
 * Proje davetiyesini kabul etme sayfası
 * Public route - token ile erişim
 * Route: /accept-invitation?token=xxx
 */

import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { InvitationService } from '../services/invitationService';
import { AuthService } from '../services/auth';
import type { ProjectInvitationDetail } from '../types';
import { Button } from '../components/ui/button';
import { toast } from 'sonner';
import { Mail, CheckCircle, Loader2, AlertCircle, UserPlus, Calendar, Crown, Eye, User } from 'lucide-react';
import { motion } from 'motion/react';

export function AcceptInvitationPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get('token');

  const [invitation, setInvitation] = useState<ProjectInvitationDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [accepting, setAccepting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    // Token kontrolü
    if (!token) {
      setError('Davetiye token\'ı bulunamadı');
      setLoading(false);
      return;
    }

    // Authentication kontrolü
    checkAuthentication();
    loadInvitation();
  }, [token]);

  const checkAuthentication = async () => {
    try {
      const user = await AuthService.getCurrentUser();
      setIsAuthenticated(!!user);
    } catch {
      setIsAuthenticated(false);
    }
  };

  const loadInvitation = async () => {
    if (!token) return;

    try {
      setLoading(true);
      setError(null);
      const invitationData = await InvitationService.getInvitationByToken(token);
      setInvitation(invitationData);
    } catch (error: any) {
      setError(error.message || 'Davetiye yüklenemedi');
    } finally {
      setLoading(false);
    }
  };

  const handleAccept = async () => {
    if (!token || !isAuthenticated) {
      toast.error('Lütfen önce giriş yapın', {
        description: 'Davetiye kabul etmek için giriş yapmanız gerekiyor',
      });
      navigate('/login', { state: { returnTo: `/accept-invitation?token=${token}` } });
      return;
    }

    try {
      setAccepting(true);
      await InvitationService.acceptInvitation(token);

      toast.success('Davetiye kabul edildi!', {
        description: 'Artık projenin üyesisiniz',
      });

      // Proje detay sayfasına yönlendir
      if (invitation?.projectId) {
        setTimeout(() => {
          navigate(`/projects/${invitation.projectId}`);
        }, 1500);
      } else {
        navigate('/projects');
      }
    } catch (error: any) {
      toast.error('Davetiye kabul edilemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setAccepting(false);
    }
  };

  const getRoleIcon = (role: string) => {
    switch (role) {
      case 'Owner':
        return <Crown className="w-4 h-4 text-yellow-500" />;
      case 'Member':
        return <User className="w-4 h-4 text-[#2DD4BF]" />;
      case 'Viewer':
        return <Eye className="w-4 h-4 text-[#6B7280]" />;
      default:
        return <User className="w-4 h-4" />;
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
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

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
          <Loader2 className="w-12 h-12 animate-spin text-[#2DD4BF] mx-auto mb-4" />
          <p className="text-[#9CA3AF] text-lg">Davetiye yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (error || !invitation) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center relative overflow-hidden">
        <div className="fixed inset-0 pointer-events-none overflow-hidden">
          <motion.div
            className="absolute -top-40 -right-40 w-[600px] h-[600px] bg-[#EF4444] opacity-[0.08] blur-[120px] rounded-full"
            animate={{ scale: [1, 1.2, 1] }}
            transition={{ duration: 20, repeat: Infinity }}
          />
        </div>
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8 max-w-md mx-4 text-center relative z-10"
        >
          <div className="w-16 h-16 bg-[#EF4444]/20 rounded-full flex items-center justify-center mx-auto mb-4">
            <AlertCircle className="w-8 h-8 text-[#EF4444]" />
          </div>
          <h1 className="text-2xl font-bold text-[#E5E7EB] mb-2">Davetiye Bulunamadı</h1>
          <p className="text-[#9CA3AF] mb-6">{error || 'Davetiye geçersiz veya süresi dolmuş olabilir'}</p>
          <Button
            onClick={() => navigate('/projects')}
            className="bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] hover:from-[#14B8A6] hover:to-[#0D9488] text-white"
          >
            Projelere Dön
          </Button>
        </motion.div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#0D1117] text-[#E5E7EB] relative overflow-hidden">
      {/* Animated Background */}
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
      <div className="relative z-10 flex items-center justify-center min-h-screen px-4 py-12">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8 max-w-2xl w-full shadow-lg"
        >
          {/* Header */}
          <div className="text-center mb-8">
            <div className="w-16 h-16 bg-gradient-to-br from-[#2DD4BF] to-[#14B8A6] rounded-full flex items-center justify-center mx-auto mb-4">
              <Mail className="w-8 h-8 text-white" />
            </div>
            <h1 className="text-3xl font-bold text-[#E5E7EB] mb-2">Proje Davetiyesi</h1>
            <p className="text-[#9CA3AF]">Aşağıdaki projeye katılmak için davetiye kabul edin</p>
          </div>

          {/* Invitation Details */}
          <div className="space-y-6 mb-8">
            {/* Project Info */}
            <div className="bg-[#0D1117]/40 rounded-xl p-6 border border-[#30363D]/30">
              <h2 className="text-xl font-semibold text-[#E5E7EB] mb-4 flex items-center gap-2">
                <UserPlus className="w-5 h-5 text-[#2DD4BF]" />
                Proje Bilgileri
              </h2>
              <div className="space-y-3">
                <div>
                  <p className="text-sm text-[#9CA3AF] mb-1">Proje Adı</p>
                  <p className="text-lg font-medium text-[#E5E7EB]">{invitation.projectTitle}</p>
                </div>
                <div>
                  <p className="text-sm text-[#9CA3AF] mb-1">Davet Edilen Email</p>
                  <p className="text-[#E5E7EB]">{invitation.email}</p>
                </div>
                <div>
                  <p className="text-sm text-[#9CA3AF] mb-1">Rol</p>
                  <div className="flex items-center gap-2">
                    {getRoleIcon(invitation.role)}
                    <span className="text-[#E5E7EB] font-medium">{getRoleLabel(invitation.role)}</span>
                  </div>
                </div>
                <div>
                  <p className="text-sm text-[#9CA3AF] mb-1">Davet Eden</p>
                  <p className="text-[#E5E7EB]">{invitation.invitedByUserName}</p>
                </div>
                <div>
                  <p className="text-sm text-[#9CA3AF] mb-1 flex items-center gap-2">
                    <Calendar className="w-4 h-4" />
                    Bitiş Tarihi
                  </p>
                  <p className="text-[#E5E7EB]">{formatDate(invitation.expiresAt)}</p>
                </div>
              </div>
            </div>

            {/* Warning if not authenticated */}
            {!isAuthenticated && (
              <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
                className="bg-[#F59E0B]/10 border border-[#F59E0B]/30 rounded-lg p-4"
              >
                <div className="flex items-start gap-3">
                  <AlertCircle className="w-5 h-5 text-[#F59E0B] flex-shrink-0 mt-0.5" />
                  <div>
                    <p className="text-[#F59E0B] font-semibold text-sm mb-1">Giriş Gerekli</p>
                    <p className="text-[#E5E7EB] text-sm">
                      Davetiye kabul etmek için önce TE4IT sistemine giriş yapmanız gerekiyor.
                    </p>
                  </div>
                </div>
              </motion.div>
            )}
          </div>

          {/* Actions */}
          <div className="flex flex-col sm:flex-row gap-3">
            <Button
              variant="outline"
              onClick={() => navigate('/projects')}
              disabled={accepting}
              className="border-[#30363D] text-[#9CA3AF] hover:bg-[#21262D] hover:text-[#E5E7EB] flex-1"
            >
              İptal
            </Button>
            {!isAuthenticated ? (
              <Button
                onClick={() => navigate('/login', { state: { returnTo: `/accept-invitation?token=${token}` } })}
                className="bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] hover:from-[#14B8A6] hover:to-[#0D9488] text-white flex-1"
              >
                Giriş Yap ve Kabul Et
              </Button>
            ) : (
              <Button
                onClick={handleAccept}
                disabled={accepting}
                className="bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] hover:from-[#14B8A6] hover:to-[#0D9488] text-white flex-1 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {accepting ? (
                  <>
                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    Kabul Ediliyor...
                  </>
                ) : (
                  <>
                    <CheckCircle className="w-4 h-4 mr-2" />
                    Davetiye Kabul Et
                  </>
                )}
              </Button>
            )}
          </div>
        </motion.div>
      </div>
    </div>
  );
}
