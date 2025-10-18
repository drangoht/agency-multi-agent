## Agency Multi-Agent — Copilot instructions

Quick, targeted guidance for AI coding agents working in this repository. Keep edits small and explicit.

- Project layout (Clean Architecture):
  - `src/Agency.Domain/` — domain models: `AgentDescriptor`, `AgentMessage`.
  - `src/Agency.Application/` — interfaces and services: `IAgent`, `IAgentOrchestrator`, `IConversationStore`, `IOllamaClient`.
  - `src/Agency.Infrastructure/` — concrete agents and the orchestrator (`SimpleOrchestrator`), agent implementations live under `Agents/` and orchestrator under `Orchestrator/`.
  - `src/Agency.Backend/` — ASP.NET Core Minimal API, SignalR hub (`AgentsHub`), static `index.html` UI.

- Big-picture flow:
  - User calls `/api/start` (POST with JSON `{ "InitialPrompt": "..." }`).
  - `SimpleOrchestrator.StartConversationAsync` drives a linear conversation:
    1. Product Manager -> adds PM message to `IConversationStore`.
    2. (Expect) an "OllamaGuy" agent call (or adjust orchestrator if not present).
    3. Developer responds.
    4. Tester writes tests.
    5. Release Manager prepares release.
  - Conversation state is held in `InMemoryConversationStore` and exposed via `/api/conversation`.
  - Frontend connects to SignalR hub at `/agentsHub` for real-time updates.

- Important code patterns & conventions (use these exact types/strings):
  - Agents implement `IAgent` and expose a `Descriptor` (id, role, parent). The orchestrator selects agents by `Descriptor.Role` strings such as `ProductManager`, `Developer`, `Tester`, `ReleaseManager` (and currently `OllamaGuy` — see gotcha).
  - LLM-backed agents derive from `LLMAgentBase` (in `Infrastructure/Agents`) and build a single prompt by combining a `SystemPrompt`, optional `instruction`, and last ~12 messages from conversation.
  - HTTP-based LLM calls use `IOllamaClient` / `OllamaClient` configured from `appsettings.json` (`Ollama:BaseUrl`, `Model`, `TimeoutSeconds`). Default base URL: `http://localhost:11434/api/generate`.
  - `Program.cs` registers services: agents as transient, `IAgentOrchestrator` as singleton, `IConversationStore` as singleton, and `IOllamaClient` via `AddHttpClient`.

- Integration & runtime notes:
  - Docker compose maps host `5000` -> container `80`. To run with Docker: `./scripts/setup.sh` (Linux/macOS) or `.\\scripts\\setup.ps1` (Windows PowerShell), then open `http://localhost:5000`.
  - To run locally without Docker (PowerShell):

```powershell
dotnet build
dotnet run --project src/Agency.Backend
```

  - Endpoints of interest:
    - GET `/api/agents` — returns agent descriptors.
    - GET `/api/conversation` — returns stored conversation messages.
    - POST `/api/start` — start conversation: JSON body `{"InitialPrompt":"..."}`.

- Tests & quick checks:
  - Tests are xUnit under `tests/Agency.Tests`. Run with `dotnet test` from repository root.
  - The unit test `OrchestratorTests` constructs an in-memory store and checks that messages from `pm` and `dev` exist. Use that test as a minimal spec for orchestrator behavior.

- Known gotchas & things to verify before code changes:
  - `SimpleOrchestrator` expects an agent with `Descriptor.Role == "OllamaGuy"` — there may be no concrete `OllamaGuy` class present. Either add/register an agent with that role or modify the orchestrator flow.
  - Some agent implementations in `Infrastructure/Agents` have differing constructor shapes (some expect `IHttpClientFactory`, others `IOllamaClient`). When adding or modifying agents, ensure DI registrations in `Program.cs` match constructors.
  - Tests may use parameterless constructors for convenience in older snapshots. Prefer updating tests to use DI-friendly constructors or provide test doubles/mocks for `IOllamaClient`/`IHttpClientFactory`.

- Small examples (copyable):
  - Start a conversation (PowerShell curl):

```powershell
Invoke-RestMethod -Method POST -Uri http://localhost:5000/api/start -Body (@{ InitialPrompt = 'Add a greeting feature' } | ConvertTo-Json) -ContentType 'application/json'
```

  - Sample `appsettings.json` values live in `src/Agency.Backend/appsettings.json` (Ollama defaults)

- When editing agents or orchestrator prefer small, focused changes:
  - Keep messages immutable (`AgentMessage`), append to `IConversationStore` only when agent returns non-null.
  - Preserve the sequential ordering performed by `SimpleOrchestrator` unless intentionally introducing concurrency — concurrency changes require rethinking `IConversationStore` (from in-memory list to thread-safe/persistent store).

If anything above is unclear or you want the file to include example unit tests or CI steps, tell me what to add and I will iterate.
