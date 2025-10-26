import { useState } from "react";
import { motion } from "motion/react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { Logo } from "./logo";
import { AuthBackground } from "./auth-background";
import { Eye, EyeOff, ArrowLeft, Loader2 } from "lucide-react";
import { AuthService, ValidationHelper } from "../services/auth";
import { ApiError } from "../services/api";
import { toast } from "sonner@2.0.3";

interface LoginPageProps {
  onNavigateToRegister: () => void;
  onNavigateToHome: () => void;
  onNavigateToForgotPassword?: () => void;
  onLogin: () => void;
}

export function LoginPage({ onNavigateToRegister, onNavigateToHome, onNavigateToForgotPassword, onLogin }: LoginPageProps) {
  // Form state'leri
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  
  // Loading ve error state'leri
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState<{ email?: string; password?: string; general?: string }>({});

  /**
   * Form validation - Frontend tarafında temel kontroller
   */
  const validateForm = (): boolean => {
    const newErrors: { email?: string; password?: string; general?: string } = {};

    // Email validation
    if (!email.trim()) {
      newErrors.email = "E-posta adresi gereklidir";
    } else if (!ValidationHelper.isValidEmail(email)) {
      newErrors.email = "Geçerli bir e-posta adresi giriniz";
    }

    // Password validation
    if (!password.trim()) {
      newErrors.password = "Şifre gereklidir";
    } else if (password.length < 6) {
      newErrors.password = "Şifre en az 6 karakter olmalıdır";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  /**
   * Login form submit handler - Backend API entegrasyonu
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Form validation
    if (!validateForm()) {
      return;
    }

    setIsLoading(true);
    setErrors({});

    try {
      // Backend API'ye login isteği gönder
      const response = await AuthService.login({
        email: email.trim(),
        password: password,
      });

      // Başarılı giriş
      toast.success("Giriş başarılı!", {
        description: `Hoş geldiniz! Profil sayfasına yönlendiriliyorsunuz...`,
        duration: 3000,
      });

      // Ana sayfaya yönlendir
      setTimeout(() => {
        onLogin();
      }, 1000);

    } catch (error) {
      // Hata yönetimi
      if (error instanceof ApiError) {
        // Backend'den gelen hata mesajları
        if (error.status === 401) {
          setErrors({ general: "E-posta veya şifre hatalı" });
        } else if (error.status === 400) {
          setErrors({ general: "Geçersiz giriş bilgileri" });
        } else if (error.status === 429) {
          setErrors({ general: "Çok fazla deneme. Lütfen bir süre bekleyin." });
        } else {
          setErrors({ general: error.message || "Giriş sırasında bir hata oluştu" });
        }

        // Toast ile hata mesajı göster
        toast.error("Giriş Başarısız", {
          description: error.message || "E-posta veya şifre hatalı",
          duration: 4000,
        });
      } else {
        // Network veya beklenmeyen hatalar
        setErrors({ general: "Bağlantı hatası. Lütfen internet bağlantınızı kontrol edin." });
        toast.error("Bağlantı Hatası", {
          description: "İnternet bağlantınızı kontrol edin",
          duration: 4000,
        });
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-[#0D1117] relative flex items-center justify-center px-4 py-12">
      {/* Animated Background */}
      <AuthBackground />

      {/* Back to Home Button */}
      <motion.button
        initial={{ opacity: 0, x: -20 }}
        animate={{ opacity: 1, x: 0 }}
        transition={{ duration: 0.5 }}
        onClick={onNavigateToHome}
        className="absolute top-8 left-8 flex items-center gap-2 text-[#9CA3AF] hover:text-[#E5E7EB] transition-colors"
      >
        <ArrowLeft className="w-4 h-4" />
        Ana Sayfa
      </motion.button>

      {/* Login Card */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.6 }}
        className="relative z-10 w-full max-w-md"
      >
        {/* Card with glassmorphism effect */}
        <div className="relative bg-[#161B22]/80 backdrop-blur-xl rounded-2xl border border-[#30363D]/50 shadow-2xl shadow-[#8B5CF6]/10 overflow-hidden">
          {/* Top gradient accent */}
          <div className="absolute top-0 left-0 right-0 h-1 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF]" />

          <div className="p-8 sm:p-10">
            {/* Logo */}
            <motion.div
              initial={{ scale: 0.8, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              transition={{ duration: 0.5, delay: 0.2 }}
              className="flex justify-center mb-8"
            >
              <Logo />
            </motion.div>

            {/* Heading */}
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.5, delay: 0.3 }}
              className="text-center mb-8"
            >
              <h2 className="text-[#E5E7EB] mb-2">Hesabına Eriş</h2>
              <p className="text-[#9CA3AF]">
                Projelerinizi yönetmeye devam edin
              </p>
            </motion.div>

            {/* Form */}
            <motion.form
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.5, delay: 0.4 }}
              onSubmit={handleSubmit}
              className="space-y-6"
            >
              {/* Email Field */}
              <div className="space-y-2">
                <Label htmlFor="email" className="text-[#E5E7EB]">
                  E-posta
                </Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="ornek@email.com"
                  value={email}
                  onChange={(e) => {
                    setEmail(e.target.value);
                    // Real-time validation - hata varsa temizle
                    if (errors.email) {
                      setErrors(prev => ({ ...prev, email: undefined }));
                    }
                  }}
                  disabled={isLoading}
                  required
                  className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#8B5CF6] focus:ring-[#8B5CF6]/20 transition-all ${
                    errors.email ? 'border-[#EF4444] focus:border-[#EF4444] focus:ring-[#EF4444]/20' : ''
                  } ${isLoading ? 'opacity-60' : ''}`}
                />
                {/* Email error message */}
                {errors.email && (
                  <p className="text-sm text-[#EF4444] flex items-center gap-1">
                    <span className="w-1 h-1 bg-[#EF4444] rounded-full" />
                    {errors.email}
                  </p>
                )}
              </div>

              {/* Password Field */}
              <div className="space-y-2">
                <Label htmlFor="password" className="text-[#E5E7EB]">
                  Şifre
                </Label>
                <div className="relative">
                  <Input
                    id="password"
                    type={showPassword ? "text" : "password"}
                    placeholder="••••••••"
                    value={password}
                    onChange={(e) => {
                      setPassword(e.target.value);
                      // Real-time validation - hata varsa temizle
                      if (errors.password) {
                        setErrors(prev => ({ ...prev, password: undefined }));
                      }
                    }}
                    disabled={isLoading}
                    required
                    className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#8B5CF6] focus:ring-[#8B5CF6]/20 pr-10 transition-all ${
                      errors.password ? 'border-[#EF4444] focus:border-[#EF4444] focus:ring-[#EF4444]/20' : ''
                    } ${isLoading ? 'opacity-60' : ''}`}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    disabled={isLoading}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-[#6B7280] hover:text-[#9CA3AF] transition-colors disabled:opacity-50"
                  >
                    {showPassword ? (
                      <EyeOff className="w-4 h-4" />
                    ) : (
                      <Eye className="w-4 h-4" />
                    )}
                  </button>
                </div>
                {/* Password error message */}
                {errors.password && (
                  <p className="text-sm text-[#EF4444] flex items-center gap-1">
                    <span className="w-1 h-1 bg-[#EF4444] rounded-full" />
                    {errors.password}
                  </p>
                )}
              </div>

              {/* General Error Message */}
              {errors.general && (
                <div className="p-3 bg-[#EF4444]/10 border border-[#EF4444]/20 rounded-lg">
                  <p className="text-sm text-[#EF4444] flex items-center gap-2">
                    <span className="w-1 h-1 bg-[#EF4444] rounded-full" />
                    {errors.general}
                  </p>
                </div>
              )}

              {/* Forgot Password Link */}
              <div className="flex justify-end">
                <button
                  type="button"
                  disabled={isLoading}
                  onClick={() => {
                    if (onNavigateToForgotPassword) {
                      onNavigateToForgotPassword();
                    } else {
                      window.location.href = "/forgot-password";
                    }
                  }}
                  className="text-sm text-[#8B5CF6] hover:text-[#9D6FFF] transition-colors disabled:opacity-50"
                >
                  Şifremi Unuttum
                </button>
              </div>

              {/* Submit Button */}
              <Button
                type="submit"
                disabled={isLoading}
                className="w-full bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/25 hover:shadow-[#8B5CF6]/40 transition-all hover:scale-[1.02] disabled:opacity-60 disabled:cursor-not-allowed disabled:hover:scale-100"
                size="lg"
              >
                {isLoading ? (
                  <>
                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    Giriş Yapılıyor...
                  </>
                ) : (
                  "Giriş Yap"
                )}
              </Button>
            </motion.form>

            {/* Divider */}
            <div className="relative my-8">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-[#30363D]" />
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-4 bg-[#161B22] text-[#6B7280]">veya</span>
              </div>
            </div>

            {/* Register Link */}
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.5, delay: 0.5 }}
              className="text-center"
            >
              <p className="text-[#9CA3AF]">
                Hesabın yok mu?{" "}
                <button
                  onClick={onNavigateToRegister}
                  className="text-[#8B5CF6] hover:text-[#9D6FFF] transition-colors"
                >
                  Ücretsiz Başla
                </button>
              </p>
            </motion.div>
          </div>
        </div>

        {/* Bottom glow effect */}
        <div className="absolute -bottom-20 left-1/2 -translate-x-1/2 w-64 h-64 bg-[#8B5CF6] opacity-20 blur-[100px] rounded-full pointer-events-none" />
      </motion.div>
    </div>
  );
}
