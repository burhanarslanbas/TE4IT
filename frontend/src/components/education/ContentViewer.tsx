/**
 * Content Viewer Component
 * Ana içerik görüntüleyici - içerik tipine göre uygun component'i render eder
 */

import { TextContent } from './TextContent';
import { VideoPlayer } from './VideoPlayer';
import { DocumentViewer } from './DocumentViewer';
import { Card, CardContent } from '../ui/card';
import { Button } from '../ui/button';
import { ExternalLink, CheckCircle2, Loader2 } from 'lucide-react';
import { EducationService } from '../../services/educationService';
import { toast } from 'sonner';
import type { CourseContent } from '../../types/education';
import { ContentType } from '../../types/education';

interface ContentViewerProps {
  content: CourseContent;
  courseId: string;
  onComplete?: () => void;
}

export function ContentViewer({ content, courseId, onComplete }: ContentViewerProps) {
  const handleExternalLinkComplete = async () => {
    try {
      await EducationService.completeContent(content.id, courseId, {});
      toast.success('İçerik tamamlandı!', {
        description: 'İçerik başarıyla tamamlandı olarak işaretlendi.',
        duration: 3000,
      });
      onComplete?.();
    } catch (error: any) {
      toast.error('Hata', {
        description: error.message || 'İçerik tamamlanamadı',
      });
    }
  };

  const handleExternalLinkClick = () => {
    if (content.linkUrl) {
      window.open(content.linkUrl, '_blank', 'noopener,noreferrer');
    }
  };

  switch (content.type) {
    case ContentType.Text:
      return (
        <TextContent
          content={content.content || ''}
          title={content.title}
        />
      );

    case ContentType.VideoLink:
      return (
        <VideoPlayer
          content={content}
          courseId={courseId}
          onComplete={onComplete}
        />
      );

    case ContentType.DocumentLink:
      return (
        <DocumentViewer
          content={content}
          courseId={courseId}
          onComplete={onComplete}
        />
      );

    case ContentType.ExternalLink:
      return (
        <Card className="bg-[#161B22]/50 border-[#30363D]">
          <CardContent className="p-8">
            <div className="flex items-start gap-4 mb-6">
              <div className="w-12 h-12 rounded-lg bg-[#2DD4BF]/20 border border-[#2DD4BF]/30 flex items-center justify-center">
                <ExternalLink className="w-6 h-6 text-[#2DD4BF]" />
              </div>
              <div className="flex-1">
                <h2 className="text-2xl font-bold text-[#E5E7EB] mb-2">
                  {content.title}
                </h2>
                <p className="text-[#9CA3AF] mb-4">
                  Bu içerik harici bir web sitesine yönlendirecek.
                </p>
                {content.linkUrl && (
                  <p className="text-sm text-[#6B7280] mb-6 break-all">
                    {content.linkUrl}
                  </p>
                )}
              </div>
            </div>

            <div className="flex gap-4">
              <Button
                onClick={handleExternalLinkClick}
                className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white"
              >
                <ExternalLink className="w-4 h-4 mr-2" />
                Linki Aç
              </Button>
              <Button
                onClick={handleExternalLinkComplete}
                variant="outline"
                className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
              >
                <CheckCircle2 className="w-4 h-4 mr-2" />
                Tamamlandı Olarak İşaretle
              </Button>
            </div>
          </CardContent>
        </Card>
      );

    default:
      return (
        <Card className="bg-[#161B22]/50 border-[#30363D]">
          <CardContent className="p-8 text-center">
            <p className="text-[#9CA3AF]">Bilinmeyen içerik tipi</p>
          </CardContent>
        </Card>
      );
  }
}

