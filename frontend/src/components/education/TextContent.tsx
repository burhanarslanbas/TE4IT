/**
 * Text Content Component
 * Text içerik görüntüleyici (HTML/Markdown)
 */

import { Card, CardContent } from '../ui/card';

interface TextContentProps {
  content: string;
  title: string;
}

export function TextContent({ content, title }: TextContentProps) {
  return (
    <Card className="bg-[#161B22]/50 border-[#30363D]">
      <CardContent className="p-8">
        <h2 className="text-2xl font-bold text-[#E5E7EB] mb-6">{title}</h2>
        <div
          className="prose prose-invert max-w-none text-[#E5E7EB]"
          dangerouslySetInnerHTML={{ __html: content }}
          style={{
            // Custom styles for HTML content
            color: '#E5E7EB',
          }}
        />
      </CardContent>
    </Card>
  );
}

