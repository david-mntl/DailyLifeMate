import React, { useEffect, useRef } from "react";

interface Star {
  x: number;
  y: number;
  radius: number;
  baseOpacity: number;
  opacity: number;
  twinkleSpeed: number;
  phase: number;
  color: string;
}

interface Meteorite {
  x: number;
  y: number;
  length: number;
  speed: number;
  angle: number;
  opacity: number;
  active: boolean;
  respawnAt: number;
}

interface AuroraCurtain {
  xBase: number;
  width: number;
  yTop: number;
  yBottom: number;
  phase: number;
  speed: number;
  frequency: number;
  amplitude: number;
  opacityPhase: number;
  opacitySpeed: number;
  opacity: number;
  color: string;
}

const CosmicBackground: React.FC = () => {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    let animationFrameId: number;
    let stars: Star[] = [];
    let meteorites: Meteorite[] = [];
    let curtains: AuroraCurtain[] = [];

    const initCanvas = () => {
      const dpr = window.devicePixelRatio || 1;
      const width = window.innerWidth;
      const height = window.innerHeight;

      canvas.width = width * dpr;
      canvas.height = height * dpr;
      canvas.style.width = `${width}px`;
      canvas.style.height = `${height}px`;
      ctx.scale(dpr, dpr);

      // --- Stars ---
      stars = Array.from({ length: 150 }, () => ({
        x: Math.random() * width,
        y: Math.random() * height,
        radius: 0.5 + Math.random() * 1.5,
        baseOpacity: 0.4 + Math.random() * 0.6,
        opacity: Math.random(),
        twinkleSpeed: 0.005 + Math.random() * 0.015,
        phase: Math.random() * Math.PI * 2,
        color: ["#ffffff", "#e0d0ff", "#c8b0ff", "#f0e8ff"][
          Math.floor(Math.random() * 4)
        ],
      }));

      // --- Meteorites ---
      meteorites = Array.from({ length: 5 }, (_, i) => ({
        x: 0,
        y: 0,
        length: 0,
        speed: 0,
        angle: 0,
        opacity: 0,
        active: false,
        respawnAt: Date.now() + i * 1200,
      }));

      // --- Aurora Curtains ---
      // 12 vertical streaks spread across the full screen width.
      // Each is a tapered bezier shape with a wavy top edge that sways slowly.
      // blur(8px) softens edges without destroying the curtain structure.
      const baseColors = [
        "rgba(0, 230, 180, A)",
        "rgba(0, 200, 255, A)",
        "rgba(60, 255, 160, A)",
        "rgba(0, 180, 240, A)",
        "rgba(80, 255, 200, A)",
        "rgba(20, 160, 255, A)",
        "rgba(0, 210, 170, A)",
        "rgba(100, 230, 255, A)",
        "rgba(0, 255, 140, A)",
        "rgba(40, 180, 255, A)",
        "rgba(0, 220, 200, A)",
        "rgba(60, 200, 255, A)",
      ];

      const segment = width / 12;
      curtains = Array.from({ length: 12 }, (_, i) => {
        const xBase =
          segment * i + segment * 0.3 + Math.random() * segment * 0.4;
        const alpha = (0.55 + Math.random() * 0.35).toFixed(2);
        const color = baseColors[i].replace("A", alpha);

        return {
          xBase,
          width: 60 + Math.random() * 120,
          yTop: height * (0.02 + Math.random() * 0.25),
          yBottom: height * (0.65 + Math.random() * 0.35),
          phase: Math.random() * Math.PI * 2,
          speed: 0.003 + Math.random() * 0.006,
          frequency: 0.8 + Math.random() * 0.8,
          amplitude: 20 + Math.random() * 60,
          opacityPhase: Math.random() * Math.PI * 2,
          opacitySpeed: 0.008 + Math.random() * 0.012,
          opacity: 0.5 + Math.random() * 0.5,
          color,
        };
      });
    };

    const spawnMeteorite = (
      m: Meteorite,
      canvasWidth: number,
      canvasHeight: number,
    ) => {
      const spawnFromTop = Math.random() > 0.5;
      m.x = spawnFromTop ? Math.random() * canvasWidth : 0;
      m.y = spawnFromTop ? 0 : Math.random() * canvasHeight;
      m.length = 40 + Math.random() * 40;
      m.speed = 2 + Math.random() * 3;
      m.angle = Math.PI / 4 + (Math.random() - 0.5) * (Math.PI / 12);
      m.opacity = 1;
      m.active = true;
    };

    const drawAurora = (width: number, height: number) => {
      ctx.save();
      // Low blur — softens edges without destroying curtain structure
      ctx.filter = "blur(8px)";

      curtains.forEach((curtain) => {
        curtain.phase += curtain.speed;
        curtain.opacityPhase += curtain.opacitySpeed;
        curtain.opacity = 0.4 + 0.6 * Math.sin(curtain.opacityPhase);

        // Diagonal lean: top shifts left relative to bottom creating a \ angle
        // diagonalOffset = ~70% of the curtain height → roughly 45° lean
        const diagonalOffset = (curtain.yBottom - curtain.yTop) * 0.7;

        // Top edge sways left/right on top of the diagonal offset
        const swayX =
          Math.sin(curtain.phase * curtain.frequency) * curtain.amplitude;
        const topX = curtain.xBase - diagonalOffset + swayX;
        const bottomX = curtain.xBase;

        // Vertical gradient: transparent → vivid color → fade out at bottom
        const grad = ctx.createLinearGradient(
          bottomX,
          curtain.yTop,
          bottomX,
          curtain.yBottom,
        );
        grad.addColorStop(0, "rgba(0,0,0,0)");
        grad.addColorStop(0.12, curtain.color);
        grad.addColorStop(0.55, curtain.color);
        grad.addColorStop(0.88, curtain.color.replace(/[\d.]+\)$/, "0.08)"));
        grad.addColorStop(1, "rgba(0,0,0,0)");

        // Tapered bezier shape: narrow swaying top, wider anchored bottom
        // This is what gives the curtain-fold appearance
        const halfTop = curtain.width * 0.25;
        const halfBottom = curtain.width * 0.55;
        const midY = curtain.yTop + (curtain.yBottom - curtain.yTop) * 0.5;

        ctx.beginPath();
        ctx.moveTo(topX - halfTop, curtain.yTop);
        ctx.bezierCurveTo(
          topX - halfTop,
          midY,
          bottomX - halfBottom,
          midY,
          bottomX - halfBottom,
          curtain.yBottom,
        );
        ctx.lineTo(bottomX + halfBottom, curtain.yBottom);
        ctx.bezierCurveTo(
          bottomX + halfBottom,
          midY,
          topX + halfTop,
          midY,
          topX + halfTop,
          curtain.yTop,
        );
        ctx.closePath();

        ctx.globalAlpha = curtain.opacity;
        ctx.fillStyle = grad;
        ctx.fill();
        ctx.globalAlpha = 1;
      });

      ctx.restore();
    };

    const draw = () => {
      const width = window.innerWidth;
      const height = window.innerHeight;
      ctx.clearRect(0, 0, width, height);

      // Draw order: aurora → stars → meteorites
      drawAurora(width, height);

      stars.forEach((star) => {
        star.phase += star.twinkleSpeed;
        star.opacity = star.baseOpacity * (0.5 + 0.5 * Math.sin(star.phase));
        ctx.beginPath();
        ctx.arc(star.x, star.y, star.radius, 0, Math.PI * 2);
        ctx.fillStyle = star.color;
        ctx.globalAlpha = star.opacity;
        ctx.fill();
        ctx.globalAlpha = 1;
      });

      meteorites.forEach((m) => {
        if (m.active) {
          m.x += Math.cos(m.angle) * m.speed;
          m.y += Math.sin(m.angle) * m.speed;

          if (m.x > width + m.length || m.y > height + m.length) {
            m.active = false;
            m.respawnAt = Date.now() + 2000 + Math.random() * 4000;
          }

          const tailX = m.x - Math.cos(m.angle) * m.length;
          const tailY = m.y - Math.sin(m.angle) * m.length;
          const gradient = ctx.createLinearGradient(m.x, m.y, tailX, tailY);
          gradient.addColorStop(0, `rgba(255, 255, 255, ${m.opacity})`);
          gradient.addColorStop(1, "rgba(255, 255, 255, 0)");

          ctx.beginPath();
          ctx.moveTo(m.x, m.y);
          ctx.lineTo(tailX, tailY);
          ctx.strokeStyle = gradient;
          ctx.lineWidth = 1.5;
          ctx.stroke();
        } else if (Date.now() >= m.respawnAt) {
          spawnMeteorite(m, width, height);
        }
      });

      animationFrameId = requestAnimationFrame(draw);
    };

    initCanvas();
    draw();

    const resizeObserver = new ResizeObserver(() => {
      initCanvas();
    });
    resizeObserver.observe(document.documentElement);

    return () => {
      cancelAnimationFrame(animationFrameId);
      resizeObserver.disconnect();
    };
  }, []);

  return (
    <div
      className="fixed inset-0 z-0 pointer-events-none"
      style={{
        background: `
          radial-gradient(
            circle at 80% 80%,
            rgba(26, 10, 46, 0.55) 0%,
            transparent 70%
          ),
          radial-gradient(circle at 20% 30%, #1a0533 0%, #0d0d2b 50%, #060610 100%)
        `,
      }}
    >
      <canvas ref={canvasRef} className="block w-full h-full" />
    </div>
  );
};

export default CosmicBackground;
