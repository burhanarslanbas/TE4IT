/**
 * Project Info Component
 * Proje açıklaması ve tarih bilgileri
 */
import { Project } from '../../../../types';
import { Calendar, Layers, UserPlus } from 'lucide-react';
import { Button } from '../../../../components/ui/button';

interface ProjectInfoProps {
  project: Project;
  onInviteMember?: () => void;
  canManage?: boolean;
}

export function ProjectInfo({ project, onInviteMember, canManage = false }: ProjectInfoProps) {
  return (
    <>
      {project.description && (
        <div className="mb-6 p-4 bg-[#0D1117]/40 rounded-xl border border-[#30363D]/30">
          <h3 className="text-sm font-semibold text-[#9CA3AF] mb-2 flex items-center gap-2">
            <Layers className="w-4 h-4" />
            Açıklama:
          </h3>
          <p className="text-[#E5E7EB] leading-relaxed">{project.description}</p>
        </div>
      )}

      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
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

        {canManage && onInviteMember && (
          <Button
            onClick={onInviteMember}
            className="bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] hover:from-[#14B8A6] hover:to-[#0D9488] text-white shadow-lg shadow-[#2DD4BF]/40"
            size="sm"
          >
            <UserPlus className="w-4 h-4 mr-2" />
            Üye Ekle
          </Button>
        )}
      </div>
    </>
  );
}

