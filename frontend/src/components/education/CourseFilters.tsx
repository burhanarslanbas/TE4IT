/**
 * Course Filters Component
 * Kurs filtreleme bileşeni
 */

import { Input } from '../ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../ui/select';
import { Search } from 'lucide-react';
import type { CourseFilters as CourseFiltersType } from '../../types/education';

interface CourseFiltersProps {
  filters: CourseFiltersType;
  onFiltersChange: (filters: CourseFiltersType) => void;
}

export function CourseFilters({ filters, onFiltersChange }: CourseFiltersProps) {
  const handleStatusChange = (status: string) => {
    onFiltersChange({
      ...filters,
      status: status === 'all' ? undefined : (status as 'active' | 'completed'),
    });
  };

  const handleSearchChange = (search: string) => {
    onFiltersChange({
      ...filters,
      search: search || undefined,
    });
  };

  return (
    <div className="flex flex-col sm:flex-row gap-4 mb-6">
      {/* Search Input */}
      <div className="relative flex-1">
        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-[#9CA3AF]" />
        <Input
          type="text"
          placeholder="Kurslarda ara..."
          value={filters.search || ''}
          onChange={(e) => handleSearchChange(e.target.value)}
          className="pl-10 bg-[#161B22]/50 border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#8B5CF6]"
        />
      </div>

      {/* Status Filter */}
      <Select
        value={filters.status || 'all'}
        onValueChange={handleStatusChange}
      >
        <SelectTrigger className="w-full sm:w-[200px] bg-[#161B22]/50 border-[#30363D] text-[#E5E7EB] focus:border-[#8B5CF6]">
          <SelectValue placeholder="Durum" />
        </SelectTrigger>
        <SelectContent className="bg-[#161B22] border-[#30363D]">
          <SelectItem value="all" className="text-[#E5E7EB] focus:bg-[#21262D]">
            Tümü
          </SelectItem>
          <SelectItem value="active" className="text-[#E5E7EB] focus:bg-[#21262D]">
            Devam Eden
          </SelectItem>
          <SelectItem value="completed" className="text-[#E5E7EB] focus:bg-[#21262D]">
            Tamamlanan
          </SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
}

