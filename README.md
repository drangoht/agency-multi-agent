# 🧠 Agency Multi-Agent System — .NET Clean Architecture

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Docker](https://img.shields.io/badge/docker-ready-blue.svg)](https://www.docker.com/)
[![Vue.js](https://img.shields.io/badge/Vue.js-3.x-green.svg)](https://vuejs.org/)
[![SignalR](https://img.shields.io/badge/SignalR-realtime-orange.svg)](https://signalr.net/)

---

## 📘 Description

**Agency Multi-Agent System** is a demonstration project in **C# / .NET 8** showcasing a **local hierarchical multi-agent system**, following **S.O.L.I.D** principles and **Clean Architecture**.

Each agent represents a typical role in a **web agency**:   
👨‍💼 Product Manager → 👨‍💻 Developer → 🧪 Tester → 🚀 Release Manager

The system orchestrates communication between these agents sequentially and traceably through a **real-time web interface** built with **Vue.js 3** and **SignalR**.

---

## 🏗️ Project Architecture

```
Agency.sln
│
├── src/
│   ├── Agency.Domain/    # Business models (Entities, Records)
│   ├── Agency.Application/     # Interfaces, Services, Abstractions
│   ├── Agency.Infrastructure/  # Concrete implementations (Agents, Orchestrator)
│   └── Agency.Backend/    # ASP.NET Core API + SignalR + Vue.js Frontend
│       └── wwwroot/        # Vue.js Frontend Assets
│           ├── index.html      # Main Vue.js SPA
│           ├── css/    # Styles (TailwindCSS + Custom)
│           │   ├── main.css    # Custom animations & styles
│           │   └── tailwind.css # TailwindCSS source
│           └── js/           # Vue.js Components & Services
│               ├── app.js # Main Vue.js application
│           ├── components/ # Vue.js components
│           │   ├── ChatMessage.js
│           │   ├── ChatInput.js
│           │   └── LoadingIndicator.js
│           └── services/   # API & SignalR services
│                 ├── apiService.js
│                 └── signalRService.js
│
├── tests/
│   └── Agency.Tests/           # Unit tests (xUnit)
│
├── scripts/ # Setup and Docker automation
├── docker-compose.yml
└── README.md
```

This structure follows **Clean Architecture**:
- **Domain**: business core, without external dependencies.
- **Application**: abstract business logic (interfaces).
- **Infrastructure**: concrete implementations and orchestrator.
- **Backend (UI)**: API + Vue.js Frontend with real-time communication.

---

## ⚙️ How it works

### 🧩 Available agents
| Role | Description | ID |
|------|--------------|----|
| 👨‍💼 Product Manager | Defines project scope | `pm` |
| 👨‍💻 Developer | Implements features | `dev` |
| 🧪 Tester | Verifies deliverables | `tester` |
| 🚀 Release Manager | Prepares deployment | `rel` |

### 🔁 Orchestration
The `SimpleOrchestrator` makes agents interact hierarchically:

```
User Prompt
   ↓
Product Manager
   ↓
Developer
   ↓
Tester
   ↓
Release Manager
```

Each agent produces a message that is added to the global conversation.

---

## 💻 Running the project

### 🔸 Option 1 — With Docker (recommended)

#### 🧱 Prerequisites
- Docker & Docker Compose installed  
- Port `5000` free on your machine

#### 🚀 Startup
```bash
git clone https://github.com/drangoht/agency-multi-agent.git
cd agency-multi-agent
./scripts/setup.sh     # or .\scripts\setup.ps1 on Windows
```

#### 🌍 Access
Open [http://localhost:5000](http://localhost:5000) in your browser.

---

### 🔸 Option 2 — Local (without Docker)

#### 🧱 Prerequisites
- .NET 8 SDK
- Node.js 18+ (optional, for TailwindCSS compilation)

#### 🚀 Backend Setup
```bash
dotnet build
dotnet run --project src/Agency.Backend
```

The application will start at:
- HTTP: http://localhost:64275
- HTTPS: https://localhost:64274

#### 🎨 Frontend Setup (Vue.js)

The frontend is already integrated and served by the ASP.NET Core backend. The Vue.js interface provides:

**🌟 Key Features:**
- **Real-time conversation display**: Watch agents interact in a Teams-like interface
- **Message input**: Send initial prompts to start agent conversations 
- **Progressive message loading**: See agent responses appear one by one (1 second delay between messages)
- **Responsive design**: Clean, modern UI with color-coded agent roles
- **Animations**: Smooth message transitions and loading indicators

**🎨 UI Design:**
- 🟦 User messages (right-aligned, blue background)
- 🟪 Product Manager (purple background)
- 🟩 Developer (green background) 
- 🟨 Tester (yellow background)
- 🟦 Release Manager (blue background)

**🛠️ Frontend Technologies:**
- Vue.js 3 (Composition API) via CDN
- TailwindCSS for styling 
- SignalR for real-time communication
- No build process required (everything served from CDN)

**📂 Frontend File Structure:**
```
src/Agency.Backend/wwwroot/
├── index.html# Main Vue.js SPA
├── css/
│   ├── main.css    # Custom styles, animations, scrollbar
│   └── tailwind.css        # TailwindCSS source (optional)
└── js/
    ├── app.js              # Main Vue.js application
    ├── components/     # Reusable Vue.js components
    │   ├── ChatMessage.js  # Message display component
    │   ├── ChatInput.js    # Message input component
    │   └── LoadingIndicator.js # Loading animation
    └── services/         # API communication services
        ├── apiService.js   # HTTP API calls
        └── signalRService.js # SignalR real-time connection
```

**🔧 For Development:**
If you want to modify the frontend, simply edit the files in `wwwroot/`. The application will serve them automatically. No compilation step needed thanks to CDN-based Vue.js.

**Optional TailwindCSS Setup:**
```bash
cd src/Agency.Backend
npm install
npm run css:build    # Generate production CSS
npm run css:watch    # Watch mode for development
```

---

### 🌍 Interface Features

You'll see a Teams-like interface allowing you to:
- Enter an **initial prompt** in the message input
- Start **agent conversations** by clicking Send
- Follow **real-time exchanges** via SignalR with progressive message display
- See **agent responses appear one by one** with 1 second intervals
- **Color-coded messages** for easy identification of speaker roles
- **Smooth animations** for message transitions
- **Auto-scroll** to follow the conversation

---

## 🧪 Tests

Simple unit tests are provided in `Agency.Tests`:

```bash
dotnet test
```

Examples:
- Verifies that multi-agent conversation produces coherent messages
- Confirms that Product Manager and Developer participated in the session
- Tests orchestrator flow and agent interactions

---

## 🧰 Technologies used

| Category | Tools / Frameworks |
|------------|---------------------|
| Backend | .NET 8, ASP.NET Core Minimal API |
| Frontend | Vue.js 3, TailwindCSS, JavaScript ES6+ |
| Communication | SignalR (real-time) |
| Tests | xUnit |
| Containerization | Docker, Docker Compose |
| Architecture | SOLID, Clean Architecture |
| Language | C# 12, JavaScript |

---

## 🧱 Architecture Schema

```
+------------------+         +-------------------+
|    Frontend      | <-----> |     Backend API   |
|(Vue.js + SignalR)|         | (ASP.NET Core)    |
+------------------+         +---------+---------+
                                       |
                                       ↓
                            +---------------------+
                            |   Orchestrator      |
                            | (SimpleOrchestrator)|
                            +----------+----------+
                                       ↓
--------------------------------------------------------
   ↓                ↓            ↓              ↓
  ProductMgr    Developer     Tester        ReleaseMgr
    (pm)      (dev)          (tester)        (rel)
```

---

## 🔧 SignalR Communication Flow

1. **User Input**: User types message and clicks Send
2. **Frontend**: Vue.js sends message via SignalR `StartConversation`
3. **Backend**: AgentsHub receives message and starts orchestration
4. **Orchestration**: SimpleOrchestrator processes agents sequentially
5. **Real-time Updates**: Each agent response sent via `ReceiveMessage` event
6. **Frontend Display**: Messages appear progressively with animations

---

## 🧩 Possible improvements

- [ ] Integrate local LLM (e.g. **Ollama**, **LM Studio**) to give real reasoning to agents
- [ ] Add **React or Blazor UI** with hierarchical agent display
- [ ] Make agents communicate via **RabbitMQ** or **Redis Pub/Sub**
- [ ] Separate each agent in its own **Docker container**
- [ ] Implement smarter orchestrator (planning, feedback loops)
- [ ] Add **TypeScript** for better frontend type safety
- [ ] Implement **Vue.js build process** with Vite for production
- [ ] Add **unit tests** for frontend components
- [ ] Add **message persistence** with database storage
- [ ] Implement **user authentication** and **conversation history**

---

## 🧑‍💻 Contribution

1. Fork the repo  
2. Create a branch (`feature/my-feature`)  
3. Commit (`git commit -m "Add feature X"`)  
4. Push (`git push origin feature/my-feature`)  
5. Create a **Pull Request**

---

## 📄 License

This project is distributed under **MIT** license.  
You are free to use, modify and redistribute it with author attribution.

---

## ❤️ Acknowledgments

Thanks to passionate .NET developers exploring **multi-agent systems** and **distributed AI**.  
This project is an **educational and experimental foundation** for building local, reproducible and controlled AI architectures.

---

### 🧭 Author
**Grégory Thognard**  
💼 .NET Developer & AI Enthusiast  
📧 _contact via GitHub_

> "Build local, think distributed — empower your agents." 🚀
