import { motion } from "motion/react";
import { useState } from "react";
import { Play, FileText, Video, Award } from "lucide-react";

const roadmapSteps = [
  {
    id: 1,
    title: "Onboarding Temelleri",
    type: "video",
    duration: "15 dk",
    completed: true,
    icon: Play
  },
  {
    id: 2,
    title: "Proje Yönetimi 101",
    type: "document",
    duration: "20 dk",
    completed: true,
    icon: FileText
  },
  {
    id: 3,
    title: "Ekip İş Birliği",
    type: "video",
    duration: "25 dk",
    completed: false,
    icon: Video
  },
  {
    id: 4,
    title: "Sertifika Değerlendirmesi",
    type: "assessment",
    duration: "30 dk",
    completed: false,
    icon: Award
  }
];

export function AnimatedRoadmap() {
  const [activeStep, setActiveStep] = useState(2);
  const [showContent, setShowContent] = useState(false);

  const handleStepClick = (stepId: number) => {
    setActiveStep(stepId);
    setShowContent(true);
    setTimeout(() => setShowContent(false), 3000);
  };

  return (
    <div className="w-full max-w-4xl mx-auto p-6 bg-[#161B22] rounded-2xl border border-[#30363D] shadow-2xl">
      <div className="space-y-6">
        <div className="text-center">
          <h4 className="text-[#E5E7EB] font-semibold text-lg mb-2">Stajyer Eğitim Yolu</h4>
          <div className="flex items-center justify-center space-x-2">
            <div className="w-32 bg-[#30363D] rounded-full h-2 overflow-hidden">
              <motion.div 
                className="h-full bg-gradient-to-r from-[#8B5CF6] to-[#EC4899]"
                initial={{ width: "0%" }}
                animate={{ width: "50%" }}
                transition={{ duration: 2, repeat: Infinity, repeatType: "reverse" }}
              />
            </div>
            <span className="text-[#9CA3AF] text-sm">50% Tamamlandı</span>
          </div>
        </div>

        <div className="space-y-4">
          {roadmapSteps.map((step, index) => {
            const Icon = step.icon;
            const isActive = activeStep === step.id;
            
            return (
              <motion.div
                key={step.id}
                layout
                className={`relative flex items-center space-x-4 p-4 rounded-xl cursor-pointer transition-all ${
                  isActive 
                    ? 'bg-[#8B5CF6]/10 border border-[#8B5CF6]/30' 
                    : 'bg-[#21262D] border border-[#30363D] hover:border-[#8B5CF6]/50'
                }`}
                onClick={() => handleStepClick(step.id)}
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
              >
                <div className={`flex-shrink-0 w-10 h-10 rounded-full flex items-center justify-center ${
                  step.completed 
                    ? 'bg-[#2DD4BF]' 
                    : isActive 
                    ? 'bg-[#8B5CF6]' 
                    : 'bg-[#30363D]'
                }`}>
                  {step.completed ? (
                    <svg className="w-5 h-5 text-[#0D1117]" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                    </svg>
                  ) : (
                    <Icon className="w-5 h-5 text-[#E5E7EB]" />
                  )}
                </div>

                <div className="flex-1">
                  <h5 className="text-[#E5E7EB] font-medium">{step.title}</h5>
                  <div className="flex items-center space-x-3 mt-1">
                    <span className="text-[#9CA3AF] text-sm">{step.duration}</span>
                    <span className={`text-xs px-2 py-1 rounded-full ${
                      step.type === 'video' ? 'bg-[#EC4899]/20 text-[#EC4899]' :
                      step.type === 'document' ? 'bg-[#F97316]/20 text-[#F97316]' :
                      'bg-[#2DD4BF]/20 text-[#2DD4BF]'
                    }`}>
                      {step.type === 'video' ? 'Video' : 
                       step.type === 'document' ? 'Doküman' : 'Değerlendirme'}
                    </span>
                  </div>
                </div>

                {index < roadmapSteps.length - 1 && (
                  <div className="absolute left-9 top-14 w-px h-6 bg-[#30363D]" />
                )}
              </motion.div>
            );
          })}
        </div>

        {showContent && (
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            className="bg-[#21262D] border border-[#8B5CF6]/30 rounded-lg p-6"
          >
            <div className="flex items-center space-x-3 mb-4">
              <div className="w-12 h-8 bg-[#8B5CF6] rounded flex items-center justify-center">
                <Play className="w-4 h-4 text-white" />
              </div>
              <div>
                <h6 className="text-[#E5E7EB] font-medium">Video Oynatılıyor</h6>
                <p className="text-[#9CA3AF] text-sm">
                  {roadmapSteps.find(s => s.id === activeStep)?.title}
                </p>
              </div>
            </div>
            <div className="w-full bg-[#30363D] rounded-full h-1">
              <motion.div 
                className="h-full bg-[#8B5CF6] rounded-full"
                initial={{ width: "0%" }}
                animate={{ width: "75%" }}
                transition={{ duration: 2 }}
              />
            </div>
          </motion.div>
        )}
      </div>
    </div>
  );
}