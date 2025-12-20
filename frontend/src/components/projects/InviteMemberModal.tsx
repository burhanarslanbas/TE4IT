/**
 * Invite Member Modal
 * Projeye üye davet etme modal'ı
 * Email ile davetiye gönderme
 */

import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '../ui/dialog';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { ProjectService } from '../../services/projectService';
import { ProjectRole } from '../../types';
import { projectRoleToNumeric } from '../../utils/projectRoleMapping';
import { toast } from 'sonner';
import { UserPlus, Loader2, Mail } from 'lucide-react';
import { motion } from 'motion/react';

interface InviteMemberModalProps {
  projectId: string;
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export function InviteMemberModal({
  projectId,
  open,
  onClose,
  onSuccess,
}: InviteMemberModalProps) {
  const [email, setEmail] = useState('');
  const [selectedRole, setSelectedRole] = useState<ProjectRole>(ProjectRole.Member);
  const [inviting, setInviting] = useState(false);
  const [emailError, setEmailError] = useState('');

  // Modal kapandığında state'i temizle
  useEffect(() => {
    if (!open) {
      setEmail('');
      setSelectedRole(ProjectRole.Member);
      setEmailError('');
    }
  }, [open]);

  // Email validation
  const validateEmail = (emailValue: string): boolean => {
    if (!emailValue.trim()) {
      setEmailError('Email adresi zorunludur');
      return false;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(emailValue.trim())) {
      setEmailError('Geçerli bir email adresi girin');
      return false;
    }

    setEmailError('');
    return true;
  };

  const handleEmailChange = (value: string) => {
    setEmail(value);
    if (emailError) {
      validateEmail(value);
    }
  };

  const handleInvite = async () => {
    // Validation
    if (!validateEmail(email)) {
      return;
    }

    try {
      setInviting(true);
      // Backend numeric role bekliyor, string enum'u numeric'e çevir
      await ProjectService.inviteProjectMember(projectId, {
        email: email.trim().toLowerCase(),
        role: projectRoleToNumeric(selectedRole),
      });

      toast.success('Davetiye gönderildi', {
        description: `${email} adresine email gönderildi`,
      });

      onSuccess();
      onClose();
    } catch (error: any) {
      toast.error('Davetiye gönderilemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setInviting(false);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !inviting && email && !emailError) {
      handleInvite();
    }
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] max-w-md">
        <DialogHeader>
          <DialogTitle className="text-xl font-semibold flex items-center gap-2">
            <UserPlus className="w-5 h-5 text-[#2DD4BF]" />
            Projeye Üye Davet Et
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-6 py-4">
          {/* Email Input */}
          <div className="space-y-2">
            <Label htmlFor="email" className="text-sm font-medium">
              Email Adresi *
            </Label>
            <div className="relative">
              <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-[#6B7280]" />
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => handleEmailChange(e.target.value)}
                onBlur={() => validateEmail(email)}
                onKeyPress={handleKeyPress}
                placeholder="ornek@email.com"
                className={`pl-10 bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:border-[#2DD4BF] focus:ring-2 focus:ring-[#2DD4BF]/20 ${
                  emailError ? 'border-[#EF4444] focus:border-[#EF4444] focus:ring-[#EF4444]/20' : ''
                }`}
                disabled={inviting}
              />
            </div>
            {emailError && (
              <p className="text-[#EF4444] text-xs flex items-center gap-1">
                <span className="w-1 h-1 bg-[#EF4444] rounded-full"></span>
                {emailError}
              </p>
            )}
            <p className="text-xs text-[#6B7280]">
              TE4IT sistemine kayıtlı kullanıcının email adresini girin
            </p>
          </div>

          {/* Rol Seçimi */}
          <div className="space-y-2">
            <Label htmlFor="role" className="text-sm font-medium">
              Rol
            </Label>
            <Select
              value={selectedRole}
              onValueChange={(value) => setSelectedRole(value as ProjectRole)}
              disabled={inviting}
            >
              <SelectTrigger
                id="role"
                className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#2DD4BF]/20"
              >
                <SelectValue />
              </SelectTrigger>
              <SelectContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                <SelectItem value={ProjectRole.Viewer} className="hover:bg-[#21262D]">
                  <div className="flex flex-col">
                    <span className="font-medium">Viewer</span>
                    <span className="text-xs text-[#6B7280]">Sadece görüntüleme</span>
                  </div>
                </SelectItem>
                <SelectItem value={ProjectRole.Member} className="hover:bg-[#21262D]">
                  <div className="flex flex-col">
                    <span className="font-medium">Member</span>
                    <span className="text-xs text-[#6B7280]">Düzenleme yetkisi</span>
                  </div>
                </SelectItem>
              </SelectContent>
            </Select>
            <p className="text-xs text-[#6B7280]">
              {selectedRole === ProjectRole.Viewer
                ? 'Kullanıcı sadece projeyi görüntüleyebilir'
                : 'Kullanıcı projeyi düzenleyebilir ve task atanabilir'}
            </p>
          </div>
        </div>

        <DialogFooter className="gap-2">
          <Button
            variant="outline"
            onClick={onClose}
            disabled={inviting}
            className="border-[#30363D] text-[#9CA3AF] hover:bg-[#21262D] hover:text-[#E5E7EB]"
          >
            İptal
          </Button>
          <Button
            onClick={handleInvite}
            disabled={!email.trim() || !!emailError || inviting}
            className="bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] hover:from-[#14B8A6] hover:to-[#0D9488] text-white disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {inviting ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Gönderiliyor...
              </>
            ) : (
              <>
                <Mail className="w-4 h-4 mr-2" />
                Davetiye Gönder
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
