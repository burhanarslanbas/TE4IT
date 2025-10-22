import { useState } from "react";
import { motion } from "motion/react";
import { Logo } from "./logo";
import { Button } from "./ui/button";
import { ProfileInfo } from "./profile-sections/profile-info";
import { SecuritySettings } from "./profile-sections/security-settings";
import { AppSettings } from "./profile-sections/app-settings";
import { User, Shield, Settings, LogOut, ArrowLeft, Menu, X } from "lucide-react";
import { Toaster } from "./ui/sonner";
import { toast } from "sonner@2.0.3";

type SectionType = "profile" | "security" | "settings";

interface ProfilePageProps {
  onNavigateToHome: () => void;
  onLogout: () => void;
}

export function ProfilePage({ onNavigateToHome, onLogout }: ProfilePageProps) {
  const [activeSection, setActiveSection] = useState<SectionType>("profile");
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const handleLogout = () => {
    toast.info("Çıkış yapılıyor...", { duration: 1000 });
    setTimeout(() => {
      onLogout();
    }, 1000);
  };

  const menuItems = [
    { id: "profile" as SectionType, label: "Profilim", icon: User },
    { id: "security" as SectionType, label: "Güvenlik", icon: Shield },
    { id: "settings" as SectionType, label: "Ayarlar", icon: Settings },
  ];

  const renderContent = () => {
    switch (activeSection) {
      case "profile":
        return <ProfileInfo />;
      case "security":
        return <SecuritySettings />;
      case "settings":
        return <AppSettings />;
    }
  };

  return (
    <div className="min-h-screen bg-[#0D1117] relative">
      {/* Toast notifications */}
      <Toaster 
        position="top-right"
        toastOptions={{
          classNames: {
            toast: "bg-[#161B22] border border-[#30363D] text-[#E5E7EB]",
            title: "text-[#E5E7EB]",
            description: "text-[#9CA3AF]",
            success: "border-[#10B981] bg-[#10B981]/5",
            error: "border-[#EF4444] bg-[#EF4444]/5",
            info: "border-[#3B82F6] bg-[#3B82F6]/5",
          },
        }}
      />

      {/* Top Navigation Bar */}
      <motion.nav
        initial={{ y: -20, opacity: 0 }}
        animate={{ y: 0, opacity: 1 }}
        transition={{ duration: 0.5 }}
        className="sticky top-0 z-50 backdrop-blur-md bg-[#0D1117]/80 border-b border-[#30363D]/50"
      >
        <div className="container mx-auto px-6 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <button
                onClick={() => setSidebarOpen(!sidebarOpen)}
                className="lg:hidden text-[#E5E7EB] hover:text-[#8B5CF6] transition-colors"
              >
                {sidebarOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
              </button>
              <Logo />
            </div>
            
            <div className="flex items-center gap-3">
              <Button
                variant="ghost"
                size="sm"
                onClick={onNavigateToHome}
                className="text-[#9CA3AF] hover:text-[#E5E7EB] hover:bg-[#8B5CF6]/10"
              >
                <ArrowLeft className="w-4 h-4 mr-2" />
                Ana Sayfa
              </Button>
              <Button
                variant="ghost"
                size="sm"
                onClick={handleLogout}
                className="text-[#EF4444] hover:text-[#DC2626] hover:bg-[#EF4444]/10"
              >
                <LogOut className="w-4 h-4 mr-2" />
                Çıkış Yap
              </Button>
            </div>
          </div>
        </div>
      </motion.nav>

      <div className="container mx-auto px-6 py-8">
        <div className="flex gap-8">
          {/* Sidebar Navigation */}
          <motion.aside
            initial={{ x: -20, opacity: 0 }}
            animate={{ x: 0, opacity: 1 }}
            transition={{ duration: 0.5, delay: 0.1 }}
            className={`
              fixed lg:sticky top-24 left-0 h-[calc(100vh-8rem)] z-40
              lg:block w-64 flex-shrink-0
              ${sidebarOpen ? "block" : "hidden"}
            `}
          >
            <div className="bg-[#161B22] rounded-2xl border border-[#30363D]/50 p-4 h-full overflow-y-auto">
              <nav className="space-y-2">
                {menuItems.map((item, index) => (
                  <motion.button
                    key={item.id}
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ duration: 0.3, delay: 0.1 + index * 0.05 }}
                    onClick={() => {
                      setActiveSection(item.id);
                      setSidebarOpen(false);
                    }}
                    className={`
                      w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all
                      ${activeSection === item.id
                        ? "bg-gradient-to-r from-[#8B5CF6]/20 to-[#2DD4BF]/20 text-[#8B5CF6] shadow-lg shadow-[#8B5CF6]/20 border border-[#8B5CF6]/30"
                        : "text-[#9CA3AF] hover:bg-[#21262D] hover:text-[#E5E7EB]"
                      }
                    `}
                  >
                    <div className={`
                      w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0
                      ${activeSection === item.id
                        ? "bg-[#8B5CF6]/20"
                        : "bg-[#21262D]"
                      }
                    `}>
                      <item.icon className="w-4 h-4" />
                    </div>
                    <span className="font-medium">{item.label}</span>
                  </motion.button>
                ))}
              </nav>

              {/* Sidebar Footer */}
              <div className="mt-8 p-4 bg-[#0D1117] rounded-xl border border-[#30363D]/30">
                <div className="flex items-center gap-2 mb-2">
                  <div className="w-2 h-2 bg-[#10B981] rounded-full animate-pulse" />
                  <span className="text-xs text-[#9CA3AF]">Hesap Durumu</span>
                </div>
                <p className="text-sm text-[#E5E7EB]">Premium Üye</p>
                <p className="text-xs text-[#6B7280] mt-1">Son giriş: Bugün, 14:32</p>
              </div>
            </div>
          </motion.aside>

          {/* Overlay for mobile sidebar */}
          {sidebarOpen && (
            <div
              className="fixed inset-0 bg-black/50 z-30 lg:hidden"
              onClick={() => setSidebarOpen(false)}
            />
          )}

          {/* Main Content Area */}
          <motion.main
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: 0.2 }}
            className="flex-1 min-w-0"
          >
            {/* Page Header */}
            <div className="mb-8">
              <h1 className="text-4xl text-[#E5E7EB] mb-2">
                {menuItems.find(item => item.id === activeSection)?.label}
              </h1>
              <p className="text-[#9CA3AF]">
                {activeSection === "profile" && "Profil bilgilerinizi görüntüleyin ve düzenleyin"}
                {activeSection === "security" && "Hesap güvenliğinizi yönetin"}
                {activeSection === "settings" && "Uygulama tercihlerinizi özelleştirin"}
              </p>
            </div>

            {/* Dynamic Content */}
            <motion.div
              key={activeSection}
              initial={{ opacity: 0, x: 20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.4 }}
            >
              {renderContent()}
            </motion.div>
          </motion.main>
        </div>
      </div>

      {/* Decorative Background Elements */}
      <div className="fixed inset-0 pointer-events-none overflow-hidden -z-10">
        <motion.div
          className="absolute -top-40 -right-40 w-96 h-96 bg-[#8B5CF6] opacity-10 blur-[128px] rounded-full"
          animate={{
            scale: [1, 1.2, 1],
            x: [0, 30, 0],
            y: [0, -20, 0],
          }}
          transition={{
            duration: 10,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        />
        <motion.div
          className="absolute -bottom-40 -left-40 w-96 h-96 bg-[#2DD4BF] opacity-10 blur-[128px] rounded-full"
          animate={{
            scale: [1, 1.3, 1],
            x: [0, -30, 0],
            y: [0, 20, 0],
          }}
          transition={{
            duration: 12,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        />
      </div>
    </div>
  );
}
