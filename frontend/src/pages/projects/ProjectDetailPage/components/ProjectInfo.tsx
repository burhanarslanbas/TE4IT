/**
 * Project Info Component
 * Proje açıklaması ve tarih bilgileri
 */
import { Project } from '../../../../types';
import { Calendar, Layers } from 'lucide-react';

interface ProjectInfoProps {
  project: Project;
}

export function ProjectInfo({ project }: ProjectInfoProps) {
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
    </>
  );
}

