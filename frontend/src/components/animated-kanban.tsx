import { motion } from "motion/react";
import { useState, useEffect } from "react";

interface Task {
  id: string;
  title: string;
  column: 'todo' | 'doing' | 'done';
}

export function AnimatedKanban() {
  const [tasks, setTasks] = useState<Task[]>([
    { id: '1', title: 'Design System', column: 'todo' },
    { id: '2', title: 'User Research', column: 'doing' },
    { id: '3', title: 'Prototype', column: 'done' }
  ]);

  useEffect(() => {
    const interval = setInterval(() => {
      setTasks(prevTasks => {
        const newTasks = [...prevTasks];
        const todoTasks = newTasks.filter(t => t.column === 'todo');
        const doingTasks = newTasks.filter(t => t.column === 'doing');
        
        if (todoTasks.length > 0) {
          todoTasks[0].column = 'doing';
        } else if (doingTasks.length > 0) {
          doingTasks[0].column = 'done';
          // Reset after completion
          setTimeout(() => {
            setTasks([
              { id: '1', title: 'Design System', column: 'todo' },
              { id: '2', title: 'User Research', column: 'doing' },
              { id: '3', title: 'Prototype', column: 'done' }
            ]);
          }, 2000);
        }
        
        return newTasks;
      });
    }, 3000);

    return () => clearInterval(interval);
  }, []);

  const columns = [
    { id: 'todo', title: 'To Do', color: '#9CA3AF' },
    { id: 'doing', title: 'Doing', color: '#8B5CF6' },
    { id: 'done', title: 'Done', color: '#2DD4BF' }
  ];

  return (
    <div className="w-full max-w-4xl mx-auto p-6 bg-[#161B22] rounded-2xl border border-[#30363D] shadow-2xl">
      <div className="grid grid-cols-3 gap-6">
        {columns.map(column => (
          <div key={column.id} className="space-y-4">
            <div className="flex items-center space-x-2">
              <div 
                className="w-3 h-3 rounded-full"
                style={{ backgroundColor: column.color }}
              />
              <h4 className="text-[#E5E7EB] font-medium">{column.title}</h4>
              <span className="text-[#9CA3AF] text-sm">
                {tasks.filter(t => t.column === column.id).length}
              </span>
            </div>
            
            <div className="space-y-3 min-h-[200px]">
              {tasks
                .filter(task => task.column === column.id)
                .map(task => (
                  <motion.div
                    key={task.id}
                    layout
                    initial={{ opacity: 0, scale: 0.8 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.8 }}
                    className="bg-[#21262D] border border-[#30363D] rounded-lg p-4 cursor-pointer hover:border-[#8B5CF6] transition-colors"
                  >
                    <p className="text-[#E5E7EB] text-sm">{task.title}</p>
                    {task.column === 'done' && (
                      <motion.div
                        initial={{ scale: 0 }}
                        animate={{ scale: 1 }}
                        className="mt-2 flex items-center space-x-2"
                      >
                        <div className="w-4 h-4 bg-[#2DD4BF] rounded-full flex items-center justify-center">
                          <svg className="w-2 h-2 text-[#0D1117]" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                          </svg>
                        </div>
                        <span className="text-[#2DD4BF] text-xs">Completed</span>
                      </motion.div>
                    )}
                  </motion.div>
                ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}