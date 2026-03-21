# Skill: Frontend Architect Expert

**ID:** `frontend-architect-expert`
**Description:** UI/UX patterns for Vite + React projects using Framer Motion, Magic UI, shadcn/ui, Tremor, and vanilla Canvas API. Reference this skill when planning or implementing components, animations, layouts, backgrounds, or data visualization in this stack.

---

## 🧠 Core Philosophy: Stack Synergy

Do not isolate libraries. Each tool has a specific role — use them in this strict hierarchy:

1. **Structure** — Standard `div` grids or `shadcn/ui` for skeleton and layout.
2. **Flair** — `Magic UI` components (e.g. `<MagicCard>`) as wrappers for visual depth and effects.
3. **Motion** — `<motion.div>` (Framer Motion) for interactivity and entrance animations. Always prefer physics-based springs over linear tweens.
4. **Canvas** — Vanilla `canvas` 2D API for ambient, generative, or particle-based visuals. Never use Three.js or any 3D library for effects achievable in 2D.

---

## 🧱 UI Patterns

### Pattern 1: 3D Flip Card (Interactive Depth)

_Use for dense data items (Anime, Books, Projects) to keep the main view clean._

**Structure:**

- Outer wrapper: `style={{ perspective: '1000px' }}` — always inline style, never Tailwind plugin (not available by default).
- Inner wrapper: `[transform-style:preserve-3d]` as a Tailwind arbitrary class — **this must be on the inner wrapper, not the outer container. This is the #1 cause of broken flip implementations.**
- Both faces: `backface-hidden` + `absolute inset-0`.
- Back face starts pre-rotated: `[transform:rotateY(180deg)]`.

**Motion:**

```tsx
// Flip is CLICK-triggered only. Never hover — breaks on mobile and keyboard navigation.
const frontVariants = {
  unflipped: { rotateY: 0 },
  flipped:   { rotateY: 180 },
};
const backVariants = {
  unflipped: { rotateY: 180 },
  flipped:   { rotateY: 360 },
};
const transition = { duration: 0.55, ease: [0.23, 1, 0.32, 1] };

// Z-index must be elevated when flipped — prevents back face clipping under grid siblings.
animate={{ zIndex: isFlipped ? 10 : 1 }}
```

**Accessibility:**

```tsx
<div
  role="button"
  tabIndex={0}
  onClick={handleFlip}
  onKeyDown={(e) => {
    if (e.key === "Enter" || e.key === " ") handleFlip();
  }}
  className="focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:ring-violet-500"
/>
```

**Content:**

- Front face: `<MagicCard>` wrapping anime/item image + title.
- Back face: `shadcn/ui` `<Card>` + `<ScrollArea>` for metadata and action links.

---

### Pattern 2: Responsive Auto-Fill Grid (Layout)

_Use for dashboard listings and card grids._

**Always use `auto-fill minmax()` — never hardcoded breakpoints.**

```tsx
// Correct — adapts to any container width automatically
<div
  className="grid gap-5"
  style={{ gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))" }}
>
  {items}
</div>

// Wrong — brittle, breaks when sidebar width changes
// className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4"
```

**Card dimensions:** Define uniform card sizes explicitly in px (e.g. `w-[280px] h-[420px]`). Uniform sizes prevent layout shift and make Skeleton matching trivial.

**Entrance animation with stagger:**

```tsx
const containerVariants = {
  hidden: {},
  visible: { transition: { staggerChildren: 0.08 } },
};
const cardVariants = {
  hidden: { opacity: 0, y: 24 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.4, ease: "easeOut" } },
};

<motion.div variants={containerVariants} initial="hidden" animate="visible">
  {items.map((item) => (
    <motion.div key={item.id} variants={cardVariants}>
      <ItemCard item={item} />
    </motion.div>
  ))}
</motion.div>;
```

---

### Pattern 3: Async Loading States (Data Resilience)

_Use because data is fetched from a PostgreSQL backend. Every component with async data needs this._

**Rule:** The `<Skeleton>` must exactly match the loaded component's dimensions to prevent Cumulative Layout Shift (CLS).

```tsx
// Skeleton matches card footprint exactly
<div className="w-[280px] h-[420px] rounded-xl overflow-hidden">
  <Skeleton className="w-full h-[70%]" />
  <div className="p-3 space-y-2">
    <Skeleton className="h-4 w-3/4" />
    <Skeleton className="h-3 w-1/2" />
  </div>
</div>
```

**Transition:** Use `AnimatePresence` to fade out Skeleton and fade in real content:

```tsx
<AnimatePresence mode="wait">
  {isLoading ? (
    <motion.div key="skeleton" exit={{ opacity: 0 }}>
      <AnimeCardSkeleton />
    </motion.div>
  ) : (
    <motion.div key="content" initial={{ opacity: 0 }} animate={{ opacity: 1 }}>
      <AnimeCard />
    </motion.div>
  )}
</AnimatePresence>
```

**Error fallback:** Every component with async data must define an explicit fallback UI for null/error states — not just loading states.

---

### Pattern 4: Canvas Ambient Animation (Generative Backgrounds)

_Use for background effects, particle systems, and generative visuals. Do NOT use Three.js or Framer Motion for these._

**Setup rules:**

```tsx
// Always apply devicePixelRatio scaling on mount AND resize
// Without this, canvas renders blurry on retina/HiDPI screens
const dpr = window.devicePixelRatio || 1;
canvas.width = window.innerWidth * dpr;
canvas.height = window.innerHeight * dpr;
canvas.style.width = `${window.innerWidth}px`;
canvas.style.height = `${window.innerHeight}px`;
ctx.scale(dpr, dpr);
```

**State rule:** Never use `useState` for animation values. Store all particle/object state in local variables or `useRef` inside `useEffect`. React state triggers re-renders that fight the animation loop.

**Respawn timing:** Use `Date.now()` timestamps for delayed respawns:

```tsx
// Correct
if (Date.now() >= particle.respawnAt) {
  spawnParticle(particle);
}

// Wrong — never use setTimeout inside requestAnimationFrame loops
setTimeout(() => spawnParticle(particle), delay);
```

**Cleanup:** Always return a cleanup function from `useEffect`:

```tsx
return () => {
  cancelAnimationFrame(animationFrameId);
  resizeObserver.disconnect();
};
```

**Global alpha reset:** Always reset `ctx.globalAlpha = 1` after drawing any element with custom opacity. Forgetting this causes all subsequent draws to inherit the wrong opacity.

---

### Pattern 5: MagicCard (Mouse-Tracking Glow Effect)

_Use on card front faces and hero elements for interactive visual depth._

**Spec:** Tracks mouse cursor via `onMouseMove` and renders a radial gradient spotlight at the cursor's coordinates.

```tsx
const MagicCard = ({
  children,
  glowColor = "rgba(139, 92, 246, 0.3)",
  className,
}) => {
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const [isHovering, setIsHovering] = useState(false);

  const handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
    const rect = e.currentTarget.getBoundingClientRect();
    setPosition({ x: e.clientX - rect.left, y: e.clientY - rect.top });
  };

  return (
    <div
      className={cn("relative overflow-hidden rounded-xl", className)}
      onMouseMove={handleMouseMove}
      onMouseEnter={() => setIsHovering(true)}
      onMouseLeave={() => setIsHovering(false)}
    >
      {/* Glow overlay — pointer-events:none is required */}
      <div
        className="pointer-events-none absolute inset-0 transition-opacity duration-300"
        style={
          isHovering
            ? {
                background: `radial-gradient(300px circle at ${position.x}px ${position.y}px, ${glowColor}, transparent 70%)`,
              }
            : {}
        }
      />
      {children}
    </div>
  );
};
```

> Reference: https://magicui.design/docs/components/magic-card

---

## 🎨 Visual Standards

**Background:** The app uses a `CosmicBackground` canvas component mounted at the root (`position: fixed`, `z-index: 0`). All layout containers must have `background: transparent` — do not apply opaque background colors to the root `<body>`, `#app`, or `<MainLayout>`. The shadcn `--background` CSS variable must be set to `transparent` in `:root`.

**Card surfaces:** `bg-zinc-900/50` with `backdrop-blur-md` for floating card elements. This ensures cards are readable over the cosmic background.

**Typography:**

- Titles: `font-bold tracking-tight`
- Secondary text: `text-zinc-400` (maintains contrast over dark backgrounds)
- Never use default system fonts — use a distinctive font pairing defined in the project's global CSS.

**Z-index stack (global convention):**

```
0   → CosmicBackground (canvas)
1   → Page layout containers (MainLayout, main)
10  → Actively flipped cards
50  → Sidebar, Header
100 → Modals, Drawers
```

**Data Viz (Tremor):** Style chart tooltips to match the dark theme. Never use Tremor's default light theme colors on dark backgrounds.

---

## ❌ Anti-Patterns (Never Use)

| Anti-Pattern                                                         | Why                                                             | Use Instead                                                         |
| -------------------------------------------------------------------- | --------------------------------------------------------------- | ------------------------------------------------------------------- |
| `layout` prop in Framer Motion for flip animations                   | For DOM position changes only, not CSS transforms               | `animate={{ rotateY }}` with a transition config                    |
| Hover as flip/toggle trigger                                         | Accidental triggers on desktop, impossible on mobile            | Click + keyboard (`Enter`/`Space`)                                  |
| Inline `style={{ perspective }}` skipped in favor of Tailwind plugin | Tailwind perspective plugin not available by default            | Always use inline `style={{ perspective: '1000px' }}`               |
| Hardcoded grid breakpoints (`grid-cols-3`)                           | Breaks when sidebar width changes                               | `auto-fill minmax(Xpx, 1fr)`                                        |
| `any` in TypeScript                                                  | Bypasses type safety                                            | Explicit interfaces for all props and state                         |
| `transform-style: preserve-3d` on outer wrapper                      | Must be on **inner** wrapper — outer wrapper holds perspective  | Apply `[transform-style:preserve-3d]` to the rotating inner element |
| Three.js for 2D particle/background effects                          | ~600KB overhead, WebGL startup cost, shader compilation stutter | Vanilla Canvas 2D API (~0KB, instant, 60fps on any device)          |
| `setTimeout` inside `requestAnimationFrame` loops                    | Race conditions and memory leaks                                | `Date.now() >= respawnAt` checked each frame                        |
| `useState` for animation particle state                              | Triggers re-renders that fight the canvas loop                  | Local variables or `useRef` inside `useEffect`                      |
| Forgetting `ctx.globalAlpha = 1` reset                               | All subsequent canvas draws inherit wrong opacity               | Reset after every draw call that sets custom alpha                  |

---

## ✅ Verification Checklist

Before any plan is handed to the executor, confirm:

**Accessibility**

- [ ] All interactive elements have `tabIndex={0}` and `onKeyDown` (Enter/Space) handlers
- [ ] Visible focus ring on interactive cards (`focus-visible:ring-2`)
- [ ] `role="button"` on non-`<button>` interactive divs

**Layout & Responsiveness**

- [ ] Grid uses `auto-fill minmax()` — no hardcoded breakpoints
- [ ] Card dimensions are explicitly defined in px
- [ ] Skeletons match card dimensions exactly (no CLS)
- [ ] Layout containers are transparent (cosmic background shows through)

**3D Flip Cards**

- [ ] `transform-style: preserve-3d` is on the **inner** wrapper
- [ ] `perspective: 1000px` is set via inline style on the **outer** wrapper
- [ ] `backface-visibility: hidden` on both faces
- [ ] Z-index elevated on flipped state
- [ ] Flip triggered by click only

**Canvas Components**

- [ ] `devicePixelRatio` scaling applied on mount and resize
- [ ] Animation state in refs/local vars, never `useState`
- [ ] `cancelAnimationFrame` + `ResizeObserver.disconnect()` in cleanup
- [ ] `ctx.globalAlpha = 1` reset after each custom-alpha draw

**TypeScript**

- [ ] Zero use of `any`
- [ ] All component props defined as explicit interfaces
- [ ] `npx tsc --noEmit` passes with zero errors

**Motion**

- [ ] No `layout` prop on flip/rotation animations
- [ ] Stagger entrance uses `staggerChildren: 0.08`
- [ ] Spring or ease-out transitions (never linear)
