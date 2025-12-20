/**
 * AI Create Project Tab
 * AI ile roadmap oluşturma ve otomatik proje oluşturma
 */

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { RoadmapService } from '../../services/roadmapService';
import { ProjectService } from '../../services/projectService';
import { ModuleService } from '../../services/moduleService';
import { UseCaseService } from '../../services/useCaseService';
import { TaskService } from '../../services/taskService';
import type { RoadmapResponse, SelectedRoadmapItems } from '../../types/roadmap';
import type { TaskType } from '../../types';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Textarea } from '../ui/textarea';
import { RoadmapPreview } from './RoadmapPreview';
import { toast } from 'sonner';
import { Sparkles, Search, Loader2, CheckCircle2 } from 'lucide-react';
import { motion } from 'motion/react';

interface AICreateProjectForm {
  title: string;
  description: string;
  max_similar_projects: number;
}

interface AICreateProjectTabProps {
  onProjectCreated: (projectId: string) => void;
  onCancel: () => void;
}

/**
 * Priority'yi TaskType'a map et
 */
function mapPriorityToTaskType(priority: 'high' | 'medium' | 'low'): TaskType {
  // Priority'ye göre mantıklı bir mapping
  // High priority genelde Bug veya Feature olabilir
  // Medium/Low genelde Feature veya Documentation
  if (priority === 'high') {
    return 'Bug'; // Yüksek öncelik genelde bug fix
  }
  return 'Feature'; // Diğerleri feature olarak
}

/**
 * Seçilen roadmap öğelerini projeye dönüştür
 */
async function createProjectFromRoadmap(
  roadmap: RoadmapResponse,
  selectedItems: SelectedRoadmapItems,
  projectTitle: string,
  projectDescription?: string
): Promise<string> {
  // 1. Proje oluştur
  const project = await ProjectService.createProject({
    title: projectTitle,
    description: projectDescription,
  });

  let createdCount = 0;
  const totalCount =
    selectedItems.modules.size +
    selectedItems.useCases.size +
    selectedItems.tasks.size;

  // 2. Seçilen modülleri oluştur
  for (const moduleData of roadmap.roadmap) {
    if (!selectedItems.modules.has(moduleData.name)) continue;

    const module = await ModuleService.createModule(project.id, {
      title: moduleData.name,
      description: moduleData.description,
    });
    createdCount++;

    // 3. Seçilen UseCase'leri oluştur
    for (const useCaseData of moduleData.use_cases) {
      const ucKey = `${moduleData.name}|${useCaseData.name}`;
      if (!selectedItems.useCases.has(ucKey)) continue;

      const useCase = await UseCaseService.createUseCase(module.id, {
        title: useCaseData.name,
        description: useCaseData.description,
      });
      createdCount++;

      // 4. Seçilen Task'ları oluştur
      for (let taskIndex = 0; taskIndex < useCaseData.tasks.length; taskIndex++) {
        const taskData = useCaseData.tasks[taskIndex];
        const taskKey = `${moduleData.name}|${useCaseData.name}|${taskIndex}`;
        if (!selectedItems.tasks.has(taskKey)) continue;

        await TaskService.createTask(useCase.id, {
          title: taskData.task,
          description: taskData.description,
          type: mapPriorityToTaskType(taskData.priority),
          // estimated_hours'u dueDate'e çevirebiliriz (opsiyonel)
        });
        createdCount++;
      }
    }
  }

  return project.id;
}

export function AICreateProjectTab({ onProjectCreated, onCancel }: AICreateProjectTabProps) {
  const [isSearching, setIsSearching] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [roadmap, setRoadmap] = useState<RoadmapResponse | null>(null);
  const [selectedItems, setSelectedItems] = useState<SelectedRoadmapItems>({
    modules: new Set(),
    useCases: new Set(),
    tasks: new Set(),
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<AICreateProjectForm>({
    mode: 'onChange',
    defaultValues: {
      max_similar_projects: 3,
    },
  });

  const handleSearch = async (data: AICreateProjectForm) => {
    try {
      setIsSearching(true);
      setRoadmap(null);
      setSelectedItems({
        modules: new Set(),
        useCases: new Set(),
        tasks: new Set(),
      });

      toast.info('AI analiz ediyor...', {
        description: 'Bu işlem 20-40 saniye sürebilir.',
        duration: 3000,
      });

      const result = await RoadmapService.generateRoadmap({
        title: data.title.trim(),
        description: data.description?.trim() || '',
        max_similar_projects: data.max_similar_projects,
      });

      setRoadmap(result);

      // Varsayılan olarak tümünü seç
      const defaultSelected: SelectedRoadmapItems = {
        modules: new Set(),
        useCases: new Set(),
        tasks: new Set(),
      };

      result.roadmap.forEach((module) => {
        defaultSelected.modules.add(module.name);
        module.use_cases.forEach((uc) => {
          const ucKey = `${module.name}|${uc.name}`;
          defaultSelected.useCases.add(ucKey);
          uc.tasks.forEach((_, taskIndex) => {
            const taskKey = `${module.name}|${uc.name}|${taskIndex}`;
            defaultSelected.tasks.add(taskKey);
          });
        });
      });

      setSelectedItems(defaultSelected);

      toast.success('AI analizi tamamlandı!', {
        description: `${result.roadmap.length} modül, ${result.roadmap.reduce(
          (sum, m) => sum + m.use_cases.length,
          0
        )} use case ve ${result.roadmap.reduce(
          (sum, m) => sum + m.use_cases.reduce((ucSum, uc) => ucSum + uc.tasks.length, 0),
          0
        )} task önerildi.`,
      });
    } catch (error: any) {
      console.error('[AI CREATE PROJECT] Search error:', error);
      toast.error('AI analizi başarısız', {
        description: error.message || 'Bir hata oluştu. Lütfen tekrar deneyin.',
        duration: 5000,
      });
    } finally {
      setIsSearching(false);
    }
  };

  const handleCreateProject = async (data: AICreateProjectForm) => {
    if (!roadmap) {
      toast.error('Önce AI analizi yapmalısınız');
      return;
    }

    // En az bir öğe seçilmiş olmalı
    if (
      selectedItems.modules.size === 0 &&
      selectedItems.useCases.size === 0 &&
      selectedItems.tasks.size === 0
    ) {
      toast.error('Lütfen en az bir öğe seçin');
      return;
    }

    try {
      setIsCreating(true);

      toast.info('Proje oluşturuluyor...', {
        description: 'Modüller, use case\'ler ve task\'lar oluşturuluyor.',
      });

      const projectId = await createProjectFromRoadmap(
        roadmap,
        selectedItems,
        data.title.trim(),
        data.description?.trim() || undefined
      );

      const totalCreated =
        selectedItems.modules.size +
        selectedItems.useCases.size +
        selectedItems.tasks.size;

      toast.success('Proje başarıyla oluşturuldu!', {
        description: `${totalCreated} öğe oluşturuldu.`,
        duration: 3000,
      });

      setTimeout(() => {
        onProjectCreated(projectId);
      }, 500);
    } catch (error: any) {
      console.error('[AI CREATE PROJECT] Create error:', error);
      toast.error('Proje oluşturulamadı', {
        description: error.message || 'Bir hata oluştu. Lütfen tekrar deneyin.',
        duration: 5000,
      });
    } finally {
      setIsCreating(false);
    }
  };

  const maxRepos = watch('max_similar_projects');

  return (
    <div className="space-y-6">
      {/* Search Form */}
      <form onSubmit={handleSubmit(handleSearch)} className="space-y-6">
        {/* Project Title */}
        <div>
          <Label htmlFor="ai-title" className="text-[#E5E7EB] text-sm font-medium mb-2 block flex items-center gap-2">
            <Sparkles className="w-4 h-4 text-[#8B5CF6]" />
            Proje Başlığı *
          </Label>
          <Input
            id="ai-title"
            {...register('title', {
              required: 'Proje başlığı zorunludur',
              minLength: { value: 3, message: 'En az 3 karakter olmalıdır' },
              maxLength: { value: 120, message: 'En fazla 120 karakter olabilir' },
            })}
            className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 h-12 text-base"
            placeholder="Örn: E-Ticaret Platformu"
            disabled={isSearching || isCreating}
          />
          {errors.title && (
            <p className="text-[#EF4444] text-xs mt-2 flex items-center gap-1">
              <span className="w-1 h-1 bg-[#EF4444] rounded-full"></span>
              {errors.title.message}
            </p>
          )}
        </div>

        {/* Project Description */}
        <div>
          <Label htmlFor="ai-description" className="text-[#E5E7EB] text-sm font-medium mb-2 block">
            Açıklama *
          </Label>
          <Textarea
            id="ai-description"
            {...register('description', {
              required: 'Açıklama zorunludur (AI analizi için)',
              minLength: { value: 10, message: 'En az 10 karakter olmalıdır' },
              maxLength: { value: 1000, message: 'En fazla 1000 karakter olabilir' },
            })}
            className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[120px] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 resize-none text-base"
            placeholder="Projenizin detaylı açıklamasını yazın. AI bu bilgiyi kullanarak benzer projeleri analiz edecek..."
            disabled={isSearching || isCreating}
          />
          {errors.description && (
            <p className="text-[#EF4444] text-xs mt-2 flex items-center gap-1">
              <span className="w-1 h-1 bg-[#EF4444] rounded-full"></span>
              {errors.description.message}
            </p>
          )}
        </div>

        {/* Max Similar Projects */}
        <div>
          <Label htmlFor="max-repos" className="text-[#E5E7EB] text-sm font-medium mb-2 block">
            Analiz Edilecek Repo Sayısı (1-10) *
          </Label>
          <Input
            id="max-repos"
            type="number"
            min="1"
            max="10"
            {...register('max_similar_projects', {
              required: 'Repo sayısı zorunludur',
              min: { value: 1, message: 'En az 1 repo olmalıdır' },
              max: { value: 10, message: 'En fazla 10 repo olabilir' },
              valueAsNumber: true,
            })}
            className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 h-12 text-base"
            disabled={isSearching || isCreating}
          />
          {errors.max_similar_projects && (
            <p className="text-[#EF4444] text-xs mt-2 flex items-center gap-1">
              <span className="w-1 h-1 bg-[#EF4444] rounded-full"></span>
              {errors.max_similar_projects.message}
            </p>
          )}
          <p className="text-[#6B7280] text-xs mt-2">
            AI, GitHub'da {maxRepos || 3} benzer projeyi analiz edecek
          </p>
        </div>

        {/* Search Button */}
        <div className="flex items-center justify-end gap-3 pt-4 border-t border-[#30363D]/30">
          <Button
            type="button"
            variant="outline"
            onClick={onCancel}
            disabled={isSearching || isCreating}
            className="border-[#30363D] text-[#9CA3AF] hover:bg-[#21262D] hover:text-[#E5E7EB] px-6"
          >
            İptal
          </Button>
          <Button
            type="submit"
            disabled={isSearching || isCreating}
            className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 hover:shadow-[#8B5CF6]/50 px-6 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSearching ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                AI Analiz Ediyor...
              </>
            ) : (
              <>
                <Search className="w-4 h-4 mr-2" />
                Araştır ve Öner
              </>
            )}
          </Button>
        </div>
      </form>

      {/* Loading State */}
      {isSearching && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-8 text-center"
        >
          <Loader2 className="w-12 h-12 text-[#8B5CF6] animate-spin mx-auto mb-4" />
          <p className="text-[#E5E7EB] font-medium mb-2">AI analiz ediyor...</p>
          <p className="text-[#9CA3AF] text-sm">
            GitHub'da benzer projeler aranıyor ve roadmap oluşturuluyor.
            <br />
            Bu işlem 20-40 saniye sürebilir.
          </p>
        </motion.div>
      )}

      {/* Roadmap Preview */}
      {roadmap && !isSearching && (
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4 }}
          className="space-y-6"
        >
          <RoadmapPreview
            roadmap={roadmap}
            selectedItems={selectedItems}
            onSelectionChange={setSelectedItems}
          />

          {/* Create Button */}
          <div className="flex items-center justify-end gap-3 pt-6 border-t border-[#30363D]/30">
            <Button
              type="button"
              variant="outline"
              onClick={onCancel}
              disabled={isCreating}
              className="border-[#30363D] text-[#9CA3AF] hover:bg-[#21262D] hover:text-[#E5E7EB] px-6"
            >
              İptal
            </Button>
            <Button
              onClick={handleSubmit(handleCreateProject)}
              disabled={isCreating}
              className="bg-gradient-to-r from-[#10B981] to-[#059669] text-white hover:from-[#059669] hover:to-[#047857] shadow-lg shadow-[#10B981]/30 hover:shadow-[#10B981]/50 px-6 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isCreating ? (
                <>
                  <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                  Oluşturuluyor...
                </>
              ) : (
                <>
                  <CheckCircle2 className="w-4 h-4 mr-2" />
                  Onayla ve Oluştur
                </>
              )}
            </Button>
          </div>
        </motion.div>
      )}
    </div>
  );
}
