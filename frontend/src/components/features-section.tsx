import { motion } from "motion/react";
import { useInView } from "motion/react";
import { useRef } from "react";
import { AnimatedKanban } from "./animated-kanban";
import { AnimatedRoadmap } from "./animated-roadmap";

function FeatureSection({ 
  title, 
  subtitle,
  children 
}: {
  title: string;
  subtitle: string;
  children: React.ReactNode;
}) {
  const ref = useRef(null);
  const isInView = useInView(ref, { once: true, margin: "-100px" });

  return (
    <div ref={ref} className="py-24">
      <div className="text-center mb-16">
        <motion.h2
          initial={{ y: 50, opacity: 0 }}
          animate={isInView ? { y: 0, opacity: 1 } : { y: 50, opacity: 0 }}
          transition={{ duration: 0.8 }}
          className="text-[#E5E7EB] mb-6"
        >
          {title}
        </motion.h2>
        <motion.p
          initial={{ y: 30, opacity: 0 }}
          animate={isInView ? { y: 0, opacity: 1 } : { y: 30, opacity: 0 }}
          transition={{ duration: 0.6, delay: 0.2 }}
          className="text-[#9CA3AF] text-xl max-w-3xl mx-auto leading-relaxed"
        >
          {subtitle}
        </motion.p>
      </div>
      
      <motion.div
        initial={{ y: 80, opacity: 0 }}
        animate={isInView ? { y: 0, opacity: 1 } : { y: 80, opacity: 0 }}
        transition={{ duration: 1, delay: 0.4 }}
        className="flex justify-center"
      >
        {children}
      </motion.div>
    </div>
  );
}

export function FeaturesSection() {
  return (
    <section id="features" className="py-20 px-6 bg-[#0D1117]">
      <div className="container mx-auto">
        {/* Feature 1: Smart Task Management */}
        <FeatureSection
          title="Sürükle-Bırak'tan Daha Fazlası."
          subtitle="Görevlerinizi sezgisel bir şekilde yönetin. Kanban panoları ile projelerinizi görselleştirin ve tamamlanan her görev için küçük zaferler kutlayın. Çünkü motivasyon, en güçlü araçtır."
        >
          <div className="relative">
            <div className="absolute inset-0 bg-gradient-to-r from-[#8B5CF6]/10 to-[#2DD4BF]/10 blur-3xl rounded-3xl scale-110"></div>
            <AnimatedKanban />
          </div>
        </FeatureSection>

        {/* Feature 2: Interactive Learning Paths */}
        <FeatureSection
          title="0'dan 100'e Rekor Sürede."
          subtitle="İnteraktif eğitim yol haritaları ile yeni çalışanlarınızın adaptasyon sürecini hızlandırın. Her adımda rehberlik, her tamamlamada motivasyon. Kişiselleştirilmiş öğrenme deneyimi ile herkes kendi hızında başarıya ulaşır."
        >
          <div className="relative">
            <div className="absolute inset-0 bg-gradient-to-r from-[#EC4899]/10 to-[#F97316]/10 blur-3xl rounded-3xl scale-110"></div>
            <AnimatedRoadmap />
          </div>
        </FeatureSection>
      </div>
    </section>
  );
}