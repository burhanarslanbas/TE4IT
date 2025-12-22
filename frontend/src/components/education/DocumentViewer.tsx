/**
 * Document Viewer Component
 * Doküman görüntüleyici (PDF, DOCX, vb.)
 */

import { useState } from 'react';
import { Button } from '../ui/button';
import { Card, CardContent } from '../ui/card';
import { FileText, Download, ExternalLink, CheckCircle2, Loader2 } from 'lucide-react';
import { EducationService } from '../../services/educationService';
import { toast } from 'sonner';
import type { CourseContent } from '../../types/education';

interface DocumentViewerProps {
  content: CourseContent;
  courseId: string;
  onComplete?: () => void;
}

export function DocumentViewer({ content, courseId, onComplete }: DocumentViewerProps) {
  const [isCompleted, setIsCompleted] = useState(false);
  const [isCompleting, setIsCompleting] = useState(false);

  const handleComplete = async () => {
    if (isCompleted) return;

    try {
      setIsCompleting(true);
      await EducationService.completeContent(content.id, courseId, {});
      setIsCompleted(true);
      toast.success('Doküman tamamlandı!', {
        description: 'İçerik başarıyla tamamlandı olarak işaretlendi.',
        duration: 3000,
      });
      onComplete?.();
    } catch (error: any) {
      toast.error('Hata', {
        description: error.message || 'Doküman tamamlanamadı',
      });
    } finally {
      setIsCompleting(false);
    }
  };

  const handleOpenInNewTab = () => {
    if (content.linkUrl) {
      window.open(content.linkUrl, '_blank', 'noopener,noreferrer');
    }
  };

  const handleDownload = () => {
    if (content.linkUrl) {
      const link = document.createElement('a');
      link.href = content.linkUrl;
      link.download = content.title;
      link.target = '_blank';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  };

  return (
    <Card className="bg-[#161B22]/50 border-[#30363D]">
      <CardContent className="p-8">
        <div className="flex items-start gap-4 mb-6">
          <div className="w-12 h-12 rounded-lg bg-[#8B5CF6]/20 border border-[#8B5CF6]/30 flex items-center justify-center">
            <FileText className="w-6 h-6 text-[#8B5CF6]" />
          </div>
          <div className="flex-1">
            <h2 className="text-2xl font-bold text-[#E5E7EB] mb-2">
              {content.title}
            </h2>
            <p className="text-[#9CA3AF]">
              Bu dokümanı görüntülemek veya indirmek için aşağıdaki butonları kullanabilirsiniz.
            </p>
          </div>
        </div>

        {/* Document Actions */}
        <div className="flex flex-wrap gap-4 mb-6">
          {content.linkUrl && (
            <>
              <Button
                onClick={handleOpenInNewTab}
                variant="outline"
                className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
              >
                <ExternalLink className="w-4 h-4 mr-2" />
                Yeni Sekmede Aç
              </Button>
              <Button
                onClick={handleDownload}
                variant="outline"
                className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
              >
                <Download className="w-4 h-4 mr-2" />
                İndir
              </Button>
            </>
          )}
        </div>

        {/* PDF Preview (eğer PDF ise) */}
        {content.linkUrl && content.linkUrl.toLowerCase().endsWith('.pdf') && (
          <div className="mb-6 border border-[#30363D] rounded-lg overflow-hidden">
            <iframe
              src={content.linkUrl}
              className="w-full h-[600px]"
              title={content.title}
            />
          </div>
        )}

        {/* Complete Button */}
        <div className="flex items-center justify-between pt-6 border-t border-[#30363D]">
          {isCompleted ? (
            <div className="flex items-center gap-2 text-[#10B981]">
              <CheckCircle2 className="w-5 h-5" />
              <span className="font-medium">Tamamlandı</span>
            </div>
          ) : (
            <Button
              onClick={handleComplete}
              disabled={isCompleting}
              className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white"
            >
              {isCompleting ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin mr-2" />
                  Tamamlanıyor...
                </>
              ) : (
                'Tamamlandı Olarak İşaretle'
              )}
            </Button>
          )}
        </div>
      </CardContent>
    </Card>
  );
}

