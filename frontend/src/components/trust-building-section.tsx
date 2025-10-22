import { motion } from "motion/react";
import { Button } from "./ui/button";
import { Rocket, Sparkles, MessageCircle } from "lucide-react";


export function TrustBuildingSection() {
  return (
    <section id="why-us" className="py-24 bg-[#0D1117]">
      <div className="container mx-auto px-6">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6 }}
          viewport={{ once: true }}
          className="text-center mb-16"
        >
          <h2 className="text-4xl md:text-5xl font-bold mb-6 bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] bg-clip-text text-transparent">
            Bu Yolculuğa Birlikte Çıkalım: İlk Kullanıcımız Olun!
          </h2>
        </motion.div>

        <div className="grid lg:grid-cols-2 gap-16 items-center">
          {/* Left side - Founder story */}
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            whileInView={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.6 }}
            viewport={{ once: true }}
            className="space-y-6"
          >
            <div className="relative">
              <div className="w-32 h-32 mx-auto lg:mx-0 mb-6 rounded-2xl overflow-hidden bg-gradient-to-br from-[#8B5CF6] to-[#2DD4BF] p-1">
                <div className="w-full h-full bg-[#161B22] rounded-xl flex items-center justify-center">
                  <div className="w-20 h-20 bg-gradient-to-br from-[#8B5CF6] to-[#2DD4BF] rounded-full flex items-center justify-center">
                    <span className="text-2xl font-bold text-white">TE</span>
                  </div>
                </div>
              </div>
            </div>
            
            <div className="text-center lg:text-left">
              <p className="text-lg text-[#E5E7EB] leading-relaxed mb-6">
                "TE4IT'ı, yıllarca yaşadığımız dağınık onboarding süreçlerine ve verimsiz proje takibine bir son vermek için geliştirdik. 
                Misyonumuz, her ekibin potansiyelini en üst seviyeye çıkarmasını sağlamak."
              </p>
              <p className="text-[#9CA3AF]">
                - TE4IT Kurucu Ekibi
              </p>
            </div>
          </motion.div>

          {/* Right side - Beta program benefits */}
          <motion.div
            initial={{ opacity: 0, x: 20 }}
            whileInView={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.6, delay: 0.2 }}
            viewport={{ once: true }}
            className="bg-[#161B22] border border-[#30363D]/50 rounded-2xl p-8"
          >
            <h3 className="text-2xl font-bold text-[#E5E7EB] mb-8 text-center">
              Beta Programının Avantajları
            </h3>

            <div className="space-y-6">
              <div className="flex items-start space-x-4">
                <div className="w-10 h-10 bg-gradient-to-br from-[#8B5CF6] to-[#2DD4BF] rounded-lg flex items-center justify-center flex-shrink-0">
                  <Rocket className="w-5 h-5 text-white" />
                </div>
                <div>
                  <h4 className="font-semibold text-[#E5E7EB] mb-2">
                    İlk 100 kullanıcıya özel ömür boyu %50 indirim
                  </h4>
                  <p className="text-[#9CA3AF] text-sm">
                    Erken katılımcılar için özel fiyatlandırma avantajı
                  </p>
                </div>
              </div>

              <div className="flex items-start space-x-4">
                <div className="w-10 h-10 bg-gradient-to-br from-[#2DD4BF] to-[#EC4899] rounded-lg flex items-center justify-center flex-shrink-0">
                  <Sparkles className="w-5 h-5 text-white" />
                </div>
                <div>
                  <h4 className="font-semibold text-[#E5E7EB] mb-2">
                    Ürünün gelişimine doğrudan yön verme imkanı
                  </h4>
                  <p className="text-[#9CA3AF] text-sm">
                    Geri bildirimleriniz ile ürünü şekillendirin
                  </p>
                </div>
              </div>

              <div className="flex items-start space-x-4">
                <div className="w-10 h-10 bg-gradient-to-br from-[#EC4899] to-[#F97316] rounded-lg flex items-center justify-center flex-shrink-0">
                  <MessageCircle className="w-5 h-5 text-white" />
                </div>
                <div>
                  <h4 className="font-semibold text-[#E5E7EB] mb-2">
                    Kurucu ekiple birebir iletişim ve öncelikli destek
                  </h4>
                  <p className="text-[#9CA3AF] text-sm">
                    Doğrudan erişim ve hızlı çözümler
                  </p>
                </div>
              </div>
            </div>

            <div className="mt-8 pt-6 border-t border-[#30363D]/50">
              <Button className="w-full bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] text-white hover:opacity-90 shadow-lg shadow-[#8B5CF6]/25 py-6">
                Beta Programına Katıl
              </Button>
            </div>
          </motion.div>
        </div>
      </div>
    </section>
  );
}