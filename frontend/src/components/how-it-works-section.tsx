import { motion } from "motion/react";
import { useInView } from "motion/react";
import { useRef } from "react";
import { Icon3D } from "./3d-icon";

const steps = [
  {
    icon: "rocket" as const,
    color: "#8B5CF6",
    title: "1. Projeni Oluştur",
    description: "Dakikalar içinde yeni bir proje başlat ve vizyonunu hayata geçirmeye başla."
  },
  {
    icon: "users" as const,
    color: "#2DD4BF",
    title: "2. Ekibini Davet Et",
    description: "Çalışanlarını platforma davet et, rolleri belirle ve işbirliğini başlat."
  },
  {
    icon: "target" as const,
    color: "#EC4899",
    title: "3. Eğitim Yolunu Çiz",
    description: "Kişiselleştirilmiş öğrenme yolları oluştur ve herkesin gelişimini takip et."
  }
];

export function HowItWorksSection() {
  const ref = useRef(null);
  const isInView = useInView(ref, { once: true, margin: "-100px" });

  return (
    <section id="how-it-works" className="py-20 px-6 bg-[#161B22]">
      <div className="container mx-auto">
        <div ref={ref} className="text-center mb-16">
          <motion.h2
            initial={{ y: 50, opacity: 0 }}
            animate={isInView ? { y: 0, opacity: 1 } : { y: 50, opacity: 0 }}
            transition={{ duration: 0.8 }}
            className="text-[#E5E7EB] mb-6"
          >
            Nasıl Çalışır?
          </motion.h2>
          <motion.p
            initial={{ y: 30, opacity: 0 }}
            animate={isInView ? { y: 0, opacity: 1 } : { y: 30, opacity: 0 }}
            transition={{ duration: 0.6, delay: 0.2 }}
            className="text-[#9CA3AF] text-xl max-w-3xl mx-auto leading-relaxed"
          >
            TE4IT ile proje yönetimine başlamak çok kolay. Sadece 3 adımda ekibinizin potansiyelini ortaya çıkarın.
          </motion.p>
        </div>

        <div className="grid md:grid-cols-3 gap-8 max-w-5xl mx-auto">
          {steps.map((step, index) => (
            <motion.div
              key={index}
              initial={{ 
                y: 60, 
                opacity: 0, 
                rotateX: 45,
                rotateY: -15
              }}
              animate={isInView ? { 
                y: 0, 
                opacity: 1, 
                rotateX: 0,
                rotateY: 0
              } : { 
                y: 60, 
                opacity: 0, 
                rotateX: 45,
                rotateY: -15
              }}
              transition={{ 
                duration: 0.8, 
                delay: index * 0.2,
                type: "spring",
                stiffness: 100
              }}
              whileHover={{ 
                y: -10,
                scale: 1.05,
                boxShadow: `0 20px 40px ${step.color}20`
              }}
              className="relative group"
            >
              <div className="relative p-8 bg-[#21262D] border border-[#30363D] rounded-2xl overflow-hidden">
                {/* Glow effect */}
                <div 
                  className="absolute inset-0 opacity-0 group-hover:opacity-100 transition-opacity duration-500"
                  style={{
                    background: `linear-gradient(135deg, ${step.color}10, transparent)`
                  }}
                />
                
                {/* Animated border glow */}
                <motion.div
                  className="absolute inset-0 rounded-2xl"
                  style={{
                    background: `linear-gradient(135deg, ${step.color}40, transparent)`,
                    padding: "1px"
                  }}
                  initial={{ opacity: 0 }}
                  whileHover={{ opacity: 1 }}
                  transition={{ duration: 0.3 }}
                >
                  <div className="w-full h-full bg-[#21262D] rounded-2xl" />
                </motion.div>
                
                <div className="relative z-10 text-center space-y-6">
                  <div className="flex justify-center">
                    <Icon3D type={step.icon} color={step.color} size="lg" />
                  </div>
                  
                  <h3 className="text-[#E5E7EB] font-semibold">{step.title}</h3>
                  
                  <p className="text-[#9CA3AF] leading-relaxed">
                    {step.description}
                  </p>
                </div>
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
}