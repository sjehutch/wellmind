# AGENTS.md

## Purpose
This repository builds **PulseCheck**, a low-drama wellness app focused on early burnout detection and sustainable habits for working professionals.

Agents working in this repo should optimize for:
- clarity over cleverness
- simple solutions over complex ones
- long-term maintainability over short-term speed

---

## Core Principles

### 1. Be Calm, Not Clever
If a solution is impressive but hard to explain to a non-developer, it is the wrong solution.

### 2. Optimize for B2B Reality
This product is designed to be sold to companies:
- no dark patterns
- no guilt-based messaging
- no medical claims
- everything must feel optional, respectful, and safe

### 3. Low Drama by Design
The app should:
- never shame users
- never alarm users
- surface trends, not judgments
- encourage reflection, not pressure

### 4. Privacy First
Assume:
- aggregated data only
- no individual scoring visible to employers
- local-first where possible
- transparency beats clever analytics

### 5. Incremental Wins
Prefer features that:
- can be built in days, not months
- provide immediate value
- can be demoed in under 2 minutes

---

## Technical Preferences

- .NET MAUI
- MVVM (simple, not over-abstracted)
- Clear naming > reuse gymnastics
- Test critical logic, not UI trivia

---

## Decision Rule
If unsure between two approaches:
> Choose the one that reduces future cognitive load.

# Repository Layout Rules

- All application code MUST live under `/src`
- No production code is allowed at the repository root
- AI agents MUST NOT create files outside `/src` unless explicitly instructed

HomeViewModel should expose:
- TodayCheckIn: CheckIn?
- HasTodayCheckIn: bool
- PrimaryActionText:
    - "Start today’s check-in" when HasTodayCheckIn == false
    - "Update today’s check-in" when HasTodayCheckIn == true