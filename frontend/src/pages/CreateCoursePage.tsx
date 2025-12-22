/**
 * Create Course Page
 * Kurs oluşturma sayfası - Admin/Manager için
 */

import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import { toast } from 'sonner';
import { ArrowLeft, Plus, X, Trash2, FileText, Video, File, Link as LinkIcon, Loader2, ArrowRight, CheckCircle2, Image as ImageIcon, AlertCircle } from 'lucide-react';
import { EducationService } from '../services/educationService';
import { ApiError } from '../services/api';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Textarea } from '../components/ui/textarea';
import { Label } from '../components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import type { ContentType, CreateCourseRequest, CreateRoadmapRequest, CreateRoadmapStepRequest, CreateContentRequest } from '../types/education';

interface StepFormData {
  title: string;
  description: string;
  order: number;
  isRequired: boolean;
  estimatedDurationMinutes: number;
  contents: ContentFormData[];
}

interface ContentFormData {
  type: ContentType;
  title: string;
  content?: string;
  linkUrl?: string;
  order: number;
  isRequired: boolean;
}

export function CreateCoursePage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [currentStep, setCurrentStep] = useState<'course' | 'roadmap'>('course');

  // Course form state
  const [courseTitle, setCourseTitle] = useState('');
  const [courseDescription, setCourseDescription] = useState('');
  const [thumbnailUrl, setThumbnailUrl] = useState('');

  // Roadmap form state
  const [roadmapTitle, setRoadmapTitle] = useState('');
  const [roadmapDescription, setRoadmapDescription] = useState('');
  const [estimatedDuration, setEstimatedDuration] = useState(0);
  const [steps, setSteps] = useState<StepFormData[]>([]);

  // Yetki kontrolü kaldırıldı - şimdilik herkes kurs oluşturabilir

  const handleAddStep = () => {
    const newStep: StepFormData = {
      title: '',
      description: '',
      order: steps.length + 1,
      isRequired: true,
      estimatedDurationMinutes: 0,
      contents: [],
    };
    setSteps([...steps, newStep]);
  };

  const handleRemoveStep = (index: number) => {
    const newSteps = steps.filter((_, i) => i !== index).map((step, i) => ({
      ...step,
      order: i + 1,
    }));
    setSteps(newSteps);
  };

  const handleUpdateStep = (index: number, field: keyof StepFormData, value: any) => {
    const newSteps = [...steps];
    newSteps[index] = { ...newSteps[index], [field]: value };
    setSteps(newSteps);
  };

  const handleAddContent = (stepIndex: number) => {
    const newContent: ContentFormData = {
      type: ContentType.Text,
      title: '',
      content: '',
      order: steps[stepIndex].contents.length + 1,
      isRequired: true,
    };
    const newSteps = [...steps];
    newSteps[stepIndex].contents.push(newContent);
    setSteps(newSteps);
  };

  const handleRemoveContent = (stepIndex: number, contentIndex: number) => {
    const newSteps = [...steps];
    newSteps[stepIndex].contents = newSteps[stepIndex].contents
      .filter((_, i) => i !== contentIndex)
      .map((content, i) => ({ ...content, order: i + 1 }));
    setSteps(newSteps);
  };

  const handleUpdateContent = (
    stepIndex: number,
    contentIndex: number,
    field: keyof ContentFormData,
    value: any
  ) => {
    const newSteps = [...steps];
    newSteps[stepIndex].contents[contentIndex] = {
      ...newSteps[stepIndex].contents[contentIndex],
      [field]: value,
    };
    setSteps(newSteps);
  };

  const handleCreateCourse = async () => {
    if (!courseTitle.trim() || !courseDescription.trim()) {
      toast.error('Lütfen tüm gerekli alanları doldurun');
      return;
    }

    setLoading(true);
    try {
      const courseData: CreateCourseRequest = {
        title: courseTitle,
        description: courseDescription,
        thumbnailUrl: thumbnailUrl || undefined,
      };

      const course = await EducationService.createCourse(courseData);
      toast.success('Kurs başarıyla oluşturuldu!', {
        description: 'Şimdi roadmap oluşturabilirsiniz.',
        duration: 3000,
      });
      
      // Course ID'yi state'te sakla (gerçek uygulamada context veya state management kullanılabilir)
      (window as any).__tempCourseId = course.id;
      
      // Roadmap oluşturma adımına geç - scroll to top
      setCurrentStep('roadmap');
      window.scrollTo({ top: 0, behavior: 'smooth' });
    } catch (error: any) {
      console.error('Error creating course:', error);
      
      // ApiError ise daha detaylı mesaj göster
      if (error instanceof ApiError) {
        if (error.status === 404) {
          toast.error('Backend API Hazır Değil', {
            description: 'Kurs oluşturma endpoint\'i henüz aktif değil. Backend deploy edildikten sonra tekrar deneyin.',
            duration: 5000,
          });
        } else if (error.status === 403) {
          toast.error('Yetki Hatası', {
            description: error.message || 'Kurs oluşturmak için yeterli yetkiniz yok.',
            duration: 5000,
          });
        } else {
          toast.error('Kurs Oluşturulamadı', {
            description: error.message || 'Bir hata oluştu. Lütfen tekrar deneyin.',
            duration: 5000,
          });
        }
      } else {
        toast.error('Kurs Oluşturulamadı', {
          description: error.message || 'Bir hata oluştu. Lütfen tekrar deneyin.',
          duration: 5000,
        });
      }
    } finally {
      setLoading(false);
    }
  };

  const handleCreateRoadmap = async () => {
    if (!roadmapTitle.trim()) {
      toast.error('Roadmap başlığı gereklidir');
      return;
    }

    if (steps.length === 0) {
      toast.error('En az bir adım eklemelisiniz');
      return;
    }

    // Validate steps
    for (let i = 0; i < steps.length; i++) {
      const step = steps[i];
      if (!step.title.trim()) {
        toast.error(`${i + 1}. adımın başlığı gereklidir`);
        return;
      }
      if (step.contents.length === 0) {
        toast.error(`${i + 1}. adımda en az bir içerik olmalıdır`);
        return;
      }
      // Validate contents
      for (let j = 0; j < step.contents.length; j++) {
        const content = step.contents[j];
        if (!content.title.trim()) {
          toast.error(`${i + 1}. adımın ${j + 1}. içeriğinin başlığı gereklidir`);
          return;
        }
        if (content.type === ContentType.Text && !content.content?.trim()) {
          toast.error(`${i + 1}. adımın ${j + 1}. içeriği (Text) için içerik gereklidir`);
          return;
        }
        if (
          (content.type === ContentType.VideoLink ||
            content.type === ContentType.DocumentLink ||
            content.type === ContentType.ExternalLink) &&
          !content.linkUrl?.trim()
        ) {
          toast.error(`${i + 1}. adımın ${j + 1}. içeriği için URL gereklidir`);
          return;
        }
      }
    }

    setLoading(true);
    try {
      const courseId = (window as any).__tempCourseId;
      if (!courseId) {
        toast.error('Kurs ID bulunamadı. Lütfen sayfayı yenileyin ve tekrar deneyin.');
        return;
      }

      const totalDuration = steps.reduce((sum, step) => sum + step.estimatedDurationMinutes, 0);

      const roadmapData: CreateRoadmapRequest = {
        title: roadmapTitle,
        description: roadmapDescription || undefined,
        estimatedDurationMinutes: estimatedDuration || totalDuration,
        steps: steps.map((step) => ({
          title: step.title,
          description: step.description || undefined,
          order: step.order,
          isRequired: step.isRequired,
          estimatedDurationMinutes: step.estimatedDurationMinutes,
          contents: step.contents.map((content) => ({
            type: content.type,
            title: content.title,
            content: content.content || undefined,
            linkUrl: content.linkUrl || undefined,
            order: content.order,
            isRequired: content.isRequired,
          })),
        })),
      };

      await EducationService.createRoadmap(courseId, roadmapData);
      toast.success('Roadmap başarıyla oluşturuldu!');
      
      // Cleanup
      delete (window as any).__tempCourseId;
      
      // Kurs detay sayfasına yönlendir
      navigate(`/trainings/${courseId}`);
    } catch (error: any) {
      console.error('Error creating roadmap:', error);
      
      // ApiError ise daha detaylı mesaj göster
      if (error instanceof ApiError) {
        if (error.status === 404) {
          toast.error('Backend API Hazır Değil', {
            description: 'Roadmap oluşturma endpoint\'i henüz aktif değil. Backend deploy edildikten sonra tekrar deneyin.',
            duration: 5000,
          });
        } else if (error.status === 403) {
          toast.error('Yetki Hatası', {
            description: error.message || 'Roadmap oluşturmak için yeterli yetkiniz yok.',
            duration: 5000,
          });
        } else {
          toast.error('Roadmap Oluşturulamadı', {
            description: error.message || 'Bir hata oluştu. Lütfen tekrar deneyin.',
            duration: 5000,
          });
        }
      } else {
        toast.error('Roadmap Oluşturulamadı', {
          description: error.message || 'Bir hata oluştu. Lütfen tekrar deneyin.',
          duration: 5000,
        });
      }
    } finally {
      setLoading(false);
    }
  };

  const getContentTypeIcon = (type: ContentType) => {
    switch (type) {
      case ContentType.Text:
        return <FileText className="h-4 w-4" />;
      case ContentType.VideoLink:
        return <Video className="h-4 w-4" />;
      case ContentType.DocumentLink:
        return <File className="h-4 w-4" />;
      case ContentType.ExternalLink:
        return <LinkIcon className="h-4 w-4" />;
      default:
        return <FileText className="h-4 w-4" />;
    }
  };

  const getContentTypeBadgeColor = (type: ContentType) => {
    switch (type) {
      case ContentType.Text:
        return 'bg-[#8B5CF6]/20 text-[#8B5CF6] border-[#8B5CF6]/50';
      case ContentType.VideoLink:
        return 'bg-[#EF4444]/20 text-[#EF4444] border-[#EF4444]/50';
      case ContentType.DocumentLink:
        return 'bg-[#3B82F6]/20 text-[#3B82F6] border-[#3B82F6]/50';
      case ContentType.ExternalLink:
        return 'bg-[#2DD4BF]/20 text-[#2DD4BF] border-[#2DD4BF]/50';
      default:
        return 'bg-[#8B5CF6]/20 text-[#8B5CF6] border-[#8B5CF6]/50';
    }
  };

  const getContentTypeLabel = (type: ContentType) => {
    switch (type) {
      case ContentType.Text:
        return 'Metin';
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

  return (
    <div className="min-h-screen bg-[#0D1117] text-[#E5E7EB] relative overflow-hidden">
      {/* Animated Background */}
      <div className="fixed inset-0 pointer-events-none overflow-hidden">
        <motion.div
          className="absolute -top-40 -right-40 w-[600px] h-[600px] bg-[#8B5CF6] opacity-[0.08] blur-[120px] rounded-full"
          animate={{
            scale: [1, 1.2, 1],
            x: [0, 40, 0],
            y: [0, -30, 0],
          }}
          transition={{
            duration: 20,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        />
      </div>

      {/* Main Content */}
      <div className="relative z-10 px-4 sm:px-6 pb-12" style={{ paddingTop: 'calc(var(--navbar-height) + 2rem)' }}>
        <div className="max-w-5xl mx-auto">
          <div className="flex items-center mb-8">
            <Button
              variant="ghost"
              onClick={() => navigate('/trainings')}
              className="text-[#9CA3AF] hover:text-[#E5E7EB] mr-4"
            >
              <ArrowLeft className="h-5 w-5 mr-2" /> Geri Dön
            </Button>
          </div>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4 }}
            className="bg-[#161B22]/60 backdrop-blur-xl border border-[#30363D]/60 rounded-3xl p-8 sm:p-12 shadow-2xl shadow-[#8B5CF6]/5"
          >
            {/* Step Indicator */}
            <div className="mb-8">
              <div className="flex items-center justify-center mb-6">
                <div className="flex items-center">
                  <motion.div
                    initial={{ scale: 0.8, opacity: 0 }}
                    animate={{ scale: 1, opacity: 1 }}
                    className={`flex items-center justify-center w-12 h-12 rounded-xl border-2 transition-all duration-300 ${
                      currentStep === 'course' 
                        ? 'bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] border-[#8B5CF6] text-white shadow-lg shadow-[#8B5CF6]/30' 
                        : 'bg-gradient-to-br from-[#10B981] to-[#059669] border-[#10B981] text-white shadow-lg shadow-[#10B981]/30'
                    }`}
                  >
                    {currentStep === 'course' ? (
                      <span className="font-bold text-lg">1</span>
                    ) : (
                      <CheckCircle2 className="h-6 w-6" />
                    )}
                  </motion.div>
                  <motion.div
                    initial={{ scaleX: 0 }}
                    animate={{ scaleX: 1 }}
                    transition={{ duration: 0.5 }}
                    className={`w-32 h-1.5 mx-3 rounded-full ${
                      currentStep === 'roadmap' 
                        ? 'bg-gradient-to-r from-[#10B981] to-[#2DD4BF]' 
                        : 'bg-[#30363D]'
                    }`}
                  />
                  <motion.div
                    initial={{ scale: 0.8, opacity: 0 }}
                    animate={{ scale: 1, opacity: 1 }}
                    className={`flex items-center justify-center w-12 h-12 rounded-xl border-2 transition-all duration-300 ${
                      currentStep === 'roadmap' 
                        ? 'bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] border-[#8B5CF6] text-white shadow-lg shadow-[#8B5CF6]/30' 
                        : 'bg-[#161B22] border-[#30363D] text-[#9CA3AF]'
                    }`}
                  >
                    <span className="font-bold text-lg">2</span>
                  </motion.div>
                </div>
              </div>
              <div className="flex justify-center gap-20 text-sm">
                <motion.span
                  animate={{ color: currentStep === 'course' ? '#8B5CF6' : '#9CA3AF' }}
                  className={`font-medium transition-colors ${currentStep === 'course' ? 'text-[#8B5CF6]' : 'text-[#9CA3AF]'}`}
                >
                  Kurs Bilgileri
                </motion.span>
                <motion.span
                  animate={{ color: currentStep === 'roadmap' ? '#8B5CF6' : '#9CA3AF' }}
                  className={`font-medium transition-colors ${currentStep === 'roadmap' ? 'text-[#8B5CF6]' : 'text-[#9CA3AF]'}`}
                >
                  Roadmap Oluştur
                </motion.span>
              </div>
            </div>

            <div className="mb-8">
              <h1 className="text-3xl sm:text-4xl font-bold text-[#E5E7EB] mb-2">
                {currentStep === 'course' ? 'Yeni Kurs Oluştur' : 'Roadmap Oluştur'}
              </h1>
              <p className="text-[#9CA3AF] text-lg">
                {currentStep === 'course' 
                  ? 'Kursunuzun temel bilgilerini girin ve başlayın' 
                  : 'Kursunuz için adım adım bir öğrenme yolu oluşturun'}
              </p>
            </div>

            {currentStep === 'course' ? (
              <div className="space-y-6">
                <motion.div
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.1 }}
                >
                  <Label htmlFor="courseTitle" className="text-[#E5E7EB] mb-2 block text-sm font-medium">
                    Kurs Başlığı <span className="text-[#EF4444]">*</span>
                  </Label>
                  <Input
                    id="courseTitle"
                    value={courseTitle}
                    onChange={(e) => setCourseTitle(e.target.value)}
                    placeholder="Örn: C# Temelleri, React ile Modern Web Geliştirme..."
                    className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all h-12 text-base"
                  />
                  {courseTitle.trim().length > 0 && courseTitle.trim().length < 3 && (
                    <motion.p
                      initial={{ opacity: 0, y: -5 }}
                      animate={{ opacity: 1, y: 0 }}
                      className="text-sm text-[#EF4444] mt-2 flex items-center gap-1"
                    >
                      <AlertCircle className="h-3 w-3" />
                      Başlık en az 3 karakter olmalıdır
                    </motion.p>
                  )}
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.2 }}
                >
                  <Label htmlFor="courseDescription" className="text-[#E5E7EB] mb-2 block text-sm font-medium">
                    Kurs Açıklaması <span className="text-[#EF4444]">*</span>
                  </Label>
                  <Textarea
                    id="courseDescription"
                    value={courseDescription}
                    onChange={(e) => setCourseDescription(e.target.value)}
                    placeholder="Kursunuz hakkında detaylı bir açıklama yazın. Öğrenciler bu açıklamayı okuyarak kursunuz hakkında bilgi sahibi olacaklar..."
                    rows={6}
                    className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all resize-none text-base leading-relaxed"
                  />
                  <div className="flex justify-between items-center mt-2">
                    {courseDescription.trim().length > 0 && courseDescription.trim().length < 10 && (
                      <motion.p
                        initial={{ opacity: 0, y: -5 }}
                        animate={{ opacity: 1, y: 0 }}
                        className="text-sm text-[#EF4444] flex items-center gap-1"
                      >
                        <AlertCircle className="h-3 w-3" />
                        Açıklama en az 10 karakter olmalıdır
                      </motion.p>
                    )}
                    <p className="text-xs text-[#9CA3AF] ml-auto">
                      {courseDescription.length} karakter
                    </p>
                  </div>
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.3 }}
                >
                  <Label htmlFor="thumbnailUrl" className="text-[#E5E7EB] mb-2 block text-sm font-medium">
                    Thumbnail URL <span className="text-[#9CA3AF] text-xs">(Opsiyonel)</span>
                  </Label>
                  <Input
                    id="thumbnailUrl"
                    value={thumbnailUrl}
                    onChange={(e) => setThumbnailUrl(e.target.value)}
                    placeholder="https://example.com/image.jpg"
                    className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all h-12 text-base"
                  />
                  {thumbnailUrl && (
                    <motion.div
                      initial={{ opacity: 0, y: 10, scale: 0.95 }}
                      animate={{ opacity: 1, y: 0, scale: 1 }}
                      transition={{ duration: 0.3 }}
                      className="mt-4"
                    >
                      <div className="relative w-full h-56 rounded-xl overflow-hidden border-2 border-[#30363D] bg-[#161B22] group">
                        <img
                          src={thumbnailUrl}
                          alt="Thumbnail preview"
                          className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
                          onError={(e) => {
                            (e.target as HTMLImageElement).style.display = 'none';
                          }}
                        />
                        <div className="absolute inset-0 flex items-center justify-center bg-[#161B22]/90 backdrop-blur-sm">
                          <div className="text-center">
                            <ImageIcon className="h-12 w-12 text-[#9CA3AF] mx-auto mb-2" />
                            <p className="text-sm text-[#9CA3AF]">Görsel yüklenemedi</p>
                            <p className="text-xs text-[#6B7280] mt-1">Lütfen geçerli bir URL girin</p>
                          </div>
                        </div>
                      </div>
                    </motion.div>
                  )}
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.4 }}
                  className="flex justify-end gap-4 pt-6 border-t border-[#30363D]"
                >
                  <Button
                    variant="outline"
                    onClick={() => navigate('/trainings')}
                    className="border-[#30363D] text-[#9CA3AF] hover:bg-[#30363D] hover:text-[#E5E7EB] px-6 h-11"
                  >
                    İptal
                  </Button>
                  <Button
                    onClick={handleCreateCourse}
                    disabled={loading || !courseTitle.trim() || !courseDescription.trim()}
                    className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white shadow-lg shadow-[#8B5CF6]/30 disabled:opacity-50 disabled:cursor-not-allowed disabled:shadow-none px-6 h-11 font-semibold"
                  >
                    {loading ? (
                      <>
                        <Loader2 className="h-4 w-4 mr-2 animate-spin" /> Oluşturuluyor...
                      </>
                    ) : (
                      <>
                        <Plus className="h-4 w-4 mr-2" /> Kursu Oluştur ve Roadmap'e Geç
                      </>
                    )}
                  </Button>
                </motion.div>
              </div>
            ) : (
              <div className="space-y-6">
                <motion.div
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.1 }}
                >
                  <Label htmlFor="roadmapTitle" className="text-[#E5E7EB] mb-2 block text-sm font-medium">
                    Roadmap Başlığı <span className="text-[#EF4444]">*</span>
                  </Label>
                  <Input
                    id="roadmapTitle"
                    value={roadmapTitle}
                    onChange={(e) => setRoadmapTitle(e.target.value)}
                    placeholder="Örn: C# Temelleri Yolu, Sıfırdan İleri Seviye..."
                    className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all h-12 text-base"
                  />
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.2 }}
                >
                  <Label htmlFor="roadmapDescription" className="text-[#E5E7EB] mb-2 block text-sm font-medium">
                    Roadmap Açıklaması <span className="text-[#9CA3AF] text-xs">(Opsiyonel)</span>
                  </Label>
                  <Textarea
                    id="roadmapDescription"
                    value={roadmapDescription}
                    onChange={(e) => setRoadmapDescription(e.target.value)}
                    placeholder="Roadmap'iniz hakkında kısa bir açıklama yazın..."
                    rows={3}
                    className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all resize-none text-base"
                  />
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.3 }}
                  className="grid grid-cols-1 sm:grid-cols-2 gap-4"
                >
                  <div>
                    <Label htmlFor="estimatedDuration" className="text-[#E5E7EB] mb-2 block text-sm font-medium">
                      Tahmini Süre (Dakika) <span className="text-[#9CA3AF] text-xs">(Opsiyonel)</span>
                    </Label>
                    <Input
                      id="estimatedDuration"
                      type="number"
                      value={estimatedDuration}
                      onChange={(e) => setEstimatedDuration(parseInt(e.target.value) || 0)}
                      placeholder="0"
                      min="0"
                      className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all h-12 text-base"
                    />
                    <p className="text-xs text-[#9CA3AF] mt-1">
                      Boş bırakılırsa adımların toplam süresi kullanılır
                    </p>
                  </div>
                  <div className="flex items-end">
                    <div className="w-full p-4 bg-[#0D1117] border border-[#30363D] rounded-lg">
                      <p className="text-xs text-[#9CA3AF] mb-1">Toplam Süre</p>
                      <p className="text-lg font-semibold text-[#8B5CF6]">
                        {steps.reduce((sum, step) => sum + step.estimatedDurationMinutes, 0)} dakika
                      </p>
                    </div>
                  </div>
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.4 }}
                  className="space-y-4"
                >
                  <div className="flex justify-between items-center pb-4 border-b border-[#30363D]">
                    <div>
                      <h2 className="text-xl font-semibold text-[#E5E7EB] mb-1">Adımlar</h2>
                      <p className="text-sm text-[#9CA3AF]">
                        Kursunuz için öğrenme adımlarını oluşturun
                      </p>
                    </div>
                    <Button
                      onClick={handleAddStep}
                      className="bg-gradient-to-r from-[#2DD4BF] to-[#24A896] hover:from-[#24A896] hover:to-[#1FA88A] text-white shadow-lg shadow-[#2DD4BF]/20"
                    >
                      <Plus className="h-4 w-4 mr-2" /> Adım Ekle
                    </Button>
                  </div>

                  {steps.map((step, stepIndex) => (
                    <motion.div
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.3 }}
                    >
                      <Card key={stepIndex} className="bg-[#0D1117] border-[#30363D] hover:border-[#8B5CF6]/50 hover:shadow-lg hover:shadow-[#8B5CF6]/10 transition-all duration-300">
                      <CardHeader className="pb-4">
                        <div className="flex justify-between items-center">
                          <div className="flex items-center gap-3">
                            <div className="w-10 h-10 rounded-lg bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] flex items-center justify-center text-white font-bold shadow-lg shadow-[#8B5CF6]/30">
                              {step.order}
                            </div>
                            <CardTitle className="text-lg text-[#E5E7EB]">
                              Adım {step.order}
                            </CardTitle>
                          </div>
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleRemoveStep(stepIndex)}
                            className="text-red-500 hover:text-red-400 hover:bg-red-500/10 rounded-lg"
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </CardHeader>
                      <CardContent className="space-y-4">
                        <div>
                          <Label className="text-[#E5E7EB] mb-2 block">
                            Adım Başlığı <span className="text-red-500">*</span>
                          </Label>
                          <Input
                            value={step.title}
                            onChange={(e) => handleUpdateStep(stepIndex, 'title', e.target.value)}
                            placeholder="Örn: C# Nedir?"
                            className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all"
                          />
                          {step.title.trim().length > 0 && step.title.trim().length < 3 && (
                            <p className="text-xs text-[#EF4444] mt-1 flex items-center gap-1">
                              <AlertCircle className="h-3 w-3" />
                              Adım başlığı en az 3 karakter olmalıdır
                            </p>
                          )}
                        </div>

                        <div>
                          <Label className="text-[#E5E7EB] mb-2 block">Açıklama</Label>
                          <Textarea
                            value={step.description}
                            onChange={(e) => handleUpdateStep(stepIndex, 'description', e.target.value)}
                            placeholder="Adım açıklaması..."
                            rows={2}
                            className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all resize-none"
                          />
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                          <div>
                            <Label className="text-[#E5E7EB] mb-2 block">Tahmini Süre (Dakika)</Label>
                            <Input
                              type="number"
                              value={step.estimatedDurationMinutes}
                              onChange={(e) =>
                                handleUpdateStep(stepIndex, 'estimatedDurationMinutes', parseInt(e.target.value) || 0)
                              }
                              min="0"
                              className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] focus:border-[#8B5CF6] transition-all"
                            />
                          </div>
                          <div className="flex items-center pt-8">
                            <Label className="flex items-center cursor-pointer group">
                              <input
                                type="checkbox"
                                checked={step.isRequired}
                                onChange={(e) => handleUpdateStep(stepIndex, 'isRequired', e.target.checked)}
                                className="mr-2 w-4 h-4 rounded border-[#30363D] bg-[#0D1117] text-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6] cursor-pointer"
                              />
                              <span className="text-[#E5E7EB] group-hover:text-[#8B5CF6] transition-colors">Zorunlu</span>
                            </Label>
                          </div>
                        </div>

                        <div className="border-t border-[#30363D] pt-4 mt-4">
                          <div className="flex justify-between items-center mb-4">
                            <div>
                              <h3 className="text-md font-semibold text-[#E5E7EB] mb-1">İçerikler</h3>
                              <p className="text-xs text-[#9CA3AF]">
                                Bu adım için içerik ekleyin (Metin, Video, Doküman, Dış Link)
                              </p>
                            </div>
                            <Button
                              onClick={() => handleAddContent(stepIndex)}
                              variant="outline"
                              size="sm"
                              className="border-[#8B5CF6] text-[#8B5CF6] hover:bg-[#8B5CF6]/10 hover:border-[#8B5CF6]"
                            >
                              <Plus className="h-4 w-4 mr-2" /> İçerik Ekle
                            </Button>
                          </div>

                          {step.contents.map((content, contentIndex) => (
                            <motion.div
                              key={contentIndex}
                              initial={{ opacity: 0, x: -10 }}
                              animate={{ opacity: 1, x: 0 }}
                              transition={{ duration: 0.2 }}
                            >
                              <Card className="bg-[#161B22] border-[#30363D] mb-4 hover:border-[#8B5CF6]/50 hover:shadow-md transition-all duration-300">
                              <CardContent className="p-4 space-y-4">
                                <div className="flex justify-between items-start">
                                  <Badge variant="outline" className={getContentTypeBadgeColor(content.type)}>
                                    {getContentTypeIcon(content.type)} {getContentTypeLabel(content.type)}
                                  </Badge>
                                  <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => handleRemoveContent(stepIndex, contentIndex)}
                                    className="text-red-500 hover:text-red-400 hover:bg-red-500/10"
                                  >
                                    <X className="h-4 w-4" />
                                  </Button>
                                </div>

                                <div>
                                  <Label className="text-[#E5E7EB] mb-2 block">
                                    İçerik Tipi
                                  </Label>
                                  <Select
                                    value={content.type.toString()}
                                    onValueChange={(value) =>
                                      handleUpdateContent(stepIndex, contentIndex, 'type', parseInt(value))
                                    }
                                  >
                                    <SelectTrigger className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]">
                                      <SelectValue />
                                    </SelectTrigger>
                                    <SelectContent className="bg-[#161B22] border-[#30363D]">
                                      <SelectItem value={ContentType.Text.toString()}>Metin</SelectItem>
                                      <SelectItem value={ContentType.VideoLink.toString()}>Video Link</SelectItem>
                                      <SelectItem value={ContentType.DocumentLink.toString()}>Doküman Link</SelectItem>
                                      <SelectItem value={ContentType.ExternalLink.toString()}>Dış Link</SelectItem>
                                    </SelectContent>
                                  </Select>
                                </div>

                                <div>
                                  <Label className="text-[#E5E7EB] mb-2 block">
                                    İçerik Başlığı <span className="text-red-500">*</span>
                                  </Label>
                                  <Input
                                    value={content.title}
                                    onChange={(e) => handleUpdateContent(stepIndex, contentIndex, 'title', e.target.value)}
                                    placeholder="İçerik başlığı..."
                                    className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]"
                                  />
                                </div>

                                {content.type === ContentType.Text ? (
                                  <div>
                                    <Label className="text-[#E5E7EB] mb-2 block">
                                      İçerik (HTML/Markdown) <span className="text-red-500">*</span>
                                    </Label>
                                    <Textarea
                                      value={content.content || ''}
                                      onChange={(e) => handleUpdateContent(stepIndex, contentIndex, 'content', e.target.value)}
                                      placeholder="HTML veya Markdown içerik..."
                                      rows={6}
                                      className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] font-mono text-sm"
                                    />
                                  </div>
                                ) : (
                                  <div>
                                    <Label className="text-[#E5E7EB] mb-2 block">
                                      URL <span className="text-red-500">*</span>
                                    </Label>
                                    <Input
                                      value={content.linkUrl || ''}
                                      onChange={(e) => handleUpdateContent(stepIndex, contentIndex, 'linkUrl', e.target.value)}
                                      placeholder="https://..."
                                      className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]"
                                    />
                                  </div>
                                )}

                                <div className="flex items-center pt-2">
                                  <Label className="flex items-center cursor-pointer group">
                                    <input
                                      type="checkbox"
                                      checked={content.isRequired}
                                      onChange={(e) => handleUpdateContent(stepIndex, contentIndex, 'isRequired', e.target.checked)}
                                      className="mr-2 w-4 h-4 rounded border-[#30363D] bg-[#0D1117] text-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6] cursor-pointer"
                                    />
                                    <span className="text-[#E5E7EB] group-hover:text-[#8B5CF6] transition-colors text-sm">Zorunlu</span>
                                  </Label>
                                </div>
                              </CardContent>
                            </Card>
                            </motion.div>
                          ))}
                        </div>
                      </CardContent>
                    </Card>
                    </motion.div>
                  ))}
                </motion.div>

                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.5 }}
                  className="flex justify-end gap-4 pt-6 border-t border-[#30363D]"
                >
                  <Button
                    variant="outline"
                    onClick={() => setCurrentStep('course')}
                    className="border-[#30363D] text-[#9CA3AF] hover:bg-[#30363D] hover:text-[#E5E7EB] px-6 h-11"
                  >
                    <ArrowLeft className="h-4 w-4 mr-2" /> Geri
                  </Button>
                  <Button
                    onClick={handleCreateRoadmap}
                    disabled={loading || steps.length === 0 || !roadmapTitle.trim()}
                    className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white shadow-lg shadow-[#8B5CF6]/30 disabled:opacity-50 disabled:cursor-not-allowed disabled:shadow-none px-6 h-11 font-semibold"
                  >
                    {loading ? (
                      <>
                        <Loader2 className="h-4 w-4 mr-2 animate-spin" /> Oluşturuluyor...
                      </>
                    ) : (
                      <>
                        <ArrowRight className="h-4 w-4 mr-2" /> Roadmap'i Oluştur ve Bitir
                      </>
                    )}
                  </Button>
                </motion.div>
              </div>
            )}
          </motion.div>
        </div>
      </div>
    </div>
  );
}

