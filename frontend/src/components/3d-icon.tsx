import { motion } from "motion/react";

interface Icon3DProps {
  type: "rocket" | "users" | "target" | "trophy";
  color: string;
  size?: "sm" | "md" | "lg";
}

export function Icon3D({ type, color, size = "md" }: Icon3DProps) {
  const sizeClasses = {
    sm: "w-12 h-12",
    md: "w-16 h-16",
    lg: "w-20 h-20"
  };

  const icons = {
    rocket: (
      <div className="relative">
        <div 
          className="absolute inset-0 rounded-full blur-md"
          style={{ backgroundColor: `${color}40` }}
        />
        <div 
          className="relative rounded-full flex items-center justify-center"
          style={{ backgroundColor: color }}
        >
          <svg className="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M12.395 2.553a1 1 0 00-1.45-.385c-.345.23-.614.558-.822.88-.214.33-.403.713-.57 1.116-.334.804-.614 1.768-.84 2.734a31.365 31.365 0 00-.613 3.58 2.64 2.64 0 01-.945-1.067c-.328-.68-.398-1.534-.398-2.654A1 1 0 005.05 6.05 6.981 6.981 0 003 11a7 7 0 1011.95-4.95c-.592-.591-.98-.985-1.348-1.467-.363-.476-.724-1.063-1.207-2.03zM12.12 15.12A3 3 0 017 13s.879.5 2.5.5c0-1 .5-4 1.25-4.5.5 1 .786 1.293 1.371 1.879A2.99 2.99 0 0113 13a2.99 2.99 0 01-.879 2.121z" clipRule="evenodd" />
          </svg>
        </div>
      </div>
    ),
    users: (
      <div className="relative">
        <div 
          className="absolute inset-0 rounded-full blur-md"
          style={{ backgroundColor: `${color}40` }}
        />
        <div 
          className="relative rounded-full flex items-center justify-center"
          style={{ backgroundColor: color }}
        >
          <svg className="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 20 20">
            <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
          </svg>
        </div>
      </div>
    ),
    target: (
      <div className="relative">
        <div 
          className="absolute inset-0 rounded-full blur-md"
          style={{ backgroundColor: `${color}40` }}
        />
        <div 
          className="relative rounded-full flex items-center justify-center"
          style={{ backgroundColor: color }}
        >
          <svg className="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM4.332 8.027a6.012 6.012 0 011.912-2.706C6.512 5.73 6.974 6 7.5 6A1.5 1.5 0 019 7.5V8a2 2 0 004 0 2 2 0 011.523-1.943A5.977 5.977 0 0116 10c0 .34-.028.675-.083 1H15a2 2 0 00-2 2v2.197A5.973 5.973 0 0110 16v-2a2 2 0 00-2-2 2 2 0 01-2-2 2 2 0 00-1.668-1.973z" clipRule="evenodd" />
          </svg>
        </div>
      </div>
    ),
    trophy: (
      <div className="relative">
        <div 
          className="absolute inset-0 rounded-full blur-md"
          style={{ backgroundColor: `${color}40` }}
        />
        <div 
          className="relative rounded-full flex items-center justify-center"
          style={{ backgroundColor: color }}
        >
          <svg className="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M10 2L3 7v11a2 2 0 002 2h10a2 2 0 002-2V7l-7-5zM8 14v2h4v-2a2 2 0 00-2-2h0a2 2 0 00-2 2z" clipRule="evenodd" />
          </svg>
        </div>
      </div>
    )
  };

  return (
    <motion.div
      className={`relative ${sizeClasses[size]}`}
      whileHover={{ 
        scale: 1.1,
        rotateY: 10,
        rotateX: 10
      }}
      transition={{ type: "spring", stiffness: 300, damping: 30 }}
    >
      {icons[type]}
    </motion.div>
  );
}