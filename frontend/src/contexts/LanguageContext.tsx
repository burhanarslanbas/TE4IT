/**
 * Language Context
 * i18n için dil yönetimi
 */

import React, { createContext, useContext, useState, useEffect } from 'react';

type Language = 'tr' | 'en';

interface LanguageContextType {
  language: Language;
  setLanguage: (lang: Language) => void;
  t: (key: string) => string;
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

// Çeviri dosyaları
const translations: Record<Language, Record<string, string>> = {
  tr: {
    // Navigation
    'nav.trainings': 'Eğitimler',
    'nav.projects': 'Projeler',
    'nav.profile': 'Profil',
    'nav.logout': 'Çıkış Yap',
    'nav.login': 'Giriş Yap',
    'nav.register': 'Ücretsiz Başla',
    'nav.home': 'Ana Sayfa',
    'nav.features': 'Özellikler',
    'nav.pricing': 'Fiyatlandırma',
    'nav.whyUs': 'Neden Biz?',
    
    // Projects List
    'projects.title': 'Projeler',
    'projects.subtitle': 'Tüm projelerinizi görüntüleyin ve yönetin',
    'projects.create': 'Yeni Proje Oluştur',
    'projects.search': 'Projelerde ara...',
    'projects.filter.all': 'Tüm Projeler',
    'projects.filter.active': 'Aktif',
    'projects.filter.archived': 'Arşivlenmiş',
    'projects.status.active': 'Aktif',
    'projects.status.archived': 'Arşivlenmiş',
    'projects.view': 'Görüntüle',
    'projects.noProjects': 'Henüz proje yok. İlk projenizi oluşturun!',
    'projects.noResults': 'Filtrelerinize uygun proje bulunamadı.',
    'projects.createModal.title': 'Yeni Proje Oluştur',
    'projects.createModal.titleLabel': 'Başlık',
    'projects.createModal.descriptionLabel': 'Açıklama',
    'projects.createModal.cancel': 'İptal',
    'projects.createModal.create': 'Oluştur',
    'projects.createModal.creating': 'Oluşturuluyor...',
    'projects.createModal.description': 'Yeni bir proje oluşturmak için aşağıdaki bilgileri doldurun.',
    'projects.createModal.titlePlaceholder': 'Örn: E-Ticaret Platformu, Mobil Uygulama...',
    'projects.createModal.descriptionPlaceholder': 'Projenizin detaylı açıklamasını buraya yazın...',
    'projects.createModal.minLength': 'Minimum 3 karakter gerekli',
    'projects.createModal.valid': 'Geçerli başlık',
    'projects.createModal.infoTitle': 'İpucu',
    'projects.createModal.infoDescription': 'Proje başlığı net ve açıklayıcı olmalıdır. Açıklama alanı projenizin detaylarını içerebilir.',
    'projects.loadError': 'Projeler yüklenirken bir hata oluştu.',
    'projects.titleRequired': 'Proje başlığı zorunludur.',
    'projects.titleLength': 'Proje başlığı 3-120 karakter arasında olmalıdır.',
    'projects.createSuccess': 'Proje başarıyla oluşturuldu.',
    'projects.createError': 'Proje oluşturulurken bir hata oluştu.',
    'projects.noPermission': 'Bu işlem için yetkiniz bulunmamaktadır.',
    
    // Project Detail
    'project.back': 'Projelere Dön',
    'project.edit': 'Düzenle',
    'project.archive': 'Arşivle',
    'project.activate': 'Aktifleştir',
    'project.delete': 'Sil',
    'project.started': 'Başlangıç',
    'project.modules': 'Modüller',
    'project.modules.subtitle': 'Bu proje için modülleri yönetin',
    'project.createModule': 'Modül Oluştur',
    'project.searchModules': 'Modüllerde ara...',
    'project.noModules': 'Henüz modül yok. İlk modülünüzü oluşturun!',
    'project.editModal.title': 'Proje Düzenle',
    'project.deleteModal.title': 'Proje Sil',
    'project.deleteModal.description': 'Bu projeyi silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.',
    'project.createModuleModal.title': 'Yeni Modül Oluştur',
    'project.stages.title': 'Proje Aşamaları',
    'project.stages.subtitle': 'Projenizin tüm modüllerini, use case\'lerini ve görevlerini görüntüleyin',
    'project.stages.noUseCases': 'Henüz use case yok',
    'project.stages.noTasks': 'Henüz görev yok',
    
    // Module Detail
    'module.back': 'Projeye Dön',
    'module.useCases': 'Use Cases',
    'module.useCases.subtitle': 'Bu modül için use case\'leri yönetin',
    'module.createUseCase': 'Use Case Oluştur',
    'module.searchUseCases': 'Use case\'lerde ara...',
    'module.noUseCases': 'Henüz use case yok. İlk use case\'inizi oluşturun!',
    'module.tasks': 'Görevler',
    
    // UseCase Detail
    'usecase.back': 'Modüle Dön',
    'usecase.tasks': 'Görevler',
    'usecase.tasks.subtitle': 'Bu use case için görevleri yönetin',
    'usecase.createTask': 'Görev Oluştur',
    'usecase.searchTasks': 'Görevlerde ara...',
    'usecase.filter.allStates': 'Tüm Durumlar',
    'usecase.filter.notStarted': 'Başlanmadı',
    'usecase.filter.inProgress': 'Devam Ediyor',
    'usecase.filter.completed': 'Tamamlandı',
    'usecase.filter.cancelled': 'İptal Edildi',
    'usecase.filter.allTypes': 'Tüm Tipler',
    'usecase.filter.documentation': 'Dokümantasyon',
    'usecase.filter.feature': 'Özellik',
    'usecase.filter.test': 'Test',
    'usecase.filter.bug': 'Hata',
    'usecase.noTasks': 'Henüz görev yok. İlk görevinizi oluşturun!',
    'usecase.importantNotes': 'Önemli Notlar',
    'usecase.unassigned': 'Atanmamış',
    'usecase.noDueDate': 'Bitiş tarihi yok',
    
    // Common
    'common.loading': 'Yükleniyor...',
    'common.error': 'Hata',
    'common.success': 'Başarılı',
    'common.cancel': 'İptal',
    'common.save': 'Kaydet',
    'common.saving': 'Kaydediliyor...',
    'common.delete': 'Sil',
    'common.deleting': 'Siliniyor...',
    'common.create': 'Oluştur',
    'common.creating': 'Oluşturuluyor...',
    'common.edit': 'Düzenle',
    'common.view': 'Görüntüle',
    'common.search': 'Ara',
    'common.filter': 'Filtrele',
    'common.previous': 'Önceki',
    'common.next': 'Sonraki',
    'common.required': 'Zorunlu',
    'common.characters': 'karakter',
    
    // Login
    'login.success': 'Giriş başarılı!',
    'login.redirecting': 'Profil sayfasına yönlendiriliyorsunuz...',
    
    // Task Detail
    'task.back': 'Use Case\'e Dön',
    'task.title': 'Başlık',
    'task.type': 'Tip',
    'task.state': 'Durum',
    'task.description': 'Açıklama',
    'task.importantNotes': 'Önemli Notlar',
    'task.assignee': 'Atanan',
    'task.unassigned': 'Atanmamış',
    'task.selectAssignee': 'Kullanıcı seçin',
    'task.assign': 'Ata',
    'task.started': 'Başlangıç',
    'task.due': 'Bitiş',
    'task.dueDate': 'Bitiş Tarihi',
    'task.start': 'Başlat',
    'task.complete': 'Tamamla',
    'task.cancel': 'İptal Et',
    'task.revert': 'Geri Al',
    'task.relations': 'Task İlişkileri',
    'task.addRelation': 'İlişki Ekle',
    'task.relationType': 'İlişki Tipi',
    'task.relatedTask': 'İlişkili Task',
    'task.relatedTaskPlaceholder': 'Task ID girin',
    'task.relatedTaskHint': 'İlişkilendirmek istediğiniz task\'ın ID\'sini girin',
    'task.taskNotFound': 'Task bulunamadı',
    'task.noRelations': 'Henüz ilişki yok. İlk ilişkinizi ekleyin!',
    'task.editModal.title': 'Task Düzenle',
    'task.deleteModal.title': 'Task Sil',
    'task.deleteModal.description': 'Bu task\'ı silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.',
    'task.addRelationModal.title': 'Task İlişkisi Ekle',
    'task.loadError': 'Task yüklenirken bir hata oluştu.',
    'task.titleRequired': 'Task başlığı zorunludur.',
    'task.updateSuccess': 'Task başarıyla güncellendi.',
    'task.updateError': 'Task güncellenirken bir hata oluştu.',
    'task.stateChangeSuccess': 'Task durumu başarıyla değiştirildi.',
    'task.stateChangeError': 'Task durumu değiştirilirken bir hata oluştu.',
    'task.assigneeRequired': 'Lütfen bir kullanıcı seçin.',
    'task.assignSuccess': 'Task başarıyla atandı.',
    'task.assignError': 'Task atanırken bir hata oluştu.',
    'task.deleteSuccess': 'Task başarıyla silindi.',
    'task.deleteError': 'Task silinirken bir hata oluştu.',
    'task.relationTargetRequired': 'Lütfen bir task seçin.',
    'task.relationAddSuccess': 'Task ilişkisi başarıyla eklendi.',
    'task.relationAddError': 'Task ilişkisi eklenirken bir hata oluştu.',
    'task.relationDeleteSuccess': 'Task ilişkisi başarıyla silindi.',
    'task.relationDeleteError': 'Task ilişkisi silinirken bir hata oluştu.',
    'task.relationsLoadError': 'Task ilişkileri yüklenirken bir hata oluştu.',
    'common.actions': 'İşlemler',
    
    // Profile
    'profile.projects': 'Projeler',
    'profile.home': 'Ana Sayfa',
    'profile.menu.profile': 'Profilim',
    'profile.menu.security': 'Güvenlik',
    'profile.menu.settings': 'Ayarlar',
    'profile.loggingOut': 'Çıkış yapılıyor...',
    'profile.subtitle.profile': 'Profil bilgilerinizi görüntüleyin ve düzenleyin',
    'profile.subtitle.security': 'Hesap güvenliğinizi yönetin',
    'profile.subtitle.settings': 'Uygulama tercihlerinizi özelleştirin',
    'profile.accountStatus': 'Hesap Durumu',
    'profile.premiumMember': 'Premium Üye',
    'profile.lastLogin': 'Son giriş: Bugün, 14:32',
  },
  en: {
    // Navigation
    'nav.trainings': 'Trainings',
    'nav.projects': 'Projects',
    'nav.profile': 'Profile',
    'nav.logout': 'Logout',
    'nav.login': 'Login',
    'nav.register': 'Get Started',
    'nav.home': 'Home',
    'nav.features': 'Features',
    'nav.pricing': 'Pricing',
    'nav.whyUs': 'Why Us',
    
    // Projects List
    'projects.title': 'Projects',
    'projects.subtitle': 'View and manage all your projects',
    'projects.create': 'Create Project',
    'projects.search': 'Search projects...',
    'projects.filter.all': 'All Projects',
    'projects.filter.active': 'Active',
    'projects.filter.archived': 'Archived',
    'projects.status.active': 'Active',
    'projects.status.archived': 'Archived',
    'projects.view': 'View',
    'projects.noProjects': 'No projects yet. Create your first project!',
    'projects.noResults': 'No projects found matching your filters.',
    'projects.createModal.title': 'Create New Project',
    'projects.createModal.titleLabel': 'Title',
    'projects.createModal.descriptionLabel': 'Description',
    'projects.createModal.cancel': 'Cancel',
    'projects.createModal.create': 'Create',
    'projects.createModal.creating': 'Creating...',
    'projects.createModal.description': 'Fill in the information below to create a new project.',
    'projects.createModal.titlePlaceholder': 'E.g: E-Commerce Platform, Mobile App...',
    'projects.createModal.descriptionPlaceholder': 'Enter your project description here...',
    'projects.createModal.minLength': 'Minimum 3 characters required',
    'projects.createModal.valid': 'Valid title',
    'projects.createModal.infoTitle': 'Tip',
    'projects.createModal.infoDescription': 'Project title should be clear and descriptive. Description field can contain project details.',
    'projects.loadError': 'An error occurred while loading projects.',
    'projects.titleRequired': 'Project title is required.',
    'projects.titleLength': 'Project title must be between 3-120 characters.',
    'projects.createSuccess': 'Project created successfully.',
    'projects.createError': 'An error occurred while creating project.',
    'projects.noPermission': 'You do not have permission for this action.',
    
    // Project Detail
    'project.back': 'Back to Projects',
    'project.edit': 'Edit',
    'project.archive': 'Archive',
    'project.activate': 'Activate',
    'project.delete': 'Delete',
    'project.started': 'Started',
    'project.modules': 'Modules',
    'project.modules.subtitle': 'Manage modules for this project',
    'project.createModule': 'Create Module',
    'project.searchModules': 'Search modules...',
    'project.noModules': 'No modules yet. Create your first module!',
    'project.editModal.title': 'Edit Project',
    'project.deleteModal.title': 'Delete Project',
    'project.deleteModal.description': 'Are you sure you want to delete this project? This action cannot be undone.',
    'project.createModuleModal.title': 'Create New Module',
    'project.stages.title': 'Project Stages',
    'project.stages.subtitle': 'View all modules, use cases, and tasks in your project',
    'project.stages.noUseCases': 'No use cases yet',
    'project.stages.noTasks': 'No tasks yet',
    
    // Module Detail
    'module.back': 'Back to Project',
    'module.useCases': 'Use Cases',
    'module.useCases.subtitle': 'Manage use cases for this module',
    'module.createUseCase': 'Create Use Case',
    'module.searchUseCases': 'Search use cases...',
    'module.noUseCases': 'No use cases yet. Create your first use case!',
    'module.tasks': 'Tasks',
    
    // UseCase Detail
    'usecase.back': 'Back to Module',
    'usecase.tasks': 'Tasks',
    'usecase.tasks.subtitle': 'Manage tasks for this use case',
    'usecase.createTask': 'Create Task',
    'usecase.searchTasks': 'Search tasks...',
    'usecase.filter.allStates': 'All States',
    'usecase.filter.notStarted': 'Not Started',
    'usecase.filter.inProgress': 'In Progress',
    'usecase.filter.completed': 'Completed',
    'usecase.filter.cancelled': 'Cancelled',
    'usecase.filter.allTypes': 'All Types',
    'usecase.filter.documentation': 'Documentation',
    'usecase.filter.feature': 'Feature',
    'usecase.filter.test': 'Test',
    'usecase.filter.bug': 'Bug',
    'usecase.noTasks': 'No tasks yet. Create your first task!',
    'usecase.importantNotes': 'Important Notes',
    'usecase.unassigned': 'Unassigned',
    'usecase.noDueDate': 'No due date',
    
    // Common
    'common.loading': 'Loading...',
    'common.error': 'Error',
    'common.success': 'Success',
    'common.cancel': 'Cancel',
    'common.save': 'Save',
    'common.saving': 'Saving...',
    'common.delete': 'Delete',
    'common.deleting': 'Deleting...',
    'common.create': 'Create',
    'common.creating': 'Creating...',
    'common.edit': 'Edit',
    'common.view': 'View',
    'common.search': 'Search',
    'common.filter': 'Filter',
    'common.previous': 'Previous',
    'common.next': 'Next',
    'common.required': 'Required',
    'common.characters': 'characters',
    
    // Login
    'login.success': 'Login successful!',
    'login.redirecting': 'Redirecting to profile page...',
    
    // Task Detail
    'task.back': 'Back to Use Case',
    'task.title': 'Title',
    'task.type': 'Type',
    'task.state': 'State',
    'task.description': 'Description',
    'task.importantNotes': 'Important Notes',
    'task.assignee': 'Assignee',
    'task.unassigned': 'Unassigned',
    'task.selectAssignee': 'Select user',
    'task.assign': 'Assign',
    'task.started': 'Started',
    'task.due': 'Due',
    'task.dueDate': 'Due Date',
    'task.start': 'Start',
    'task.complete': 'Complete',
    'task.cancel': 'Cancel',
    'task.revert': 'Revert',
    'task.relations': 'Task Relations',
    'task.addRelation': 'Add Relation',
    'task.relationType': 'Relation Type',
    'task.relatedTask': 'Related Task',
    'task.relatedTaskPlaceholder': 'Enter task ID',
    'task.relatedTaskHint': 'Enter the ID of the task you want to relate',
    'task.taskNotFound': 'Task not found',
    'task.noRelations': 'No relations yet. Add your first relation!',
    'task.editModal.title': 'Edit Task',
    'task.deleteModal.title': 'Delete Task',
    'task.deleteModal.description': 'Are you sure you want to delete this task? This action cannot be undone.',
    'task.addRelationModal.title': 'Add Task Relation',
    'task.loadError': 'An error occurred while loading task.',
    'task.titleRequired': 'Task title is required.',
    'task.updateSuccess': 'Task updated successfully.',
    'task.updateError': 'An error occurred while updating task.',
    'task.stateChangeSuccess': 'Task state changed successfully.',
    'task.stateChangeError': 'An error occurred while changing task state.',
    'task.assigneeRequired': 'Please select a user.',
    'task.assignSuccess': 'Task assigned successfully.',
    'task.assignError': 'An error occurred while assigning task.',
    'task.deleteSuccess': 'Task deleted successfully.',
    'task.deleteError': 'An error occurred while deleting task.',
    'task.relationTargetRequired': 'Please select a task.',
    'task.relationAddSuccess': 'Task relation added successfully.',
    'task.relationAddError': 'An error occurred while adding task relation.',
    'task.relationDeleteSuccess': 'Task relation deleted successfully.',
    'task.relationDeleteError': 'An error occurred while deleting task relation.',
    'task.relationsLoadError': 'An error occurred while loading task relations.',
    'common.actions': 'Actions',
    
    // Profile
    'profile.projects': 'Projects',
    'profile.home': 'Home',
    'profile.menu.profile': 'Profile',
    'profile.menu.security': 'Security',
    'profile.menu.settings': 'Settings',
    'profile.loggingOut': 'Logging out...',
    'profile.subtitle.profile': 'View and edit your profile information',
    'profile.subtitle.security': 'Manage your account security',
    'profile.subtitle.settings': 'Customize your application preferences',
    'profile.accountStatus': 'Account Status',
    'profile.premiumMember': 'Premium Member',
    'profile.lastLogin': 'Last login: Today, 14:32',
  },
};

export const LanguageProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [language, setLanguageState] = useState<Language>(() => {
    // localStorage'dan dil tercihini yükle
    const saved = localStorage.getItem('te4it_language') as Language;
    return saved || 'tr';
  });

  const setLanguage = (lang: Language) => {
    setLanguageState(lang);
    localStorage.setItem('te4it_language', lang);
  };

  const t = (key: string): string => {
    return translations[language][key] || key;
  };

  return (
    <LanguageContext.Provider value={{ language, setLanguage, t }}>
      {children}
    </LanguageContext.Provider>
  );
};

export const useLanguage = () => {
  const context = useContext(LanguageContext);
  if (!context) {
    throw new Error('useLanguage must be used within LanguageProvider');
  }
  return context;
};

