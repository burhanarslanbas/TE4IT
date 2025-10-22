import { motion } from "motion/react";

interface GradientBlobProps {
  colors: string[];
  size?: "sm" | "md" | "lg";
  position?: "top-left" | "top-right" | "bottom-left" | "bottom-right" | "center";
  className?: string;
}

export function GradientBlob({ 
  colors, 
  size = "md", 
  position = "center",
  className = "" 
}: GradientBlobProps) {
  const sizeClasses = {
    sm: "w-32 h-32",
    md: "w-64 h-64", 
    lg: "w-96 h-96"
  };

  const positionClasses = {
    "top-left": "top-0 left-0 -translate-x-1/2 -translate-y-1/2",
    "top-right": "top-0 right-0 translate-x-1/2 -translate-y-1/2",
    "bottom-left": "bottom-0 left-0 -translate-x-1/2 translate-y-1/2",
    "bottom-right": "bottom-0 right-0 translate-x-1/2 translate-y-1/2",
    "center": "top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"
  };

  const gradientString = `linear-gradient(45deg, ${colors.join(", ")})`;

  return (
    <motion.div
      className={`absolute ${sizeClasses[size]} ${positionClasses[position]} rounded-full blur-3xl opacity-20 ${className}`}
      style={{
        background: gradientString,
      }}
      animate={{
        scale: [1, 1.2, 1],
        rotate: [0, 180, 360],
      }}
      transition={{
        duration: 20,
        repeat: Infinity,
        ease: "linear",
      }}
    />
  );
}