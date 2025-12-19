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
import { ForgotPasswordPage } from "./components/forgot-password-page";
import { ProjectsListPage } from "./pages/ProjectsListPage";
import { ProjectDetailPage } from "./pages/projects/ProjectDetailPage/ProjectDetailPage";
import { ModuleDetailPage } from "./pages/ModuleDetailPage";
import { UseCaseDetailPage } from "./pages/UseCaseDetailPage";
import { CreateProjectPage } from "./pages/CreateProjectPage";
import { CreateModulePage } from "./pages/CreateModulePage";
import { CreateUseCasePage } from "./pages/CreateUseCasePage";
import { CreateTaskPage } from "./pages/CreateTaskPage";
import { EditTaskPage } from "./pages/EditTaskPage";
import { TaskDetailPage } from "./pages/TaskDetailPage";
import { AuthService } from "./services/auth";
import { apiClient } from "./services/api";
import { Toaster } from "./components/ui/sonner";
import { toast } from "sonner";
import { ErrorBoundary } from "./components/ErrorBoundary";

/**
 * Protected Route Bileşeni
 * Kullanıcının authenticated olması gereken sayfalar için
 */
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const checkAuthStatus = () => {
      try {
        // Token kontrolü yap - API'ye istek atmadan
        if (AuthService.isAuthenticated()) {
          setIsAuthenticated(true);
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
          // Token varsa authenticated olarak işaretle
          // Backend endpoint'leri henüz hazır değilse token kontrolü yeterli
          setIsAuthenticated(true);
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
      // Token'ı temizle
      apiClient.clearToken();
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
  const handleLogin = async () => {
    try {
      toast.success("Giriş başarılı!", {
        description: "Profil sayfasına yönlendiriliyorsunuz...",
        duration: 2000,
      });
      // Redirect with a small delay to show the toast
      setTimeout(() => {
        window.location.href = "/profile";
      }, 1000);
    } catch (error) {
      console.error('Login error:', error);
    }
  };

  return (
    <LoginPage
      onNavigateToRegister={() => {
        window.location.href = "/register";
      }}
      onNavigateToHome={() => {
        window.location.href = "/";
      }}
      onNavigateToForgotPassword={() => {
        window.location.href = "/forgot-password";
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
      // Token'ı temizle
      apiClient.clearToken();
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
  // API client'a unauthorized callback ekle (401 durumunda logout)
  useEffect(() => {
    apiClient.setUnauthorizedCallback(() => {
      AuthService.logout();
      toast.error('Oturum süreniz dolmuş', {
        description: 'Lütfen tekrar giriş yapın.',
        duration: 3000,
      });
      window.location.href = '/login';
    });
  }, []);

  return (
    <ErrorBoundary>
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
          
          {/* Forgot Password Sayfası */}
          <Route 
            path="/forgot-password" 
            element={
              <div className="min-h-screen bg-[#0D1117]">
                <ForgotPasswordPage
                  onNavigateToLogin={() => {
                    window.location.href = "/login";
                  }}
                  onNavigateToHome={() => {
                    window.location.href = "/";
                  }}
                />
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

          {/* Projects List - Protected Route */}
          <Route 
            path="/projects" 
            element={
              <ProtectedRoute>
                <ProjectsListPage />
              </ProtectedRoute>
            } 
          />

          {/* Create Project - Protected Route */}
          <Route 
            path="/projects/new" 
            element={
              <ProtectedRoute>
                <CreateProjectPage />
              </ProtectedRoute>
            } 
          />

          {/* Project Detail - Protected Route */}
          <Route 
            path="/projects/:projectId" 
            element={
              <ProtectedRoute>
                <ProjectDetailPage />
              </ProtectedRoute>
            } 
          />

          {/* Create Module - Protected Route */}
          <Route 
            path="/projects/:projectId/modules/new" 
            element={
              <ProtectedRoute>
                <CreateModulePage />
              </ProtectedRoute>
            } 
          />

          {/* Module Detail - Protected Route */}
          <Route 
            path="/projects/:projectId/modules/:moduleId" 
            element={
              <ProtectedRoute>
                <ModuleDetailPage />
              </ProtectedRoute>
            } 
          />

          {/* Create UseCase - Protected Route */}
          <Route 
            path="/projects/:projectId/modules/:moduleId/usecases/new" 
            element={
              <ProtectedRoute>
                <CreateUseCasePage />
              </ProtectedRoute>
            } 
          />

          {/* Create Task - Protected Route */}
          <Route 
            path="/projects/:projectId/modules/:moduleId/usecases/:useCaseId/tasks/new" 
            element={
              <ProtectedRoute>
                <CreateTaskPage />
              </ProtectedRoute>
            } 
          />

          {/* UseCase Detail - Protected Route */}
          <Route 
            path="/projects/:projectId/modules/:moduleId/usecases/:useCaseId" 
            element={
              <ProtectedRoute>
                <UseCaseDetailPage />
              </ProtectedRoute>
            } 
          />

          {/* Task Detail - Protected Route */}
          <Route 
            path="/projects/:projectId/modules/:moduleId/usecases/:useCaseId/tasks/:taskId" 
            element={
              <ProtectedRoute>
                <TaskDetailPage />
              </ProtectedRoute>
            } 
          />

          {/* Edit Task - Protected Route */}
          <Route 
            path="/projects/:projectId/modules/:moduleId/usecases/:useCaseId/tasks/:taskId/edit" 
            element={
              <ProtectedRoute>
                <EditTaskPage />
              </ProtectedRoute>
            } 
          />
          
          {/* 404 - Not Found */}
          <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </Layout>
      </Router>
    </ErrorBoundary>
  );
}