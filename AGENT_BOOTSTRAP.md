# AGENT_BOOTSTRAP.md

You are bootstrapping **PulseCheck**, a cross-platform wellness app built with .NET MAUI.

## Product Goal
PulseCheck is a **burnout early-warning system** for employees, designed to be calm, private, and non-judgmental.

It helps users notice trends in:
- mental load
- energy
- work stress
- basic habits
without medical claims or employer surveillance.

---

## Core Constraints

- Platform: .NET MAUI
- Architecture: MVVM
- UI: Material-inspired components (clean, neutral, modern)
- Target user: working professionals
- Tone: supportive, optional, low-pressure

---

## MVP Features (Phase 1)

### 1. Daily Check-In (30 seconds)
Simple sliders or buttons:
- Energy level
- Stress level
- Focus level
- Sleep quality (self-reported)

No scores. No red/yellow/green alerts.

---

### 2. Weekly Trend View
- Line or bar charts
- Show *patterns*, not grades
- Language like:
  “Here’s what your last 7 days looked like”

---

### 3. Gentle Insight Prompts
Examples:
- “You’ve marked low energy a few days in a row.”
- “This week looks heavier than usual.”

No recommendations unless the user taps for more.

---

### 4. Optional Reflection
One optional question:
> “Anything you want to note about today?”

Stored locally by default.

---

## Non-Goals (Do NOT build)
- No gamification
- No streaks
- No rankings
- No employer dashboards
- No medical advice

---

## Technical Expectations

- Clean folder structure
- Minimal dependencies
- Clear naming
- ViewModels explainable to a junior dev
- Comments where intent matters

---

## Output Requested

1. MAUI project structure
2. Core models (CheckIn, Trend)
3. ViewModels for:
   - Daily Check-In
   - Weekly Trends
4. Basic navigation
5. Placeholder data (no backend yet)

Focus on correctness, simplicity, and readability.