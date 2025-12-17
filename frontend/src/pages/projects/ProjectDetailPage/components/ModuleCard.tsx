/**
 * Module Card Component
 * Tek bir modül kartı
 */
import { Module } from '../../../../types';
import { Badge } from '../../../../components/ui/badge';
import { Archive, Eye, Layers } from 'lucide-react';
import { motion } from 'motion/react';

interface ModuleCardProps {
  module: Module;
  onClick: () => void;
  index: number;
}

export function ModuleCard({ module, onClick, index }: ModuleCardProps) {
  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.95, y: 20 }}
      animate={{ opacity: 1, scale: 1, y: 0 }}
      exit={{ opacity: 0, scale: 0.95 }}
      transition={{ duration: 0.3, delay: index * 0.03 }}
      whileHover={{ scale: 1.01, y: -4 }}
      onClick={onClick}
      className="group relative bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6 cursor-pointer overflow-hidden transition-all duration-300 hover:border-[#8B5CF6]/40 hover:shadow-[0_8px_30px_rgba(139,92,246,0.12)] hover:bg-[#161B22]/70"
    >
      {/* Top Gradient Line */}
      <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-[#8B5CF6]/50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500" />

      {/* Hover Gradient Overlay */}
      <div className="absolute inset-0 bg-gradient-to-br from-[#8B5CF6]/[0.03] via-transparent to-[#2DD4BF]/[0.03] opacity-0 group-hover:opacity-100 transition-opacity duration-500" />

      <div className="relative z-10 space-y-4">
        {/* Header Row */}
        <div className="flex items-start justify-between gap-3">
          <div className="flex-1 min-w-0">
            <h3 className="text-lg font-semibold text-[#E5E7EB] group-hover:text-white transition-colors truncate mb-1.5">
              {module.title}
            </h3>
            {module.description && (
              <p className="text-[#9CA3AF] text-sm line-clamp-2 leading-relaxed">
                {module.description}
              </p>
            )}
          </div>

          {/* Status Badge */}
          <Badge
            className={`${
              module.status === 'Active'
                ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
            } border px-2.5 py-1 flex-shrink-0`}
          >
            {module.status === 'Active' ? (
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
        </div>

        {/* Footer Info */}
        <div className="flex items-center justify-between pt-2 border-t border-[#30363D]/30">
          <div className="flex items-center gap-2 text-xs text-[#6B7280]">
            <Layers className="w-3.5 h-3.5" />
            <span>{module.useCaseCount || 0} Use Case</span>
          </div>

          {/* View Action */}
          <div className="flex items-center gap-1.5 text-[#8B5CF6] opacity-0 group-hover:opacity-100 transition-opacity">
            <span className="text-xs font-medium">Görüntüle</span>
            <Eye className="w-3.5 h-3.5" />
          </div>
        </div>
      </div>

      {/* Subtle Corner Accent */}
      <div className="absolute top-0 right-0 w-24 h-24 bg-gradient-to-br from-[#8B5CF6]/5 to-transparent rounded-bl-full opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
    </motion.div>
  );
}

