# WellMind Product Brief (MVP)

## What this is
WellMind is a low-drama wellness app that helps busy people notice early burnout signals and build small, sustainable habits. It focuses on reflection, not judgment.

## Who it serves
- Busy professionals who want quick, private check-ins.
- People who prefer gentle nudges over pressure.

## What it does (MVP)
- Daily 60-second check-in for mood/energy/stress/sleep.
- Simple trends based on recent check-ins.
- Gentle, optional suggestions based on answers.
- Optional gentle reminder (local daily notification at a chosen time).
- Daily affirmations screen shown on launch with multiple rotating sets.
- Shareable affirmations (screenshot share).
- Home personalization via a calm preset background color palette.
- Offline-first local storage; no account needed.

## What it does not do (MVP)
- No medical claims or diagnosis.
- No employer or company dashboard.
- No social features.
- No external sentiment/news analysis.

## Core experience principles
- Calm and optional. No shame, no alarm.
- Transparent and simple. No hidden scoring.
- Local-first privacy by default.

## Behavior rules (MVP)
- Check-ins are saved locally and never sent to a server.
- Suggestions are phrased as options, not instructions.
- Trends show recent patterns, not judgments.
- If there is no recent data, show neutral empty states.

## Energy Windows (formerly “Microshifting” concept)
Energy Windows is a calm reflection layer that helps users notice short-term energy patterns without pressure, scoring, or advice — supporting flexible, human pacing rather than rigid schedules.

WellMind includes an Energy Windows section on the Home screen that provides a calm, non-judgmental summary of recent check-ins.

Energy Windows reflects simple patterns across the last 7 days (such as steadiness, variability, or mismatches between stress and rest) without offering advice, instructions, or diagnoses.

Its purpose is to help users notice when their energy naturally rises or dips across short windows of time, supporting flexible, human pacing rather than rigid daily optimization.

Energy Windows:
- Is read-only and informational.
- Uses normalizing language (“That happens,” “This can vary week to week”).
- Requires no action and makes no recommendations.
- Exists to reduce anxiety, not create it.
- Must never:
  - Tell the user what action to take.
  - Suggest productivity changes.
  - Compare the user to others.
  - Use alerting or warning language.

## MVP screens
- Home: daily check-in entry, weekly trends, gentle tips, today’s balance, resource links, personalize link.
- Check-In: sliders for energy/stress/focus/sleep, optional note, save or cancel.
- In-app browser: opens learn-more links.
- Gentle Reminder (modal): simple toggle + time for a daily nudge.
- Daily Affirmations (launch screen): rotating sets, share, favorites-first ordering.
- Settings (planned): reminder toggle, export/delete data.

## To do (near-term)
- Add a small “Daily wins” section where users can log small wins that gently support their overall score.
- Explore a lightweight “Daily goals” section for small, optional actions that make the day feel better.
- Add an “Inspiration” menu page with grounded, real-life quotes (not always cheerful; honest about struggle) and easy sharing.
- Explore an anonymous, opt-in collective score for company dashboards (aggregate only), e.g., if overall stress trends high, suggest lighter meeting cadence.
- Explore an anonymous, opt-in chat option with a counselor/therapist or HR representative for companies.
- Explore anonymous employee recommendations with gentle, privacy-first copy:
  - Helper text: “Share ideas for making work more sustainable. Your feedback is anonymous and aggregated.”
  - Helper text: “Offer a suggestion to your company—no names, no individual data.”
  - Helper text: “What would make this week feel better? We share only trends, never identities.”
  - Helper text: “Your voice stays anonymous. We only report group themes.”
  - Button text: “Share a suggestion”
  - Button text: “Send anonymous idea”
  - Button text: “Add a workplace note”
- Explore an anonymous, opt-in wellness recognition feature (e.g., team-level badges or shared milestones) that rewards participation without identifying individuals.
- Explore anonymous, opt-in points that a company could incentivize while keeping individual identities private.

## Future (not in MVP)
- Company mode with anonymized, aggregated signals only.

## Questions / Assumptions
- Are reminders required in MVP or can they be deferred to a later milestone?
- Should “Today’s balance” be shown only after a check-in, or with an empty state?
- Do we want a dedicated Trends screen separate from Home, or keep trends on Home only?
- What export format is expected (CSV, JSON, plain text)?
