import { motion } from "motion/react";
import { useInView } from "motion/react";
import { useRef } from "react";
import { Button } from "./ui/button";

interface FinalCtaSectionProps {
  onNavigateToRegister: () => void;
}

export function FinalCtaSection({ onNavigateToRegister }: FinalCtaSectionProps) {
  const ref = useRef(null);
  const isInView = useInView(ref, { once: true, margin: "-100px" });

  return (
    <section className="relative py-20 px-6 overflow-hidden">
      {/* Animated gradient background */}
      <div className="absolute inset-0">
        <motion.div
          className="absolute inset-0"
          style={{
            background: "linear-gradient(135deg, #8B5CF6, #EC4899, #F97316, #2DD4BF, #8B5CF6)"
          }}
          animate={{
            backgroundPosition: ["0% 0%", "100% 100%", "0% 0%"]
          }}
          transition={{
            duration: 10,
            repeat: Infinity,
            ease: "linear"
          }}
        />
        <div className="absolute inset-0 bg-[#0D1117]/80" />
      </div>
      
      <div className="container mx-auto text-center relative z-10">
        <div ref={ref} className="max-w-4xl mx-auto">
          <motion.h2
            initial={{ scale: 0.8, opacity: 0, y: 50 }}
            animate={isInView ? { scale: 1, opacity: 1, y: 0 } : { scale: 0.8, opacity: 0, y: 50 }}
            transition={{ duration: 1, type: "spring", stiffness: 100 }}
            className="text-[#E5E7EB] mb-8"
          >
            Potansiyelinizi{" "}
            <span className="bg-gradient-to-r from-[#8B5CF6] to-[#EC4899] bg-clip-text text-transparent">
              Ertelemeyin.
            </span>
          </motion.h2>
          
          <motion.p
            initial={{ scale: 0.9, opacity: 0, y: 30 }}
            animate={isInView ? { scale: 1, opacity: 1, y: 0 } : { scale: 0.9, opacity: 0, y: 30 }}
            transition={{ duration: 0.8, delay: 0.2 }}
            className="text-[#9CA3AF] mb-12 text-xl leading-relaxed max-w-2xl mx-auto"
          >
            Bugün başlayın ve ekibinizin ne kadar verimli olabileceğini keşfedin. 
            Modern proje yönetiminin gücüyle tanışma zamanı.
          </motion.p>
          
          <motion.div
            initial={{ scale: 0.8, opacity: 0, y: 30 }}
            animate={isInView ? { scale: 1, opacity: 1, y: 0 } : { scale: 0.8, opacity: 0, y: 30 }}
            transition={{ duration: 0.8, delay: 0.4 }}
            className="flex flex-col sm:flex-row gap-4 justify-center items-center"
          >
            <motion.div
              whileHover={{ scale: 1.05, y: -2 }}
              whileTap={{ scale: 0.95 }}
            >
              <Button 
                size="lg" 
                className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90 px-12 py-6 text-xl font-semibold shadow-2xl shadow-[#8B5CF6]/50 hover:shadow-[#8B5CF6]/70 transition-all duration-300"
                onClick={onNavigateToRegister}
              >
                TE4IT'yi Ücretsiz Deneyin
              </Button>
            </motion.div>
            
            <Button 
              variant="outline"
              size="lg"
              className="border-[#30363D] text-[#E5E7EB] hover:bg-[#8B5CF6]/10 hover:border-[#8B5CF6] px-8 py-6 text-lg"
            >
              Demo Rezervasyonu
            </Button>
          </motion.div>
          
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 20 }}
            transition={{ duration: 0.6, delay: 0.6 }}
            className="mt-8 flex flex-wrap justify-center gap-6 text-[#9CA3AF] text-sm"
          >
            <div className="flex items-center space-x-2">
              <svg className="w-4 h-4 text-[#2DD4BF]" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
              </svg>
              <span>Kredi kartı gerektirmez</span>
            </div>
            <div className="flex items-center space-x-2">
              <svg className="w-4 h-4 text-[#2DD4BF]" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
              </svg>
              <span>14 gün ücretsiz deneme</span>
            </div>
            <div className="flex items-center space-x-2">
              <svg className="w-4 h-4 text-[#2DD4BF]" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
              </svg>
              <span>İstediğin zaman iptal et</span>
            </div>
          </motion.div>
        </div>
      </div>
    </section>
  );
}