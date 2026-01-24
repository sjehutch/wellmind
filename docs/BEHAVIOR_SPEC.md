# WellMind Behavior Spec (MVP)

This document describes the user-facing behavior of WellMind. It focuses on what users experience and what the app guarantees. The app is offline-first and uses local storage only in MVP.

## Feature: Onboarding
- User goal: Set a name, confirm timezone, and choose reminder preference.
- Entry points: First launch after install or after local data reset.
- Preconditions: No existing local profile.
- Steps:
  1. User enters a display name.
  2. App shows detected timezone with a confirm option.
  3. User opts into reminders (optional).
  4. User finishes onboarding and lands on Home.
- Rules:
  - Reminders are off by default.
  - The user can skip name entry and remain anonymous.
- Failure modes:
  - If the app is force-closed mid-onboarding, restart onboarding on next launch.
  - If the app crashes, partial data is discarded unless explicitly saved.
- Data touched: Local profile (name, timezone, reminder opt-in).
- Done when: Home screen is shown with onboarding data stored locally.

## Feature: Daily Check-In
- User goal: Record how they are feeling in under 60 seconds.
- Entry points: Home “Start check-in” button; optional reminder notification.
- Preconditions: App is launched; no auth required.
- Steps:
  1. User rates Energy, Stress, Focus, and Sleep on 1–5 sliders.
  2. User optionally adds a short note.
  3. User taps “Save check-in.”
- Rules:
  - Check-ins are saved locally only.
  - No scoring or judgment is shown during entry.
  - Cancel returns to Home without saving.
- Failure modes:
  - If the app is force-closed before save, the check-in is not stored.
  - If the app crashes after save, the most recent check-in remains stored.
  - Offline does not block saving (local-only).
- Data touched: Local check-in list (timestamp, slider values, optional note).
- Done when: User returns to Home and the latest check-in updates trends/tips.

## Feature: Insights / Trends
- User goal: See simple patterns from recent check-ins.
- Entry points: Home “This week” section.
- Preconditions: At least one check-in for meaningful trends.
- Steps:
  1. App loads recent check-ins (up to 7 days).
  2. App shows average, last value, and range per metric.
- Rules:
  - Trends show “No check-ins yet” when data is missing.
  - Trends avoid judgmental language.
- Failure modes:
  - If local data is missing or corrupted, trends fall back to empty state.
  - Offline does not affect trend loading.
- Data touched: Local check-in list (read-only).
- Done when: Trends are displayed or an empty state is shown.

## Feature: Today’s Balance (visual gauge)
- User goal: Get a quick, gentle read of today’s check-in.
- Entry points: Home “Today’s balance” card.
- Preconditions: A check-in exists for today.
- Steps:
  1. App calculates a simple normalized value from today’s check-in.
  2. App displays a ring gauge and a 1–5 score inside it.
  3. App shows a descriptor (Low/Steady/Strong) and a short note.
- Rules:
  - Gauge is hidden when no check-in exists today.
  - Colors transition from red to green based on the value; no alarm states.
- Failure modes:
  - If calculation fails, the gauge is hidden and no value is shown.
- Data touched: Local check-in list (read-only).
- Done when: Gauge reflects the most recent check-in for today.

## Feature: Nudge Suggestions
- User goal: Receive gentle, optional ideas based on recent patterns.
- Entry points: Home “Gentle tips” section.
- Preconditions: At least one check-in in the last 7 days.
- Steps:
  1. App reviews recent averages (energy, stress, focus, sleep).
  2. App selects up to three short tips.
- Rules:
  - Tips are optional and framed as suggestions.
  - If few conditions match, show a neutral micro-break prompt.
  - Tips do not diagnose or claim medical outcomes.
- Failure modes:
  - If no data exists, the tips section is hidden.
- Data touched: Local check-in list (read-only).
- Done when: Up to three tips are shown.

## Feature: Resource Links
- User goal: Open trusted resources for self-care.
- Entry points: Home “Learn more” buttons.
- Preconditions: Links are available in the local list.
- Steps:
  1. User taps a link.
  2. App opens the URL inside the in-app browser.
- Rules:
  - Links open in-app, not a system browser.
  - Titles are displayed as provided in the link list.
- Failure modes:
  - If a link is missing or empty, nothing happens.
  - If offline, the web view may fail to load.
- Data touched: Local link list (read-only).
- Done when: Link opens or gracefully fails.

## Feature: Settings (MVP scope)
- User goal: Manage reminders and privacy controls.
- Entry points: Settings screen (planned).
- Preconditions: App is installed.
- Steps:
  1. User toggles reminders.
  2. User chooses export or delete data.
- Rules:
  - Export creates a local file only.
  - Delete removes all local check-ins and profile data.
- Failure modes:
  - If the app is force-closed during export, the export may be incomplete.
  - If the app crashes during delete, the app reverts to onboarding on next launch.
- Data touched: Local profile, local check-ins.
- Done when: Reminders update, and data export/delete completes.

## Cross-cutting edge cases
- Force close: Unsaved input is discarded; saved check-ins persist.
- Crash on launch: App restarts and uses the most recent local data.
- Offline: All features continue except external links.
- Expired auth window: Not applicable in MVP (no auth).
- Missing SKU cache: Not applicable in MVP (no purchases).

## Questions / Assumptions
- Is a dedicated Trends screen required, or is Home sufficient for MVP?
- What is the desired reminder cadence (time of day, frequency, snooze)?
- Should check-ins be limited to one per day, or allow multiple?
- Do we want a neutral empty state for Today’s balance when no check-in exists?
- Should resource links be editable or fixed in MVP?
