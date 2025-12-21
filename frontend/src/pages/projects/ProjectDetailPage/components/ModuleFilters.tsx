/**
 * Module Filters Component
 * Modül arama ve filtreleme
 */
import { Input } from '../../../../components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../../../../components/ui/select';
import { Search, Filter } from 'lucide-react';
import type { ModuleFilters } from '../../../../types';

interface ModuleFiltersProps {
  filters: ModuleFilters;
  onSearchChange: (value: string) => void;
  onStatusChange: (value: string) => void;
}

export function ModuleFiltersComponent({
  filters,
  onSearchChange,
  onStatusChange,
}: ModuleFiltersProps) {
  return (
    <div className="flex flex-col sm:flex-row gap-3 w-full lg:w-auto">
      {/* Search Input */}
      <div className="relative flex-1 lg:flex-initial lg:w-80">
        <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-[#6B7280] w-4 h-4 pointer-events-none" />
        <Input
          placeholder="Modül ara..."
          value={filters.search || ''}
          onChange={(e) => onSearchChange(e.target.value)}
          className="pl-11 pr-4 bg-[#0D1117]/60 backdrop-blur-sm border-[#30363D]/80 text-[#E5E7EB] placeholder:text-[#6B7280] focus:border-[#8B5CF6] focus:ring-1 focus:ring-[#8B5CF6]/30 transition-all h-10 rounded-lg"
        />
      </div>
      
      {/* Filter Select */}
      <Select value={filters.status || 'All'} onValueChange={onStatusChange}>
        <SelectTrigger className="w-full sm:w-[140px] bg-[#0D1117]/60 backdrop-blur-sm border-[#30363D]/80 text-[#E5E7EB] h-10 rounded-lg">
          <Filter className="w-4 h-4 mr-2 text-[#6B7280]" />
          <SelectValue placeholder="Filtre" />
        </SelectTrigger>
        <SelectContent className="bg-[#161B22] border-[#30363D]">
          <SelectItem value="All">Tümü</SelectItem>
          <SelectItem value="Active">Aktif</SelectItem>
          <SelectItem value="Archived">Arşivlenmiş</SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
}

