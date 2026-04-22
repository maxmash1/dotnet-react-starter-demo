---
name: app-runner
description: 'Start the full-stack application (backend + frontend). Use when asked to "start the app", "run the app", "start the server", "start in dev", "start in prod", "launch the app", "run backend", "run frontend". Handles dev mode (Swagger enabled) and prod mode (Swagger disabled).'
---

# App Runner

Start the dotnet-react-starter application — backend (.NET 8) and frontend (React + Vite).

## When to Use

- User says "start the app", "run the app", "launch the app"
- User says "start the app in dev" or "start the app in prod"
- User says "start the backend" or "start the frontend"

## Procedure

**IMPORTANT — Terminal mode:** All commands in this skill MUST use `mode=sync` (NOT `mode=async`). The scripts background the server processes internally and exit cleanly. This prevents the chat agent from monitoring the terminals after startup.

### 1. Determine Backend Mode

| User says | Mode |
|-----------|------|
| "start the app **in dev**" or "dev mode" | `dev` — sets `ASPNETCORE_ENVIRONMENT=Development`, enables Swagger UI |
| "start the app **in prod**" or "prod mode" | `prod` — sets `ASPNETCORE_ENVIRONMENT=Production`, Swagger disabled |
| "start the app" *(no mode specified)* | **Ask the user**: "Do you want to start the backend in **dev** (Swagger enabled) or **prod** mode?" |

### 2. Start the Backend

Run the backend start script using `run_in_terminal` with **`mode=sync`** and a **timeout of 45000ms**:

```bash
.github/skills/app-runner/scripts/start-backend.sh dev   # or prod
```

The script backgrounds the dotnet process, polls the health endpoint until ready, prints a confirmation line, and **exits**. Because the command exits, the terminal is no longer monitored.

Wait for the script to print "Backend is ready" before proceeding.

### 3. Start the Frontend

Run the frontend start script using `run_in_terminal` with **`mode=sync`** and a **timeout of 30000ms**:

```bash
.github/skills/app-runner/scripts/start-frontend.sh
```

The script backgrounds the Vite process, polls until the dev server responds, prints a confirmation line, and **exits**.

Wait for the script to print "Frontend is ready" before proceeding.

### 4. Display Summary

After both terminals are running, display the following output using **exactly** this format (ASCII art banner + endpoint table + stop commands):

```
 ____  _   _ _   _ _   _ ___ _   _  ____
|  _ \| | | | \ | | \ | |_ _| \ | |/ ___|
| |_) | | | |  \| |  \| || ||  \| | |  _
|  _ <| |_| | |\  | |\  || || |\  | |_| |
|_| \_\\___/|_| \_|_| \_|___|_| \_|\____|
```

Then show this table:

| Service  | URL                           | Notes                        |
|----------|-------------------------------|------------------------------|
| Backend  | http://localhost:5000          | .NET 8 Web API               |
| Swagger  | http://localhost:5000/swagger  | *(dev mode only)*            |
| Frontend | http://localhost:5173          | React + Vite (proxies `/v1`) |

- If **dev mode**: include the Swagger row.
- If **prod mode**: omit the Swagger row and note "Swagger disabled (prod mode)".

Immediately after the table, show this block so the user can stop the servers cleanly later:

```bash
# Stop both
.github/skills/app-runner/scripts/stop-servers.sh

# Stop backend only
.github/skills/app-runner/scripts/stop-servers.sh backend

# Stop frontend only
.github/skills/app-runner/scripts/stop-servers.sh frontend
```

### 5. Done — No Further Monitoring

The scripts background the servers and exit. The terminal commands are **complete** — there is nothing left to monitor. Do NOT check terminal output, poll URLs, or respond to any terminal notifications after showing the summary. The skill is finished.

If the user later asks to stop the servers, run the same commands already shown in the summary:

```bash
.github/skills/app-runner/scripts/stop-servers.sh          # stops both
.github/skills/app-runner/scripts/stop-servers.sh backend   # backend only
.github/skills/app-runner/scripts/stop-servers.sh frontend  # frontend only
```

To view server logs:

```bash
cat .github/skills/app-runner/logs/backend.log   # backend logs
cat .github/skills/app-runner/logs/frontend.log  # frontend logs
```

### 6. Additional Notes

- The Vite dev server proxies `/v1/*` requests to `http://localhost:5000`, so the frontend can reach the API without CORS issues in local development.
- If the user only asks to start the backend or only the frontend, start just that service.
- Always confirm both services are listening before showing the summary.
