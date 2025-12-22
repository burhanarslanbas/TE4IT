/**
 * Trainings Page
 * Route: /trainings
 * Eğitimler listesi sayfası - CoursesListPage'e yönlendirme yapar
 * Not: Bu sayfa artık CoursesListPage'i render ediyor (backward compatibility için)
 */
import { CoursesListPage } from './CoursesListPage';

export function TrainingsPage() {
  // Backward compatibility için CoursesListPage'i render et
  return <CoursesListPage />;
}
