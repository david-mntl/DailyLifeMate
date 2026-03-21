import { cn } from "@/lib/utils";
import React, { useState } from "react";

interface MagicCardProps {
  children: React.ReactNode;
  className?: string;
  glowColor?: string;
}

const MagicCard: React.FC<MagicCardProps> = ({
  children,
  className,
  glowColor = "rgba(139, 92, 246, 0.3)",
}) => {
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const [isHovering, setIsHovering] = useState(false);

  const handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
    const rect = e.currentTarget.getBoundingClientRect();
    setPosition({ x: e.clientX - rect.left, y: e.clientY - rect.top });
  };

  const gradientStyle = isHovering
    ? {
        background: `radial-gradient(300px circle at ${position.x}px ${position.y}px, ${glowColor}, transparent 70%)`,
      }
    : {};

  return (
    <div
      className={cn(
        "relative overflow-hidden rounded-xl h-full w-full",
        className,
      )}
      onMouseMove={handleMouseMove}
      onMouseEnter={() => setIsHovering(true)}
      onMouseLeave={() => setIsHovering(false)}
    >
      {/* Glow overlay — pointer-events:none so it doesn't block clicks */}
      <div
        className="pointer-events-none absolute inset-0 transition-opacity duration-300"
        style={gradientStyle}
      />
      {children}
    </div>
  );
};

export default MagicCard;
