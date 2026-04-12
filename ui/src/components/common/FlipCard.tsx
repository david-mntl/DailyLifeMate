import { cn } from "@/lib/utils";
import { motion } from "framer-motion";
import React, { useState } from "react";

interface FlipCardProps {
  front: React.ReactNode;
  back: React.ReactNode;
  width?: number | string;
  height?: number | string;
  className?: string;
}

const FlipCard: React.FC<FlipCardProps> = ({
  front,
  back,
  width = 280,
  height = 420,
  className,
}) => {
  const [isFlipped, setIsFlipped] = useState(false);

  const handleFlip = () => {
    setIsFlipped(!isFlipped);
  };

  const transition = { duration: 0.55, ease: [0.23, 1, 0.32, 1] }; // ease-out-quint

  return (
    <div
      className={cn("flip-card-wrapper relative outline-none", className)}
      style={{
        width,
        height,
        perspective: "1000px",
      }}
      role="button"
      tabIndex={0}
      onClick={handleFlip}
      onKeyDown={(e) => {
        if (e.key === "Enter" || e.key === " ") {
          e.preventDefault();
          handleFlip();
        }
      }}
    >
      <motion.div
        className="flip-card-inner relative w-full h-full [transform-style:preserve-3d]"
        initial={false}
        animate={{
          rotateY: isFlipped ? 180 : 0,
          zIndex: isFlipped ? 10 : 1,
        }}
        transition={transition}
      >
        {/* Front Face */}
        <div className="flip-card-front absolute inset-0 w-full h-full [backface-visibility:hidden] [-webkit-backface-visibility:hidden]">
          {front}
        </div>

        {/* Back Face */}
        <div className="flip-card-back absolute inset-0 w-full h-full [backface-visibility:hidden] [-webkit-backface-visibility:hidden] [transform:rotateY(180deg)]">
          {back}
        </div>
      </motion.div>
    </div>
  );
};

export default FlipCard;
