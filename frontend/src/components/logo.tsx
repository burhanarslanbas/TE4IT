import { CheckCircle, BookOpen } from "lucide-react";

export function Logo() {
  return (
    <div className="flex items-center space-x-3">
      <div className="relative">
        {/* Logo icon combining task (check) and education (book) */}
        <div className="relative w-10 h-10 bg-gradient-to-br from-[#8B5CF6] to-[#2DD4BF] rounded-lg flex items-center justify-center">
          <CheckCircle className="w-6 h-6 text-white absolute z-10" />
          <BookOpen className="w-4 h-4 text-white/70 absolute bottom-1 right-1" />
        </div>
      </div>
      <div className="flex flex-col">
        <div className="font-bold text-2xl text-[#E5E7EB] leading-tight">
          TE4IT
        </div>
        <div className="text-xs text-[#9CA3AF] leading-tight">
          Task & Education for IT
        </div>
      </div>
    </div>
  );
}