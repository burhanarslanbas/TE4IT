import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { motion } from "motion/react";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Switch } from "../ui/switch";
import { Badge } from "../ui/badge";
import { Lock, Shield, Smartphone, Monitor, Eye, EyeOff, LogOut, CheckCircle2, Loader2 } from "lucide-react";
import { toast } from "sonner@2.0.3";
import { AuthService } from "../../services/auth";
import { ApiError } from "../../services/api";

export function SecuritySettings() {
  const navigate = useNavigate();
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPasswords, setShowPasswords] = useState({
    current: false,
    new: false,
    confirm: false,
  });
  const [twoFactorEnabled, setTwoFactorEnabled] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  // Password strength calculation
  const calculatePasswordStrength = (password: string) => {
    let strength = 0;
    if (password.length >= 8) strength++;
    if (password.length >= 12) strength++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) strength++;
    return strength;
  };

  const passwordStrength = calculatePasswordStrength(newPassword);
  const strengthLabel = passwordStrength <= 1 ? "Zayıf" : passwordStrength <= 3 ? "Orta" : "Güçlü";
  const strengthColor = passwordStrength <= 1 ? "#EF4444" : passwordStrength <= 3 ? "#F59E0B" : "#10B981";

  const handlePasswordChange = async () => {
    // Validation
    if (!currentPassword) {
      toast.error("Mevcut şifre gereklidir!", {
        description: "Lütfen mevcut şifrenizi girin.",
      });
      return;
    }

    if (newPassword !== confirmPassword) {
      toast.error("Şifreler eşleşmiyor!", {
        description: "Lütfen yeni şifrenizi kontrol edin.",
      });
      return;
    }

    if (passwordStrength < 3) {
      toast.error("Şifre çok zayıf!", {
        description: "Daha güçlü bir şifre seçin.",
      });
      return;
    }

    setIsLoading(true);

    try {
      // Backend API'ye şifre değiştirme isteği gönder
      await AuthService.changePassword(currentPassword, newPassword);

      toast.success("Şifre başarıyla değiştirildi!", {
        description: "Yeni şifrenizle giriş yapabilirsiniz.",
        duration: 3000,
      });

      // Form'u temizle
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");

    } catch (error) {
      // Hata yönetimi
      if (error instanceof ApiError) {
        if (error.status === 400 || error.message?.includes('yanlış')) {
          toast.error("Mevcut şifre yanlış!", {
            description: "Lütfen mevcut şifrenizi doğru girdiğinizden emin olun.",
            duration: 5000,
          });
        } else if (error.status === 401) {
          toast.error("Oturum sonlandırıldı!", {
            description: "Lütfen tekrar giriş yapın.",
            duration: 4000,
          });
          // Redirect to login after 2 seconds
          setTimeout(() => {
            navigate("/login");
          }, 2000);
        } else {
          toast.error("Hata Oluştu", {
            description: error.message || "Şifre değiştirilemedi. Lütfen tekrar deneyin.",
            duration: 5000,
          });
        }
      } else {
        toast.error("Bağlantı Hatası", {
          description: "İnternet bağlantınızı kontrol edin ve tekrar deneyin",
          duration: 4000,
        });
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handle2FAToggle = (enabled: boolean) => {
    setTwoFactorEnabled(enabled);
    
    setTimeout(() => {
      if (enabled) {
        toast.success("2FA Aktif Edildi!", {
          description: "Hesabınıza ekstra güvenlik katmanı eklendi.",
          duration: 3000,
        });
      } else {
        toast.info("2FA Kapatıldı", {
          description: "Tek adımlı giriş kullanılacak.",
          duration: 3000,
        });
      }
    }, 300);
  };

  const activeSessions = [
    { id: 1, device: "Chrome on Windows", location: "İstanbul, Türkiye", lastActive: "Şimdi aktif", current: true },
    { id: 2, device: "Safari on iPhone", location: "İzmir, Türkiye", lastActive: "2 saat önce", current: false },
    { id: 3, device: "Firefox on MacOS", location: "Ankara, Türkiye", lastActive: "1 gün önce", current: false },
  ];

  return (
    <div className="space-y-6">
      {/* Password Change Section */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="bg-[#161B22] rounded-2xl p-8 border border-[#30363D]/50 relative overflow-hidden"
      >
        <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF]" />
        
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-[#8B5CF6]/10 rounded-lg flex items-center justify-center">
            <Lock className="w-5 h-5 text-[#8B5CF6]" />
          </div>
          <h3 className="text-xl text-[#E5E7EB]">Şifreni Güncelle</h3>
        </div>

        <div className="space-y-5">
          {/* Current Password */}
          <div className="space-y-2">
            <Label htmlFor="currentPassword" className="text-[#E5E7EB]">
              Mevcut Şifre
            </Label>
            <div className="relative">
              <Input
                id="currentPassword"
                type={showPasswords.current ? "text" : "password"}
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
                placeholder="Mevcut şifrenizi girin"
                className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] pr-10 focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 focus:shadow-[0_0_20px_rgba(139,92,246,0.3)] transition-all"
              />
              <button
                type="button"
                onClick={() => setShowPasswords({ ...showPasswords, current: !showPasswords.current })}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-[#6B7280] hover:text-[#9CA3AF]"
              >
                {showPasswords.current ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
              </button>
            </div>
          </div>

          {/* New Password */}
          <div className="space-y-2">
            <Label htmlFor="newPassword" className="text-[#E5E7EB]">
              Yeni Şifre
            </Label>
            <div className="relative">
              <Input
                id="newPassword"
                type={showPasswords.new ? "text" : "password"}
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                placeholder="Yeni şifrenizi girin"
                className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] pr-10 focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 focus:shadow-[0_0_20px_rgba(139,92,246,0.3)] transition-all"
              />
              <button
                type="button"
                onClick={() => setShowPasswords({ ...showPasswords, new: !showPasswords.new })}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-[#6B7280] hover:text-[#9CA3AF]"
              >
                {showPasswords.new ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
              </button>
            </div>

            {/* Password Strength Meter */}
            {newPassword && (
              <div className="space-y-2">
                <div className="flex items-center justify-between text-xs">
                  <span className="text-[#9CA3AF]">Şifre Gücü</span>
                  <span style={{ color: strengthColor }}>{strengthLabel}</span>
                </div>
                <div className="h-2 bg-[#21262D] rounded-full overflow-hidden">
                  <motion.div
                    className="h-full rounded-full"
                    style={{ backgroundColor: strengthColor }}
                    initial={{ width: 0 }}
                    animate={{ width: `${(passwordStrength / 5) * 100}%` }}
                    transition={{ duration: 0.3 }}
                  />
                </div>
                <div className="space-y-1">
                  <PasswordRequirement met={newPassword.length >= 8} text="En az 8 karakter" />
                  <PasswordRequirement met={/[A-Z]/.test(newPassword)} text="En az bir büyük harf" />
                  <PasswordRequirement met={/\d/.test(newPassword)} text="En az bir rakam" />
                  <PasswordRequirement met={/[!@#$%^&*(),.?":{}|<>]/.test(newPassword)} text="En az bir özel karakter" />
                </div>
              </div>
            )}
          </div>

          {/* Confirm Password */}
          <div className="space-y-2">
            <Label htmlFor="confirmPassword" className="text-[#E5E7EB]">
              Yeni Şifre Tekrar
            </Label>
            <div className="relative">
              <Input
                id="confirmPassword"
                type={showPasswords.confirm ? "text" : "password"}
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                placeholder="Yeni şifrenizi tekrar girin"
                className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] pr-10 focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 focus:shadow-[0_0_20px_rgba(139,92,246,0.3)] transition-all"
              />
              <button
                type="button"
                onClick={() => setShowPasswords({ ...showPasswords, confirm: !showPasswords.confirm })}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-[#6B7280] hover:text-[#9CA3AF]"
              >
                {showPasswords.confirm ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
              </button>
            </div>
            {confirmPassword && newPassword !== confirmPassword && (
              <p className="text-xs text-[#EF4444] flex items-center gap-1">
                <span className="w-1 h-1 bg-[#EF4444] rounded-full animate-pulse" />
                Şifreler eşleşmiyor
              </p>
            )}
          </div>

          <Button
            onClick={handlePasswordChange}
            disabled={!currentPassword || !newPassword || !confirmPassword || isLoading}
            className="w-full bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 hover:shadow-[#8B5CF6]/50 transition-all disabled:opacity-50"
            size="lg"
          >
            {isLoading ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Değiştiriliyor...
              </>
            ) : (
              "Şifreyi Değiştir"
            )}
          </Button>
        </div>
      </motion.div>

      {/* 2FA Section */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.1 }}
        className="bg-[#161B22] rounded-2xl p-8 border border-[#30363D]/50 relative overflow-hidden"
      >
        <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#2DD4BF] via-[#8B5CF6] to-[#EC4899]" />
        
        <div className="flex items-start justify-between">
          <div className="flex items-start gap-3 flex-1">
            <div className="w-10 h-10 bg-[#2DD4BF]/10 rounded-lg flex items-center justify-center flex-shrink-0">
              <Shield className="w-5 h-5 text-[#2DD4BF]" />
            </div>
            <div className="flex-1">
              <div className="flex items-center gap-3 mb-2">
                <h3 className="text-xl text-[#E5E7EB]">İki Faktörlü Kimlik Doğrulama (2FA)</h3>
                <Badge
                  className={twoFactorEnabled 
                    ? "bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30" 
                    : "bg-[#6B7280]/10 text-[#9CA3AF] border-[#6B7280]/30"
                  }
                  variant="outline"
                >
                  {twoFactorEnabled ? "Aktif" : "Kapalı"}
                </Badge>
              </div>
              <p className="text-[#9CA3AF] text-sm mb-4">
                Hesabınıza ek güvenlik katmanı ekleyin. Her girişte telefonunuza gönderilecek doğrulama kodu ile oturum açın.
              </p>
              {twoFactorEnabled && (
                <div className="flex items-center gap-2 text-sm text-[#2DD4BF] bg-[#2DD4BF]/5 px-3 py-2 rounded-lg border border-[#2DD4BF]/20">
                  <Smartphone className="w-4 h-4" />
                  <span>+90 555 *** **67 numarasına kod gönderilecek</span>
                </div>
              )}
            </div>
          </div>
          
          <Switch
            checked={twoFactorEnabled}
            onCheckedChange={handle2FAToggle}
            className="data-[state=checked]:bg-[#10B981] data-[state=unchecked]:bg-[#30363D] shadow-lg"
          />
        </div>
      </motion.div>

      {/* Active Sessions */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.2 }}
        className="bg-[#161B22] rounded-2xl p-8 border border-[#30363D]/50 relative overflow-hidden"
      >
        <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#EC4899] via-[#F97316] to-[#8B5CF6]" />
        
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-[#EC4899]/10 rounded-lg flex items-center justify-center">
            <Monitor className="w-5 h-5 text-[#EC4899]" />
          </div>
          <h3 className="text-xl text-[#E5E7EB]">Aktif Oturumlar</h3>
        </div>

        <div className="space-y-3">
          {activeSessions.map((session, index) => (
            <motion.div
              key={session.id}
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.3, delay: index * 0.05 }}
              className="flex items-center justify-between p-4 bg-[#0D1117] rounded-xl border border-[#30363D]/30 hover:border-[#8B5CF6]/30 transition-all"
            >
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-[#21262D] rounded-lg flex items-center justify-center">
                  <Monitor className="w-5 h-5 text-[#9CA3AF]" />
                </div>
                <div>
                  <div className="flex items-center gap-2">
                    <p className="text-[#E5E7EB]">{session.device}</p>
                    {session.current && (
                      <Badge className="bg-[#10B981]/10 text-[#10B981] text-xs border-[#10B981]/30" variant="outline">
                        Bu Cihaz
                      </Badge>
                    )}
                  </div>
                  <p className="text-sm text-[#6B7280]">{session.location} · {session.lastActive}</p>
                </div>
              </div>
              
              {!session.current && (
                <Button
                  variant="ghost"
                  size="sm"
                  className="text-[#EF4444] hover:text-[#DC2626] hover:bg-[#EF4444]/10"
                  onClick={() => {
                    toast.info("Oturum sonlandırıldı", {
                      description: `${session.device} cihazından çıkış yapıldı.`,
                    });
                  }}
                >
                  <LogOut className="w-4 h-4" />
                </Button>
              )}
            </motion.div>
          ))}
        </div>
      </motion.div>
    </div>
  );
}

function PasswordRequirement({ met, text }: { met: boolean; text: string }) {
  return (
    <div className="flex items-center gap-2 text-xs">
      <CheckCircle2
        className={`w-3.5 h-3.5 transition-colors ${
          met ? "text-[#10B981]" : "text-[#6B7280]"
        }`}
      />
      <span className={met ? "text-[#9CA3AF]" : "text-[#6B7280]"}>{text}</span>
    </div>
  );
}
