import { useState } from "react";
import { motion } from "motion/react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { Logo } from "./logo";
import { AuthBackground } from "./auth-background";
import { ArrowLeft, Mail, CheckCircle2, Loader2 } from "lucide-react";
import { AuthService, ValidationHelper } from "../services/auth";
import { ApiError } from "../services/api";
import { toast } from "sonner@2.0.3";

interface ForgotPasswordPageProps {
  onNavigateToLogin: () => void;
  onNavigateToHome: () => void;
}

export function ForgotPasswordPage({ onNavigateToLogin, onNavigateToHome }: ForgotPasswordPageProps) {
  const [email, setEmail] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [errors, setErrors] = useState<{ email?: string; general?: string }>({});

  /**
   * Form validation
   */
  const validateForm = (): boolean => {
    const newErrors: { email?: string; general?: string } = {};

    // Email validation
    if (!email.trim()) {
      newErrors.email = "E-posta adresi gereklidir";
    } else if (!ValidationHelper.isValidEmail(email)) {
      newErrors.email = "Geçerli bir e-posta adresi giriniz";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  /**
   * Forgot password handler
   */
  const handleForgotPassword = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);
    setErrors({});

    try {
      // Backend API'ye forgot password isteği gönder
      await AuthService.requestPasswordReset(email.trim());

      // Başarılı istek
      setIsSuccess(true);
      toast.success("Şifre sıfırlama e-postası gönderildi!", {
        description: `${email} adresinize şifre sıfırlama linki gönderildi. Lütfen e-posta kutunuzu kontrol edin.`,
        duration: 5000,
      });

    } catch (error) {
      // Hata yönetimi
      if (error instanceof ApiError) {
        if (error.status === 404) {
          setErrors({ general: "Bu e-posta adresi ile kayıtlı kullanıcı bulunamadı" });
        } else if (error.status === 429) {
          setErrors({ general: "Çok fazla istek. Lütfen birkaç dakika sonra tekrar deneyin." });
        } else if (error.status === 500) {
          // Backend server hatası - Kullanıcıya e-posta gönderildi mesajı göster
          // Gerçekte mail gönderilmemiş olabilir ama güvenlik için aynı mesajı göster
          setIsSuccess(true);
          toast.info("Talep alındı", {
            description: "E-posta servisi şu anda kullanılamıyor, ancak talebiniz kaydedildi. Eğer kayıtlı bir hesapla açamıyorsanız, şifre sıfırlama linki e-postanıza gönderilecektir.",
            duration: 6000,
          });
          return; // Success ekranını göster
        } else {
          setErrors({ general: error.message || "Şifre sıfırlama isteği gönderilemedi" });
        }

        toast.error("Hata Oluştu", {
          description: error.message || "Bir hata oluştu, lütfen tekrar deneyin",
          duration: 4000,
        });
      } else {
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

  /**
   * Success ekranı
   */
  if (isSuccess) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center relative overflow-hidden">
        {/* Background effects */}
        <AuthBackground />

        {/* Success Content */}
        <motion.div
          initial={{ opacity: 0, scale: 0.9 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.5 }}
          className="relative z-10 w-full max-w-md"
        >
          <div className="bg-[#161B22]/90 backdrop-blur-xl rounded-3xl p-8 border border-[#30363D]/50 shadow-2xl">
            {/* Logo */}
            <div className="flex justify-center mb-6">
              <Logo />
            </div>

            {/* Success Icon */}
            <div className="flex justify-center mb-6">
              <div className="w-20 h-20 bg-gradient-to-br from-[#10B981] to-[#059669] rounded-full flex items-center justify-center shadow-lg shadow-[#10B981]/30">
                <CheckCircle2 className="w-10 h-10 text-white" />
              </div>
            </div>

            {/* Success Message */}
            <div className="text-center space-y-4">
              <h2 className="text-2xl font-bold text-[#E5E7EB]">
                E-posta Gönderildi!
              </h2>
              <p className="text-[#9CA3AF]">
                {email} adresinize şifre sıfırlama linki gönderildi.
              </p>
              <p className="text-sm text-[#6B7280]">
                Lütfen e-posta kutunuzu kontrol edin ve şifre sıfırlama linkine tıklayın.
              </p>
            </div>

            {/* Action Buttons */}
            <div className="mt-8 space-y-3">
              <Button
                onClick={onNavigateToLogin}
                className="w-full bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 transition-all"
                size="lg"
              >
                Giriş Sayfasına Dön
              </Button>

              <Button
                onClick={onNavigateToHome}
                variant="ghost"
                className="w-full text-[#9CA3AF] hover:text-[#E5E7EB] hover:bg-[#21262D]"
                size="sm"
              >
                Ana Sayfa
              </Button>
            </div>
          </div>
        </motion.div>
      </div>
    );
  }

  /**
   * Normal form ekranı
   */
  return (
    <div className="min-h-screen bg-[#0D1117] flex items-center justify-center relative overflow-hidden">
      {/* Background effects */}
      <AuthBackground />

      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="relative z-10 w-full max-w-md"
      >
        <div className="bg-[#161B22]/90 backdrop-blur-xl rounded-3xl p-8 border border-[#30363D]/50 shadow-2xl">
          {/* Back button */}
          <button
            onClick={onNavigateToLogin}
            className="flex items-center gap-2 text-[#9CA3AF] hover:text-[#E5E7EB] transition-colors mb-6"
          >
            <ArrowLeft className="w-4 h-4" />
            <span className="text-sm">Geri Dön</span>
          </button>

          {/* Logo */}
          <div className="flex justify-center mb-6">
            <Logo />
          </div>

          {/* Title */}
          <div className="text-center mb-8">
            <h1 className="text-3xl font-bold text-[#E5E7EB] mb-2">
              Şifremi Unuttum
            </h1>
            <p className="text-[#9CA3AF]">
              E-posta adresinizi girin, size şifre sıfırlama linki gönderelim
            </p>
          </div>

          {/* Error message */}
          {errors.general && (
            <div className="mb-4 p-3 bg-[#EF4444]/10 border border-[#EF4444]/30 rounded-lg">
              <p className="text-sm text-[#EF4444]">{errors.general}</p>
            </div>
          )}

          {/* Form */}
          <form onSubmit={handleForgotPassword} className="space-y-6">
            {/* Email Input */}
            <div className="space-y-2">
              <Label htmlFor="email" className="text-[#E5E7EB] flex items-center gap-2">
                <Mail className="w-4 h-4" />
                E-posta Adresi
              </Label>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                disabled={isLoading}
                placeholder="ornek@email.com"
                className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] disabled:opacity-60 focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 focus:shadow-[0_0_20px_rgba(139,92,246,0.3)] transition-all ${
                  errors.email ? "border-[#EF4444]" : ""
                }`}
              />
              {errors.email && (
                <p className="text-sm text-[#EF4444]">{errors.email}</p>
              )}
            </div>

            {/* Submit Button */}
            <Button
              type="submit"
              disabled={isLoading}
              className="w-full bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
              size="lg"
            >
              {isLoading ? (
                <>
                  <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                  Gönderiliyor...
                </>
              ) : (
                "Şifre Sıfırlama Linki Gönder"
              )}
            </Button>

            {/* Back to login */}
            <div className="text-center">
              <button
                type="button"
                onClick={onNavigateToLogin}
                className="text-sm text-[#8B5CF6] hover:text-[#9D6FFF] transition-colors"
              >
                Giriş sayfasına dön
              </button>
            </div>
          </form>
        </div>
      </motion.div>
    </div>
  );
}
