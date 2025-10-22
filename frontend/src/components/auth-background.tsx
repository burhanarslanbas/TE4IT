import { motion } from "motion/react";

export function AuthBackground() {
  return (
    <div className="absolute inset-0 overflow-hidden pointer-events-none">
      {/* Gradient Blobs */}
      <motion.div
        className="absolute -top-40 -right-40 w-96 h-96 bg-[#8B5CF6] opacity-20 blur-[128px] rounded-full"
        animate={{
          scale: [1, 1.2, 1],
          x: [0, 30, 0],
          y: [0, -20, 0],
        }}
        transition={{
          duration: 8,
          repeat: Infinity,
          ease: "easeInOut",
        }}
      />
      <motion.div
        className="absolute -bottom-40 -left-40 w-96 h-96 bg-[#2DD4BF] opacity-20 blur-[128px] rounded-full"
        animate={{
          scale: [1, 1.3, 1],
          x: [0, -30, 0],
          y: [0, 20, 0],
        }}
        transition={{
          duration: 10,
          repeat: Infinity,
          ease: "easeInOut",
        }}
      />

      {/* Geometric Grid Pattern */}
      <svg className="absolute inset-0 w-full h-full opacity-[0.03]">
        <defs>
          <pattern
            id="grid"
            width="40"
            height="40"
            patternUnits="userSpaceOnUse"
          >
            <path
              d="M 40 0 L 0 0 0 40"
              fill="none"
              stroke="currentColor"
              strokeWidth="1"
              className="text-[#8B5CF6]"
            />
          </pattern>
        </defs>
        <rect width="100%" height="100%" fill="url(#grid)" />
      </svg>

      {/* Circuit Board Style Lines */}
      <svg className="absolute inset-0 w-full h-full opacity-[0.05]">
        <motion.path
          d="M 100 300 L 300 300 L 300 400 L 500 400"
          stroke="url(#circuit-gradient-1)"
          strokeWidth="2"
          fill="none"
          initial={{ pathLength: 0, opacity: 0 }}
          animate={{ pathLength: 1, opacity: 1 }}
          transition={{ duration: 3, repeat: Infinity, ease: "linear" }}
        />
        <motion.path
          d="M 800 200 L 1000 200 L 1000 500 L 1200 500"
          stroke="url(#circuit-gradient-2)"
          strokeWidth="2"
          fill="none"
          initial={{ pathLength: 0, opacity: 0 }}
          animate={{ pathLength: 1, opacity: 1 }}
          transition={{ duration: 4, delay: 1, repeat: Infinity, ease: "linear" }}
        />
        <motion.path
          d="M 500 100 L 500 300 L 700 300"
          stroke="url(#circuit-gradient-3)"
          strokeWidth="2"
          fill="none"
          initial={{ pathLength: 0, opacity: 0 }}
          animate={{ pathLength: 1, opacity: 1 }}
          transition={{ duration: 2.5, delay: 0.5, repeat: Infinity, ease: "linear" }}
        />

        <defs>
          <linearGradient id="circuit-gradient-1" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop offset="0%" stopColor="#8B5CF6" stopOpacity="0" />
            <stop offset="50%" stopColor="#8B5CF6" stopOpacity="1" />
            <stop offset="100%" stopColor="#8B5CF6" stopOpacity="0" />
          </linearGradient>
          <linearGradient id="circuit-gradient-2" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop offset="0%" stopColor="#2DD4BF" stopOpacity="0" />
            <stop offset="50%" stopColor="#2DD4BF" stopOpacity="1" />
            <stop offset="100%" stopColor="#2DD4BF" stopOpacity="0" />
          </linearGradient>
          <linearGradient id="circuit-gradient-3" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" stopColor="#EC4899" stopOpacity="0" />
            <stop offset="50%" stopColor="#EC4899" stopOpacity="1" />
            <stop offset="100%" stopColor="#EC4899" stopOpacity="0" />
          </linearGradient>
        </defs>
      </svg>

      {/* Floating Particles */}
      {[...Array(20)].map((_, i) => (
        <motion.div
          key={i}
          className="absolute w-1 h-1 bg-[#8B5CF6] rounded-full"
          style={{
            left: `${Math.random() * 100}%`,
            top: `${Math.random() * 100}%`,
          }}
          animate={{
            y: [0, -100, 0],
            opacity: [0, 1, 0],
          }}
          transition={{
            duration: 3 + Math.random() * 2,
            repeat: Infinity,
            delay: Math.random() * 5,
            ease: "easeInOut",
          }}
        />
      ))}
    </div>
  );
}
