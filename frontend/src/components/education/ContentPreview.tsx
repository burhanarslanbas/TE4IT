/**
 * Content Preview Component
 * İçerik önizleme bileşeni
 */

import { FileText, Video, ExternalLink, Clock } from 'lucide-react';
import { Badge } from '../ui/badge';
import type { CourseContent } from '../../types/education';
import { ContentType } from '../../types/education';

interface ContentPreviewProps {
  content: CourseContent;
  isCompleted?: boolean;
}

export function ContentPreview({ content, isCompleted = false }: ContentPreviewProps) {
  const getContentIcon = (type: ContentType) => {
    switch (type) {
      case ContentType.VideoLink:
        return <Video className="w-4 h-4" />;
      case ContentType.DocumentLink:
        return <FileText className="w-4 h-4" />;
      case ContentType.ExternalLink:
        return <ExternalLink className="w-4 h-4" />;
      default:
        return <FileText className="w-4 h-4" />;
    }
  };

  const getContentTypeLabel = (type: ContentType) => {
    switch (type) {
      case ContentType.VideoLink:
        return 'Video';
      case ContentType.DocumentLink:
        return 'Doküman';
      case ContentType.ExternalLink:
        return 'Dış Link';
      default:
        return 'Metin';
    }
  };

  const getContentTypeColor = (type: ContentType) => {
    switch (type) {
      case ContentType.VideoLink:
        return 'bg-pink-500/20 text-pink-400 border-pink-500/30';
      case ContentType.DocumentLink:
        return 'bg-orange-500/20 text-orange-400 border-orange-500/30';
      case ContentType.ExternalLink:
        return 'bg-blue-500/20 text-blue-400 border-blue-500/30';
      default:
        return 'bg-[#8B5CF6]/20 text-[#C4B5FD] border-[#8B5CF6]/30';
    }
  };

  return (
    <div className="flex items-center gap-3 p-3 bg-[#161B22] border border-[#30363D] rounded-lg hover:border-[#8B5CF6]/50 transition-colors">
      <div className="flex-shrink-0">
        {isCompleted ? (
          <div className="w-8 h-8 rounded-full bg-[#10B981]/20 border border-[#10B981]/30 flex items-center justify-center">
            <span className="text-[#10B981] text-xs">✓</span>
          </div>
        ) : (
          <div className="w-8 h-8 rounded-full bg-[#30363D] flex items-center justify-center">
            {getContentIcon(content.type)}
          </div>
        )}
      </div>

      <div className="flex-1 min-w-0">
        <h4 className="text-sm font-medium text-[#E5E7EB] mb-1">
          {content.title}
        </h4>
        <div className="flex items-center gap-2">
          <Badge className={`${getContentTypeColor(content.type)} text-xs`}>
            {getContentTypeLabel(content.type)}
          </Badge>
          {content.isRequired && (
            <Badge className="bg-[#8B5CF6]/20 text-[#C4B5FD] border-[#8B5CF6]/30 text-xs">
              Zorunlu
            </Badge>
          )}
        </div>
      </div>
    </div>
  );
}

