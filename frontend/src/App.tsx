import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Routes, Route, Navigate, useLocation } from "react-router-dom";
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

/**
 * Protected Route Bileşeni
 * Kullanıcının authenticated olması gereken sayfalar için
 */
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const checkAuthStatus = async () => {
      try {
        if (AuthService.isAuthenticated()) {
          try {
            await AuthService.getCurrentUser();
            setIsAuthenticated(true);
          } catch (error) {
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

  if (!isAuthenticated) {
    toast.error("Giriş yapmalısınız", {
      description: "Bu sayfaya erişmek için lütfen giriş yapın.",
      duration: 3000,
    });
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

/**
 * Ana Layout Bileşeni
 * Her sayfada tekrar eden bileşenler için
 */
const Layout = ({ children }: { children: React.ReactNode }) => {
  const location = useLocation();
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const checkAuthStatus = async () => {
      try {
        if (AuthService.isAuthenticated()) {
          try {
            await AuthService.getCurrentUser();
            setIsAuthenticated(true);
          } catch (error) {
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
  }, [location]);

  const handleLogout = async () => {
    try {
      await AuthService.logout();
      setIsAuthenticated(false);
      toast.info("Çıkış yapıldı", {
        description: "Güvenli bir şekilde çıkış yaptınız.",
        duration: 2000,
      });
      window.location.href = "/";
    } catch (error) {
      console.error('Logout error:', error);
      setIsAuthenticated(false);
      window.location.href = "/";
    }
  };

  // Login, Register ve Profile sayfalarında Navigation gösterme
  const showNavigation = !["/login", "/register", "/profile"].includes(location.pathname);

  return (
    <>
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
      
      {showNavigation && (
        <Navigation 
          isAuthenticated={isAuthenticated}
          onLogout={handleLogout}
        />
      )}
      
      {children}
      
      {showNavigation && <Footer />}
    </>
  );
};

/**
 * Ana Sayfa Bileşeni
 */
const HomePage = () => {
  const handleNavigateToRegister = () => {
    window.location.href = "/register";
  };

  return (
    <div className="min-h-screen bg-[#0D1117]">
      <HeroSection onNavigateToRegister={handleNavigateToRegister} />
      <FeaturesSection />
      <DetailedFeaturesSection />
      <TrustBuildingSection />
      <HowItWorksSection />
      <PricingSection onNavigateToRegister={handleNavigateToRegister} />
      <FinalCtaSection onNavigateToRegister={handleNavigateToRegister} />
    </div>
  );
};

/**
 * Login Page Handler
 */
const LoginPageWrapper = () => {
  const handleLogin = () => {
    toast.success("Giriş başarılı!", {
      description: "Profil sayfasına yönlendiriliyorsunuz...",
      duration: 2000,
    });
    // Redirect with a small delay to show the toast
    setTimeout(() => {
      window.location.href = "/profile";
    }, 1000);
  };

  return (
    <LoginPage
      onNavigateToRegister={() => {
        window.location.href = "/register";
      }}
      onNavigateToHome={() => {
        window.location.href = "/";
      }}
      onLogin={handleLogin}
    />
  );
};

/**
 * Register Page Handler
 */
const RegisterPageWrapper = () => {
  return (
    <RegisterPage
      onNavigateToLogin={() => {
        window.location.href = "/login";
      }}
      onNavigateToHome={() => {
        window.location.href = "/";
      }}
    />
  );
};

/**
 * Profile Page Handler
 */
const ProfilePageWrapper = () => {
  const handleLogout = async () => {
    try {
      await AuthService.logout();
      toast.info("Çıkış yapıldı", {
        description: "Güvenli bir şekilde çıkış yaptınız.",
        duration: 2000,
      });
      window.location.href = "/";
    } catch (error) {
      console.error('Logout error:', error);
      window.location.href = "/";
    }
  };

  return (
    <ProfilePage
      onNavigateToHome={() => {
        window.location.href = "/";
      }}
      onLogout={handleLogout}
    />
  );
};

/**
 * Ana App Bileşeni
 */
export default function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          {/* Ana Sayfa */}
          <Route path="/" element={<HomePage />} />
          
          {/* Login Sayfası */}
          <Route 
            path="/login" 
            element={
              <div className="min-h-screen bg-[#0D1117]">
                <LoginPageWrapper />
              </div>
            } 
          />
          
          {/* Register Sayfası */}
          <Route 
            path="/register" 
            element={
              <div className="min-h-screen bg-[#0D1117]">
                <RegisterPageWrapper />
              </div>
            } 
          />
          
          {/* Profile Sayfası - Protected Route */}
          <Route 
            path="/profile" 
            element={
              <ProtectedRoute>
                <div className="min-h-screen bg-[#0D1117]">
                  <ProfilePageWrapper />
                </div>
              </ProtectedRoute>
            } 
          />
          
          {/* 404 - Not Found */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Layout>
    </Router>
  );
}