<div align="center">
   <img width="100" height="100" alt="kronos-app-logo" title="Kronos | Roster Orchestration Agent" src="https://github.com/user-attachments/assets/28d1ef66-3b9c-4151-9dda-f36d3451b978" />
  
# KronOS | Ward Roster Lab

![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23662D91.svg?style=for-the-badge&logo=cshrp&logoColor=white)
![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-044A64?style=for-the-badge&logo=sqlite&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-384d54?style=for-the-badge&logo=docker&logoColor=0db7ed)
![Ollama](https://img.shields.io/badge/Ollama-000000?style=for-the-badge&logo=ollama&logoColor=white)

~✦~

[![Demo](https://img.shields.io/badge/YouTube-Recorded%20Demo-181717?style=for-the-badge&logo=youtube)](https://youtu.be/1hNatlNmlsc)

`KronOS` is a small AI agent that automatically builds a hospital ward’s roster on a timer (fortnightly in real use, every 2 minutes for demos). A local LLM (Ollama) proposes the shifts, and a C# rules engine checks capacity (AM 5, PM 4, Night 3) and skillmix, retrying or falling back to a rule-based roster if the proposal is wrong.

</div>

## Branch Notes: `lab/roster-lab`

**Branched from:** `dev`

**Merges into:** `dev`

**Purpose**
A proof of concept for an agentic roster updater for a single ward. On a timer, an LLM proposes a roster. Deterministic C# rules check it, and the agent retries until it’s valid. This is the base for later work, not a finished product.

#### **Details**

- **Scope**: single ward, hardcoded staff (18 names with skill levels) and hardcoded rules.
- **Constraints**: slot capacity `AM=5`, `PM=4`, `Night=3`. Every shift has at least one RN or SeniorRN. Night has at least one SeniorRN. Nobody works two shifts in a day.
- **Agent loop**: generate one day at a time with a local Ollama model, validate, and feed violations back for up to 3 retries. If it still fails, use the deterministic greedy fallback.
- **Schedule**: configurable with `Roster:Interval`. The demo uses 2 minutes. Real use is a fortnight (`14.00:00:00`).
- Window: `Roster:WindowDays`, 3 for the demo and 14 for production.
- **Stack**: C# .NET 8 Worker Service, EF Core with SQLite, OllamaSharp / Microsoft.Extensions.AI, Docker Compose (agent and ollama).
- **Persistence**: assignments are regenerated each run, and a `RosterRun` row records attempts and whether the fallback was used.

**Status**: Merged to `dev`

## UI Snapshots

Illustration of agentic roster updates over simulated time, compressing a 14‑day period into 2‑minute intervals.

<table>
   <tr align="center">
      <td><img width="400"alt="Screenshot 2026-10-30 024149" title="First Agent Run" src="https://github.com/user-attachments/assets/9fdc8be2-3fe5-47e1-bf3f-868679dd74e8" /></td>
      <td><img width="400" alt="Screenshot 2026-10-30 024241" title="Second Agent Run" src="https://github.com/user-attachments/assets/56d74219-6b45-4af9-97cb-53577d197f26" /></td>
   </tr>
   <tr align="center">
      <td>First run: 0 Minute (Day 0)</td>
      <td>Second run: 2 Minutes later (Day 14)</td>
   </tr>
</table>

## **Design decision**

> The LLM only proposes and C# decides. Small local models miscount, so correctness lives in the validator, not the prompt.

## **How to run**

> Pull the model first: ollama pull llama3.2:3b.

```bash
dotnet run
```

or

```bash
docker compose up --build
```

## **Known limitations**

- No leave, preferences, fairness balancing, or rest-day rules.
- Each run replaces the previous roster with no history or versioning.
- Day-by-day generation ignores the previous day’s shifts (for example Night followed by AM).
- SQLite only, single process, no auth or UI.

## **Next steps**

- Rest rules (no Night followed by AM).
- Fairness and workload balancing.
- Leave and availability.
- Postgres.
- Run history and a diff view.
- A roster viewer or API.
- Tests for the retry loop with a mocked `IChatClient`.

---

Created by Arsenie — 2026
