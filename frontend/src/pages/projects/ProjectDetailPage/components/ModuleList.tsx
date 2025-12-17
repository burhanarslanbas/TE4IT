/**
 * Module List Component
 * Modül listesi ve boş durum
 */
import { Module } from '../../../../types';
import { Button } from '../../../../components/ui/button';
import { Plus, Layers } from 'lucide-react';
import { motion, AnimatePresence } from 'motion/react';
import { hasPermission, PERMISSIONS } from '../../../../utils/permissions';
import { ModuleCard } from './ModuleCard';

interface ModuleListProps {
  modules: Module[];
  loading: boolean;
  projectId: string;
  onCreateModule: () => void;
  onModuleClick: (moduleId: string) => void;
}

export function ModuleList({
  modules,
  loading,
  projectId,
  onCreateModule,
  onModuleClick,
}: ModuleListProps) {
  if (loading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {[1, 2, 3, 4, 5, 6].map((i) => (
          <motion.div
            key={i}
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.3, delay: i * 0.05 }}
            className="bg-[#161B22]/60 backdrop-blur-sm border border-[#30363D] rounded-2xl p-6 h-40 animate-pulse"
          />
        ))}
      </div>
    );
  }

  if (modules.length === 0) {
    return (
      <motion.div
        initial={{ opacity: 0, scale: 0.9 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.5 }}
        className="text-center py-20"
      >
        {true ? (
          <motion.div
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            onClick={onCreateModule}
            className="bg-gradient-to-br from-[#161B22]/60 via-[#161B22]/40 to-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-3xl p-12 sm:p-16 max-w-2xl mx-auto relative overflow-hidden cursor-pointer group hover:border-[#8B5CF6]/50 transition-all duration-300"
          >
            {/* Animated Background Gradient */}
            <div className="absolute inset-0 bg-gradient-to-br from-[#8B5CF6]/5 via-transparent to-[#2DD4BF]/5 opacity-50 group-hover:opacity-70 transition-opacity" />
            <div className="absolute top-0 right-0 w-64 h-64 bg-[#8B5CF6]/10 rounded-full blur-3xl -translate-y-1/2 translate-x-1/2 group-hover:bg-[#8B5CF6]/20 transition-colors" />
            <div className="absolute bottom-0 left-0 w-64 h-64 bg-[#2DD4BF]/10 rounded-full blur-3xl translate-y-1/2 -translate-x-1/2 group-hover:bg-[#2DD4BF]/20 transition-colors" />
            
            <div className="relative z-10">
              <motion.div
                className="w-28 h-28 bg-gradient-to-br from-[#8B5CF6]/30 via-[#7C3AED]/20 to-[#2DD4BF]/30 rounded-3xl flex items-center justify-center mx-auto mb-8 shadow-2xl shadow-[#8B5CF6]/20 relative group-hover:shadow-[#8B5CF6]/40 transition-shadow"
                animate={{ 
                  scale: [1, 1.08, 1],
                  rotate: [0, 5, -5, 0]
                }}
                transition={{ 
                  duration: 3, 
                  repeat: Infinity, 
                  ease: "easeInOut" 
                }}
              >
                <div className="absolute inset-0 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-3xl blur-xl" />
                <Layers className="w-14 h-14 text-[#8B5CF6] relative z-10 group-hover:scale-110 transition-transform" />
              </motion.div>
              
              <h3 className="text-3xl sm:text-4xl font-bold text-[#E5E7EB] mb-4 bg-gradient-to-r from-[#E5E7EB] to-[#9CA3AF] bg-clip-text text-transparent group-hover:from-[#8B5CF6] group-hover:to-[#2DD4BF] transition-all">
                Henüz modül yok
              </h3>
              <p className="text-[#9CA3AF] mb-10 text-lg max-w-md mx-auto leading-relaxed group-hover:text-[#E5E7EB] transition-colors">
                Projenize ilk modülünüzü ekleyerek başlayın. Modüller, projenizi organize etmenize yardımcı olur.
              </p>
              
              <motion.div
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                onClick={(e) => {
                  e.stopPropagation(); // Kartın onClick'ini tetiklemesini engelle
                  onCreateModule();
                }}
              >
                <Button
                  size="lg"
                  className="bg-gradient-to-r from-[#8B5CF6] via-[#7C3AED] to-[#6D28D9] text-white hover:from-[#7C3AED] hover:via-[#6D28D9] hover:to-[#5B21B6] shadow-2xl shadow-[#8B5CF6]/40 hover:shadow-[#8B5CF6]/60 transition-all duration-300 h-14 px-8 rounded-xl font-semibold text-lg group/btn relative overflow-hidden"
                >
                  <span className="relative z-10 flex items-center gap-3">
                    <Plus className="w-6 h-6 group-hover/btn:rotate-90 transition-transform duration-300" />
                    <span>İlk Modülü Oluştur</span>
                  </span>
                  <motion.div
                    className="absolute inset-0 bg-gradient-to-r from-transparent via-white/20 to-transparent"
                    initial={{ x: '-100%' }}
                    whileHover={{ x: '100%' }}
                    transition={{ duration: 0.8 }}
                  />
                </Button>
              </motion.div>
              
              <p className="text-[#6B7280] text-sm mt-4 group-hover:text-[#9CA3AF] transition-colors">
                💡 Kartın herhangi bir yerine tıklayarak da modül oluşturabilirsiniz
              </p>
            </div>
          </motion.div>
        ) : (
          <div className="bg-gradient-to-br from-[#161B22]/60 via-[#161B22]/40 to-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-3xl p-12 sm:p-16 max-w-2xl mx-auto relative overflow-hidden">
            {/* Animated Background Gradient */}
            <div className="absolute inset-0 bg-gradient-to-br from-[#8B5CF6]/5 via-transparent to-[#2DD4BF]/5 opacity-50" />
            <div className="absolute top-0 right-0 w-64 h-64 bg-[#8B5CF6]/10 rounded-full blur-3xl -translate-y-1/2 translate-x-1/2" />
            <div className="absolute bottom-0 left-0 w-64 h-64 bg-[#2DD4BF]/10 rounded-full blur-3xl translate-y-1/2 -translate-x-1/2" />
            
            <div className="relative z-10">
              <motion.div
                className="w-28 h-28 bg-gradient-to-br from-[#8B5CF6]/30 via-[#7C3AED]/20 to-[#2DD4BF]/30 rounded-3xl flex items-center justify-center mx-auto mb-8 shadow-2xl shadow-[#8B5CF6]/20 relative"
                animate={{ 
                  scale: [1, 1.08, 1],
                  rotate: [0, 5, -5, 0]
                }}
                transition={{ 
                  duration: 3, 
                  repeat: Infinity, 
                  ease: "easeInOut" 
                }}
              >
                <div className="absolute inset-0 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-3xl blur-xl" />
                <Layers className="w-14 h-14 text-[#8B5CF6] relative z-10" />
              </motion.div>
              
              <h3 className="text-3xl sm:text-4xl font-bold text-[#E5E7EB] mb-4 bg-gradient-to-r from-[#E5E7EB] to-[#9CA3AF] bg-clip-text text-transparent">
                Henüz modül yok
              </h3>
              <p className="text-[#9CA3AF] mb-10 text-lg max-w-md mx-auto leading-relaxed">
                Projenize ilk modülünüzü ekleyerek başlayın. Modüller, projenizi organize etmenize yardımcı olur.
              </p>
            </div>
          </div>
        )}
      </motion.div>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <AnimatePresence>
        {modules.map((module, index) => (
          <ModuleCard
            key={module.id}
            module={module}
            onClick={() => onModuleClick(module.id)}
            index={index}
          />
        ))}
      </AnimatePresence>
    </div>
  );
}

