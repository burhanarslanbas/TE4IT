import { useState } from "react";
import { motion } from "motion/react";
import { Switch } from "../ui/switch";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "../ui/select";
import { Label } from "../ui/label";
import { Globe, Bell, Mail, MessageSquare, Calendar } from "lucide-react";
import { toast } from "sonner@2.0.3";

export function AppSettings() {
  const [language, setLanguage] = useState("tr");
  const [notifications, setNotifications] = useState({
    email: true,
    push: true,
    taskReminders: true,
    teamUpdates: false,
    weeklyReport: true,
  });

  const handleLanguageChange = (value: string) => {
    setLanguage(value);
    toast.success("Dil tercihi güncellendi!", {
      description: `Uygulama dili ${value === "tr" ? "Türkçe" : "İngilizce"} olarak ayarlandı.`,
    });
  };

  const handleNotificationToggle = (key: keyof typeof notifications, value: boolean) => {
    setNotifications({ ...notifications, [key]: value });
    
    const labels = {
      email: "E-posta bildirimleri",
      push: "Push bildirimleri",
      taskReminders: "Görev hatırlatıcıları",
      teamUpdates: "Takım güncellemeleri",
      weeklyReport: "Haftalık rapor",
    };

    toast.info(
      value ? `${labels[key]} aktif edildi` : `${labels[key]} kapatıldı`,
      { duration: 2000 }
    );
  };

  return (
    <div className="space-y-6">
      {/* Language Settings */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="bg-[#161B22] rounded-2xl p-8 border border-[#30363D]/50 relative overflow-hidden"
      >
        <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF]" />
        
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-[#8B5CF6]/10 rounded-lg flex items-center justify-center">
            <Globe className="w-5 h-5 text-[#8B5CF6]" />
          </div>
          <h3 className="text-xl text-[#E5E7EB]">Dil Tercihi</h3>
        </div>

        <div className="space-y-2">
          <Label htmlFor="language" className="text-[#E5E7EB]">
            Uygulama Dili
          </Label>
          <Select value={language} onValueChange={handleLanguageChange}>
            <SelectTrigger 
              id="language"
              className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 focus:shadow-[0_0_20px_rgba(139,92,246,0.3)] transition-all"
            >
              <SelectValue />
            </SelectTrigger>
            <SelectContent className="bg-[#161B22] border-[#30363D]">
              <SelectItem value="tr" className="text-[#E5E7EB] focus:bg-[#8B5CF6]/10 focus:text-[#8B5CF6]">
                🇹🇷 Türkçe
              </SelectItem>
              <SelectItem value="en" className="text-[#E5E7EB] focus:bg-[#8B5CF6]/10 focus:text-[#8B5CF6]">
                🇬🇧 English
              </SelectItem>
            </SelectContent>
          </Select>
          <p className="text-xs text-[#6B7280]">
            Uygulama arayüzünün görüneceği dili seçin.
          </p>
        </div>
      </motion.div>

      {/* Notification Preferences */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.1 }}
        className="bg-[#161B22] rounded-2xl p-8 border border-[#30363D]/50 relative overflow-hidden"
      >
        <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#2DD4BF] via-[#8B5CF6] to-[#EC4899]" />
        
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-[#2DD4BF]/10 rounded-lg flex items-center justify-center">
            <Bell className="w-5 h-5 text-[#2DD4BF]" />
          </div>
          <h3 className="text-xl text-[#E5E7EB]">Bildirim Tercihleri</h3>
        </div>

        <div className="space-y-5">
          {/* Email Notifications */}
          <NotificationItem
            icon={<Mail className="w-5 h-5 text-[#8B5CF6]" />}
            title="E-posta Bildirimleri"
            description="Önemli güncellemeler e-posta ile bildirilsin"
            checked={notifications.email}
            onCheckedChange={(checked) => handleNotificationToggle("email", checked)}
          />

          {/* Push Notifications */}
          <NotificationItem
            icon={<Bell className="w-5 h-5 text-[#2DD4BF]" />}
            title="Push Bildirimleri"
            description="Tarayıcı ve mobil bildirimler"
            checked={notifications.push}
            onCheckedChange={(checked) => handleNotificationToggle("push", checked)}
          />

          {/* Task Reminders */}
          <NotificationItem
            icon={<Calendar className="w-5 h-5 text-[#EC4899]" />}
            title="Görev Hatırlatıcıları"
            description="Yaklaşan görevler için hatırlatıcı al"
            checked={notifications.taskReminders}
            onCheckedChange={(checked) => handleNotificationToggle("taskReminders", checked)}
          />

          {/* Team Updates */}
          <NotificationItem
            icon={<MessageSquare className="w-5 h-5 text-[#F97316]" />}
            title="Takım Güncellemeleri"
            description="Takım üyelerinin aktiviteleri"
            checked={notifications.teamUpdates}
            onCheckedChange={(checked) => handleNotificationToggle("teamUpdates", checked)}
          />

          {/* Weekly Report */}
          <NotificationItem
            icon={<Mail className="w-5 h-5 text-[#8B5CF6]" />}
            title="Haftalık Rapor"
            description="Her hafta özet e-posta al"
            checked={notifications.weeklyReport}
            onCheckedChange={(checked) => handleNotificationToggle("weeklyReport", checked)}
          />
        </div>
      </motion.div>
    </div>
  );
}

interface NotificationItemProps {
  icon: React.ReactNode;
  title: string;
  description: string;
  checked: boolean;
  onCheckedChange: (checked: boolean) => void;
}

function NotificationItem({ icon, title, description, checked, onCheckedChange }: NotificationItemProps) {
  return (
    <div className="flex items-center justify-between p-4 bg-[#0D1117] rounded-xl border border-[#30363D]/30 hover:border-[#8B5CF6]/30 transition-all">
      <div className="flex items-start gap-3 flex-1">
        <div className="w-10 h-10 bg-[#21262D] rounded-lg flex items-center justify-center flex-shrink-0">
          {icon}
        </div>
        <div className="flex-1">
          <h4 className="text-[#E5E7EB] mb-1">{title}</h4>
          <p className="text-sm text-[#6B7280]">{description}</p>
        </div>
      </div>
      
      <Switch
        checked={checked}
        onCheckedChange={onCheckedChange}
        className="data-[state=checked]:bg-[#8B5CF6] data-[state=unchecked]:bg-[#30363D] shadow-lg"
      />
    </div>
  );
}
