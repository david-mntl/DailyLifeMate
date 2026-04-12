# ARCH_PLAN — Cosmic Animated Background

> **Reviewed & Corrected by:** AI Orchestrator (Senior Frontend Architect)
> **Original Author:** 01-Architect (Gemini Flash 3)
> **Status:** Executor-Ready ✅

---

## 1. Executive Summary

The **Cosmic Animated Background** is a global UI enhancement that replaces the current flat black background with an immersive, living deep-space environment.

**UX Vision:** The user should feel like they are operating a dashboard from inside a spaceship. The background is ambient and cinematic — it adds atmosphere without competing with content. Stars twinkle slowly and independently. Meteorites drift diagonally across the screen with glowing trails and fade out naturally. A static nebula gradient provides color depth.

**Technical Approach:** Three-layer composition:

1. **CSS nebula layer** — two overlapping `radial-gradient` definitions. Static. Zero runtime cost.
2. **Canvas star layer** — ~150 particles with independent sine-wave opacity pulses (twinkle). No position movement.
3. **Canvas meteorite layer** — up to 5 active shooting stars at any time, each with a glowing gradient trail, diagonal trajectory, and timestamp-based respawn logic.

**Interaction model:** Purely ambient. No user input affects the background. The component is fully self-contained — no props, no context, no side effects outside its own canvas.

**Mount point:** `src/App.tsx` or root layout, as the first child, `position: fixed`, `inset: 0`, `z-index: 0`. All existing UI sits on `z-index: 1` or above.

---

## 2. Component Architecture (Modular)

| File                                         | Action     | Responsibility                                                        |
| -------------------------------------------- | ---------- | --------------------------------------------------------------------- |
| `src/components/common/CosmicBackground.tsx` | **Create** | CSS nebula + canvas animation. Fully self-contained.                  |
| `src/App.tsx`                                | **Update** | Mount `<CosmicBackground />` as first child of root fragment          |
| `src/index.css`                              | **Update** | Override shadcn/ui `--background` CSS variable to `transparent`       |
| `src/components/common/MainLayout.tsx`       | **Update** | Confirm existing layout containers allow transparency to show through |

**Component hierarchy:**

```
App.tsx
├── <CosmicBackground />        ← fixed, z-index: 0, renders behind everything
└── <MainLayout />              ← z-index: 1+, all existing UI lives here
    ├── <Sidebar />
    ├── <Header />
    └── <main> (content area)
```

**Responsibility rule:** `CosmicBackground` owns nothing outside its own DOM node. It does not read from any store, context, or prop. It sets up its own `ResizeObserver` and animation loop, and tears them down on unmount.

---

## 3. State & Logic

### TypeScript Interfaces

```typescript
// src/components/common/CosmicBackground.tsx

interface Star {
  x: number; // canvas x position (CSS pixels)
  y: number; // canvas y position (CSS pixels)
  radius: number; // circle radius: randomized between 0.5 and 2
  baseOpacity: number; // max opacity ceiling: randomized between 0.4 and 1.0
  opacity: number; // current opacity — driven by sine wave each frame
  twinkleSpeed: number; // phase increment per frame: randomized between 0.005 and 0.02
  phase: number; // current sine phase (radians): randomized 0 to 2π on init
  color: string; // one of: '#ffffff', '#e0d0ff', '#c8b0ff', '#f0e8ff'
}

interface Meteorite {
  x: number; // current head x position (CSS pixels)
  y: number; // current head y position (CSS pixels)
  length: number; // trail length in px: randomized between 40 and 80
  speed: number; // px moved per frame: randomized between 2 and 5
  angle: number; // trajectory in radians — 45° (Math.PI/4) ± 15° variation
  opacity: number; // current opacity — fades to 0 near canvas edges
  active: boolean; // true = currently animating across canvas
  respawnAt: number; // Date.now() timestamp — when to reactivate after despawn
}
```

### Local State & Refs

```typescript
const canvasRef = useRef<HTMLCanvasElement>(null);
// No useState — all animation state lives in refs or local variables
// inside the useEffect to prevent re-renders from touching the animation loop
```

### Initialization Logic

```typescript
// Stars — generated once on mount
const stars: Star[] = Array.from({ length: 150 }, () => ({
  x: Math.random() * canvasWidth,
  y: Math.random() * canvasHeight,
  radius: 0.5 + Math.random() * 1.5,
  baseOpacity: 0.4 + Math.random() * 0.6,
  opacity: Math.random(),
  twinkleSpeed: 0.005 + Math.random() * 0.015,
  phase: Math.random() * Math.PI * 2,
  color: ["#ffffff", "#e0d0ff", "#c8b0ff", "#f0e8ff"][
    Math.floor(Math.random() * 4)
  ],
}));

// Meteorites — 5 slots, all start inactive with staggered initial respawn times
const meteorites: Meteorite[] = Array.from({ length: 5 }, (_, i) => ({
  x: 0,
  y: 0,
  length: 0,
  speed: 0,
  angle: 0,
  opacity: 0,
  active: false,
  respawnAt: Date.now() + i * 1200, // stagger initial spawns by 1.2s each
}));
```

### Meteorite Spawn / Respawn Logic

```typescript
// Called when a meteorite needs to be (re)initialized — either on first spawn
// or after exiting the canvas.
function spawnMeteorite(
  m: Meteorite,
  canvasWidth: number,
  canvasHeight: number,
): void {
  const spawnFromTop = Math.random() > 0.5;
  m.x = spawnFromTop ? Math.random() * canvasWidth : 0;
  m.y = spawnFromTop ? 0 : Math.random() * canvasHeight;
  m.length = 40 + Math.random() * 40;
  m.speed = 2 + Math.random() * 3;
  m.angle = Math.PI / 4 + (Math.random() - 0.5) * (Math.PI / 12); // 45° ± 15°
  m.opacity = 1;
  m.active = true;
}

// Inside the animation loop — per meteorite, per frame:
if (m.active) {
  m.x += Math.cos(m.angle) * m.speed;
  m.y += Math.sin(m.angle) * m.speed;
  // Despawn when fully off canvas
  if (m.x > canvasWidth + m.length || m.y > canvasHeight + m.length) {
    m.active = false;
    m.respawnAt = Date.now() + 2000 + Math.random() * 4000; // 2s–6s delay
  }
} else {
  // Check if it's time to respawn
  if (Date.now() >= m.respawnAt) {
    spawnMeteorite(m, canvasWidth, canvasHeight);
  }
}
```

> ⚠️ **Executor Note:** Do NOT use `setTimeout` inside the animation loop.
> Use `Date.now() >= m.respawnAt` checked on every frame. `setTimeout` inside
> `requestAnimationFrame` causes race conditions and memory leaks.

### Star Twinkle Formula

```typescript
// Per frame, per star — exact formula the executor must use:
star.phase += star.twinkleSpeed; // increment phase each frame
star.opacity = star.baseOpacity * (0.5 + 0.5 * Math.sin(star.phase));
// Result: opacity oscillates smoothly between 0 and baseOpacity.
// Each star has a different phase offset so they never pulse in sync.
```

### Resize Handling

```typescript
const resizeObserver = new ResizeObserver(() => {
  const dpr = window.devicePixelRatio || 1;
  canvas.width = window.innerWidth * dpr;
  canvas.height = window.innerHeight * dpr;
  canvas.style.width = `${window.innerWidth}px`;
  canvas.style.height = `${window.innerHeight}px`;
  ctx.scale(dpr, dpr); // re-apply scale after resize
});
resizeObserver.observe(document.documentElement);
```

### Cleanup on Unmount

```typescript
return () => {
  cancelAnimationFrame(animationFrameId);
  resizeObserver.disconnect();
};
```

### Error Handling

- **Canvas not supported:** Wrap canvas access in a null check. If `canvas.getContext('2d')` returns null, the component renders only the CSS nebula div and returns early — no animation loop is started.
- **No props:** Component accepts no props. Cannot receive invalid input.

---

## 4. Visual & Motion Specs

### Nebula CSS (Static Layer)

```css
/* Two overlapping radial gradients — applied as background on the fixed wrapper div */

background:
  radial-gradient(
    circle at 80% 80%,
    rgba(26, 10, 46, 0.55) 0%,
    transparent 70%
  ),
  radial-gradient(circle at 20% 30%, #1a0533 0%, #0d0d2b 50%, #060610 100%);
```

The first gradient (bottom-right) adds a faint cyan-magenta depth accent.
The second gradient (top-left) is the primary nebula — deep purple bleeding into near-black.

### Stars (Canvas Layer 1)

```typescript
// Drawing a single star
ctx.beginPath();
ctx.arc(star.x, star.y, star.radius, 0, Math.PI * 2);
ctx.fillStyle = star.color;
ctx.globalAlpha = star.opacity; // set by twinkle formula above
ctx.fill();
ctx.globalAlpha = 1; // always reset after each draw call
```

### Meteorites (Canvas Layer 2)

```typescript
// Drawing a single meteorite trail
const tailX = star.x - Math.cos(m.angle) * m.length;
const tailY = star.y - Math.sin(m.angle) * m.length;

const gradient = ctx.createLinearGradient(m.x, m.y, tailX, tailY);
gradient.addColorStop(0, `rgba(255, 255, 255, ${m.opacity})`); // head: bright white
gradient.addColorStop(1, "rgba(255, 255, 255, 0)"); // tail: fully transparent

ctx.beginPath();
ctx.moveTo(m.x, m.y);
ctx.lineTo(tailX, tailY);
ctx.strokeStyle = gradient;
ctx.lineWidth = 1.5;
ctx.stroke();
```

### devicePixelRatio — Required for Retina/HiDPI Screens

```typescript
// Apply on mount AND on every resize. Without this, the canvas renders
// blurry on MacBooks and any HiDPI display.
const dpr = window.devicePixelRatio || 1;
canvas.width = window.innerWidth * dpr;
canvas.height = window.innerHeight * dpr;
canvas.style.width = `${window.innerWidth}px`;
canvas.style.height = `${window.innerHeight}px`;
ctx.scale(dpr, dpr);
```

### Anti-Patterns — Executor Must Never Use

- ❌ `setTimeout` inside the animation loop — use `Date.now() >= respawnAt` instead
- ❌ `useState` for animation state — causes re-renders that fight the canvas loop; use refs or local variables inside `useEffect`
- ❌ Framer Motion — not needed here; vanilla canvas API only
- ❌ Three.js or any 3D library — canvas 2D API only
- ❌ CSS animated gradients — keep the nebula layer static for performance
- ❌ `position: absolute` on the canvas wrapper — must be `position: fixed`
- ❌ Forgetting `ctx.globalAlpha = 1` reset after each draw — causes all subsequent draws to inherit the wrong opacity

---

## 5. Executor Checklist

Each step is atomic (one file or one concern), verifiable, and ordered so that no step references something not yet built.

- [ ] Create `src/components/common/CosmicBackground.tsx` — scaffold the component shell: a `position: fixed; inset: 0; z-index: 0` wrapper div containing the CSS nebula gradients (two `radial-gradient` layers as specified in Section 4). Include a `<canvas>` element in the JSX. No animation logic yet.

- [ ] Apply `devicePixelRatio` scaling: on mount, set `canvas.width/height` to viewport dimensions multiplied by `window.devicePixelRatio`. Set `canvas.style.width/height` to CSS pixel dimensions. Call `ctx.scale(dpr, dpr)`. This must happen before any drawing.

- [ ] Implement star initialization: generate the array of 150 `Star` objects using the exact field ranges from Section 3. Store in a local variable inside `useEffect` (not in state).

- [ ] Implement the star draw + twinkle loop: per frame, update `star.phase += star.twinkleSpeed`, compute opacity via `baseOpacity * (0.5 + 0.5 * Math.sin(star.phase))`, draw with `ctx.arc`. Reset `ctx.globalAlpha = 1` after each star.

- [ ] Implement meteorite initialization: generate the array of 5 `Meteorite` objects, all `active: false`, with staggered `respawnAt` timestamps (1.2s apart).

- [ ] Implement the `spawnMeteorite` function as specified in Section 3: randomizes `x/y` (top or left edge), `length`, `speed`, `angle` (45° ± 15°), sets `active: true`.

- [ ] Implement the meteorite update + draw loop: per frame, move active meteorites by `speed` along `angle`, check for canvas exit and set `active: false` + `respawnAt`. Check inactive meteorites against `Date.now()` and call `spawnMeteorite` when ready. Draw trail using `createLinearGradient` as specified in Section 4.

- [ ] Add `ResizeObserver` on `document.documentElement`: on callback, reapply `devicePixelRatio` scaling (width, height, style, and `ctx.scale`). Re-randomize star positions to fill the new canvas size.

- [ ] Implement cleanup: return function from `useEffect` must call `cancelAnimationFrame(animationFrameId)` and `resizeObserver.disconnect()`.

- [ ] Add canvas null guard: if `canvas.getContext('2d')` returns null, return early from `useEffect` without starting the animation loop. The CSS nebula wrapper will still render.

- [ ] Update `src/index.css`: override the shadcn/ui CSS variable `--background` to `transparent` under `:root`. Do **not** remove `bg-background` from JSX components — change the variable value only. This prevents breaking shadcn component styles.

- [ ] Update `src/components/common/MainLayout.tsx`: verify that the main layout containers and the root `#app` div do not have an opaque `background-color` set via inline style or Tailwind. If they do, remove only the background color — preserve all other styles.

- [ ] Update `src/App.tsx`: mount `<CosmicBackground />` as the **first child** of the root fragment, before `<MainLayout />` or any other component.

- [ ] Run `npx tsc --noEmit` — resolve all type errors before marking complete. Zero errors required.

- [ ] Visual QA checklist:
  - [ ] Stars twinkle at different rates and phases (not in sync)
  - [ ] Meteorites appear from top or left edge, drift diagonally, and respawn after a delay
  - [ ] Canvas is crisp (not blurry) on a HiDPI/retina screen
  - [ ] Nebula gradient is visible as a purple color wash behind all UI
  - [ ] Sidebar and dashboard cards remain fully readable on top of the background
  - [ ] Canvas resizes correctly when the browser window is resized
  - [ ] No console errors or memory leaks after 60 seconds of idle

---

> **Executor reminder:** Do not ask questions. Do not explain what you are about to do.
> Execute each checklist item in order and output each completed file as a full,
> copy-paste ready code block labeled with its file path.
