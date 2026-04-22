# dotnet-react-starter

A minimal full-stack application template for demonstrating GitHub Copilot capabilities.

> **📋 Note:** This is a **demo/starter template** designed to showcase how organizations can configure and leverage GitHub Copilot (instructions, prompts, skills, and agents) within a real-world project structure. The development standards, patterns, and configurations included here are **illustrative examples** — they do not represent any proprietary or organization-specific standards. Feel free to use this as a template and adapt the standards to your own team's needs.

## Overview

- **Backend:** .NET 8 Web API with Entity Framework Core
- **Frontend:** React 18 + TypeScript + Vite + Tailwind CSS
- **Testing:** xUnit + Moq (backend)

## Getting Started with Codespaces (Recommended)

The fastest way to get started — no local setup required.

1. Click the green **"Use this template"** button → **Create a new repository**
2. In your new repo, click **Code** → **Codespaces** → **Create codespace on main**
3. Wait for the environment to build (installs .NET 8 SDK, Node.js 20, and all dependencies automatically)
4. Once ready, start the backend and frontend:

```bash
# Terminal 1 — Backend
cd backend
dotnet run --project src/Api
```

```bash
# Terminal 2 — Frontend
cd frontend
npm run dev
```

- **Backend API:** Port `5000` (Swagger UI at `/swagger`)
- **Frontend:** Port `5173` (auto-opens in preview)

> Both ports are auto-forwarded. Codespaces will show notifications when they're ready.

## Local Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)

### Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/Api
```

The API will be available at `http://localhost:5000` with Swagger UI at `/swagger`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`.

## Project Structure

```
├── .devcontainer/             # Codespaces / Dev Container config
├── backend/
│   ├── src/Api/               # Web API project
│   └── tests/Api.Tests/       # Unit tests
├── frontend/                  # React + TypeScript application
├── legacy/                    # Classic ASP files (for migration demo)
└── .github/
    ├── copilot-instructions.md  # Organization standards
    ├── agents/                   # Copilot agents
    ├── prompts/                  # Reusable prompts
    └── skills/                   # Copilot skills
```

## API Endpoints

| Method | Endpoint     | Description              |
|--------|--------------|--------------------------|
| GET    | /v1/health   | Health check endpoint    |

## Development Standards

See `.github/copilot-instructions.md` for complete development standards including:
- API design patterns (envelope responses, naming conventions)
- C# coding standards (async/await, DTOs, repositories, services)
- Testing standards (naming, AAA pattern, organization)
- Frontend standards (TypeScript, Tailwind)

## License

MIT

## Sample prompt

I was working on another app and I loved the styling so much I want to apply it to this one, modify the front-end completely based on the file is in the directory called design-system.instructions.md. The stack may be different so make sure you appropriate styling here based on this application's stack. Create this in a demo-1 branch, if it works well I'll create a separate one to show it off in another demo