import { motion } from "motion/react";
import { useNavigate } from "react-router-dom";
import { Button } from "./ui/button";
import { Logo } from "./logo";
import { ThemeToggle } from "./theme-toggle";

interface NavigationProps {
  isAuthenticated?: boolean;
  onLogout?: () => void;
}

export function Navigation({ isAuthenticated = false, onLogout }: NavigationProps) {
  const navigate = useNavigate();

  return (
    <motion.nav
      initial={{ y: -20, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.5 }}
      className="fixed top-0 left-0 right-0 z-50 backdrop-blur-md bg-[#0D1117]/80 border-b border-[#30363D]/50"
    >
      <div className="container mx-auto px-6 py-4">
        <div className="flex items-center justify-between">
          {/* Logo */}
          <Logo />
          
          {/* Navigation Links */}
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
          
          {/* Buttons */}
          <div className="flex items-center space-x-4">
            <ThemeToggle />
            
            {/* Authentication buttons - Conditional rendering */}
            {isAuthenticated ? (
              // Authenticated user buttons
              <>
                <Button 
                  variant="ghost" 
                  className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                  onClick={() => navigate("/profile")}
                >
                  Profil
                </Button>
                <Button 
                  variant="ghost" 
                  className="text-[#EF4444] hover:bg-[#EF4444]/10 border border-[#EF4444]/30"
                  onClick={onLogout}
                >
                  Çıkış Yap
                </Button>
              </>
            ) : (
              // Guest user buttons
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