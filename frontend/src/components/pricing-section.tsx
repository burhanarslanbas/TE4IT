import { motion } from "motion/react";
import { Button } from "./ui/button";
import { Badge } from "./ui/badge";
import { Check, Star } from "lucide-react";

const plans = [
  {
    name: "Başlangıç",
    price: "Ücretsiz",
    period: "",
    description: "Küçük ekipler için temel özellikler",
    features: [
      "5 kullanıcıya kadar",
      "3 proje limiti",
      "Temel Kanban panosu",
      "Basit görev yönetimi",
      "Email desteği"
    ],
    cta: "Ücretsiz Başla",
    popular: false
  },
  {
    name: "Pro",
    price: "₺299",
    period: "/ay",
    description: "Büyüyen ekipler için gelişmiş özellikler",
    features: [
      "50 kullanıcıya kadar",
      "Sınırsız proje",
      "İnteraktif eğitim yolları",
      "Akıllı raporlama",
      "Entegrasyon merkezi",
      "Otomasyon kuralları",
      "Öncelikli destek",
      "Özel onboarding"
    ],
    cta: "Pro'yu Dene",
    popular: true
  },
  {
    name: "Kurumsal",
    price: "Özel Fiyat",
    period: "",
    description: "Büyük organizasyonlar için özel çözümler",
    features: [
      "Sınırsız kullanıcı",
      "Sınırsız proje",
      "Tüm Pro özellikleri",
      "Özel entegrasyonlar",
      "Gelişmiş güvenlik",
      "SSO desteği",
      "Özel eğitim",
      "7/24 destek"
    ],
    cta: "İletişime Geç",
    popular: false
  }
];

interface PricingSectionProps {
  onNavigateToRegister: () => void;
}

export function PricingSection({ onNavigateToRegister }: PricingSectionProps) {
  return (
    <section id="pricing" className="py-24 bg-gradient-to-b from-[#161B22] to-[#0D1117]">
      <div className="container mx-auto px-6">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6 }}
          viewport={{ once: true }}
          className="text-center mb-16"
        >
          <h2 className="text-4xl md:text-5xl font-bold mb-6 bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] bg-clip-text text-transparent">
            Şeffaf ve Anlaşılır Fiyatlandırma
          </h2>
          <p className="text-xl text-[#9CA3AF] max-w-3xl mx-auto mb-8">
            İhtiyacınıza uygun planı seçin ve hemen başlayın
          </p>
          <div className="inline-flex items-center bg-[#161B22] border border-[#30363D]/50 rounded-xl px-6 py-3">
            <div className="w-3 h-3 bg-[#2DD4BF] rounded-full mr-3 animate-pulse"></div>
            <span className="text-[#E5E7EB] font-medium">
              Tüm planlarda 14 gün ücretsiz deneme. Kredi kartı gerekmez.
            </span>
          </div>
        </motion.div>

        <div className="grid md:grid-cols-3 gap-8 max-w-6xl mx-auto">
          {plans.map((plan, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: index * 0.1 }}
              viewport={{ once: true }}
              className={`relative bg-[#161B22] border rounded-2xl p-8 ${
                plan.popular 
                  ? 'border-[#8B5CF6] shadow-xl shadow-[#8B5CF6]/20 scale-105' 
                  : 'border-[#30363D]/50 hover:border-[#8B5CF6]/30'
              } transition-all duration-300`}
            >
              {plan.popular && (
                <div className="absolute -top-4 left-1/2 transform -translate-x-1/2">
                  <Badge className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] text-white px-4 py-1 flex items-center space-x-1">
                    <Star className="w-3 h-3" />
                    <span>En Popüler</span>
                  </Badge>
                </div>
              )}

              <div className="text-center mb-8">
                <h3 className="text-2xl font-bold text-[#E5E7EB] mb-2">
                  {plan.name}
                </h3>
                <p className="text-[#9CA3AF] mb-4">
                  {plan.description}
                </p>
                <div className="flex items-baseline justify-center mb-6">
                  <span className="text-4xl font-bold text-[#E5E7EB]">
                    {plan.price}
                  </span>
                  <span className="text-[#9CA3AF] ml-1">
                    {plan.period}
                  </span>
                </div>
              </div>

              <div className="space-y-4 mb-8">
                {plan.features.map((feature, featureIndex) => (
                  <div key={featureIndex} className="flex items-start space-x-3">
                    <div className="w-5 h-5 bg-gradient-to-br from-[#8B5CF6] to-[#2DD4BF] rounded-full flex items-center justify-center flex-shrink-0 mt-0.5">
                      <Check className="w-3 h-3 text-white" />
                    </div>
                    <span className="text-[#E5E7EB]">{feature}</span>
                  </div>
                ))}
              </div>

              <Button 
                className={`w-full py-6 ${
                  plan.popular
                    ? 'bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] text-white hover:opacity-90 shadow-lg shadow-[#8B5CF6]/25'
                    : 'bg-[#30363D] text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]'
                }`}
                onClick={onNavigateToRegister}
              >
                {plan.cta}
              </Button>
            </motion.div>
          ))}
        </div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6, delay: 0.4 }}
          viewport={{ once: true }}
          className="text-center mt-16"
        >
          <p className="text-[#9CA3AF] mb-4">
            Daha fazla bilgi mi istiyorsunuz? Detaylı karşılaştırma tablosu için
          </p>
          <Button variant="ghost" className="text-[#8B5CF6] hover:bg-[#8B5CF6]/10">
            Tüm Özellikleri Karşılaştır →
          </Button>
        </motion.div>
      </div>
    </section>
  );
}