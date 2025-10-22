import { useState } from "react";
import { motion } from "motion/react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { Logo } from "./logo";
import { AuthBackground } from "./auth-background";
import { Eye, EyeOff, ArrowLeft, CheckCircle2, Loader2 } from "lucide-react";
import { AuthService, ValidationHelper } from "../services/auth";
import { ApiError } from "../services/api";
import { toast } from "sonner@2.0.3";

interface RegisterPageProps {
  onNavigateToLogin: () => void;
  onNavigateToHome: () => void;
}

export function RegisterPage({ onNavigateToLogin, onNavigateToHome }: RegisterPageProps) {
  // Form state'leri
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  // Loading ve error state'leri
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState<{ 
    userName?: string; 
    email?: string; 
    password?: string; 
    confirmPassword?: string; 
    general?: string 
  }>({});

  // Şifre gücü kontrolü
  const [passwordStrength, setPasswordStrength] = useState({
    length: false,
    hasNumber: false,
    hasSpecial: false,
  });

  /**
   * Şifre değişikliği handler - Güç kontrolü ile birlikte
   */
  const handlePasswordChange = (value: string) => {
    setFormData({ ...formData, password: value });
    setPasswordStrength({
      length: value.length >= 8,
      hasNumber: /\d/.test(value),
      hasSpecial: /[!@#$%^&*(),.?":{}|<>]/.test(value),
    });

    // Şifre hatası varsa temizle
    if (errors.password) {
      setErrors(prev => ({ ...prev, password: undefined }));
    }
  };

  /**
   * Form validation - Frontend tarafında kapsamlı kontroller
   */
  const validateForm = (): boolean => {
    const newErrors: { 
      userName?: string; 
      email?: string; 
      password?: string; 
      confirmPassword?: string; 
      general?: string 
    } = {};

    // Username validation
    if (!formData.userName.trim()) {
      newErrors.userName = "Kullanıcı adı gereklidir";
    } else if (!ValidationHelper.isValidUsername(formData.userName)) {
      newErrors.userName = "Kullanıcı adı en az 3 karakter olmalı ve sadece harf, rakam, alt çizgi içermelidir";
    }

    // Email validation
    if (!formData.email.trim()) {
      newErrors.email = "E-posta adresi gereklidir";
    } else if (!ValidationHelper.isValidEmail(formData.email)) {
      newErrors.email = "Geçerli bir e-posta adresi giriniz";
    }

    // Password validation
    if (!formData.password.trim()) {
      newErrors.password = "Şifre gereklidir";
    } else {
      const passwordValidation = ValidationHelper.validatePassword(formData.password);
      if (!passwordValidation.isValid) {
        newErrors.password = passwordValidation.errors[0]; // İlk hatayı göster
      }
    }

    // Confirm password validation
    if (!formData.confirmPassword.trim()) {
      newErrors.confirmPassword = "Şifre tekrarı gereklidir";
    } else if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = "Şifreler eşleşmiyor";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  /**
   * Register form submit handler - Backend API entegrasyonu
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
      // Backend API'ye register isteği gönder
      const response = await AuthService.register({
        userName: formData.userName.trim(),
        email: formData.email.trim(),
        password: formData.password,
      });

      // Başarılı kayıt
      toast.success("Kayıt başarılı!", {
        description: `Hoş geldiniz! Hesabınız oluşturuldu. Giriş sayfasına yönlendiriliyorsunuz...`,
        duration: 4000,
      });

      // Login sayfasına yönlendir
      setTimeout(() => {
        onNavigateToLogin();
      }, 1500);

    } catch (error) {
      // Hata yönetimi
      if (error instanceof ApiError) {
        // Backend'den gelen hata mesajları
        if (error.status === 400) {
          // Validation hataları
          if (error.errors && error.errors.length > 0) {
            // Backend'den gelen spesifik hata mesajları
            const backendErrors = error.errors;
            const newErrors: any = {};
            
            backendErrors.forEach((errMsg: string) => {
              if (errMsg.toLowerCase().includes('email')) {
                newErrors.email = errMsg;
              } else if (errMsg.toLowerCase().includes('username') || errMsg.toLowerCase().includes('kullanıcı')) {
                newErrors.userName = errMsg;
              } else if (errMsg.toLowerCase().includes('password') || errMsg.toLowerCase().includes('şifre')) {
                newErrors.password = errMsg;
              } else {
                newErrors.general = errMsg;
              }
            });
            
            setErrors(newErrors);
          } else {
            setErrors({ general: error.message || "Geçersiz kayıt bilgileri" });
          }
        } else if (error.status === 409) {
          setErrors({ general: "Bu e-posta adresi veya kullanıcı adı zaten kullanılıyor" });
        } else if (error.status === 429) {
          setErrors({ general: "Çok fazla deneme. Lütfen bir süre bekleyin." });
        } else {
          setErrors({ general: error.message || "Kayıt sırasında bir hata oluştu" });
        }

        // Toast ile hata mesajı göster
        toast.error("Kayıt Başarısız", {
          description: error.message || "Kayıt sırasında bir hata oluştu",
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

      {/* Register Card */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.6 }}
        className="relative z-10 w-full max-w-md"
      >
        {/* Card with glassmorphism effect */}
        <div className="relative bg-[#161B22]/80 backdrop-blur-xl rounded-2xl border border-[#30363D]/50 shadow-2xl shadow-[#2DD4BF]/10 overflow-hidden">
          {/* Top gradient accent */}
          <div className="absolute top-0 left-0 right-0 h-1 bg-gradient-to-r from-[#2DD4BF] via-[#8B5CF6] to-[#EC4899]" />

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
              <h2 className="text-[#E5E7EB] mb-2">Hemen Başla</h2>
              <p className="text-[#9CA3AF]">
                Ücretsiz hesabınızı oluşturun
              </p>
            </motion.div>

            {/* Form */}
            <motion.form
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.5, delay: 0.4 }}
              onSubmit={handleSubmit}
              className="space-y-5"
            >
              {/* Username Field */}
              <div className="space-y-2">
                <Label htmlFor="userName" className="text-[#E5E7EB]">
                  Kullanıcı Adı
                </Label>
                <Input
                  id="userName"
                  type="text"
                  placeholder="kullaniciadi"
                  value={formData.userName}
                  onChange={(e) => {
                    setFormData({ ...formData, userName: e.target.value });
                    // Real-time validation - hata varsa temizle
                    if (errors.userName) {
                      setErrors(prev => ({ ...prev, userName: undefined }));
                    }
                  }}
                  disabled={isLoading}
                  required
                  className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#2DD4BF] focus:ring-[#2DD4BF]/20 transition-all ${
                    errors.userName ? 'border-[#EF4444] focus:border-[#EF4444] focus:ring-[#EF4444]/20' : ''
                  } ${isLoading ? 'opacity-60' : ''}`}
                />
                {/* Username error message */}
                {errors.userName && (
                  <p className="text-sm text-[#EF4444] flex items-center gap-1">
                    <span className="w-1 h-1 bg-[#EF4444] rounded-full" />
                    {errors.userName}
                  </p>
                )}
              </div>

              {/* Email Field */}
              <div className="space-y-2">
                <Label htmlFor="email" className="text-[#E5E7EB]">
                  E-posta
                </Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="ornek@email.com"
                  value={formData.email}
                  onChange={(e) => {
                    setFormData({ ...formData, email: e.target.value });
                    // Real-time validation - hata varsa temizle
                    if (errors.email) {
                      setErrors(prev => ({ ...prev, email: undefined }));
                    }
                  }}
                  disabled={isLoading}
                  required
                  className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#2DD4BF] focus:ring-[#2DD4BF]/20 transition-all ${
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
                    value={formData.password}
                    onChange={(e) => handlePasswordChange(e.target.value)}
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
                
                {/* Password Strength Indicators */}
                {formData.password && (
                  <div className="space-y-1.5 pt-2">
                    <PasswordStrengthItem
                      met={passwordStrength.length}
                      text="En az 8 karakter"
                    />
                    <PasswordStrengthItem
                      met={passwordStrength.hasNumber}
                      text="En az bir rakam"
                    />
                    <PasswordStrengthItem
                      met={passwordStrength.hasSpecial}
                      text="En az bir özel karakter"
                    />
                  </div>
                )}
              </div>

              {/* Confirm Password Field */}
              <div className="space-y-2">
                <Label htmlFor="confirmPassword" className="text-[#E5E7EB]">
                  Şifre Tekrarı
                </Label>
                <div className="relative">
                  <Input
                    id="confirmPassword"
                    type={showConfirmPassword ? "text" : "password"}
                    placeholder="••••••••"
                    value={formData.confirmPassword}
                    onChange={(e) => {
                      setFormData({ ...formData, confirmPassword: e.target.value });
                      // Real-time validation - hata varsa temizle
                      if (errors.confirmPassword) {
                        setErrors(prev => ({ ...prev, confirmPassword: undefined }));
                      }
                    }}
                    disabled={isLoading}
                    required
                    className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#8B5CF6] focus:ring-[#8B5CF6]/20 pr-10 transition-all ${
                      errors.confirmPassword ? 'border-[#EF4444] focus:border-[#EF4444] focus:ring-[#EF4444]/20' : ''
                    } ${isLoading ? 'opacity-60' : ''}`}
                  />
                  <button
                    type="button"
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                    disabled={isLoading}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-[#6B7280] hover:text-[#9CA3AF] transition-colors disabled:opacity-50"
                  >
                    {showConfirmPassword ? (
                      <EyeOff className="w-4 h-4" />
                    ) : (
                      <Eye className="w-4 h-4" />
                    )}
                  </button>
                </div>
                {/* Confirm password error message */}
                {errors.confirmPassword && (
                  <p className="text-sm text-[#EF4444] flex items-center gap-1">
                    <span className="w-1 h-1 bg-[#EF4444] rounded-full" />
                    {errors.confirmPassword}
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

              {/* Submit Button */}
              <Button
                type="submit"
                disabled={isLoading}
                className="w-full bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] text-[#0D1117] hover:from-[#14B8A6] hover:to-[#0D9488] shadow-lg shadow-[#2DD4BF]/25 hover:shadow-[#2DD4BF]/40 transition-all hover:scale-[1.02] disabled:opacity-60 disabled:cursor-not-allowed disabled:hover:scale-100"
                size="lg"
              >
                {isLoading ? (
                  <>
                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    Hesap Oluşturuluyor...
                  </>
                ) : (
                  "Hesap Oluştur"
                )}
              </Button>

              {/* Terms */}
              <p className="text-xs text-center text-[#6B7280]">
                Hesap oluşturarak{" "}
                <a href="#" className="text-[#8B5CF6] hover:underline">
                  Kullanım Koşullarını
                </a>{" "}
                ve{" "}
                <a href="#" className="text-[#8B5CF6] hover:underline">
                  Gizlilik Politikasını
                </a>{" "}
                kabul etmiş olursunuz.
              </p>
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

            {/* Login Link */}
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.5, delay: 0.5 }}
              className="text-center"
            >
              <p className="text-[#9CA3AF]">
                Zaten bir hesabın var mı?{" "}
                <button
                  onClick={onNavigateToLogin}
                  className="text-[#2DD4BF] hover:text-[#5EEAD4] transition-colors"
                >
                  Giriş Yap
                </button>
              </p>
            </motion.div>
          </div>
        </div>

        {/* Bottom glow effect */}
        <div className="absolute -bottom-20 left-1/2 -translate-x-1/2 w-64 h-64 bg-[#2DD4BF] opacity-20 blur-[100px] rounded-full pointer-events-none" />
      </motion.div>
    </div>
  );
}

function PasswordStrengthItem({ met, text }: { met: boolean; text: string }) {
  return (
    <div className="flex items-center gap-2 text-xs">
      <CheckCircle2
        className={`w-3.5 h-3.5 transition-colors ${
          met ? "text-[#2DD4BF]" : "text-[#6B7280]"
        }`}
      />
      <span className={met ? "text-[#9CA3AF]" : "text-[#6B7280]"}>{text}</span>
    </div>
  );
}
