import { motion } from "motion/react";
import { Logo } from "./logo";
import { Linkedin, Twitter, Mail, MapPin } from "lucide-react";

const footerLinks = {
  product: [
    { name: "Özellikler", href: "#features" },
    { name: "Fiyatlandırma", href: "#pricing" },
    { name: "Entegrasyonlar", href: "#integrations" },
    { name: "Güvenlik", href: "#security" }
  ],
  company: [
    { name: "Hakkımızda", href: "#about" },
    { name: "Blog", href: "#blog" },
    { name: "Kariyer", href: "#careers" },
    { name: "İletişim", href: "#contact" }
  ],
  legal: [
    { name: "Gizlilik Politikası", href: "#privacy" },
    { name: "Kullanım Şartları", href: "#terms" },
    { name: "Çerez Politikası", href: "#cookies" }
  ]
};

const socialLinks = [
  { name: "LinkedIn", href: "#", icon: Linkedin },
  { name: "Twitter", href: "#", icon: Twitter },
  { name: "Email", href: "mailto:hello@te4it.com", icon: Mail }
];

export function Footer() {
  return (
    <footer className="bg-[#0A0E13] border-t border-[#30363D]/30">
      <div className="container mx-auto px-6 py-16">
        <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-12">
          {/* Company Info */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6 }}
            viewport={{ once: true }}
            className="lg:col-span-1"
          >
            <Logo />
            <p className="text-[#9CA3AF] mt-4 leading-relaxed">
              Her ekibin potansiyelini en üst seviyeye çıkarmak için tasarlanan, 
              yeni nesil proje yönetimi ve onboarding platformu.
            </p>
            <div className="flex items-center space-x-2 mt-4 text-[#9CA3AF]">
              <MapPin className="w-4 h-4" />
              <span className="text-sm">İstanbul, Türkiye</span>
            </div>
          </motion.div>

          {/* Product Links */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6, delay: 0.1 }}
            viewport={{ once: true }}
          >
            <h4 className="font-semibold text-[#E5E7EB] mb-6">Ürün</h4>
            <ul className="space-y-4">
              {footerLinks.product.map((link, index) => (
                <li key={index}>
                  <a 
                    href={link.href}
                    className="text-[#9CA3AF] hover:text-[#8B5CF6] transition-colors"
                  >
                    {link.name}
                  </a>
                </li>
              ))}
            </ul>
          </motion.div>

          {/* Company Links */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6, delay: 0.2 }}
            viewport={{ once: true }}
          >
            <h4 className="font-semibold text-[#E5E7EB] mb-6">Şirket</h4>
            <ul className="space-y-4">
              {footerLinks.company.map((link, index) => (
                <li key={index}>
                  <a 
                    href={link.href}
                    className="text-[#9CA3AF] hover:text-[#8B5CF6] transition-colors"
                  >
                    {link.name}
                  </a>
                </li>
              ))}
            </ul>
          </motion.div>

          {/* Social & Legal */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6, delay: 0.3 }}
            viewport={{ once: true }}
          >
            <h4 className="font-semibold text-[#E5E7EB] mb-6">Sosyal Medya</h4>
            <div className="flex space-x-4 mb-8">
              {socialLinks.map((social, index) => (
                <a
                  key={index}
                  href={social.href}
                  className="w-10 h-10 bg-[#161B22] border border-[#30363D]/50 rounded-lg flex items-center justify-center text-[#9CA3AF] hover:text-[#8B5CF6] hover:border-[#8B5CF6]/50 transition-all"
                >
                  <social.icon className="w-5 h-5" />
                </a>
              ))}
            </div>
            
            <div className="space-y-3">
              <h5 className="font-medium text-[#E5E7EB] text-sm">Yasal</h5>
              {footerLinks.legal.map((link, index) => (
                <div key={index}>
                  <a 
                    href={link.href}
                    className="text-sm text-[#9CA3AF] hover:text-[#8B5CF6] transition-colors block"
                  >
                    {link.name}
                  </a>
                </div>
              ))}
            </div>
          </motion.div>
        </div>

        {/* Bottom Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6, delay: 0.4 }}
          viewport={{ once: true }}
          className="border-t border-[#30363D]/30 pt-8 mt-12"
        >
          <div className="flex flex-col md:flex-row justify-between items-center space-y-4 md:space-y-0">
            <p className="text-[#9CA3AF] text-sm">
              © 2025 TE4IT. Tüm hakları saklıdır.
            </p>
            <div className="flex items-center space-x-6 text-sm text-[#9CA3AF]">
              <span>Türkiye'de ❤️ ile geliştirildi</span>
              <div className="w-2 h-2 bg-[#2DD4BF] rounded-full animate-pulse"></div>
              <span>v1.0.0 Beta</span>
            </div>
          </div>
        </motion.div>
      </div>
    </footer>
  );
}