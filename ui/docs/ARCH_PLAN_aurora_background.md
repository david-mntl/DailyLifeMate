# Amendment 01 — Aurora Borealis Layer

## Executive Summary

This amendment adds a generative **Aurora Borealis (Northern Lights)** layer to the `CosmicBackground` component.
The aurora consists of 3-4 flowing, translucent ribbons of color that undulate slowly at the top of the screen.

## Component Architecture (Modular)

- **Modified File:** `src/components/common/CosmicBackground.tsx`
- **Responsibility:** Add a third canvas layer (Layer 0, behind stars and meteorites) for the aurora effect.

## State & Logic

### TypeScript Interfaces

```typescript
interface AuroraWave {
  points: { x: number; y: number }[]; // dynamically generated spline points
  baseY: number; // vertical anchor point (upper 30% of screen)
  amplitude: number; // wave height (30px to 80px)
  frequency: number; // wave tightness
  speed: number; // horizontal drift speed
  phase: number; // current animation phase
  color: string; // rgba(0, 255, 150, 0.15) to rgba(0, 200, 255, 0.1)
  height: number; // vertical thickness of the ribbon
}
```

### Initialization

- Generate 3 `AuroraWave` objects.
- `baseY` should be randomized between `height * 0.1` and `height * 0.3`.
- `color` should be a variation of emerald green or cyan with low alpha (0.1 - 0.2).

### Animation Logic

- Per frame: `wave.phase += wave.speed`.
- Calculate `y` for each `x` across the screen: `y = baseY + Math.sin(x * frequency + phase) * amplitude`.
- To create the "ribbon" effect, draw vertical lines from `y` to `y + wave.height` with a vertical gradient.

## Visual Specs

- **Colors:** `#00ff96` (emerald), `#00c8ff` (cyan), `#7de2ff` (soft blue).
- **Blur:** The aurora should feel soft. Use `ctx.filter = 'blur(40px)'` before drawing the waves, then reset `ctx.filter = 'none'`.
- **Z-Index:** Renders behind stars.

## Step-by-Step Executor Checklist

- [ ] Update `AuroraWave` interface in `src/components/common/CosmicBackground.tsx`.
- [ ] Initialize `auroraWaves` array in `useEffect`.
- [ ] Implement `drawAurora` function using `ctx.beginPath()`, `ctx.moveTo()`, and a series of `ctx.lineTo()` calls or a vertical line loop to create the ribbon.
- [ ] Apply `ctx.filter = 'blur(40px)'` specifically for the aurora layer.
- [ ] Integrate `drawAurora()` into the main `draw()` loop _before_ the star drawing logic.
- [ ] Run `npx tsc --noEmit` and verify zero errors.
