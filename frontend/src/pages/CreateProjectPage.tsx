/**
 * Create Project Page
 * Route: /projects/new
 * Modern Glassmorphism Design with Tab Structure
 */
import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import { Button } from '../components/ui/button';
import { ManuelCreateProjectTab } from '../components/projects/ManuelCreateProjectTab';
import { AICreateProjectTab } from '../components/projects/AICreateProjectTab';
import { ArrowLeft, Plus, Sparkles, FileText } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';

export function CreateProjectPage() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<'manual' | 'ai'>('manual');

  const handleProjectCreated = (projectId: string) => {
      // Kısa bir gecikme sonrası proje detay sayfasına yönlendir
      setTimeout(() => {
      navigate(`/projects/${projectId}`);
      }, 500);
  };

  const handleCancel = () => {
    navigate('/projects');
  };

  return (
    <div className="min-h-screen bg-[#0D1117] text-[#E5E7EB] relative overflow-hidden">
      {/* Animated Background Blobs */}
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

      {/* Main Content Container */}
      <div className="relative z-10 pt-24 px-6 pb-12">
        <div className="max-w-3xl mx-auto">
          {/* Breadcrumb */}
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3 }}
            className="mb-6"
          >
            <Breadcrumb>
              <BreadcrumbList>
                <BreadcrumbItem>
                  <BreadcrumbLink asChild>
                    <Link to="/projects" className="hover:text-[#8B5CF6] transition-colors">Projeler</Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                  <BreadcrumbPage>Yeni Proje</BreadcrumbPage>
                </BreadcrumbItem>
              </BreadcrumbList>
            </Breadcrumb>
          </motion.div>

          {/* Back Button */}
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.3, delay: 0.1 }}
          >
            <Button
              variant="ghost"
              onClick={handleCancel}
              className="mb-6 text-[#E5E7EB] hover:text-[#8B5CF6] hover:bg-[#21262D]/50"
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
              Projelere Dön
            </Button>
          </motion.div>

          {/* Create Project Form Card */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4, delay: 0.2 }}
            className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8 shadow-lg"
          >
            {/* Header */}
            <div className="mb-8">
              <div className="flex items-center gap-3 mb-2">
                <div className="w-12 h-12 bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] rounded-xl flex items-center justify-center shadow-lg shadow-[#8B5CF6]/30">
                  <Plus className="w-6 h-6 text-white" />
                </div>
                <h1 className="text-3xl font-bold text-[#E5E7EB]">
                  Yeni Proje Oluştur
                </h1>
              </div>
              <p className="text-[#9CA3AF] text-sm ml-16">
                Manuel olarak oluşturun veya AI ile otomatik roadmap oluşturun
              </p>
            </div>

            {/* Creation Mode Selection Buttons */}
            <div className="flex flex-col sm:flex-row gap-4 mb-8">
              <Button
                variant={activeTab === 'manual' ? 'default' : 'outline'}
                onClick={() => setActiveTab('manual')}
                className={`flex-1 h-auto py-4 px-6 ${
                  activeTab === 'manual'
                    ? 'bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white border-[#8B5CF6] shadow-lg shadow-[#8B5CF6]/30'
                    : 'bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:border-[#8B5CF6]/50'
                } transition-all`}
              >
                <FileText className="w-5 h-5 mr-2" />
                <div className="text-left">
                  <div className="font-semibold">Manuel Oluştur</div>
                  <div className="text-xs opacity-80 mt-1">Kendiniz oluşturun</div>
                </div>
              </Button>
              
              <Button
                variant={activeTab === 'ai' ? 'default' : 'outline'}
                onClick={() => setActiveTab('ai')}
                className={`flex-1 h-auto py-4 px-6 ${
                  activeTab === 'ai'
                    ? 'bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white border-[#8B5CF6] shadow-lg shadow-[#8B5CF6]/30'
                    : 'bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:border-[#8B5CF6]/50'
                } transition-all`}
              >
                <Sparkles className="w-5 h-5 mr-2" />
                <div className="text-left">
                  <div className="font-semibold">AI ile Oluştur</div>
                  <div className="text-xs opacity-80 mt-1">Otomatik roadmap</div>
                </div>
              </Button>
            </div>

            {/* Content Based on Selected Mode */}
            {activeTab === 'manual' ? (
              <ManuelCreateProjectTab
                onProjectCreated={handleProjectCreated}
                onCancel={handleCancel}
              />
            ) : (
              <AICreateProjectTab
                onProjectCreated={handleProjectCreated}
                onCancel={handleCancel}
              />
            )}
          </motion.div>

          {/* Help Text */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 0.5, delay: 0.4 }}
            className="mt-6 p-4 bg-[#161B22]/30 backdrop-blur-sm border border-[#30363D]/30 rounded-lg"
          >
            <p className="text-[#9CA3AF] text-sm text-center">
              💡 <strong className="text-[#E5E7EB]">İpucu:</strong>{' '}
              {activeTab === 'manual'
                ? 'Manuel olarak proje oluşturduktan sonra modüller, use case\'ler ve task\'lar ekleyebilirsiniz.'
                : 'AI ile oluşturma, GitHub\'daki benzer projeleri analiz ederek otomatik roadmap oluşturur.'}
            </p>
          </motion.div>
        </div>
      </div>
    </div>
  );
}

