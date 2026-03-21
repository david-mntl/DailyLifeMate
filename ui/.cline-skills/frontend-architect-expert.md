# Skill: Frontend Architect Expert (Vite + React 2026)
**ID:** `frontend-architect-expert`
**Description:** Specialist in high-end UI/UX. Provides architectural patterns for combining Framer Motion, Magic UI, Tremor, and shadcn/ui seamlessly.

## 🧠 Core Philosophy: Stack Synergy
Do not isolate libraries. Combine them using this strict hierarchy:
1. **Structure:** Use standard `div` grids or `shadcn/ui` for the skeleton and layout.
2. **Flair:** Apply `Magic UI` components (like `<MagicCard>`) as wrappers for visual depth and glowing borders.
3. **Motion:** Wrap elements in `<motion.div>` (Framer Motion) for interactivity, ensuring physics-based transitions (springs) rather than linear tweens.

## 🧱 Reusable UI Patterns

### Pattern 1: The 3D Flip-Card (Interactive Depth)
*Use for dense data items (Anime, Books, Projects) to keep the main view clean.*
- **Container:** Apply `perspective: 1000px` to the parent wrapper.
- **Motion Logic:** Use `<motion.div>` with `transform-style: preserve-3d` and a transition duration of `0.6s`.
- **Faces:** Stack two divs (`Front` and `Back`) using `absolute inset-0`.
- **Backface:** Apply Tailwind's `backface-hidden` to both faces.
- **Integration:** Front face uses `<MagicCard>` (image + title). Back face uses `shadcn/ui` `Card` + `ScrollArea` (data + actions).

### Pattern 2: The Bento Grid (Layout)
*Use for dashboard overviews and listing items.*
- **Structure:** Use CSS Grid: `grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6`.
- **Entrance Animation:** Use Framer Motion's `staggerChildren` to animate cards in sequentially on mount.

### Pattern 3: Async Loading States (Data Resilience)
*Use because data is fetched from a PostgreSQL backend.*
- **Skeletons:** Always build a `shadcn` `<Skeleton>` component that perfectly matches the dimensions of the loaded component to prevent Cumulative Layout Shift (CLS).
- **Transitions:** Use `AnimatePresence` from Framer Motion to smoothly fade out the Skeleton and fade in the actual data.

## 🎨 Visual Standards (Dynamic Dark Mode)
- **Colors:** Base background is `bg-zinc-950`. Cards are `bg-zinc-900/50`. Use `backdrop-blur-md` for floating elements.
- **Typography:** Titles should be `font-bold tracking-tight`. Text should use `text-zinc-400` for secondary information to maintain high contrast.
- **Data Viz (Tremor):** When using Tremor charts, ensure the chart tooltips are styled to match the dark theme.

## 🛠️ Verification Checklist
1. **Accessibility:** Do interactive elements have `tabIndex={0}` and `onKeyDown` (Enter/Space) handlers?
2. **Responsiveness:** Does the grid collapse to 1 column on mobile?
3. **Motion:** Are the 3D rotations and layout shifts smooth without flickering?