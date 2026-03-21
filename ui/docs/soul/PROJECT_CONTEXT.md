---
alwaysApply: true
---

# Project Context: Personal Context Hub 2026

**Vision:** A premium platform for creating personalized, topic-based dashboards (e.g., Anime, Finance, Projects).
**Vibe:** Dynamic, interactive, and high-end. High-contrast dark mode. The layout should be structurally clean, but components should feature innovative, eye-catching transitions, glowing effects, and fluid motion.

## Core Domain Architecture

The application follows a hierarchical structure:

1. **Contexts (Dashboards):** The user can create multiple thematic dashboards.
2. **Items (Cards):** Each dashboard contains items specific to that context.
3. **Actions:** Items hold metadata (images, synopsis) and actionable links (e.g., "Watch Episode") to redirect the user.

## Data & State Strategy

- **Backend:** Data is fetched asynchronously from an existing PostgreSQL backend.
- **UI Implication:** All component architectures MUST account for asynchronous loading states (e.g., shadcn Skeletons) and error handling for failed fetches.

## Core Tech Stack

- **Base:** Vite + React + TypeScript
- **UI Structure:** shadcn/ui (Sidebar, Modals, ScrollAreas, Skeletons)
- **Special FX:** Magic UI (Encouraged for glowing borders, premium card effects, and eye-catching details)
- **Data Viz:** Tremor (For stats/charts specific to certain dashboards)
- **Animation:** Framer Motion (Required for innovative, physical-feeling 3D flips, layout animations, and hover states)
- **Icons:** Lucide React

## Universal Design Directives

1. **Clean Grids (Bento):** All dashboard item lists must use responsive CSS Grid / Bento Grid patterns to keep the structure organized.
2. **Interactive Depth:** Heavily utilize **Flip Cards** (Image/Title on front, Data/Links on back) or **Slide-out Panels**. The interaction should feel tactile and premium.
3. **Motion with Intent:** Animations should be eye-catching but physical (using spring physics). Elements should react fluidly to user interaction (hover, click, drag).
4. **Modularity First:** UI components must be topic-agnostic. A "Flip Card" component should accept generic `title`, `image`, and `backContent` props so it can be reused across Anime, Books, or Tech dashboards.
