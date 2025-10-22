import { motion } from "motion/react";
import { Button } from "./ui/button";
import { AnimatedKanban } from "./animated-kanban";
import { GradientBlob } from "./gradient-blob";

interface HeroSectionProps {
  onNavigateToRegister: () => void;
}

export function HeroSection({ onNavigateToRegister }: HeroSectionProps) {
  return (
    <section className="relative pt-24 pb-16 px-6 overflow-hidden">
      {/* Background Gradient Blobs */}
      <GradientBlob 
        colors={["#8B5CF6", "#EC4899"]} 
        size="lg" 
        position="top-right" 
      />
      <GradientBlob 
        colors={["#2DD4BF", "#F97316"]} 
        size="md" 
        position="bottom-left" 
      />
      
      <div className="container mx-auto relative z-10">
        <div className="grid lg:grid-cols-2 gap-12 items-center">
          {/* Left side - Text content */}
          <div className="space-y-8">
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.6, delay: 0.6 }}
            >
              <motion.h1
                initial={{ y: 50, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ duration: 0.8, delay: 0.7 }}
                className="text-[#E5E7EB] leading-tight"
              >
                Proje Yönetiminin{" "}
                <span className="text-[#8B5CF6] relative">
                  Geleceğiyle
                  <motion.div
                    className="absolute -bottom-2 left-0 right-0 h-1 bg-gradient-to-r from-[#8B5CF6] to-[#EC4899] rounded-full"
                    initial={{ scaleX: 0 }}
                    animate={{ scaleX: 1 }}
                    transition={{ duration: 1, delay: 1.5 }}
                  />
                </span>{" "}
                Tanışın.
              </motion.h1>
            </motion.div>
            
            <motion.p
              initial={{ y: 30, opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              transition={{ duration: 0.6, delay: 1.0 }}
              className="text-[#9CA3AF] max-w-lg text-xl leading-relaxed"
            >
              TE4IT, ekibinizin potansiyelini ortaya çıkarır. Karmaşık onboarding süreçlerini basitleştirin, görevleri akıllıca yönetin ve başarıyı birlikte inşa edin.
            </motion.p>
            
            <motion.div
              initial={{ y: 30, opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              transition={{ duration: 0.6, delay: 1.2 }}
              className="flex flex-col sm:flex-row gap-4"
            >
              <Button 
                size="lg" 
                className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90 px-8 py-6 text-lg shadow-lg shadow-[#8B5CF6]/25 hover:shadow-[#8B5CF6]/40 transition-all hover:scale-105"
                onClick={onNavigateToRegister}
              >
                Projeni Yönetmeye Başla
              </Button>
              <Button 
                variant="outline" 
                size="lg"
                className="border-[#30363D] text-[#E5E7EB] hover:bg-[#8B5CF6]/10 hover:border-[#8B5CF6] px-8 py-6 text-lg"
              >
                Demo İzle
              </Button>
            </motion.div>
          </div>
          
          {/* Right side - Animated product interface */}
          <motion.div
            initial={{ x: 100, opacity: 0, scale: 0.8 }}
            animate={{ x: 0, opacity: 1, scale: 1 }}
            transition={{ duration: 1, delay: 1.4 }}
            className="relative"
          >
            <motion.div
              animate={{ 
                y: [0, -20, 0],
                rotateY: [0, 5, 0]
              }}
              transition={{ 
                duration: 6, 
                repeat: Infinity, 
                ease: "easeInOut" 
              }}
              className="relative"
            >
              <div className="absolute inset-0 bg-gradient-to-r from-[#8B5CF6]/20 to-[#EC4899]/20 blur-3xl rounded-3xl transform rotate-6 scale-110"></div>
              <AnimatedKanban />
            </motion.div>
          </motion.div>
        </div>
      </div>
    </section>
  );
}