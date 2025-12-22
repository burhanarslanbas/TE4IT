"use client";

import { Toaster as Sonner, ToasterProps } from "sonner@2.0.3";

const Toaster = ({ toastOptions, ...props }: ToasterProps) => {
  return (
    <Sonner
      theme="dark"
      className="toaster group"
      toastOptions={{
        classNames: {
          toast: "bg-[#161B22] border border-[#30363D] text-[#E5E7EB]",
          title: "text-[#E5E7EB]",
          description: "text-[#9CA3AF]",
          success: "border-[#10B981] bg-[#10B981]/5",
          error: "border-[#EF4444] bg-[#EF4444]/5",
          info: "border-[#3B82F6] bg-[#3B82F6]/5",
          warning: "border-[#F59E0B] bg-[#F59E0B]/5",
          actionButton: "bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90",
          cancelButton: "bg-[#21262D] text-[#E5E7EB] hover:bg-[#30363D] border border-[#30363D]",
          ...toastOptions?.classNames,
        },
        ...toastOptions,
      }}
      style={
        {
          "--normal-bg": "#161B22",
          "--normal-text": "#E5E7EB",
          "--normal-border": "#30363D",
        } as React.CSSProperties
      }
      {...props}
    />
  );
};

export { Toaster };
