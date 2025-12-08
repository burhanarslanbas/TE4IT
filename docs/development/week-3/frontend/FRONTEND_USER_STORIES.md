# Frontend User Stories - TE4IT Platform

> **Hedef Kitle:** Frontend Developers  
> **Güncelleme:** 2024-12-08  
> **Sürüm:** 2.0

Bu dokümantasyon, TE4IT platformunda frontend geliştirme yaparken ihtiyaç duyacağınız tüm akışları **kullanıcı hikayeleri** üzerinden anlatır. Her user story için:
- ✅ Başarı kriterleri
- 🔄 Adım adım akış
- 💻 Kod örnekleri
- 🎨 UI mockup'ları
- ⚠️ Edge case'ler

## İçindekiler

### 🏗️ Proje Yönetimi
1. [Yeni Proje Oluşturma](#1-yeni-proje-oluşturma)
2. [Proje Listesini Görüntüleme](#2-proje-listesini-görüntüleme)
3. [Proje Düzenleme](#3-proje-düzenleme)
4. [Proje Silme](#4-proje-silme)
5. [Proje Durumunu Değiştirme](#5-proje-durumunu-değiştirme)

### 👥 Üye ve Davet Yönetimi
6. [Email ile Üye Davet Etme](#6-email-ile-üye-davet-etme)
7. [Daveti Kabul Etme (Davet Edilen Perspektifi)](#7-daveti-kabul-etme)
8. [Bekleyen Davetleri Görüntüleme](#8-bekleyen-davetleri-görüntüleme)
9. [Daveti İptal Etme](#9-daveti-iptal-etme)
10. [Proje Üyelerini Listeleme](#10-proje-üyelerini-listeleme)
11. [Üye Rolü Değiştirme](#11-üye-rolü-değiştirme)
12. [Üyeyi Projeden Çıkarma](#12-üyeyi-projeden-çıkarma)

---

## 🏗️ Proje Yönetimi

### 1. Yeni Proje Oluşturma

**👤 User Story:**  
"Sistem yöneticisi veya yetkili kullanıcı olarak, yeni bir yazılım projesi başlatmak istiyorum."

#### ✅ Başarı Kriterleri
- Proje başarıyla oluşturulur
- Kullanıcı otomatik olarak Owner rolü alır
- Proje listesine yeni proje eklenir
- Kullanıcı proje detay sayfasına yönlendirilir

#### 🔐 Yetki Gereksinimi
- `ProjectCreate` policy
- Roller: Administrator, OrganizationManager, TeamLead, Trainer

#### 🔄 Adım Adım Akış

##### Adım 1: Kullanıcı "Yeni Proje" butonuna tıklar

```tsx
// ProjectsPage.tsx
const ProjectsPage = () => {
  const [showCreateModal, setShowCreateModal] = useState(false);
  
  return (
    <div>
      <Button onClick={() => setShowCreateModal(true)}>
        + Yeni Proje
      </Button>
      
      {showCreateModal && (
        <CreateProjectModal 
          onClose={() => setShowCreateModal(false)} 
        />
      )}
    </div>
  );
};
```

##### Adım 2: Form validasyonu (Client-side)

```tsx
// CreateProjectModal.tsx
import { z } from 'zod';

const projectSchema = z.object({
  title: z.string()
    .min(3, 'Başlık en az 3 karakter olmalıdır')
    .max(100, 'Başlık en fazla 100 karakter olabilir')
    .trim(),
  description: z.string()
    .max(1000, 'Açıklama en fazla 1000 karakter olabilir')
    .optional()
});

type ProjectFormData = z.infer<typeof projectSchema>;

const CreateProjectModal = ({ onClose }) => {
  const [formData, setFormData] = useState<ProjectFormData>({
    title: '',
    description: ''
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  
  const validate = () => {
    try {
      projectSchema.parse(formData);
      setErrors({});
      return true;
    } catch (error) {
      if (error instanceof z.ZodError) {
        const fieldErrors: Record<string, string> = {};
        error.errors.forEach(err => {
          if (err.path[0]) {
            fieldErrors[err.path[0] as string] = err.message;
          }
        });
        setErrors(fieldErrors);
      }
      return false;
    }
  };
  
  return (
    <Modal>
      <form>
        <Input 
          label="Proje Başlığı *"
          value={formData.title}
          onChange={(e) => setFormData({...formData, title: e.target.value})}
          error={errors.title}
          maxLength={100}
        />
        <small>{formData.title.length}/100 karakter</small>
        
        <Textarea
          label="Açıklama"
          value={formData.description}
          onChange={(e) => setFormData({...formData, description: e.target.value})}
          error={errors.description}
          maxLength={1000}
          rows={4}
        />
        <small>{formData.description?.length || 0}/1000 karakter</small>
      </form>
    </Modal>
  );
};
```

##### Adım 3: API çağrısı

```typescript
// services/projectService.ts
import axios from './axios';

interface CreateProjectRequest {
  title: string;
  description?: string;
}

interface CreateProjectResponse {
  id: string;
  title: string;
  description?: string;
  createdAt: string;
}

export const createProject = async (
  data: CreateProjectRequest
): Promise<CreateProjectResponse> => {
  const response = await axios.post('/projects', data);
  return response.data;
};
```

##### Adım 4: Submit handler

```tsx
const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  
  if (!validate()) return;
  
  setLoading(true);
  try {
    // API çağrısı
    const newProject = await createProject({
      title: formData.title.trim(),
      description: formData.description?.trim()
    });
    
    // Redux/Zustand store'u güncelle
    dispatch(addProject(newProject));
    
    // Toast bildirim
    toast.success('Proje başarıyla oluşturuldu!');
    
    // Modal kapat
    onClose();
    
    // Proje detay sayfasına yönlendir
    navigate(`/projects/${newProject.id}`);
    
  } catch (error) {
    handleApiError(error);
  } finally {
    setLoading(false);
  }
};
```

##### Adım 5: Error handling

```typescript
const handleApiError = (error: any) => {
  if (error.response?.status === 400) {
    // Validation hataları
    const apiErrors = error.response.data.errors;
    const fieldErrors: Record<string, string> = {};
    
    Object.keys(apiErrors).forEach(key => {
      fieldErrors[key.toLowerCase()] = apiErrors[key][0];
    });
    
    setErrors(fieldErrors);
    toast.error('Lütfen form hatalarını düzeltin');
    
  } else if (error.response?.status === 403) {
    // Yetki yok
    toast.error('Proje oluşturma yetkiniz bulunmuyor');
    
  } else if (error.response?.status === 401) {
    // Token süresi dolmuş
    toast.error('Oturumunuz sonlanmış. Lütfen tekrar giriş yapın');
    navigate('/login');
    
  } else {
    // Genel hata
    toast.error('Bir hata oluştu. Lütfen tekrar deneyin');
    console.error('Create project error:', error);
  }
};
```

#### 🎨 UI Mockup

```
┌────────────────────────────────────────┐
│  Yeni Proje Oluştur              [X]   │
├────────────────────────────────────────┤
│                                        │
│  Proje Başlığı *                       │
│  ┌──────────────────────────────────┐  │
│  │ E-Ticaret Platformu              │  │
│  └──────────────────────────────────┘  │
│  25/100 karakter                       │
│                                        │
│  Açıklama                              │
│  ┌──────────────────────────────────┐  │
│  │ Online alışveriş sistemi         │  │
│  │ geliştirme projesi               │  │
│  │                                  │  │
│  └──────────────────────────────────┘  │
│  45/1000 karakter                      │
│                                        │
│              [İptal]  [Oluştur]        │
│                         [⏳ loading]   │
└────────────────────────────────────────┘
```

#### ⚠️ Edge Cases

1. **İnternet bağlantısı kesilirse:**
```typescript
catch (error) {
  if (error.code === 'ERR_NETWORK') {
    toast.error('İnternet bağlantınızı kontrol edin');
    // Retry mekanizması sunabilirsiniz
  }
}
```

2. **Duplicate proje ismi (backend'de kontrol yoksa):**
```typescript
// Client-side'da da kontrol edebilirsiniz
const existingProjects = await getProjects({ search: formData.title });
if (existingProjects.items.some(p => 
  p.title.toLowerCase() === formData.title.toLowerCase()
)) {
  setErrors({ title: 'Bu isimde bir proje zaten var' });
}
```

3. **Form açıkken sayfa yenilenirse:**
```typescript
useEffect(() => {
  const handleBeforeUnload = (e: BeforeUnloadEvent) => {
    if (formData.title || formData.description) {
      e.preventDefault();
      e.returnValue = 'Değişiklikler kaydedilmedi. Çıkmak istediğinize emin misiniz?';
    }
  };
  
  window.addEventListener('beforeunload', handleBeforeUnload);
  return () => window.removeEventListener('beforeunload', handleBeforeUnload);
}, [formData]);
```

#### 📊 State Management (Zustand Örneği)

```typescript
// stores/projectStore.ts
import { create } from 'zustand';

interface ProjectStore {
  projects: Project[];
  loading: boolean;
  addProject: (project: Project) => void;
  removeProject: (id: string) => void;
  updateProject: (id: string, data: Partial<Project>) => void;
}

export const useProjectStore = create<ProjectStore>((set) => ({
  projects: [],
  loading: false,
  
  addProject: (project) => set((state) => ({
    projects: [project, ...state.projects]
  })),
  
  removeProject: (id) => set((state) => ({
    projects: state.projects.filter(p => p.id !== id)
  })),
  
  updateProject: (id, data) => set((state) => ({
    projects: state.projects.map(p => 
      p.id === id ? { ...p, ...data } : p
    )
  }))
}));
```

---

### 2. Proje Listesini Görüntüleme

**👤 User Story:**  
"Kullanıcı olarak, dahil olduğum tüm projeleri görmek, filtrelemek ve aramak istiyorum."

#### ✅ Başarı Kriterleri
- Kullanıcının erişebildiği tüm projeler listelenir
- Sayfalama (pagination) çalışır
- Arama ve filtreleme işlevseldir
- Yükleme durumu görünür

#### 🔄 Adım Adım Akış

##### Adım 1: İlk yükleme

```tsx
// ProjectListPage.tsx
const ProjectListPage = () => {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 20,
    totalPages: 1,
    totalCount: 0
  });
  const [filters, setFilters] = useState({
    search: '',
    isActive: undefined as boolean | undefined
  });
  
  useEffect(() => {
    loadProjects();
  }, [pagination.page, filters]);
  
  const loadProjects = async () => {
    setLoading(true);
    try {
      const result = await getProjects({
        page: pagination.page,
        pageSize: pagination.pageSize,
        search: filters.search || undefined,
        isActive: filters.isActive
      });
      
      setProjects(result.items);
      setPagination(prev => ({
        ...prev,
        totalPages: result.totalPages,
        totalCount: result.totalCount
      }));
    } catch (error) {
      toast.error('Projeler yüklenirken hata oluştu');
      console.error(error);
    } finally {
      setLoading(false);
    }
  };
  
  return (
    <div className="project-list-page">
      <Header>
        <h1>Projeler ({pagination.totalCount})</h1>
        <Button onClick={() => navigate('/projects/new')}>
          + Yeni Proje
        </Button>
      </Header>
      
      <Filters>
        <SearchInput
          value={filters.search}
          onChange={(value) => setFilters({...filters, search: value})}
          placeholder="Proje ara..."
          debounce={300}
        />
        
        <Select
          value={filters.isActive?.toString()}
          onChange={(value) => setFilters({
            ...filters, 
            isActive: value === 'all' ? undefined : value === 'true'
          })}
        >
          <option value="all">Tümü</option>
          <option value="true">Aktif</option>
          <option value="false">Pasif</option>
        </Select>
      </Filters>
      
      {loading ? (
        <LoadingSpinner />
      ) : projects.length === 0 ? (
        <EmptyState 
          message="Henüz proje yok" 
          action="Yeni Proje Oluştur"
          onAction={() => navigate('/projects/new')}
        />
      ) : (
        <>
          <ProjectGrid projects={projects} />
          
          <Pagination
            currentPage={pagination.page}
            totalPages={pagination.totalPages}
            onPageChange={(page) => setPagination({...pagination, page})}
          />
        </>
      )}
    </div>
  );
};
```

##### Adım 2: Debounced search

```typescript
// hooks/useDebounce.ts
import { useEffect, useState } from 'react';

export const useDebounce = <T,>(value: T, delay: number = 300): T => {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);
  
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);
    
    return () => clearTimeout(handler);
  }, [value, delay]);
  
  return debouncedValue;
};

// Kullanımı:
const debouncedSearch = useDebounce(filters.search, 300);

useEffect(() => {
  if (debouncedSearch !== undefined) {
    loadProjects();
  }
}, [debouncedSearch]);
```

##### Adım 3: Proje kartı component

```tsx
// ProjectCard.tsx
interface ProjectCardProps {
  project: Project;
}

const ProjectCard: React.FC<ProjectCardProps> = ({ project }) => {
  const navigate = useNavigate();
  
  return (
    <Card 
      onClick={() => navigate(`/projects/${project.id}`)}
      className="project-card"
    >
      <CardHeader>
        <h3>{project.title}</h3>
        <Badge variant={project.isActive ? 'success' : 'secondary'}>
          {project.isActive ? 'Aktif' : 'Pasif'}
        </Badge>
      </CardHeader>
      
      <CardBody>
        <p className="description">
          {project.description || 'Açıklama yok'}
        </p>
      </CardBody>
      
      <CardFooter>
        <div className="stats">
          <span>👥 {project.memberCount} üye</span>
          <span>📦 {project.moduleCount} modül</span>
        </div>
        <span className="date">
          {new Date(project.startedDate).toLocaleDateString('tr-TR')}
        </span>
      </CardFooter>
    </Card>
  );
};
```

#### 🎨 UI Mockup

```
┌────────────────────────────────────────────────────────┐
│  Projeler (42)                      [+ Yeni Proje]     │
├────────────────────────────────────────────────────────┤
│  [🔍 Proje ara...]  [▼ Durum: Tümü]                   │
├────────────────────────────────────────────────────────┤
│                                                        │
│  ┌──────────────────┐  ┌──────────────────┐          │
│  │ E-Ticaret     [Aktif]│  Mobile App   [Aktif]│          │
│  │ Online alışveriş│  │ iOS & Android    │          │
│  │ sistemi          │  │ uygulama         │          │
│  │ 👥 5  📦 3       │  │ 👥 3  📦 2       │          │
│  │ 15.01.2024       │  │ 18.01.2024       │          │
│  └──────────────────┘  └──────────────────┘          │
│                                                        │
│  ┌──────────────────┐  ┌──────────────────┐          │
│  │ CRM System [Pasif]│  │ Blog Platform[Aktif]│          │
│  │ ...              │  │ ...              │          │
│  └──────────────────┘  └──────────────────┘          │
│                                                        │
│         [◄ Önceki]  1 / 3  [Sonraki ►]               │
└────────────────────────────────────────────────────────┘
```

---

## 👥 Üye ve Davet Yönetimi

### 6. Email ile Üye Davet Etme

**👤 User Story:**  
"Proje sahibi olarak, sistemde kayıtlı bir arkadaşımı email ile davet ederek projeye katılmasını istiyorum."

#### ✅ Başarı Kriterleri
- Davet başarıyla gönderilir
- Kullanıcıya email ulaşır
- Davet "beklemede" listesine eklenir
- Toast bildirimi gösterilir

#### ⚠️ Ön Koşullar
- Kullanıcı Owner veya Member rolünde olmalı
- Davet edilecek kişi **sistemde kayıtlı** olmalı
- Davet edilecek kişi henüz projede olmamalı
- Aynı email için bekleyen davet olmamalı

#### 🔄 Adım Adım Akış

##### Adım 1: "Üye Ekle" butonu

```tsx
// ProjectMembersPage.tsx
const ProjectMembersPage = ({ projectId }: { projectId: string }) => {
  const [showInviteModal, setShowInviteModal] = useState(false);
  const [members, setMembers] = useState<ProjectMember[]>([]);
  const [pendingInvitations, setPendingInvitations] = useState<Invitation[]>([]);
  
  useEffect(() => {
    loadMembers();
    loadInvitations();
  }, [projectId]);
  
  return (
    <div>
      <Header>
        <h2>Proje Üyeleri</h2>
        <Button onClick={() => setShowInviteModal(true)}>
          ✉️ Email ile Davet Et
        </Button>
      </Header>
      
      <Tabs>
        <Tab label={`Üyeler (${members.length})`}>
          <MembersList members={members} />
        </Tab>
        <Tab label={`Bekleyen Davetler (${pendingInvitations.length})`}>
          <InvitationsList invitations={pendingInvitations} />
        </Tab>
      </Tabs>
      
      {showInviteModal && (
        <InviteMemberModal
          projectId={projectId}
          onClose={() => setShowInviteModal(false)}
          onSuccess={handleInvitationSent}
        />
      )}
    </div>
  );
};
```

##### Adım 2: Davet formu

```tsx
// InviteMemberModal.tsx
const InviteMemberModal = ({ projectId, onClose, onSuccess }) => {
  const [email, setEmail] = useState('');
  const [role, setRole] = useState<ProjectRole>(ProjectRole.Member);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  const validateEmail = (email: string) => {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
  };
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    
    // Client-side validation
    if (!email.trim()) {
      setError('Email adresi gereklidir');
      return;
    }
    
    if (!validateEmail(email)) {
      setError('Geçerli bir email adresi girin');
      return;
    }
    
    setLoading(true);
    try {
      const result = await inviteProjectMember(projectId, {
        email: email.trim().toLowerCase(),
        role
      });
      
      toast.success(`Davet ${email} adresine gönderildi!`);
      onSuccess(result);
      onClose();
      
    } catch (error: any) {
      if (error.response?.status === 400) {
        const message = error.response.data.message || 
                       error.response.data.errors?.Email?.[0];
        
        if (message?.includes('sistemde kayıtlı değil')) {
          setError('Bu email sistemde kayıtlı değil. Kullanıcının önce kayıt olması gerekiyor.');
        } else if (message?.includes('zaten üye')) {
          setError('Bu kullanıcı zaten projenin üyesi.');
        } else {
          setError(message || 'Geçersiz email adresi');
        }
      } else if (error.response?.status === 409) {
        setError('Bu email adresine zaten bekleyen bir davet var.');
      } else if (error.response?.status === 403) {
        setError('Davet gönderme yetkiniz yok. Sadece Owner rolü davet gönderebilir.');
      } else {
        setError('Davet gönderilirken bir hata oluştu. Lütfen tekrar deneyin.');
      }
    } finally {
      setLoading(false);
    }
  };
  
  return (
    <Modal onClose={onClose}>
      <ModalHeader>
        <h3>Projeye Üye Davet Et</h3>
      </ModalHeader>
      
      <ModalBody>
        <Alert variant="info">
          ℹ️ Davet edilecek kişinin sistemde kayıtlı olması gerekir.
        </Alert>
        
        <form onSubmit={handleSubmit}>
          <Input
            type="email"
            label="Email Adresi *"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="ornek@email.com"
            autoFocus
            error={error}
          />
          
          <Select
            label="Rol *"
            value={role.toString()}
            onChange={(e) => setRole(Number(e.target.value) as ProjectRole)}
          >
            <option value={ProjectRole.Viewer}>
              Viewer - Sadece görüntüleme
            </option>
            <option value={ProjectRole.Member}>
              Member - Düzenleme yetkisi
            </option>
            <option value={ProjectRole.Owner}>
              Owner - Tam yetki
            </option>
          </Select>
          
          <InfoBox>
            <h4>Roller Hakkında:</h4>
            <ul>
              <li><strong>Viewer:</strong> Projeyi görüntüleyebilir, değişiklik yapamaz</li>
              <li><strong>Member:</strong> Modül, use case ve task oluşturabilir</li>
              <li><strong>Owner:</strong> Projeyi silebilir, üye ekleyip çıkarabilir</li>
            </ul>
          </InfoBox>
        </form>
      </ModalBody>
      
      <ModalFooter>
        <Button variant="secondary" onClick={onClose}>
          İptal
        </Button>
        <Button 
          variant="primary" 
          onClick={handleSubmit}
          loading={loading}
          disabled={!email || loading}
        >
          {loading ? 'Gönderiliyor...' : 'Davet Gönder'}
        </Button>
      </ModalFooter>
    </Modal>
  );
};
```

##### Adım 3: API servis

```typescript
// services/projectService.ts
interface InviteMemberRequest {
  email: string;
  role: ProjectRole;
}

interface InviteMemberResponse {
  invitationId: string;
  email: string;
  role: number;
  expiresAt: string;
  invitationLink: string;
  message: string;
}

export const inviteProjectMember = async (
  projectId: string,
  data: InviteMemberRequest
): Promise<InviteMemberResponse> => {
  const response = await axios.post(
    `/projects/${projectId}/invitations`,
    data
  );
  return response.data;
};
```

#### 🎨 UI Mockup

```
┌──────────────────────────────────────────┐
│  Projeye Üye Davet Et             [X]    │
├──────────────────────────────────────────┤
│                                          │
│  ℹ️ Davet edilecek kişinin sistemde     │
│     kayıtlı olması gerekir.             │
│                                          │
│  Email Adresi *                          │
│  ┌────────────────────────────────────┐  │
│  │ ornek@email.com                    │  │
│  └────────────────────────────────────┘  │
│                                          │
│  Rol *                                   │
│  ┌────────────────────────────────────┐  │
│  │ Member - Düzenleme yetkisi      ▼│  │
│  └────────────────────────────────────┘  │
│                                          │
│  ┌────────────────────────────────────┐  │
│  │ Roller Hakkında:                   │  │
│  │ • Viewer: Sadece görüntüleme       │  │
│  │ • Member: Düzenleme yetkisi        │  │
│  │ • Owner: Tam yetki                 │  │
│  └────────────────────────────────────┘  │
│                                          │
│            [İptal]  [Davet Gönder]       │
└──────────────────────────────────────────┘

✅ Toast: "Davet ornek@email.com adresine gönderildi!"
```

#### 📧 Email Şablonu (Backend tarafından gönderilir)

```
Konu: Proje Daveti: E-Ticaret Platformu

Merhaba,

John Doe sizi "E-Ticaret Platformu" projesine 
Member rolüyle davet etti.

Daveti kabul etmek için aşağıdaki linke tıklayın:
https://te4it.com/accept-invitation?token=ABC123XYZ

Bu davet 7 gün içinde geçerlidir.

TE4IT Platform
```

---

### 7. Daveti Kabul Etme

**👤 User Story:**  
"Davet edilen kullanıcı olarak, email'imdeki davet linkine tıklayarak projeye katılmak istiyorum."

#### ✅ Başarı Kriterleri
- Kullanıcı davet bilgilerini görür
- Giriş yapar (veya zaten giriş yapmışsa devam eder)
- Davet kabul edilir
- Kullanıcı projeye eklenir
- Proje sayfasına yönlendirilir

#### 🔄 Adım Adım Akış

##### Adım 1: Email'deki linke tıklama

```
URL: https://te4it.com/accept-invitation?token=ABC123XYZ
```

##### Adım 2: Token'ı URL'den alma ve davet bilgilerini çekme

```tsx
// AcceptInvitationPage.tsx
const AcceptInvitationPage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();
  
  const token = searchParams.get('token');
  
  const [invitation, setInvitation] = useState<InvitationDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [accepting, setAccepting] = useState(false);
  
  useEffect(() => {
    if (!token) {
      setError('Geçersiz davet linki');
      setLoading(false);
      return;
    }
    
    loadInvitation();
  }, [token]);
  
  const loadInvitation = async () => {
    try {
      // ⚠️ Bu PUBLIC endpoint - Auth token gerektirmez
      const data = await getInvitationByToken(token!);
      setInvitation(data);
      
      // Kullanıcı zaten giriş yapmışsa otomatik kabul et
      if (isAuthenticated) {
        handleAutoAccept();
      }
    } catch (error: any) {
      if (error.response?.status === 404) {
        setError('Davet bulunamadı veya süresi dolmuş.');
      } else {
        setError('Davet bilgileri yüklenirken hata oluştu.');
      }
    } finally {
      setLoading(false);
    }
  };
  
  const handleAutoAccept = async () => {
    // Email kontrolü
    if (user?.email.toLowerCase() !== invitation?.email.toLowerCase()) {
      setError(
        `Bu davet ${invitation?.email} adresine gönderilmiş. ` +
        `Lütfen o hesapla giriş yapın veya hesap değiştirin.`
      );
      return;
    }
    
    await acceptInvitation();
  };
  
  const acceptInvitation = async () => {
    setAccepting(true);
    try {
      const result = await acceptProjectInvitation(token!);
      
      toast.success(`${invitation!.projectTitle} projesine katıldınız!`);
      navigate(`/projects/${result.projectId}`);
      
    } catch (error: any) {
      if (error.response?.status === 400) {
        const message = error.response.data.message;
        if (message?.includes('zaten kabul edilmiş')) {
          setError('Bu davet zaten kabul edilmiş.');
        } else if (message?.includes('iptal edilmiş')) {
          setError('Bu davet iptal edilmiş.');
        } else {
          setError(message);
        }
      } else if (error.response?.status === 403) {
        setError('Bu davet sizin email adresinize gönderilmemiş.');
      } else if (error.response?.status === 409) {
        setError('Zaten bu projenin üyesisiniz.');
        // Projeye yönlendir
        setTimeout(() => navigate(`/projects/${invitation!.projectId}`), 2000);
      } else {
        setError('Davet kabul edilirken hata oluştu.');
      }
    } finally {
      setAccepting(false);
    }
  };
  
  if (loading) {
    return <LoadingScreen message="Davet bilgileri yükleniyor..." />;
  }
  
  if (error) {
    return (
      <ErrorScreen 
        message={error}
        action="Ana Sayfaya Dön"
        onAction={() => navigate('/')}
      />
    );
  }
  
  if (!invitation) {
    return <ErrorScreen message="Davet bulunamadı" />;
  }
  
  return (
    <Container>
      <Card className="invitation-card">
        <CardHeader>
          <Icon name="mail" size="large" />
          <h2>Proje Daveti</h2>
        </CardHeader>
        
        <CardBody>
          <InvitationInfo>
            <p>
              <strong>{invitation.invitedBy}</strong> sizi
            </p>
            <h3>"{invitation.projectTitle}"</h3>
            <p>
              projesine <Badge>{invitation.roleName}</Badge> rolüyle davet etti.
            </p>
          </InvitationInfo>
          
          <InvitationDetails>
            <DetailItem>
              <Icon name="user" />
              <span>Davet eden: {invitation.invitedByEmail}</span>
            </DetailItem>
            <DetailItem>
              <Icon name="mail" />
              <span>Davet edilen: {invitation.email}</span>
            </DetailItem>
            <DetailItem>
              <Icon name="clock" />
              <span>
                Geçerlilik: {new Date(invitation.expiresAt).toLocaleDateString('tr-TR')}
              </span>
            </DetailItem>
          </InvitationDetails>
          
          {!isAuthenticated ? (
            <Alert variant="info">
              Daveti kabul etmek için giriş yapmanız gerekiyor.
            </Alert>
          ) : user?.email.toLowerCase() !== invitation.email.toLowerCase() ? (
            <Alert variant="warning">
              ⚠️ Bu davet <strong>{invitation.email}</strong> adresine gönderilmiş.
              Şu anda <strong>{user?.email}</strong> hesabıyla giriş yaptınız.
              Lütfen hesap değiştirin veya doğru email ile giriş yapın.
            </Alert>
          ) : null}
        </CardBody>
        
        <CardFooter>
          {!isAuthenticated ? (
            <>
              <Button 
                variant="secondary" 
                onClick={() => navigate('/login', { 
                  state: { returnUrl: `/accept-invitation?token=${token}` }
                })}
              >
                Giriş Yap
              </Button>
              <Button 
                variant="primary" 
                onClick={() => navigate('/register', {
                  state: { returnUrl: `/accept-invitation?token=${token}` }
                })}
              >
                Kayıt Ol
              </Button>
            </>
          ) : user?.email.toLowerCase() !== invitation.email.toLowerCase() ? (
            <>
              <Button 
                variant="secondary" 
                onClick={() => navigate('/')}
              >
                Ana Sayfa
              </Button>
              <Button 
                variant="primary" 
                onClick={() => {
                  // Logout ve login sayfasına yönlendir
                  logout();
                  navigate('/login', {
                    state: { returnUrl: `/accept-invitation?token=${token}` }
                  });
                }}
              >
                Hesap Değiştir
              </Button>
            </>
          ) : (
            <>
              <Button 
                variant="secondary" 
                onClick={() => navigate('/')}
              >
                Reddet
              </Button>
              <Button 
                variant="primary" 
                onClick={acceptInvitation}
                loading={accepting}
              >
                {accepting ? 'Kabul Ediliyor...' : 'Daveti Kabul Et'}
              </Button>
            </>
          )}
        </CardFooter>
      </Card>
    </Container>
  );
};
```

##### Adım 3: API servisleri

```typescript
// services/invitationService.ts

// PUBLIC endpoint - Auth gerektirmez
export const getInvitationByToken = async (
  token: string
): Promise<InvitationDetails> => {
  const response = await axios.get(`/projects/invitations/${token}`);
  return response.data;
};

// AUTH gerektirir
export const acceptProjectInvitation = async (
  token: string
): Promise<AcceptInvitationResponse> => {
  const response = await axios.post(
    `/projects/invitations/${token}/accept`
  );
  return response.data;
};
```

#### 🔄 Sequence Diagram

```
Davet Eden     Frontend     Backend     Email      Davet Alan
    │             │           │          │            │
    ├─"Davet"────>│           │          │            │
    │             ├─POST inv──>│          │            │
    │             │<─201───────┤          │            │
    │             │            ├─Send────>│            │
    │             │            │          ├─Deliver──>│
    │             │            │          │            ├─Click
    │             │            │<─GET/inv/{token}──────┤
    │             │            ├─200 (public)──────────>
    │             │            │          │            ├─Login
    │             │            │<─POST/accept──────────┤
    │             │            ├─200 (+ add member)───>
    │             │            │          │            └─Navigate
```

#### 🎨 UI Mockup

```
┌──────────────────────────────────────────────┐
│              📧 Proje Daveti                  │
├──────────────────────────────────────────────┤
│                                              │
│        John Doe sizi                         │
│                                              │
│     "E-Ticaret Platformu"                    │
│                                              │
│  projesine [Member] rolüyle davet etti.     │
│                                              │
│  ┌────────────────────────────────────────┐  │
│  │ 👤 Davet eden: john@example.com        │  │
│  │ ✉️  Davet edilen: jane@example.com     │  │
│  │ ⏰ Geçerlilik: 25.01.2024              │  │
│  └────────────────────────────────────────┘  │
│                                              │
│  ℹ️ Daveti kabul etmek için giriş yapın     │
│                                              │
│        [Giriş Yap]  [Kayıt Ol]              │
│                                              │
└──────────────────────────────────────────────┘

// GİRİŞ YAPTIKTAN SONRA:
┌──────────────────────────────────────────────┐
│  ...aynı bilgiler...                         │
│                                              │
│        [Reddet]  [Daveti Kabul Et]          │
│                    [⏳ loading]              │
└──────────────────────────────────────────────┘

// KABUL EDİLDİKTEN SONRA:
✅ Toast: "E-Ticaret Platformu projesine katıldınız!"
→ Navigate: /projects/{projectId}
```

#### ⚠️ Edge Cases

1. **Token süresi dolmuş:**
```typescript
if (invitation.status === 'Expired') {
  return (
    <Alert variant="error">
      Bu davet süresi dolmuş. Lütfen proje sahibinden yeni davet isteyin.
    </Alert>
  );
}
```

2. **Davet iptal edilmiş:**
```typescript
if (invitation.status === 'Cancelled') {
  return (
    <Alert variant="warning">
      Bu davet iptal edilmiş.
    </Alert>
  );
}
```

3. **Farklı hesapla giriş yapılmış:**
```typescript
if (user.email !== invitation.email) {
  // Hesap değiştir opsiyonu sun
  <Button onClick={handleSwitchAccount}>Hesap Değiştir</Button>
}
```

4. **Kullanıcı zaten projede:**
```typescript
if (error.status === 409) {
  toast.info('Zaten bu projenin üyesisiniz');
  navigate(`/projects/${invitation.projectId}`);
}
```

---

### 8. Bekleyen Davetleri Görüntüleme

**👤 User Story:**  
"Proje yöneticisi olarak, gönderdiğim ve henüz kabul edilmemiş davetleri görmek istiyorum."

#### 🔄 Adım Adım Akış

```tsx
// InvitationsList.tsx
const InvitationsList = ({ projectId }: { projectId: string }) => {
  const [invitations, setInvitations] = useState<Invitation[]>([]);
  const [filter, setFilter] = useState<'all' | 'Pending' | 'Accepted'>('Pending');
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    loadInvitations();
  }, [projectId, filter]);
  
  const loadInvitations = async () => {
    setLoading(true);
    try {
      const data = await getProjectInvitations(
        projectId, 
        filter === 'all' ? undefined : filter
      );
      setInvitations(data);
    } catch (error) {
      toast.error('Davetler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };
  
  const handleCancelInvitation = async (invitationId: string) => {
    if (!confirm('Bu daveti iptal etmek istediğinize emin misiniz?')) {
      return;
    }
    
    try {
      await cancelInvitation(projectId, invitationId);
      toast.success('Davet iptal edildi');
      loadInvitations();
    } catch (error) {
      toast.error('Davet iptal edilirken hata oluştu');
    }
  };
  
  const handleResendInvitation = async (email: string, role: number) => {
    try {
      await inviteProjectMember(projectId, { email, role });
      toast.success('Davet yeniden gönderildi');
      loadInvitations();
    } catch (error) {
      toast.error('Davet gönderilemedi');
    }
  };
  
  return (
    <div className="invitations-list">
      <FilterBar>
        <Button 
          variant={filter === 'Pending' ? 'primary' : 'secondary'}
          onClick={() => setFilter('Pending')}
        >
          Bekleyen
        </Button>
        <Button 
          variant={filter === 'Accepted' ? 'primary' : 'secondary'}
          onClick={() => setFilter('Accepted')}
        >
          Kabul Edilenler
        </Button>
        <Button 
          variant={filter === 'all' ? 'primary' : 'secondary'}
          onClick={() => setFilter('all')}
        >
          Tümü
        </Button>
      </FilterBar>
      
      {loading ? (
        <LoadingSpinner />
      ) : invitations.length === 0 ? (
        <EmptyState message="Davet bulunamadı" />
      ) : (
        <Table>
          <thead>
            <tr>
              <th>Email</th>
              <th>Rol</th>
              <th>Durum</th>
              <th>Davet Eden</th>
              <th>Tarih</th>
              <th>Geçerlilik</th>
              <th>İşlemler</th>
            </tr>
          </thead>
          <tbody>
            {invitations.map(invitation => (
              <tr key={invitation.invitationId}>
                <td>{invitation.email}</td>
                <td>
                  <Badge>{invitation.roleName}</Badge>
                </td>
                <td>
                  <StatusBadge status={invitation.status} />
                </td>
                <td>{invitation.invitedBy}</td>
                <td>
                  {new Date(invitation.createdAt).toLocaleDateString('tr-TR')}
                </td>
                <td>
                  {invitation.status === 'Pending' && (
                    <Countdown 
                      endDate={invitation.expiresAt}
                      onExpire={loadInvitations}
                    />
                  )}
                  {invitation.status === 'Accepted' && invitation.acceptedAt && (
                    <span>
                      {new Date(invitation.acceptedAt).toLocaleDateString('tr-TR')}
                    </span>
                  )}
                </td>
                <td>
                  {invitation.status === 'Pending' && (
                    <>
                      <IconButton
                        icon="refresh"
                        tooltip="Yeniden Gönder"
                        onClick={() => handleResendInvitation(
                          invitation.email, 
                          invitation.role
                        )}
                      />
                      <IconButton
                        icon="cancel"
                        tooltip="İptal Et"
                        variant="danger"
                        onClick={() => handleCancelInvitation(
                          invitation.invitationId
                        )}
                      />
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </div>
  );
};
```

#### 🎨 UI Mockup

```
┌────────────────────────────────────────────────────────────────────┐
│  Davetler                                                           │
├────────────────────────────────────────────────────────────────────┤
│  [Bekleyen] [Kabul Edilenler] [Tümü]                               │
├────────────────────────────────────────────────────────────────────┤
│  Email            Rol      Durum    Davet Eden  Tarih    İşlemler  │
├────────────────────────────────────────────────────────────────────┤
│  jane@mail.com   [Member] [Pending] John Doe   20.01    [↻] [✕]   │
│  bob@mail.com    [Viewer] [Pending] Jane Smith 19.01    [↻] [✕]   │
│  alice@mail.com  [Owner]  [Accepted] John Doe  18.01    -          │
└────────────────────────────────────────────────────────────────────┘
```

---

*Dokümantasyon devam ediyor... Bir sonraki bölümde Modül, UseCase ve Task yönetimi akışları gelecek.*

**Not:** Bu dokümantasyon user story bazlı olduğu için frontend developer'ların gerçek kullanım senaryolarını görmesini sağlıyor. Her akış için:
- Başarı kriterleri net
- Kod örnekleri hazır (copy-paste edilebilir)
- UI mockup'ları var
- Error handling stratejileri dahil
- Edge case'ler belirtilmiş

Devam edelim mi? 🚀
