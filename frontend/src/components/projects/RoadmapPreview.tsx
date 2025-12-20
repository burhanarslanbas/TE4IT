/**
 * Roadmap Preview Component
 * AI'dan gelen roadmap önerilerini gösterir ve seçim yapılmasını sağlar
 */

import { useState } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import type { RoadmapResponse, SelectedRoadmapItems } from '../../types/roadmap';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Checkbox } from '../ui/checkbox';
import { Label } from '../ui/label';
import { ChevronDown, ChevronRight, Folder, FileText, CheckSquare, Sparkles } from 'lucide-react';

interface RoadmapPreviewProps {
  roadmap: RoadmapResponse;
  selectedItems: SelectedRoadmapItems;
  onSelectionChange: (selectedItems: SelectedRoadmapItems) => void;
}

export function RoadmapPreview({ roadmap, selectedItems, onSelectionChange }: RoadmapPreviewProps) {
  const [expandedModules, setExpandedModules] = useState<Set<string>>(new Set());
  const [expandedUseCases, setExpandedUseCases] = useState<Set<string>>(new Set());

  // Module toggle
  const toggleModule = (moduleName: string) => {
    const newExpanded = new Set(expandedModules);
    if (newExpanded.has(moduleName)) {
      newExpanded.delete(moduleName);
    } else {
      newExpanded.add(moduleName);
    }
    setExpandedModules(newExpanded);
  };

  // UseCase toggle
  const toggleUseCase = (key: string) => {
    const newExpanded = new Set(expandedUseCases);
    if (newExpanded.has(key)) {
      newExpanded.delete(key);
    } else {
      newExpanded.add(key);
    }
    setExpandedUseCases(newExpanded);
  };

  // Module seçimi
  const handleModuleToggle = (moduleName: string, checked: boolean) => {
    const newSelected = { ...selectedItems };
    if (checked) {
      newSelected.modules.add(moduleName);
      // Modül seçildiğinde tüm UseCase ve Task'ları da seç
      const module = roadmap.roadmap.find((m) => m.name === moduleName);
      if (module) {
        module.use_cases.forEach((uc) => {
          const ucKey = `${moduleName}|${uc.name}`;
          newSelected.useCases.add(ucKey);
          uc.tasks.forEach((_, taskIndex) => {
            const taskKey = `${moduleName}|${uc.name}|${taskIndex}`;
            newSelected.tasks.add(taskKey);
          });
        });
      }
    } else {
      newSelected.modules.delete(moduleName);
      // Modül kaldırıldığında tüm UseCase ve Task'ları da kaldır
      const module = roadmap.roadmap.find((m) => m.name === moduleName);
      if (module) {
        module.use_cases.forEach((uc) => {
          const ucKey = `${moduleName}|${uc.name}`;
          newSelected.useCases.delete(ucKey);
          uc.tasks.forEach((_, taskIndex) => {
            const taskKey = `${moduleName}|${uc.name}|${taskIndex}`;
            newSelected.tasks.delete(taskKey);
          });
        });
      }
    }
    onSelectionChange(newSelected);
  };

  // UseCase seçimi
  const handleUseCaseToggle = (moduleName: string, useCaseName: string, checked: boolean) => {
    const newSelected = { ...selectedItems };
    const ucKey = `${moduleName}|${useCaseName}`;
    if (checked) {
      newSelected.useCases.add(ucKey);
      // UseCase seçildiğinde tüm Task'ları da seç
      const module = roadmap.roadmap.find((m) => m.name === moduleName);
      const useCase = module?.use_cases.find((uc) => uc.name === useCaseName);
      if (useCase) {
        useCase.tasks.forEach((_, taskIndex) => {
          const taskKey = `${moduleName}|${useCaseName}|${taskIndex}`;
          newSelected.tasks.add(taskKey);
        });
      }
      // Modülü de seç
      newSelected.modules.add(moduleName);
    } else {
      newSelected.useCases.delete(ucKey);
      // UseCase kaldırıldığında tüm Task'ları da kaldır
      const module = roadmap.roadmap.find((m) => m.name === moduleName);
      const useCase = module?.use_cases.find((uc) => uc.name === useCaseName);
      if (useCase) {
        useCase.tasks.forEach((_, taskIndex) => {
          const taskKey = `${moduleName}|${useCaseName}|${taskIndex}`;
          newSelected.tasks.delete(taskKey);
        });
      }
    }
    onSelectionChange(newSelected);
  };

  // Task seçimi
  const handleTaskToggle = (moduleName: string, useCaseName: string, taskIndex: number, checked: boolean) => {
    const newSelected = { ...selectedItems };
    const taskKey = `${moduleName}|${useCaseName}|${taskIndex}`;
    if (checked) {
      newSelected.tasks.add(taskKey);
      // Task seçildiğinde UseCase ve Modülü de seç
      newSelected.useCases.add(`${moduleName}|${useCaseName}`);
      newSelected.modules.add(moduleName);
    } else {
      newSelected.tasks.delete(taskKey);
    }
    onSelectionChange(newSelected);
  };

  // Tümünü seç/kaldır
  const handleSelectAll = (checked: boolean) => {
    const newSelected: SelectedRoadmapItems = {
      modules: new Set(),
      useCases: new Set(),
      tasks: new Set(),
    };

    if (checked) {
      roadmap.roadmap.forEach((module) => {
        newSelected.modules.add(module.name);
        module.use_cases.forEach((uc) => {
          const ucKey = `${module.name}|${uc.name}`;
          newSelected.useCases.add(ucKey);
          uc.tasks.forEach((_, taskIndex) => {
            const taskKey = `${module.name}|${uc.name}|${taskIndex}`;
            newSelected.tasks.add(taskKey);
          });
        });
      });
    }

    onSelectionChange(newSelected);
  };

  const allSelected =
    roadmap.roadmap.length > 0 &&
    roadmap.roadmap.every((module) => {
      return (
        selectedItems.modules.has(module.name) &&
        module.use_cases.every((uc) => {
          const ucKey = `${module.name}|${uc.name}`;
          return (
            selectedItems.useCases.has(ucKey) &&
            uc.tasks.every((_, taskIndex) => {
              const taskKey = `${module.name}|${uc.name}|${taskIndex}`;
              return selectedItems.tasks.has(taskKey);
            })
          );
        })
      );
    });

  return (
    <div className="space-y-4">
      {/* Summary Section */}
      <div className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6">
        <div className="flex items-start justify-between mb-4">
          <div>
            <h3 className="text-lg font-semibold text-[#E5E7EB] mb-2 flex items-center gap-2">
              <Sparkles className="w-5 h-5 text-[#8B5CF6]" />
              AI Önerileri
            </h3>
            <p className="text-[#9CA3AF] text-sm">{roadmap.project_summary}</p>
          </div>
          <div className="flex items-center gap-2">
            <Checkbox
              id="select-all"
              checked={allSelected}
              onCheckedChange={handleSelectAll}
              className="border-[#30363D]"
            />
            <Label htmlFor="select-all" className="text-sm text-[#E5E7EB] cursor-pointer">
              Tümünü Seç
            </Label>
          </div>
        </div>

        {/* Tech Stack */}
        {roadmap.tech_stack && roadmap.tech_stack.length > 0 && (
          <div className="mb-4">
            <p className="text-sm text-[#9CA3AF] mb-2">Teknoloji Stack:</p>
            <div className="flex flex-wrap gap-2">
              {roadmap.tech_stack.map((tech, index) => (
                <Badge key={index} className="bg-[#8B5CF6]/20 text-[#8B5CF6] border-[#8B5CF6]/30">
                  {tech}
                </Badge>
              ))}
            </div>
          </div>
        )}

        {/* Key Insights */}
        {roadmap.key_insights && roadmap.key_insights.length > 0 && (
          <div>
            <p className="text-sm text-[#9CA3AF] mb-2">Önemli İpuçları:</p>
            <ul className="list-disc list-inside space-y-1">
              {roadmap.key_insights.map((insight, index) => (
                <li key={index} className="text-sm text-[#E5E7EB]">
                  {insight}
                </li>
              ))}
            </ul>
          </div>
        )}

        <div className="mt-4 pt-4 border-t border-[#30363D]/30">
          <p className="text-xs text-[#6B7280]">
            {roadmap.similar_projects_found} benzer proje analiz edildi
          </p>
        </div>
      </div>

      {/* Roadmap Modules */}
      <div className="space-y-3">
        {roadmap.roadmap.map((module, moduleIndex) => {
          const isModuleExpanded = expandedModules.has(module.name);
          const isModuleSelected = selectedItems.modules.has(module.name);

          return (
            <motion.div
              key={moduleIndex}
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.3, delay: moduleIndex * 0.1 }}
              className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl overflow-hidden"
            >
              {/* Module Header */}
              <div className="p-4 hover:bg-[#21262D]/50 transition-colors">
                <div className="flex items-start gap-3">
                  <Checkbox
                    id={`module-${moduleIndex}`}
                    checked={isModuleSelected}
                    onCheckedChange={(checked) => handleModuleToggle(module.name, checked as boolean)}
                    className="mt-1 border-[#30363D]"
                  />
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => toggleModule(module.name)}
                        className="h-6 w-6 p-0 text-[#9CA3AF] hover:text-[#E5E7EB]"
                      >
                        {isModuleExpanded ? (
                          <ChevronDown className="w-4 h-4" />
                        ) : (
                          <ChevronRight className="w-4 h-4" />
                        )}
                      </Button>
                      <Folder className="w-4 h-4 text-[#8B5CF6]" />
                      <Label
                        htmlFor={`module-${moduleIndex}`}
                        className="text-base font-semibold text-[#E5E7EB] cursor-pointer"
                      >
                        {module.name}
                      </Label>
                    </div>
                    <p className="text-sm text-[#9CA3AF] ml-7">{module.description}</p>
                    <div className="flex items-center gap-4 mt-2 ml-7">
                      <span className="text-xs text-[#6B7280]">
                        {module.use_cases.length} Use Case
                      </span>
                      <span className="text-xs text-[#6B7280]">
                        {module.use_cases.reduce((sum, uc) => sum + uc.tasks.length, 0)} Task
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Module Content (UseCases) */}
              <AnimatePresence>
                {isModuleExpanded && (
                  <motion.div
                    initial={{ height: 0, opacity: 0 }}
                    animate={{ height: 'auto', opacity: 1 }}
                    exit={{ height: 0, opacity: 0 }}
                    transition={{ duration: 0.2 }}
                    className="border-t border-[#30363D]/30"
                  >
                    <div className="p-4 space-y-3">
                      {module.use_cases.map((useCase, ucIndex) => {
                        const ucKey = `${module.name}|${useCase.name}`;
                        const isUseCaseExpanded = expandedUseCases.has(ucKey);
                        const isUseCaseSelected = selectedItems.useCases.has(ucKey);

                        return (
                          <div
                            key={ucIndex}
                            className="bg-[#0D1117]/50 border border-[#30363D]/30 rounded-lg p-4"
                          >
                            {/* UseCase Header */}
                            <div className="flex items-start gap-3 mb-2">
                              <Checkbox
                                id={`usecase-${moduleIndex}-${ucIndex}`}
                                checked={isUseCaseSelected}
                                onCheckedChange={(checked) =>
                                  handleUseCaseToggle(module.name, useCase.name, checked as boolean)
                                }
                                className="mt-1 border-[#30363D]"
                              />
                              <div className="flex-1">
                                <div className="flex items-center gap-2 mb-1">
                                  <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => toggleUseCase(ucKey)}
                                    className="h-6 w-6 p-0 text-[#9CA3AF] hover:text-[#E5E7EB]"
                                  >
                                    {isUseCaseExpanded ? (
                                      <ChevronDown className="w-4 h-4" />
                                    ) : (
                                      <ChevronRight className="w-4 h-4" />
                                    )}
                                  </Button>
                                  <FileText className="w-4 h-4 text-[#2DD4BF]" />
                                  <Label
                                    htmlFor={`usecase-${moduleIndex}-${ucIndex}`}
                                    className="text-sm font-medium text-[#E5E7EB] cursor-pointer"
                                  >
                                    {useCase.name}
                                  </Label>
                                </div>
                                <p className="text-xs text-[#9CA3AF] ml-7">{useCase.description}</p>
                                <span className="text-xs text-[#6B7280] ml-7">
                                  {useCase.tasks.length} Task
                                </span>
                              </div>
                            </div>

                            {/* Tasks */}
                            <AnimatePresence>
                              {isUseCaseExpanded && (
                                <motion.div
                                  initial={{ height: 0, opacity: 0 }}
                                  animate={{ height: 'auto', opacity: 1 }}
                                  exit={{ height: 0, opacity: 0 }}
                                  transition={{ duration: 0.2 }}
                                  className="mt-3 ml-7 space-y-2 border-l border-[#30363D]/30 pl-4"
                                >
                                  {useCase.tasks.map((task, taskIndex) => {
                                    const taskKey = `${module.name}|${useCase.name}|${taskIndex}`;
                                    const isTaskSelected = selectedItems.tasks.has(taskKey);

                                    return (
                                      <div
                                        key={taskIndex}
                                        className="flex items-start gap-2 p-2 rounded bg-[#161B22]/30 hover:bg-[#161B22]/50 transition-colors"
                                      >
                                        <Checkbox
                                          id={`task-${moduleIndex}-${ucIndex}-${taskIndex}`}
                                          checked={isTaskSelected}
                                          onCheckedChange={(checked) =>
                                            handleTaskToggle(
                                              module.name,
                                              useCase.name,
                                              taskIndex,
                                              checked as boolean
                                            )
                                          }
                                          className="mt-1 border-[#30363D]"
                                        />
                                        <div className="flex-1">
                                          <div className="flex items-center gap-2 mb-1">
                                            <CheckSquare className="w-3 h-3 text-[#F59E0B]" />
                                            <Label
                                              htmlFor={`task-${moduleIndex}-${ucIndex}-${taskIndex}`}
                                              className="text-xs font-medium text-[#E5E7EB] cursor-pointer"
                                            >
                                              {task.task}
                                            </Label>
                                            <Badge
                                              className={`text-xs ${
                                                task.priority === 'high'
                                                  ? 'bg-[#EF4444]/20 text-[#EF4444] border-[#EF4444]/30'
                                                  : task.priority === 'medium'
                                                  ? 'bg-[#F59E0B]/20 text-[#F59E0B] border-[#F59E0B]/30'
                                                  : 'bg-[#10B981]/20 text-[#10B981] border-[#10B981]/30'
                                              }`}
                                            >
                                              {task.priority}
                                            </Badge>
                                            <span className="text-xs text-[#6B7280]">
                                              ~{task.estimated_hours}s
                                            </span>
                                          </div>
                                          <p className="text-xs text-[#9CA3AF] ml-5">{task.description}</p>
                                        </div>
                                      </div>
                                    );
                                  })}
                                </motion.div>
                              )}
                            </AnimatePresence>
                          </div>
                        );
                      })}
                    </div>
                  </motion.div>
                )}
              </AnimatePresence>
            </motion.div>
          );
        })}
      </div>
    </div>
  );
}
