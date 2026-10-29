<div align="center">
   <img width="100" height="100" alt="kronos-app-logo" title="Kronos | Roster Orchestration Agent" src="https://github.com/user-attachments/assets/28d1ef66-3b9c-4151-9dda-f36d3451b978" />
  
# Ward Roster Lab

![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23662D91.svg?style=for-the-badge&logo=cshrp&logoColor=white)
![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-044A64?style=for-the-badge&logo=sqlite&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-384d54?style=for-the-badge&logo=docker&logoColor=0db7ed)
![Ollama](https://img.shields.io/badge/Ollama-000000?style=for-the-badge&logo=ollama&logoColor=white)

~✦~

</div>

## Branch Notes: `dev`

**Purpose**: Shared integration branch. Holds the latest working state of RosterLab. Feature branches are merged here first. Nothing is released from here directly.

**Rules**

- Always builds (`dotnet build`) and tests pass (`dotnet test`).
- No direct commits except trivial fixes (docs, typos). Use a feature branch and merge.
- Merge feature branches with a PR (or `--no-ff` merge if working solo) so history shows each feature.
- Delete the feature branch after merging.

**Contents**

- .NET 8 Worker Service skeleton
- SQLite database with seeded staff
- Roster rules and validator
- Ollama-backed roster agent with retry and greedy fallback
- Dockerfile and docker-compose (Ollama + agent)

---

Created by Arsenie — 2026
