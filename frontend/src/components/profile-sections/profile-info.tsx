import { useState, useEffect } from "react";
import { motion } from "motion/react";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Avatar, AvatarFallback, AvatarImage } from "../ui/avatar";
import { User, Mail, Phone, Camera } from "lucide-react";
import { toast } from "sonner@2.0.3";
import { TokenHelper } from "../../services/auth";

export function ProfileInfo() {
  // Token'dan kullanıcı bilgilerini al
  const currentUser = TokenHelper.getCurrentUser();
  
  const [formData, setFormData] = useState({
    fullName: currentUser ? `${currentUser.firstName || ''} ${currentUser.lastName || ''}`.trim() || currentUser.userName : "Kullanıcı",
    userName: currentUser?.userName || "",
    email: currentUser?.email || "",
    phone: "", // Kullanıcı kendi ekleyecek
  });

  const [isEditing, setIsEditing] = useState(false);
  const [originalData, setOriginalData] = useState(formData);

  // İlk render'da kullanıcı bilgilerini set et
  useEffect(() => {
    if (currentUser) {
      const initialData = {
        fullName: `${currentUser.firstName || ''} ${currentUser.lastName || ''}`.trim() || currentUser.userName,
        userName: currentUser.userName || "",
        email: currentUser.email || "",
        phone: "", // Kullanıcı kendi ekleyecek
      };
      setFormData(initialData);
      setOriginalData(initialData);
    }
  }, [currentUser]);

  // İptal butonuna basıldığında değişiklikleri geri al
  const handleCancel = () => {
    setFormData(originalData);
    setIsEditing(false);
  };

  const handleSave = () => {
    // API call simulation
    setTimeout(() => {
      // Değişiklikleri kaydet
      setOriginalData(formData);
      setIsEditing(false);
      toast.success("Profil bilgileri başarıyla güncellendi!", {
        description: "Değişiklikler kaydedildi.",
        duration: 3000,
      });
    }, 500);
  };

  return (
    <div className="space-y-6">
      {/* Avatar Section */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="bg-[#161B22] rounded-2xl p-8 border border-[#30363D]/50 relative overflow-hidden"
      >
        {/* Neon glow effect */}
        <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF]" />
        
        <div className="flex flex-col items-center space-y-4">
          <div className="relative group">
            <Avatar className="w-32 h-32 border-4 border-[#8B5CF6]/30 shadow-lg shadow-[#8B5CF6]/20">
              <AvatarImage src="" />
              <AvatarFallback className="bg-gradient-to-br from-[#8B5CF6] to-[#EC4899] text-white text-4xl">
                {(formData.fullName || formData.userName || "K").substring(0, 2).toUpperCase()}
              </AvatarFallback>
            </Avatar>
            
            {/* Upload button overlay */}
            <button className="absolute inset-0 bg-black/60 rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
              <Camera className="w-8 h-8 text-white" />
            </button>
          </div>
          
          <div className="text-center">
            <h3 className="text-2xl text-[#E5E7EB] mb-1">{formData.fullName || formData.userName || formData.email?.split('@')[0] || "Kullanıcı"}</h3>
            <p className="text-[#9CA3AF]">{formData.email}</p>
          </div>
        </div>
      </motion.div>

      {/* Contact Information */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.1 }}
        className="bg-[#161B22] rounded-2xl p-8 border border-[#30363D]/50 relative overflow-hidden"
      >
        <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#2DD4BF] via-[#8B5CF6] to-[#EC4899]" />
        
        <div className="flex items-center justify-between mb-6">
          <h3 className="text-xl text-[#E5E7EB]">İletişim Bilgileri</h3>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => {
              if (isEditing) {
                handleCancel();
              } else {
                setIsEditing(true);
              }
            }}
            className="text-[#8B5CF6] hover:text-[#9D6FFF] hover:bg-[#8B5CF6]/10"
          >
            {isEditing ? "İptal" : "Düzenle"}
          </Button>
        </div>

        <div className="space-y-5">
          {/* User Name */}
          <div className="space-y-2">
            <Label htmlFor="userName" className="text-[#E5E7EB] flex items-center gap-2">
              <User className="w-4 h-4" />
              Kullanıcı Adı
            </Label>
            <Input
              id="userName"
              value={formData.userName}
              disabled
              className="bg-[#0D1117] border-[#30363D] text-[#9CA3AF]"
            />
          </div>

          {/* Full Name */}
          <div className="space-y-2">
            <Label htmlFor="fullName" className="text-[#E5E7EB] flex items-center gap-2">
              Tam Adı
            </Label>
            <Input
              id="fullName"
              value={formData.fullName}
              onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
              disabled={!isEditing}
              className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] disabled:opacity-60 disabled:cursor-not-allowed focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 focus:shadow-[0_0_20px_rgba(139,92,246,0.3)] transition-all"
              placeholder="İsim Soyisim"
            />
          </div>

          {/* Email */}
          <div className="space-y-2">
            <Label htmlFor="email" className="text-[#E5E7EB] flex items-center gap-2">
              <Mail className="w-4 h-4" />
              E-posta Adresi
            </Label>
            <div className="relative">
              <Input
                id="email"
                type="email"
                value={formData.email}
                disabled
                className="bg-[#0D1117] border-[#30363D] text-[#9CA3AF] pr-20"
              />
              <span className="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-[#6B7280] bg-[#21262D] px-2 py-1 rounded">
                Doğrulanmış
              </span>
            </div>
            <p className="text-xs text-[#6B7280]">
              E-posta adresinizi değiştirmek için destek ekibiyle iletişime geçin.
            </p>
          </div>

          {/* Phone */}
          <div className="space-y-2">
            <Label htmlFor="phone" className="text-[#E5E7EB] flex items-center gap-2">
              <Phone className="w-4 h-4" />
              Telefon Numarası
            </Label>
            <Input
              id="phone"
              value={formData.phone}
              onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
              disabled={!isEditing}
              className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] disabled:opacity-60 disabled:cursor-not-allowed focus:border-[#2DD4BF] focus:ring-2 focus:ring-[#2DD4BF]/20 focus:shadow-[0_0_20px_rgba(45,212,191,0.3)] transition-all"
              placeholder="+90 5XX XXX XX XX"
            />
          </div>

          {/* Save Button */}
          {isEditing && (
            <motion.div
              initial={{ opacity: 0, y: -10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.3 }}
            >
              <Button
                onClick={handleSave}
                className="w-full bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 hover:shadow-[#8B5CF6]/50 transition-all"
                size="lg"
              >
                Bilgileri Güncelle
              </Button>
            </motion.div>
          )}
        </div>
      </motion.div>
    </div>
  );
}
