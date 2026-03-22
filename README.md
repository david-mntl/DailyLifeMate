# 🌌 DailyLifeMate

> **A personal dashboard platform built to centralize what matters — powered by a microservices architecture and developed end-to-end with AI-assisted engineering.**

---

## 🧭 Vision

DailyLifeMate is a **premium, topic-based dashboard platform** that lets users create personalized spaces to organize the things they care about — whether that's anime they're currently watching across Netflix, Crunchyroll, and Amazon Prime, YouTube playlists with custom descriptions, ongoing projects, finances, or anything else worth tracking.

The core idea is simple but powerful: **stop hunting across tabs and apps**. Everything lives in one dashboard, organized by topic, accessible in one click.

**What sparked this project:** As an anime fan watching up to 8 series per season, I kept running into the same frustration — new episodes drop on different days across Crunchyroll, Netflix, Amazon Prime, etc, and there was no single place to track them all. DailyLifeMate started as a solution to that: one dashboard where I can see every anime I'm following, know when the next episode drops, and reach it in no more than 3–4 clicks. What began as a personal tool quickly became a platform concept — the same idea applies to any content scattered across the web.

This project serves a dual purpose:
1. **A real-world sandbox** to learn and apply modern AI-assisted development techniques — including multi-agent orchestration workflows.
2. **A showcase** of full-stack engineering capabilities: microservices, containerization, API design, and modern frontend architecture.

---

## 🎬 Preview

### Dashboard & Card Flip
> The Anime Dashboard displaying auto-fetched cover art for each title. Click any card to flip it and reveal its full metadata - synopsis, episode count, genre tags, and release date — pulled live from MyAnimeList via the Jikan API.

https://github.com/user-attachments/assets/e02043dd-c06a-4277-b0c4-24878f8cd28f

---

### Adding an Anime & Cosmic Background
> Type any anime title, confirm and the card appears fully populated with no manual input. Information is retrieved using the Jikan API. The ambient cosmic background runs on a vanilla Canvas 2D animation loop with zero library overhead.

https://github.com/user-attachments/assets/7c595c7d-3129-4867-acd4-1f99f8a3843d

---

## ✨ Key Features

### 🃏 Topic-Based Dashboards
Create multiple dashboards organized by topic (e.g., *Anime*, *Finance*, *My Projects*). Each dashboard contains **cards** — individual items that hold the information and links relevant to that topic.

### 🔗 Centralized Link Management
Add multiple links to any card. Perfect for content that's spread across platforms — click directly from the card to your next episode, video, or resource without remembering where you saved it.

### 🎌 Anime Dashboard *(Featured — nearly complete)*
The flagship feature: add any anime title and the platform **automatically fetches all metadata** (synopsis, cover art, genres, episodes, scores) via the **Jikan API** (MyAnimeList wrapper). Cards display rich anime details with an interactive 3D flip-card UI, giving both a visual overview on the front and full metadata + action links on the back.

### 📺 Use examples for other Dashboards (YouTube Playlist Tracking)
A gap that YouTube itself doesn't fill — save video links with **custom descriptions per video**, organized by topic. Native YouTube playlists don't support this kind of per-video annotation.

---

## 🏗️ Architecture

DailyLifeMate is built as **two independent microservices**, each living in its own isolated environment:

```
DAILYLIFEMATE/
├── backend/              # Microservice 1 — .NET C# REST API
│   ├── .devcontainer/    # Docker-based dev environment
│   ├── src/              # API controllers, services, models
│   └── tests/            # Integrations tests
│
└── ui/                   # Microservice 2 — React Frontend
    ├── .devcontainer/    # Docker-based dev environment
    ├── src/              # Components, pages, hooks, services
    ├── docs/             # Contains the soul/context of the project for AI use purposes
    ├── .cline-rules/     # AI development rules
    └── .cline-skills/    # AI development skills
```

### Backend — C# / .NET REST API
- RESTful API handling all business logic and data persistence
- **PostgreSQL** database for storing dashboards, cards, and user data
- **Jikan API integration** — the backend acts as a proxy/orchestration layer, enriching card creation requests with live anime data before persisting to the database
- Dockerized development environment via `.devcontainer`

### Frontend — React / TypeScript / Vite
- Built with **React + TypeScript + Vite** for a fast, type-safe development experience
- **Tailwind CSS** for utility-first styling
- **Framer Motion** for physics-based animations and entrance transitions
- **shadcn/ui** for accessible, composable UI primitives
- **MagicCard** mouse-tracking glow effects for premium visual depth
- Vanilla **Canvas 2D API** for ambient generative background animations — zero Three.js overhead
- Fully responsive auto-fill grid layout

---

## 🐳 Dev Environment — Docker & DevContainers

Both microservices run in isolated **Docker Dev Containers**, configured via `.devcontainer/devcontainer.json` and `docker-compose.dev.yml`. This setup ensures:

- **Reproducible environments** — no "works on my machine" issues
- **Service isolation** — backend and frontend run independently with their own runtimes
- **Consistent tooling** — every dependency, extension, and runtime config is version-locked inside the container
- **VSCode integration** — Dev Containers attach directly to the running container, giving a full IDE experience inside the isolated environment

> A `demo/` folder with shell scripts to spin up both containers with a single command is currently in development.

---

## 🤖 AI-Assisted Development Workflow

This project is also an **experiment in AI orchestration for software development**. The entire frontend was built using a structured multi-agent workflow running inside **VSCode + Cline**.

### The Architect / Executor Pattern

Rather than giving one AI model a vague instruction and hoping for the best, this project implements a **two-role pipeline** inspired by real engineering team structures:

```
┌─────────────────────────────────────────────────────┐
│                   ARCHITECT ROLE                    │
│                                                     │
│  Model: Gemini 3.0 Pro / Flash (+ Claude Sonnet     │
│         for web-based design review)                │
│                                                     │
│  Responsibilities:                                  │
│  • Analyze the feature request                      │
│  • Design the component architecture                │
│  • Plan implementation step-by-step                 │
│  • Produce a precise, unambiguous execution plan    │
└────────────────────┬────────────────────────────────┘
                     │
                     │  Structured plan handed off
                     ▼
┌─────────────────────────────────────────────────────┐
│                   EXECUTOR ROLE                     │
│                                                     │
│  Model: Gemini 3.0 Flash (faster, cost-efficient)   │
│                                                     │
│  Responsibilities:                                  │
│  • Receive the Architect's plan                     │
│  • Execute implementation with no ambiguity         │
│  • Write production-ready code                      │
│  • Follow the skill constraints exactly             │
└─────────────────────────────────────────────────────┘
```

**Why this matters:** Using an expensive, reasoning-heavy model only for planning and a faster, cheaper model for execution, mirrors real-world engineering team dynamics. It reduces cost, improves output quality, and makes the AI's work **auditable**: the plan is a human-readable artifact before any code is written.

### The Frontend Architect Expert Skill

The frontend was developed with a custom **Cline Skill** (`frontend-architect-expert`) — a structured prompt document that encodes the entire frontend architecture as rules the AI must follow. It defines:

- **UI pattern library** — 3D flip cards, responsive auto-fill grids, async loading states, canvas ambient animations, MagicCard glow effects
- **Anti-pattern blacklist** — documented decisions on what *not* to do and why (e.g., no Three.js for 2D effects, no hover-triggered flip animations, no hardcoded grid breakpoints)
- **A pre-flight checklist** — the Architect verifies every plan against accessibility, layout, 3D correctness, canvas cleanup, and TypeScript strictness before handing it to the Executor

This means the AI doesn't just write code — it writes code that conforms to a well-reasoned, documented technical standard. The result is consistent, maintainable, and reviewable output across every feature.

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Frontend Framework | React 18 + TypeScript |
| Build Tool | Vite |
| Styling | Tailwind CSS |
| UI Components | shadcn/ui |
| Animations | Framer Motion |
| Visual Effects | Vanilla Canvas 2D API, MagicCard |
| Backend Framework | C# / .NET |
| Database | PostgreSQL |
| External API | Jikan API (MyAnimeList) |
| Containerization | Docker + DevContainers |
| AI Dev Tools | Cline (VSCode), Gemini 3.0 Pro/Flash, Claude Sonnet |
| IDE | VSCode |

---

## 📍 Current Status

| Feature | Status |
|---|---|
| Project architecture & devcontainer setup | ✅ Complete |
| Backend API + PostgreSQL integration | ✅ Core complete |
| Jikan API integration (anime metadata) | ✅ Complete |
| Anime dashboard + 3D flip card UI | 🔄 Nearly complete |
| Frontend component system & design system | 🔄 In progress |
| Multi-topic dashboard CRUD | 🔄 In progress |
| Demo launch scripts (Docker) | 📋 Planned |
| Amazon alexa voice control integration | 📋 Planned |

---

## 🚀 Getting Started

> **Note:** A `demo/` folder with one-command launch scripts is currently in development. Instructions will be updated once available.

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [VSCode](https://code.visualstudio.com/) with the [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

### Launch
1. Clone the repository
   ```bash
   git clone https://github.com/david_mntl/DailyLifeMate.git
   cd DailyLifeMate
   ```
2. Open the `backend/` folder in VSCode and reopen in Dev Container
3. Open the `ui/` folder in a separate VSCode window and reopen in Dev Container
4. Both services will start automatically inside their respective containers

---

## 👤 Author

**David** — [@david_mntl](https://github.com/david_mntl)

*Built as a learning project exploring AI-assisted development, microservices architecture, and modern frontend engineering.*

---
