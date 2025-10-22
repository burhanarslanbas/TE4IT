import { motion } from "motion/react";
import { 
  GraduationCap, 
  BarChart3, 
  Puzzle, 
  Shield, 
  Zap, 
  Target 
} from "lucide-react";

const features = [
  {
    icon: GraduationCap,
    title: "İnteraktif Eğitim Yolları",
    description: "Yeni çalışanlarınızı otomatik pilotta eğitin ve gelişimlerini takip edin."
  },
  {
    icon: BarChart3,
    title: "Akıllı Raporlama",
    description: "Proje ve ekip performansını anlık verilerle takip edin ve analiz edin."
  },
  {
    icon: Puzzle,
    title: "Entegrasyon Merkezi",
    description: "Slack, GitHub, Google Drive gibi araçlarla sorunsuz çalışın."
  },
  {
    icon: Shield,
    title: "Rol ve Yetki Yönetimi",
    description: "Ekip üyelerinize özel erişim izinleri tanımlayın ve güvenliği sağlayın."
  },
  {
    icon: Zap,
    title: "Otomasyon Kuralları",
    description: "Tekrarlayan görevleri otomatikleştirerek zaman kazanın ve verimlilik artırın."
  },
  {
    icon: Target,
    title: "Hedef ve Başarım Takibi",
    description: "Ekip hedefleri belirleyin ve ilerlemeyi görselleştirerek motivasyonu artırın."
  }
];

export function DetailedFeaturesSection() {
  return (
    <section id="detailed-features" className="py-24 bg-gradient-to-b from-[#0D1117] to-[#161B22]">
      <div className="container mx-auto px-6">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6 }}
          viewport={{ once: true }}
          className="text-center mb-16"
        >
          <h2 className="text-4xl md:text-5xl font-bold mb-6 bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] bg-clip-text text-transparent">
            Sadece Bir Kanban Panosundan Çok Daha Fazlası
          </h2>
          <p className="text-xl text-[#9CA3AF] max-w-3xl mx-auto">
            TE4IT ile proje yönetimi ve ekip gelişimini tek platformda deneyimleyin
          </p>
        </motion.div>

        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
          {features.map((feature, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: index * 0.1 }}
              viewport={{ once: true }}
              className="group relative"
            >
              <div className="bg-[#161B22] border border-[#30363D]/50 rounded-2xl p-8 h-full hover:border-[#8B5CF6]/50 transition-all duration-300 hover:shadow-lg hover:shadow-[#8B5CF6]/10">
                <div className="mb-6">
                  <div className="w-14 h-14 bg-gradient-to-br from-[#8B5CF6] to-[#2DD4BF] rounded-xl flex items-center justify-center mb-4">
                    <feature.icon className="w-7 h-7 text-white" />
                  </div>
                  <h3 className="text-xl font-semibold text-[#E5E7EB] mb-3">
                    {feature.title}
                  </h3>
                  <p className="text-[#9CA3AF] leading-relaxed">
                    {feature.description}
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