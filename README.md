<div align="center">
   <img width="100" height="100" alt="kronos-app-logo" title="Kronos | Roster Orchestration Agent" src="https://github.com/user-attachments/assets/28d1ef66-3b9c-4151-9dda-f36d3451b978" />
  
# KronOS | Ward Roster Lab

![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23662D91.svg?style=for-the-badge&logo=cshrp&logoColor=white)
![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-044A64?style=for-the-badge&logo=sqlite&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-384d54?style=for-the-badge&logo=docker&logoColor=0db7ed)
![Ollama](https://img.shields.io/badge/Ollama-000000?style=for-the-badge&logo=ollama&logoColor=white)

~✦~

`KronOS` is a small AI agent that automatically builds a hospital ward’s roster on a timer (fortnightly in real use, every 2 minutes for demos). A local LLM (Ollama) proposes the shifts, and a C# rules engine checks capacity (AM 5, PM 4, Night 3) and skillmix, retrying or falling back to a rule-based roster if the proposal is wrong.

</div>

## Branch Notes: `feature/ward-roster-agent-lab`

**Branched from:** `dev`

**Merges into:** `dev`

**Goal**
A simple agent lab that updates a single ward's roster on a timer. Real cadence is fortnightly (`14.00:00:00`); demo cadence is every 2 minutes (`00:02:00`). Staff names and dates are hardcoded for now.

> The LLM proposes, plain C# validates, retry up to 3 times, then fall back to a deterministic greedy roster.

**Configuration**
| Setting | Real | Demo |
|---|---|---|
| `Roster:Interval` | `14.00:00:00` | `00:02:00` |
| `Roster:WindowDays` | 14 | 3 |
| `Ollama:Model` | `llama3.1:8b` | `llama3.2:3b` |

**Constraints implemented**

- Timeslot capacity: AM = 5, PM = 4, Night = 3
- Skillmix: every shift needs at least one RN or SeniorRN; Night needs at least one SeniorRN
- One shift per person per day

**Known limits / out of scope**

- No days off, leave, or preferences
- No UI or API
- Single ward, hardcoded staff
- Regenerates the window each run instead of rolling it forward
- SQLite only; Postgres swap is a later branch

---

Created by Arsenie — 2026
