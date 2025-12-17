/**
 * Project Header Component
 * Proje başlığı, durum ve aksiyon butonları
 */
import { Project } from '../../../../types';
import { Button } from '../../../../components/ui/button';
import { Badge } from '../../../../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '../../../../components/ui/dialog';
import { hasPermission, PERMISSIONS } from '../../../../utils/permissions';
import { Edit, Trash2, Archive, ArchiveRestore } from 'lucide-react';
import { EditProjectForm } from './EditProjectForm';
import { ConfirmDeleteDialog } from '../../../../components/ConfirmDeleteDialog';

interface ProjectHeaderProps {
  project: Project;
  onStatusChange: () => void;
  onEdit: (data: { title: string; description: string }) => Promise<void>;
  onDelete: () => Promise<void>;
  editDialogOpen: boolean;
  setEditDialogOpen: (open: boolean) => void;
  deleteDialogOpen: boolean;
  setDeleteDialogOpen: (open: boolean) => void;
  moduleCount?: number; // Alt öğe uyarısı için
}

export function ProjectHeader({
  project,
  onStatusChange,
  onEdit,
  onDelete,
  editDialogOpen,
  setEditDialogOpen,
  deleteDialogOpen,
  setDeleteDialogOpen,
  moduleCount = 0,
}: ProjectHeaderProps) {
  return (
    <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-6 mb-6">
      <div className="flex-1 min-w-0">
        <h1 className="text-2xl sm:text-3xl font-bold mb-3 text-[#E5E7EB] break-words">
          {project.title}
        </h1>
        <div className="flex flex-wrap items-center gap-3 mb-4">
          <Badge
            className={
              project.status === 'Active'
                ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30 border px-3 py-1'
                : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30 border px-3 py-1'
            }
          >
            {project.status === 'Active' ? (
              <span className="flex items-center gap-1.5">
                <span className="w-1.5 h-1.5 bg-[#10B981] rounded-full animate-pulse" />
                <span className="text-xs font-medium">Aktif</span>
              </span>
            ) : (
              <span className="flex items-center gap-1.5">
                <Archive className="w-3 h-3" />
                <span className="text-xs font-medium">Arşiv</span>
              </span>
            )}
          </Badge>
          <Button
            variant="outline"
            size="sm"
            onClick={onStatusChange}
            className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]/80 h-8 text-xs"
          >
            {project.status === 'Active' ? (
              <>
                <Archive className="w-3 h-3 mr-1.5" />
                Arşivle
              </>
            ) : (
              <>
                <ArchiveRestore className="w-3 h-3 mr-1.5" />
                Aktifleştir
              </>
            )}
          </Button>
        </div>
      </div>
      <div className="flex flex-wrap gap-2">
        {hasPermission(PERMISSIONS.PROJECT_UPDATE) && (
          <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
            <DialogTrigger asChild>
              <Button variant="outline" className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]/80 h-9 text-sm">
                <Edit className="w-4 h-4 mr-2" />
                <span className="hidden sm:inline">Düzenle</span>
                <span className="sm:hidden">Düzenle</span>
              </Button>
            </DialogTrigger>
            <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
              <DialogHeader>
                <DialogTitle>Projeyi Düzenle</DialogTitle>
              </DialogHeader>
              <EditProjectForm
                project={project}
                onSubmit={onEdit}
                onCancel={() => setEditDialogOpen(false)}
              />
            </DialogContent>
          </Dialog>
        )}
        {hasPermission(PERMISSIONS.PROJECT_DELETE) && (
          <>
            <Button
              variant="destructive"
              onClick={() => setDeleteDialogOpen(true)}
              className="bg-[#EF4444] hover:bg-[#DC2626] text-white h-9 text-sm"
            >
              <Trash2 className="w-4 h-4 mr-2" />
              <span className="hidden sm:inline">Sil</span>
              <span className="sm:hidden">Sil</span>
            </Button>
            <ConfirmDeleteDialog
              open={deleteDialogOpen}
              onOpenChange={setDeleteDialogOpen}
              entityType="Project"
              entityName={project.title}
              onConfirm={onDelete}
              children={moduleCount > 0 ? [{ count: moduleCount, type: 'modül' }] : undefined}
            />
          </>
        )}
      </div>
    </div>
  );
}

