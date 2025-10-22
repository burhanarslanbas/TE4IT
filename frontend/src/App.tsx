import React, { useState, useEffect } from "react";
import { Navigation } from "./components/navigation";
import { HeroSection } from "./components/hero-section";
import { FeaturesSection } from "./components/features-section";
import { DetailedFeaturesSection } from "./components/detailed-features-section";
import { TrustBuildingSection } from "./components/trust-building-section";
import { HowItWorksSection } from "./components/how-it-works-section";
import { PricingSection } from "./components/pricing-section";
import { FinalCtaSection } from "./components/final-cta-section";
import { Footer } from "./components/footer";
import { LoginPage } from "./components/login-page";
import { RegisterPage } from "./components/register-page";
import { ProfilePage } from "./components/profile-page";
import { AuthService } from "./services/auth";
import { Toaster } from "./components/ui/sonner";
import { toast } from "sonner";

type PageType = "home" | "login" | "register" | "profile";

export default function App() {
  const [currentPage, setCurrentPage] = useState<PageType>("home");
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  /**
   * Uygulama başlangıcında authentication durumunu kontrol et
   */
  useEffect(() => {
    const checkAuthStatus = async () => {
      try {
        // Token'ın varlığını kontrol et
        if (AuthService.isAuthenticated()) {
          // Token varsa kullanıcı bilgilerini doğrula
          try {
            await AuthService.getCurrentUser();
            setIsAuthenticated(true);
          } catch (error) {
            // Token geçersizse temizle
            await AuthService.logout();
            setIsAuthenticated(false);
          }
        } else {
          setIsAuthenticated(false);
        }
      } catch (error) {
        console.error('Auth check error:', error);
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };

    checkAuthStatus();
  }, []);

  /**
   * Login handler - Başarılı giriş sonrası
   */
  const handleLogin = () => {
    setIsAuthenticated(true);
    setCurrentPage("profile");
    toast.success("Giriş başarılı!", {
      description: "Profil sayfasına yönlendiriliyorsunuz...",
      duration: 2000,
    });
  };

  /**
   * Logout handler - Çıkış işlemi
   */
  const handleLogout = async () => {
    try {
      await AuthService.logout();
      setIsAuthenticated(false);
      setCurrentPage("home");
      toast.info("Çıkış yapıldı", {
        description: "Güvenli bir şekilde çıkış yaptınız.",
        duration: 2000,
      });
    } catch (error) {
      console.error('Logout error:', error);
      // Hata olsa bile state'i temizle
      setIsAuthenticated(false);
      setCurrentPage("home");
    }
  };

  // Loading state - İlk yükleme sırasında
  if (isLoading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-2 border-[#8B5CF6] border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-[#9CA3AF]">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (currentPage === "login") {
    return (
      <LoginPage
        onNavigateToRegister={() => setCurrentPage("register")}
        onNavigateToHome={() => setCurrentPage("home")}
        onLogin={handleLogin}
      />
    );
  }

  if (currentPage === "register") {
    return (
      <RegisterPage
        onNavigateToLogin={() => setCurrentPage("login")}
        onNavigateToHome={() => setCurrentPage("home")}
      />
    );
  }

  if (currentPage === "profile" && isAuthenticated) {
    return (
      <ProfilePage
        onNavigateToHome={() => setCurrentPage("home")}
        onLogout={handleLogout}
      />
    );
  }

  return (
    <div className="min-h-screen bg-[#0D1117]">
      {/* Toast notifications - Global toast container */}
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

      <Navigation 
        onNavigateToLogin={() => setCurrentPage("login")}
        onNavigateToRegister={() => setCurrentPage("register")}
        onNavigateToProfile={() => setCurrentPage("profile")}
        isAuthenticated={isAuthenticated}
        onLogout={handleLogout}
      />
      <HeroSection onNavigateToRegister={() => setCurrentPage("register")} />
      <FeaturesSection />
      <DetailedFeaturesSection />
      <TrustBuildingSection />
      <HowItWorksSection />
      <PricingSection onNavigateToRegister={() => setCurrentPage("register")} />
      <FinalCtaSection onNavigateToRegister={() => setCurrentPage("register")} />
      <Footer />
    </div>
  );
}