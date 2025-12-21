/**
 * Task Relations List Component
 * İlişkili task'ların listesi
 */
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Plus, Eye, Trash2, Loader2 } from 'lucide-react';
import type { TaskRelation } from '../../types';
import { taskRelationConfig } from '../../utils/taskHelpers';
import { motion, AnimatePresence } from 'motion/react';
import { useState } from 'react';

interface TaskRelationsListProps {
  relations: TaskRelation[];
  loading: boolean;
  onDelete: (relationId: string) => Promise<void>;
  onAddRelation: () => void;
  onViewTask: (targetTaskId: string) => void;
}

export function TaskRelationsList({ 
  relations, 
  loading, 
  onDelete, 
  onAddRelation,
  onViewTask 
}: TaskRelationsListProps) {
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const handleDelete = async (relationId: string) => {
    try {
      setDeletingId(relationId);
      await onDelete(relationId);
    } finally {
      setDeletingId(null);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-8">
        <Loader2 className="w-6 h-6 text-[#8B5CF6] animate-spin" />
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Add Relation Button */}
      <Button
        onClick={onAddRelation}
        variant="outline"
        size="sm"
        className="w-full border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:text-[#8B5CF6] hover:border-[#8B5CF6]/30"
      >
        <Plus className="w-4 h-4 mr-2" />
        İlişki Ekle
      </Button>

      {/* Relations List */}
      {relations.length === 0 ? (
        <div className="text-center py-8">
          <p className="text-[#6B7280] text-sm">İlişkili task bulunmuyor.</p>
          <p className="text-[#6B7280] text-xs mt-1">Yeni bir ilişki ekleyerek başlayın.</p>
        </div>
      ) : (
        <AnimatePresence mode="popLayout">
          {relations.map((relation, index) => {
            const config = taskRelationConfig[relation.relationType];
            const isDeleting = deletingId === relation.id;
            
            return (
              <motion.div
                key={relation.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, x: -20 }}
                transition={{ duration: 0.3, delay: index * 0.05 }}
                className={`bg-[#161B22]/40 backdrop-blur-sm border ${config.color} rounded-lg p-4 hover:bg-[#161B22]/60 transition-all group`}
              >
                <div className="flex items-start justify-between gap-3">
                  <div className="flex-1 min-w-0">
                  {/* Relation Type Badge */}
                    <Badge className={`${config.color} border mb-2`}>
                      <span className="flex items-center gap-1.5">
                        {config.icon}
                        <span className="text-xs font-medium">{config.label}</span>
                      </span>
                    </Badge>

                    {/* Target Task Info */}
                    <p className="text-[#E5E7EB] font-medium mb-1 line-clamp-1">
                      {relation.targetTask?.title || `Task ${relation.targetTaskId.substring(0, 8)}...`}
                    </p>
                    
                    {/* Description */}
                    <p className="text-[#6B7280] text-xs">
                      {config.description}
                    </p>
                  </div>

                  {/* Action Buttons */}
                  <div className="flex items-center gap-1 flex-shrink-0 opacity-0 group-hover:opacity-100 transition-opacity">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onViewTask(relation.targetTaskId)}
                      className="h-8 w-8 p-0 text-[#8B5CF6] hover:text-[#7C3AED] hover:bg-[#8B5CF6]/10"
                      title="Görüntüle"
                    >
                      <Eye className="w-4 h-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleDelete(relation.id)}
                      disabled={isDeleting}
                      className="h-8 w-8 p-0 text-[#EF4444] hover:text-[#DC2626] hover:bg-[#EF4444]/10"
                      title="Sil"
                    >
                      {isDeleting ? (
                        <Loader2 className="w-4 h-4 animate-spin" />
                      ) : (
                        <Trash2 className="w-4 h-4" />
                      )}
                    </Button>
                  </div>
                </div>
              </motion.div>
            );
          })}
        </AnimatePresence>
      )}
    </div>
  );
}
