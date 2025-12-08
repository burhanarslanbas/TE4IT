# Task Management Flows - Frontend Guide

> **Hedef Kitle:** Frontend Developers  
> **Güncelleme:** 2024-12-08  
> **Odak:** Modül, UseCase ve Task yönetimi akışları

Bu dokümantasyon, TE4IT platformunda görev yönetimi (task management) özelliklerini implement ederken ihtiyaç duyacağınız tüm akışları kapsar.

## İçindekiler

### 📦 Modül Yönetimi
1. [Modül Oluşturma](#1-modül-oluşturma)
2. [Modül Listesi ve Filtreleme](#2-modül-listesi-ve-filtreleme)
3. [Modül Düzenleme](#3-modül-düzenleme)
4. [Modül Aktif/Pasif Yapma](#4-modül-aktif-pasif-yapma)

### 📋 UseCase Yönetimi
5. [UseCase Oluşturma](#5-usecase-oluşturma)
6. [UseCase Düzenleme](#6-usecase-düzenleme)
7. [UseCase ile Task İlişkisi](#7-usecase-ile-task-ilişkisi)

### ✅ Task Yönetimi
8. [Task Oluşturma (Opsiyonel Atama)](#8-task-oluşturma)
9. [Kanban Board Implementasyonu](#9-kanban-board-implementasyonu)
10. [Task Durumu Değiştirme (Drag & Drop)](#10-task-durumu-değiştirme)
11. [Task Atama](#11-task-atama)
12. [Task İlişkileri (Dependencies)](#12-task-ilişkileri)
13. [Task Filtreleme ve Arama](#13-task-filtreleme-ve-arama)

---

## 📦 Modül Yönetimi

### 1. Modül Oluşturma

**👤 User Story:**  
"Proje yöneticisi olarak, projemin altına 'Kullanıcı Yönetimi' gibi bir modül eklemek istiyorum."

#### ✅ Başarı Kriterleri
- Modül başarıyla oluşturulur
- Modül listesine eklenir
- İlgili use case'ler eklenebilir hale gelir

#### 🔐 Yetki
- ModuleCreate policy (Owner, Member)

#### 🔄 Adım Adım Akış

##### Proje Detay Sayfasında Modül Ekle

```tsx
// ProjectDetailPage.tsx
const ProjectDetailPage = () => {
  const { projectId } = useParams();
  const [modules, setModules] = useState<Module[]>([]);
  const [showCreateModal, setShowCreateModal] = useState(false);
  
  return (
    <div className="project-detail">
      <Header>
        <h1>Proje: {project.title}</h1>
        <Button onClick={() => setShowCreateModal(true)}>
          + Yeni Modül
        </Button>
      </Header>
      
      <ModulesList modules={modules} />
      
      {showCreateModal && (
        <CreateModuleModal
          projectId={projectId}
          onClose={() => setShowCreateModal(false)}
          onSuccess={(newModule) => {
            setModules([...modules, newModule]);
            setShowCreateModal(false);
          }}
        />
      )}
    </div>
  );
};
```

##### Modül Oluşturma Formu

```tsx
// CreateModuleModal.tsx
const CreateModuleModal = ({ projectId, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    title: '',
    description: ''
  });
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  
  const validate = () => {
    const newErrors: Record<string, string> = {};
    
    if (formData.title.trim().length < 3) {
      newErrors.title = 'Başlık en az 3 karakter olmalıdır';
    }
    if (formData.title.length > 100) {
      newErrors.title = 'Başlık en fazla 100 karakter olabilir';
    }
    if (formData.description && formData.description.length > 1000) {
      newErrors.description = 'Açıklama en fazla 1000 karakter olabilir';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validate()) return;
    
    setLoading(true);
    try {
      const newModule = await createModule(projectId, {
        title: formData.title.trim(),
        description: formData.description?.trim()
      });
      
      toast.success('Modül oluşturuldu!');
      onSuccess(newModule);
      
    } catch (error: any) {
      if (error.response?.status === 403) {
        toast.error('Modül oluşturma yetkiniz yok');
      } else if (error.response?.status === 404) {
        toast.error('Proje bulunamadı');
      } else {
        toast.error('Modül oluşturulurken hata oluştu');
      }
    } finally {
      setLoading(false);
    }
  };
  
  return (
    <Modal onClose={onClose}>
      <ModalHeader>Yeni Modül Oluştur</ModalHeader>
      <ModalBody>
        <form onSubmit={handleSubmit}>
          <Input
            label="Modül Adı *"
            value={formData.title}
            onChange={(e) => setFormData({...formData, title: e.target.value})}
            error={errors.title}
            maxLength={100}
            placeholder="örn: Kullanıcı Yönetimi"
            autoFocus
          />
          <small>{formData.title.length}/100 karakter</small>
          
          <Textarea
            label="Açıklama"
            value={formData.description}
            onChange={(e) => setFormData({...formData, description: e.target.value})}
            error={errors.description}
            maxLength={1000}
            rows={4}
            placeholder="Modül hakkında detaylı bilgi..."
          />
          <small>{formData.description.length}/1000 karakter</small>
        </form>
      </ModalBody>
      <ModalFooter>
        <Button variant="secondary" onClick={onClose}>İptal</Button>
        <Button variant="primary" onClick={handleSubmit} loading={loading}>
          Oluştur
        </Button>
      </ModalFooter>
    </Modal>
  );
};
```

##### API Servis

```typescript
// services/moduleService.ts
export const createModule = async (
  projectId: string,
  data: { title: string; description?: string }
): Promise<Module> => {
  const response = await axios.post(
    `/modules/projects/${projectId}`,
    data
  );
  return response.data;
};
```

#### 🎨 UI Mockup

```
┌──────────────────────────────────────┐
│  Yeni Modül Oluştur           [X]    │
├──────────────────────────────────────┤
│  Modül Adı *                         │
│  ┌────────────────────────────────┐  │
│  │ Kullanıcı Yönetimi             │  │
│  └────────────────────────────────┘  │
│  18/100 karakter                     │
│                                      │
│  Açıklama                            │
│  ┌────────────────────────────────┐  │
│  │ Kullanıcı kayıt, giriş ve      │  │
│  │ profil yönetimi işlemleri      │  │
│  └────────────────────────────────┘  │
│  45/1000 karakter                    │
│                                      │
│          [İptal]  [Oluştur]         │
└──────────────────────────────────────┘
```

---

## ✅ Task Yönetimi

### 8. Task Oluşturma

**👤 User Story:**  
"Kullanıcı olarak, use case altında bir task oluşturmak ve isterisem bir kişiye atamak istiyorum."

#### ⚠️ ÖNEMLİ: Opsiyonel Atama
Task oluşturulurken **assigneeId opsiyoneldir**. İki yol:
1. Task oluştururken atama yapabilirsiniz
2. Task'ı sonradan atayabilirsiniz (AssignTask endpoint)

#### 🔄 Adım Adım Akış

##### Task Oluşturma Formu

```tsx
// CreateTaskModal.tsx
import { TaskType, TaskState } from '@/types/task';

const CreateTaskModal = ({ useCaseId, onClose, onSuccess }) => {
  const [formData, setFormData] = useState({
    title: '',
    taskType: TaskType.Feature,
    description: '',
    importantNotes: '',
    dueDate: null as Date | null,
    assigneeId: null as string | null
  });
  
  const [projectMembers, setProjectMembers] = useState<ProjectMember[]>([]);
  const [loading, setLoading] = useState(false);
  
  useEffect(() => {
    // Proje üyelerini yükle (atama için)
    loadProjectMembers();
  }, []);
  
  const loadProjectMembers = async () => {
    try {
      const members = await getProjectMembers(projectId);
      setProjectMembers(members);
    } catch (error) {
      console.error('Üyeler yüklenemedi:', error);
    }
  };
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validate()) return;
    
    setLoading(true);
    try {
      const newTask = await createTask(useCaseId, {
        title: formData.title.trim(),
        taskType: formData.taskType,
        description: formData.description?.trim(),
        importantNotes: formData.importantNotes?.trim(),
        dueDate: formData.dueDate?.toISOString(),
        assigneeId: formData.assigneeId // null olabilir!
      });
      
      toast.success(
        formData.assigneeId 
          ? 'Task oluşturuldu ve atandı!' 
          : 'Task oluşturuldu! (Atanmamış)'
      );
      
      onSuccess(newTask);
      
    } catch (error: any) {
      handleError(error);
    } finally {
      setLoading(false);
    }
  };
  
  return (
    <Modal onClose={onClose} size="large">
      <ModalHeader>Yeni Task Oluştur</ModalHeader>
      
      <ModalBody>
        <form onSubmit={handleSubmit}>
          {/* Başlık */}
          <Input
            label="Task Başlığı *"
            value={formData.title}
            onChange={(e) => setFormData({...formData, title: e.target.value})}
            maxLength={200}
            placeholder="örn: Kayıt formu tasarımı"
          />
          
          {/* Task Tipi */}
          <Select
            label="Task Tipi *"
            value={formData.taskType.toString()}
            onChange={(e) => setFormData({
              ...formData, 
              taskType: Number(e.target.value) as TaskType
            })}
          >
            <option value={TaskType.Documentation}>
              📄 Documentation - Dokümantasyon
            </option>
            <option value={TaskType.Feature}>
              ✨ Feature - Özellik Geliştirme
            </option>
            <option value={TaskType.Test}>
              🧪 Test - Test Yazma
            </option>
            <option value={TaskType.Bug}>
              🐛 Bug - Hata Düzeltme
            </option>
          </Select>
          
          {/* Açıklama */}
          <Textarea
            label="Açıklama"
            value={formData.description}
            onChange={(e) => setFormData({...formData, description: e.target.value})}
            maxLength={2000}
            rows={4}
            placeholder="Task hakkında detaylı bilgi..."
          />
          
          {/* Önemli Notlar */}
          <Textarea
            label="Önemli Notlar"
            value={formData.importantNotes}
            onChange={(e) => setFormData({...formData, importantNotes: e.target.value})}
            maxLength={1000}
            rows={3}
            placeholder="Business rules, kısıtlamalar, dikkat edilmesi gerekenler..."
          />
          
          {/* Due Date */}
          <DatePicker
            label="Bitiş Tarihi"
            value={formData.dueDate}
            onChange={(date) => setFormData({...formData, dueDate: date})}
            minDate={new Date()}
          />
          
          {/* Atama (Opsiyonel) */}
          <Select
            label="Atama (Opsiyonel)"
            value={formData.assigneeId || ''}
            onChange={(e) => setFormData({
              ...formData, 
              assigneeId: e.target.value || null
            })}
          >
            <option value="">-- Atanmamış (Sonra ata) --</option>
            {projectMembers.map(member => (
              <option key={member.userId} value={member.userId}>
                {member.fullName} ({member.roleName})
              </option>
            ))}
          </Select>
          
          {!formData.assigneeId && (
            <Alert variant="info">
              💡 Task'ı atamasız oluşturabilir, sonra "Task Ata" ile atayabilirsiniz.
            </Alert>
          )}
        </form>
      </ModalBody>
      
      <ModalFooter>
        <Button variant="secondary" onClick={onClose}>İptal</Button>
        <Button variant="primary" onClick={handleSubmit} loading={loading}>
          Oluştur
        </Button>
      </ModalFooter>
    </Modal>
  );
};
```

##### API Servis

```typescript
// services/taskService.ts
export enum TaskType {
  Documentation = 1,
  Feature = 2,
  Test = 3,
  Bug = 4
}

export enum TaskState {
  NotStarted = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}

export const createTask = async (
  useCaseId: string,
  data: {
    title: string;
    taskType: TaskType;
    description?: string;
    importantNotes?: string;
    dueDate?: string;
    assigneeId?: string | null; // NULLABLE!
  }
): Promise<Task> => {
  const response = await axios.post(
    `/tasks/usecases/${useCaseId}`,
    data
  );
  return response.data;
};
```

---

### 9. Kanban Board Implementasyonu

**👤 User Story:**  
"Kullanıcı olarak, task'ları Kanban board üzerinde görmek ve drag&drop ile durumlarını değiştirmek istiyorum."

#### 🔄 Implementasyon

```tsx
// KanbanBoard.tsx
import { DndContext, DragEndEvent, DragOverlay } from '@dnd-kit/core';
import { useState, useEffect } from 'react';

const KanbanBoard = ({ useCaseId }: { useCaseId: string }) => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeDragTask, setActiveDragTask] = useState<Task | null>(null);
  
  const columns = [
    { id: TaskState.NotStarted, title: '📋 Yapılacak', color: '#gray' },
    { id: TaskState.InProgress, title: '🚀 Devam Ediyor', color: '#blue' },
    { id: TaskState.Completed, title: '✅ Tamamlandı', color: '#green' },
    { id: TaskState.Cancelled, title: '❌ İptal', color: '#red' }
  ];
  
  useEffect(() => {
    loadTasks();
  }, [useCaseId]);
  
  const loadTasks = async () => {
    setLoading(true);
    try {
      const result = await getTasks(useCaseId, {
        page: 1,
        pageSize: 100 // Kanban için tüm task'ları çek
      });
      setTasks(result.items);
    } catch (error) {
      toast.error('Task\'lar yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };
  
  const handleDragStart = (event: any) => {
    const task = tasks.find(t => t.id === event.active.id);
    setActiveDragTask(task || null);
  };
  
  const handleDragEnd = async (event: DragEndEvent) => {
    setActiveDragTask(null);
    
    const { active, over } = event;
    
    if (!over) return;
    
    const taskId = active.id as string;
    const newState = Number(over.id) as TaskState;
    
    const task = tasks.find(t => t.id === taskId);
    if (!task || task.taskState === newState) return;
    
    // Optimistic update
    setTasks(prevTasks => 
      prevTasks.map(t => 
        t.id === taskId 
          ? { ...t, taskState: newState }
          : t
      )
    );
    
    try {
      await changeTaskState(taskId, newState);
      toast.success(`Task "${task.title}" durumu güncellendi`);
    } catch (error) {
      // Rollback on error
      setTasks(prevTasks => 
        prevTasks.map(t => 
          t.id === taskId 
            ? { ...t, taskState: task.taskState }
            : t
        )
      );
      toast.error('Task durumu değiştirilemedi');
    }
  };
  
  const getTasksByState = (state: TaskState) => {
    return tasks.filter(task => task.taskState === state);
  };
  
  if (loading) {
    return <LoadingSpinner />;
  }
  
  return (
    <DndContext 
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
    >
      <div className="kanban-board">
        {columns.map(column => (
          <KanbanColumn
            key={column.id}
            column={column}
            tasks={getTasksByState(column.id)}
          />
        ))}
      </div>
      
      {/* Drag overlay */}
      <DragOverlay>
        {activeDragTask && (
          <TaskCard task={activeDragTask} isDragging />
        )}
      </DragOverlay>
    </DndContext>
  );
};

// Kanban Column Component
const KanbanColumn = ({ column, tasks }) => {
  const { setNodeRef } = useDroppable({
    id: column.id
  });
  
  return (
    <div 
      ref={setNodeRef}
      className="kanban-column"
      style={{ borderTop: `4px solid ${column.color}` }}
    >
      <div className="column-header">
        <h3>{column.title}</h3>
        <Badge>{tasks.length}</Badge>
      </div>
      
      <div className="column-body">
        {tasks.length === 0 ? (
          <EmptyState message="Task yok" />
        ) : (
          tasks.map(task => (
            <DraggableTaskCard key={task.id} task={task} />
          ))
        )}
      </div>
    </div>
  );
};

// Draggable Task Card
const DraggableTaskCard = ({ task }) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging
  } = useDraggable({
    id: task.id
  });
  
  const style = {
    transform: transform 
      ? `translate3d(${transform.x}px, ${transform.y}px, 0)` 
      : undefined,
    transition,
    opacity: isDragging ? 0.5 : 1
  };
  
  return (
    <div
      ref={setNodeRef}
      style={style}
      {...listeners}
      {...attributes}
    >
      <TaskCard task={task} />
    </div>
  );
};

// Task Card Component
const TaskCard = ({ task, isDragging = false }) => {
  const navigate = useNavigate();
  
  const getTaskTypeIcon = (type: TaskType) => {
    switch (type) {
      case TaskType.Documentation: return '📄';
      case TaskType.Feature: return '✨';
      case TaskType.Test: return '🧪';
      case TaskType.Bug: return '🐛';
    }
  };
  
  const getTaskTypeColor = (type: TaskType) => {
    switch (type) {
      case TaskType.Documentation: return '#purple';
      case TaskType.Feature: return '#blue';
      case TaskType.Test: return '#green';
      case TaskType.Bug: return '#red';
    }
  };
  
  return (
    <Card 
      className={`task-card ${isDragging ? 'dragging' : ''}`}
      onClick={() => navigate(`/tasks/${task.id}`)}
    >
      <div className="task-header">
        <Badge 
          style={{ backgroundColor: getTaskTypeColor(task.taskType) }}
        >
          {getTaskTypeIcon(task.taskType)} {TaskType[task.taskType]}
        </Badge>
        {task.dueDate && (
          <DueDateBadge 
            dueDate={task.dueDate}
            isOverdue={new Date(task.dueDate) < new Date()}
          />
        )}
      </div>
      
      <h4 className="task-title">{task.title}</h4>
      
      {task.description && (
        <p className="task-description">
          {task.description.substring(0, 100)}
          {task.description.length > 100 && '...'}
        </p>
      )}
      
      <div className="task-footer">
        {task.assigneeId ? (
          <Avatar 
            name={task.assigneeName} 
            size="small"
            tooltip={task.assigneeName}
          />
        ) : (
          <Tooltip content="Atanmamış">
            <div className="unassigned-avatar">👤</div>
          </Tooltip>
        )}
        
        {task.relations && task.relations.length > 0 && (
          <Tooltip content={`${task.relations.length} ilişki`}>
            <span className="relations-badge">
              🔗 {task.relations.length}
            </span>
          </Tooltip>
        )}
      </div>
    </Card>
  );
};
```

#### 🎨 Kanban Board UI

```
┌────────────────────────────────────────────────────────────────────┐
│  📋 Yapılacak(5)  🚀 Devam(3)  ✅ Tamamlandı(12)  ❌ İptal(1)     │
├──────────────────┬──────────────┬──────────────────┬──────────────┤
│ ┌──────────────┐ │┌──────────────┐│┌──────────────┐│┌──────────┐  │
│ │ ✨ Feature   │ ││ 📄 Docs      ││ 🧪 Test      ││ 🐛 Bug   │  │
│ │ Kayıt formu  │ ││ API dökümant.││ Unit test    ││ Login fix│  │
│ │ tasarımı     │ ││              ││              ││          │  │
│ │ 👤 Jane      │ ││ 👤 Bob       ││ 👤 Alice     ││ 👤 John  │  │
│ │ 📅 25.01     │ ││ 📅 24.01     ││ ✅ Done      ││ ❌ Cancel│  │
│ └──────────────┘ │└──────────────┘│└──────────────┘│└──────────┘  │
│ ┌──────────────┐ │┌──────────────┐│                │              │
│ │ 🐛 Bug       │ ││ ✨ Feature   ││                │              │
│ │ Email bug    │ ││ Backend API  ││                │              │
│ │ 👤 Unassigned│ ││ 👤 Jane      ││                │              │
│ └──────────────┘ │└──────────────┘│                │              │
└──────────────────┴──────────────┴──────────────────┴──────────────┘
```

---

### 10. Task Durumu Değiştirme

**API Endpoint:**
```
PATCH /api/v1/tasks/{taskId}/state
Body: { "newState": 2 }
```

**Durum Geçişleri:**

| Mevcut | İzin Verilen Geçişler |
|--------|----------------------|
| NotStarted (1) | → InProgress (2), Cancelled (4) |
| InProgress (2) | → Completed (3), Cancelled (4), NotStarted (1) |
| Completed (3) | → InProgress (2) |
| Cancelled (4) | → NotStarted (1), InProgress (2) |

```typescript
export const changeTaskState = async (
  taskId: string,
  newState: TaskState
): Promise<void> => {
  await axios.patch(`/tasks/${taskId}/state`, { newState });
};
```

---

### 11. Task Atama

**👤 User Story:**  
"Proje yöneticisi olarak, atamasız bir task'ı bir üyeye atamak istiyorum."

```tsx
// AssignTaskModal.tsx
const AssignTaskModal = ({ task, onClose, onSuccess }) => {
  const [selectedUserId, setSelectedUserId] = useState<string>('');
  const [projectMembers, setProjectMembers] = useState<ProjectMember[]>([]);
  const [loading, setLoading] = useState(false);
  
  useEffect(() => {
    loadProjectMembers();
  }, []);
  
  const handleAssign = async () => {
    if (!selectedUserId) {
      toast.error('Lütfen bir kullanıcı seçin');
      return;
    }
    
    setLoading(true);
    try {
      await assignTask(task.id, selectedUserId);
      toast.success('Task atandı!');
      onSuccess();
    } catch (error) {
      toast.error('Task atanamadı');
    } finally {
      setLoading(false);
    }
  };
  
  return (
    <Modal onClose={onClose}>
      <ModalHeader>Task Ata: {task.title}</ModalHeader>
      <ModalBody>
        <Select
          label="Kullanıcı Seç"
          value={selectedUserId}
          onChange={(e) => setSelectedUserId(e.target.value)}
        >
          <option value="">-- Kullanıcı Seçin --</option>
          {projectMembers.map(member => (
            <option key={member.userId} value={member.userId}>
              {member.fullName} ({member.roleName})
            </option>
          ))}
        </Select>
      </ModalBody>
      <ModalFooter>
        <Button variant="secondary" onClick={onClose}>İptal</Button>
        <Button variant="primary" onClick={handleAssign} loading={loading}>
          Ata
        </Button>
      </ModalFooter>
    </Modal>
  );
};
```

**API:**
```typescript
export const assignTask = async (
  taskId: string,
  assigneeId: string
): Promise<void> => {
  await axios.post(`/tasks/${taskId}/assign`, { assigneeId });
};
```

---

### 12. Task İlişkileri

**👤 User Story:**  
"Geliştiriciolarak, task'lar arasında bağımlılık tanımlamak istiyorum."

#### İlişki Tipleri

| Tip | Değer | Açıklama | Örnek |
|-----|-------|----------|-------|
| **Blocks** | 1 | Bu task, diğer task'ı bloklar | "API geliştirme" → "Frontend entegrasyonu" |
| **RelatesTo** | 2 | İlişkili ancak bloklamaz | "Kayıt API" ↔ "Email servisi" |
| **Fixes** | 3 | Bu task bir bug'ı düzeltiyor | "Login fix" → "Login çalışmıyor" |
| **Duplicates** | 4 | Task tekrardır | "Kayıt formu 1" = "Kayıt formu 2" |

#### İlişki Oluşturma

```tsx
// CreateTaskRelationModal.tsx
const CreateTaskRelationModal = ({ 
  sourceTask, 
  availableTasks, 
  onClose, 
  onSuccess 
}) => {
  const [targetTaskId, setTargetTaskId] = useState('');
  const [relationType, setRelationType] = useState(TaskRelationType.Blocks);
  const [loading, setLoading] = useState(false);
  
  const handleCreate = async () => {
    if (!targetTaskId) {
      toast.error('Lütfen bir task seçin');
      return;
    }
    
    setLoading(true);
    try {
      await createTaskRelation(sourceTask.id, {
        targetTaskId,
        relationType
      });
      
      toast.success('İlişki oluşturuldu!');
      onSuccess();
      
    } catch (error: any) {
      if (error.response?.status === 400) {
        if (error.response.data.message?.includes('aynı ilişki')) {
          toast.error('Bu ilişki zaten var');
        } else if (error.response.data.message?.includes('kendisiyle')) {
          toast.error('Task kendisiyle ilişkilendirilemez');
        }
      } else {
        toast.error('İlişki oluşturulamadı');
      }
    } finally {
      setLoading(false);
    }
  };
  
  return (
    <Modal onClose={onClose}>
      <ModalHeader>
        Task İlişkisi Oluştur
      </ModalHeader>
      
      <ModalBody>
        <Alert variant="info">
          Kaynak Task: <strong>{sourceTask.title}</strong>
        </Alert>
        
        <Select
          label="Hedef Task"
          value={targetTaskId}
          onChange={(e) => setTargetTaskId(e.target.value)}
        >
          <option value="">-- Task Seçin --</option>
          {availableTasks
            .filter(t => t.id !== sourceTask.id)
            .map(task => (
              <option key={task.id} value={task.id}>
                {task.title}
              </option>
            ))
          }
        </Select>
        
        <Select
          label="İlişki Tipi"
          value={relationType.toString()}
          onChange={(e) => setRelationType(Number(e.target.value))}
        >
          <option value={TaskRelationType.Blocks}>
            🚫 Blocks - Bu task diğerini bloklar
          </option>
          <option value={TaskRelationType.RelatesTo}>
            🔗 Relates To - İlişkili
          </option>
          <option value={TaskRelationType.Fixes}>
            🔧 Fixes - Bu task diğer bug'ı düzeltir
          </option>
          <option value={TaskRelationType.Duplicates}>
            📋 Duplicates - Tekrar
          </option>
        </Select>
        
        <InfoBox>
          <h4>İlişki Tipleri:</h4>
          <ul>
            <li><strong>Blocks:</strong> Kritik bağımlılık</li>
            <li><strong>Relates To:</strong> Gevşek ilişki</li>
            <li><strong>Fixes:</strong> Bug-fix çifti</li>
            <li><strong>Duplicates:</strong> Aynı task</li>
          </ul>
        </InfoBox>
      </ModalBody>
      
      <ModalFooter>
        <Button variant="secondary" onClick={onClose}>İptal</Button>
        <Button variant="primary" onClick={handleCreate} loading={loading}>
          Oluştur
        </Button>
      </ModalFooter>
    </Modal>
  );
};
```

#### İlişkileri Görselleştirme

```tsx
// TaskRelationsView.tsx
const TaskRelationsView = ({ task }: { task: Task }) => {
  const [relations, setRelations] = useState<TaskRelation[]>([]);
  
  useEffect(() => {
    loadRelations();
  }, [task.id]);
  
  const loadRelations = async () => {
    try {
      const data = await getTaskRelations(task.id);
      setRelations(data);
    } catch (error) {
      toast.error('İlişkiler yüklenemedi');
    }
  };
  
  const handleDeleteRelation = async (relationId: string) => {
    if (!confirm('İlişkiyi silmek istediğinize emin misiniz?')) return;
    
    try {
      await deleteTaskRelation(task.id, relationId);
      toast.success('İlişki silindi');
      loadRelations();
    } catch (error) {
      toast.error('İlişki silinemedi');
    }
  };
  
  const getRelationIcon = (type: TaskRelationType) => {
    switch (type) {
      case TaskRelationType.Blocks: return '🚫';
      case TaskRelationType.RelatesTo: return '🔗';
      case TaskRelationType.Fixes: return '🔧';
      case TaskRelationType.Duplicates: return '📋';
    }
  };
  
  return (
    <div className="task-relations">
      <SectionHeader>
        <h3>İlişkiler ({relations.length})</h3>
        <Button size="small" onClick={() => setShowCreateModal(true)}>
          + İlişki Ekle
        </Button>
      </SectionHeader>
      
      {relations.length === 0 ? (
        <EmptyState message="Henüz ilişki yok" />
      ) : (
        <RelationsList>
          {relations.map(relation => (
            <RelationItem key={relation.id}>
              <span className="relation-icon">
                {getRelationIcon(relation.relationType)}
              </span>
              <span className="relation-type">
                {TaskRelationType[relation.relationType]}
              </span>
              <ArrowIcon>→</ArrowIcon>
              <Link to={`/tasks/${relation.targetTaskId}`}>
                {relation.targetTaskTitle}
              </Link>
              <IconButton
                icon="delete"
                variant="danger"
                size="small"
                onClick={() => handleDeleteRelation(relation.id)}
              />
            </RelationItem>
          ))}
        </RelationsList>
      )}
    </div>
  );
};
```

**API:**
```typescript
export const createTaskRelation = async (
  taskId: string,
  data: {
    targetTaskId: string;
    relationType: TaskRelationType;
  }
): Promise<void> => {
  await axios.post(`/tasks/${taskId}/relations`, data);
};

export const getTaskRelations = async (
  taskId: string
): Promise<TaskRelation[]> => {
  const response = await axios.get(`/tasks/${taskId}/relations`);
  return response.data;
};

export const deleteTaskRelation = async (
  taskId: string,
  relationId: string
): Promise<void> => {
  await axios.delete(`/tasks/${taskId}/relations/${relationId}`);
};
```

---

### 13. Task Filtreleme ve Arama

**👤 User Story:**  
"Kullanıcı olarak, task'ları durum, tip, atanan kişi ve tarihe göre filtrelemek istiyorum."

```tsx
// TaskFilterBar.tsx
const TaskFilterBar = ({ onFilterChange }) => {
  const [filters, setFilters] = useState({
    state: undefined as TaskState | undefined,
    type: undefined as TaskType | undefined,
    assigneeId: undefined as string | undefined,
    dueDateFrom: undefined as Date | undefined,
    dueDateTo: undefined as Date | undefined,
    search: ''
  });
  
  const debouncedSearch = useDebounce(filters.search, 300);
  
  useEffect(() => {
    onFilterChange({
      ...filters,
      search: debouncedSearch
    });
  }, [
    filters.state,
    filters.type,
    filters.assigneeId,
    filters.dueDateFrom,
    filters.dueDateTo,
    debouncedSearch
  ]);
  
  const clearFilters = () => {
    setFilters({
      state: undefined,
      type: undefined,
      assigneeId: undefined,
      dueDateFrom: undefined,
      dueDateTo: undefined,
      search: ''
    });
  };
  
  return (
    <div className="task-filter-bar">
      <SearchInput
        value={filters.search}
        onChange={(value) => setFilters({...filters, search: value})}
        placeholder="Task ara..."
      />
      
      <Select
        value={filters.state?.toString() || ''}
        onChange={(e) => setFilters({
          ...filters,
          state: e.target.value ? Number(e.target.value) as TaskState : undefined
        })}
      >
        <option value="">Tüm Durumlar</option>
        <option value={TaskState.NotStarted}>📋 Başlanmadı</option>
        <option value={TaskState.InProgress}>🚀 Devam Ediyor</option>
        <option value={TaskState.Completed}>✅ Tamamlandı</option>
        <option value={TaskState.Cancelled}>❌ İptal</option>
      </Select>
      
      <Select
        value={filters.type?.toString() || ''}
        onChange={(e) => setFilters({
          ...filters,
          type: e.target.value ? Number(e.target.value) as TaskType : undefined
        })}
      >
        <option value="">Tüm Tipler</option>
        <option value={TaskType.Documentation}>📄 Documentation</option>
        <option value={TaskType.Feature}>✨ Feature</option>
        <option value={TaskType.Test}>🧪 Test</option>
        <option value={TaskType.Bug}>🐛 Bug</option>
      </Select>
      
      <UserSelect
        value={filters.assigneeId}
        onChange={(userId) => setFilters({...filters, assigneeId: userId})}
        placeholder="Atanan kişi..."
      />
      
      <DateRangePicker
        from={filters.dueDateFrom}
        to={filters.dueDateTo}
        onChange={(from, to) => setFilters({
          ...filters,
          dueDateFrom: from,
          dueDateTo: to
        })}
        placeholder="Bitiş tarihi aralığı..."
      />
      
      <Button variant="secondary" onClick={clearFilters}>
        ✕ Filtreleri Temizle
      </Button>
    </div>
  );
};
```

---

## 📊 Özet: Task Management Best Practices

### 1. State Management
```typescript
// Zustand Store Örneği
const useTaskStore = create<TaskStore>((set) => ({
  tasks: [],
  filters: {},
  
  setTasks: (tasks) => set({ tasks }),
  addTask: (task) => set((state) => ({ 
    tasks: [...state.tasks, task] 
  })),
  updateTask: (id, updates) => set((state) => ({
    tasks: state.tasks.map(t => t.id === id ? { ...t, ...updates } : t)
  })),
  removeTask: (id) => set((state) => ({
    tasks: state.tasks.filter(t => t.id !== id)
  }))
}));
```

### 2. Optimistic Updates
```typescript
// Optimistic update pattern
const handleTaskStateChange = async (taskId, newState) => {
  const oldState = tasks.find(t => t.id === taskId)?.taskState;
  
  // 1. Önce UI'ı güncelle
  updateTaskInUI(taskId, { taskState: newState });
  
  try {
    // 2. API çağrısı
    await changeTaskState(taskId, newState);
  } catch (error) {
    // 3. Hata olursa geri al
    updateTaskInUI(taskId, { taskState: oldState });
    toast.error('İşlem başarısız');
  }
};
```

### 3. Real-time Updates (SignalR)
```typescript
// SignalR connection
const connection = new HubConnectionBuilder()
  .withUrl('/hubs/tasks')
  .build();

connection.on('TaskUpdated', (task: Task) => {
  updateTask(task.id, task);
  toast.info(`${task.title} güncellendi`);
});

connection.on('TaskStateChanged', ({ taskId, newState }) => {
  updateTask(taskId, { taskState: newState });
});
```

### 4. Performance Optimization
```typescript
// React.memo ile gereksiz re-render'ları önle
const TaskCard = React.memo(({ task }) => {
  // ...
}, (prevProps, nextProps) => {
  return prevProps.task.id === nextProps.task.id &&
         prevProps.task.taskState === nextProps.task.taskState;
});

// Virtualization for large lists
import { FixedSizeList } from 'react-window';

const TaskList = ({ tasks }) => {
  return (
    <FixedSizeList
      height={600}
      itemCount={tasks.length}
      itemSize={80}
      width="100%"
    >
      {({ index, style }) => (
        <div style={style}>
          <TaskCard task={tasks[index]} />
        </div>
      )}
    </FixedSizeList>
  );
};
```

---

**Sonraki Dokümantasyon:** Common Patterns (Auth, Error Handling, Pagination) 🚀
