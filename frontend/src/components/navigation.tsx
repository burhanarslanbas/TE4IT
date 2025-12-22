import { motion } from "motion/react";
import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Button } from "./ui/button";
import { Logo } from "./logo";
import { Home, BookOpen, FolderKanban, User, LogOut } from "lucide-react";
import { TokenHelper } from "../services/auth";
import type { User } from "../services/auth";

interface NavigationProps {
  isAuthenticated?: boolean;
  onLogout?: () => void;
}

export function Navigation({ isAuthenticated = false, onLogout }: NavigationProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const isHomePage = location.pathname === '/';
  const [currentUser, setCurrentUser] = useState<User | null>(null);

  useEffect(() => {
    if (isAuthenticated) {
      const user = TokenHelper.getCurrentUser();
      setCurrentUser(user);
    }
  }, [isAuthenticated]);

  return (
    <motion.nav
      initial={{ y: -20, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.5 }}
      className="fixed top-0 left-0 right-0 z-50 backdrop-blur-md bg-[#0D1117]/80 border-b border-[#30363D]/50"
    >
      <div className="container mx-auto px-6 py-4">
        <div className="flex items-center justify-between">
          {/* Sol taraf: Logo */}
          <div className="flex items-center">
            <Logo />
          </div>
          
          {/* Orta: Navigation Links */}
          {isAuthenticated ? (
            // Authenticated kullanıcılar için: Ana Sayfa, Eğitimler ve Projeler (kutucuk içinde)
            <div className="hidden md:flex items-center space-x-4">
              <Button
                variant="ghost"
                className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                onClick={() => navigate("/")}
              >
                <Home className="w-4 h-4 mr-2" />
                Ana Sayfa
              </Button>
              <Button
                variant="ghost"
                className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                onClick={() => navigate("/education")}
              >
                <BookOpen className="w-4 h-4 mr-2" />
                Eğitimler
              </Button>
              <Button
                variant="ghost"
                className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                onClick={() => navigate("/projects")}
              >
                <FolderKanban className="w-4 h-4 mr-2" />
                Projeler
              </Button>
            </div>
          ) : (
            // Guest kullanıcılar için: Sadece ana sayfada Özellikler, Fiyatlandırma, Neden Biz?
            isHomePage && (
              <div className="hidden md:flex items-center space-x-8">
                <a href="#features" className="text-[#E5E7EB] hover:text-[#8B5CF6] transition-colors">
                  Özellikler
                </a>
                <a href="#pricing" className="text-[#E5E7EB] hover:text-[#8B5CF6] transition-colors">
                  Fiyatlandırma
                </a>
                <a href="#why-us" className="text-[#E5E7EB] hover:text-[#8B5CF6] transition-colors">
                  Neden Biz?
                </a>
              </div>
            )
          )}
          
          {/* Sağ taraf: Hoşgeldiniz mesajı, Profil ve Çıkış Yap */}
          <div className="flex items-center space-x-4">
            {isAuthenticated ? (
              // Authenticated user: Hoşgeldiniz mesajı, Profil ve Çıkış Yap
              <>
                {currentUser && (
                  <span className="hidden lg:block text-[#E5E7EB] text-sm">
                    Hoşgeldiniz! <span className="text-[#8B5CF6] font-semibold">{currentUser.userName}</span>
                  </span>
                )}
                <Button
                  variant="ghost"
                  className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                  onClick={() => navigate("/profile")}
                >
                  <User className="w-4 h-4 mr-2" />
                  Profil
                </Button>
                <Button
                  variant="ghost"
                  className="text-[#EF4444] hover:bg-[#EF4444]/10 border border-[#EF4444]/30"
                  onClick={onLogout}
                >
                  <LogOut className="w-4 h-4 mr-2" />
                  Çıkış Yap
                </Button>
              </>
            ) : (
              // Guest user: Giriş Yap ve Ücretsiz Başla
              <>
                <Button
                  variant="ghost"
                  className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                  onClick={() => navigate("/login")}
                >
                  Giriş Yap
                </Button>
                <Button
                  className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90 shadow-lg shadow-[#8B5CF6]/25"
                  onClick={() => navigate("/register")}
                >
                  Ücretsiz Başla
                </Button>
              </>
            )}
          </div>
        </div>
      </div>
    </motion.nav>
  );
}