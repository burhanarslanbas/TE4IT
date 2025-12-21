import { motion } from "motion/react";
import { useNavigate } from "react-router-dom";
import { Button } from "./ui/button";
import { Logo } from "./logo";
import { ThemeToggle } from "./theme-toggle";
import { useLanguage } from "../contexts/LanguageContext";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import { Globe } from "lucide-react";

interface NavigationProps {
  isAuthenticated?: boolean;
  onLogout?: () => void;
}

export function Navigation({ isAuthenticated = false, onLogout }: NavigationProps) {
  const navigate = useNavigate();
  const { language, setLanguage, t } = useLanguage();

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
              {t('nav.features')}
            </a>
            <a href="#pricing" className="text-[#E5E7EB] hover:text-[#8B5CF6] transition-colors">
              {t('nav.pricing')}
            </a>
            <a href="#why-us" className="text-[#E5E7EB] hover:text-[#8B5CF6] transition-colors">
              {t('nav.whyUs')}
            </a>
          </div>
          
          {/* Buttons */}
          <div className="flex items-center space-x-4">
            {/* Language Selector */}
            <Select value={language} onValueChange={(value) => setLanguage(value as 'tr' | 'en')}>
              <SelectTrigger className="w-[100px] h-9 bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]">
                <Globe className="w-4 h-4 mr-2" />
                <SelectValue />
              </SelectTrigger>
              <SelectContent className="bg-[#161B22] border-[#30363D]">
                <SelectItem value="tr" className="text-[#E5E7EB] focus:bg-[#8B5CF6]/10">
                  🇹🇷 TR
                </SelectItem>
                <SelectItem value="en" className="text-[#E5E7EB] focus:bg-[#8B5CF6]/10">
                  🇬🇧 EN
                </SelectItem>
              </SelectContent>
            </Select>
            <ThemeToggle />
            
            {/* Authentication buttons - Conditional rendering */}
            {isAuthenticated ? (
              // Authenticated user buttons
              <>
                <Button 
                  variant="ghost" 
                  className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                  onClick={() => navigate("/trainings")}
                >
                  {t('nav.trainings')}
                </Button>
                <Button 
                  variant="ghost" 
                  className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                  onClick={() => navigate("/projects")}
                >
                  {t('nav.projects')}
                </Button>
                <Button 
                  variant="ghost" 
                  className="text-[#E5E7EB] hover:bg-[#8B5CF6]/10 border border-[#30363D]/50"
                  onClick={() => navigate("/profile")}
                >
                  {t('nav.profile')}
                </Button>
                <Button 
                  variant="ghost" 
                  className="text-[#EF4444] hover:bg-[#EF4444]/10 border border-[#EF4444]/30"
                  onClick={onLogout}
                >
                  {t('nav.logout')}
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
                  {t('nav.login')}
                </Button>
                <Button 
                  className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90 shadow-lg shadow-[#8B5CF6]/25"
                  onClick={() => navigate("/register")}
                >
                  {t('nav.register')}
                </Button>
              </>
            )}
          </div>
        </div>
      </div>
    </motion.nav>
  );
}