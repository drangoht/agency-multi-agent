# 🧠 Agency Multi-Agent System — .NET Clean Architecture

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Docker](https://img.shields.io/badge/docker-ready-blue.svg)](https://www.docker.com/)
[![Build](https://img.shields.io/badge/build-passing-brightgreen)]()
[![codecov](https://codecov.io/gh/drangoht/agency-multi-agent/branch/master/graph/badge.svg)](https://codecov.io/gh/drangoht/agency-multi-agent)

---

## 📘 Description

**Agency Multi-Agent System** est un projet démonstratif en **C# / .NET 8** illustrant un système **multi-agent hiérarchique local**, suivant les principes **S.O.L.I.D** et **Clean Architecture**.

Chaque agent représente un rôle typique d'une **web agency** :  
👨‍💼 Product Manager → 👨‍💻 Developer → 🧪 Tester → 🚀 Release Manager

Le système orchestre la communication entre ces agents de manière séquentielle et traçable via une **interface web temps réel** (SignalR).

---

## 🏗️ Architecture du projet

```
Agency.sln
│
├── src/
│   ├── Agency.Domain/          # Modèles métier (Entities, Records)
│   ├── Agency.Application/     # Interfaces, Services, Abstractions
│   ├── Agency.Infrastructure/  # Implémentations concrètes (Agents, Orchestrator)
│   └── Agency.Backend/         # API ASP.NET Core + SignalR + Front minimal
│
├── tests/
│   └── Agency.Tests/           # Tests unitaires (xUnit)
│
├── scripts/                    # Setup et automatisation Docker
├── docker-compose.yml
└── README.md
```

Cette structure respecte la **Clean Architecture** :
- **Domain** : cœur métier, sans dépendances externes.
- **Application** : logique métier abstraite (interfaces).
- **Infrastructure** : implémentations concrètes et orchestrateur.
- **Backend (UI)** : API + Frontend minimal (SignalR/HTML).

---

## ⚙️ Fonctionnement

### 🧩 Agents disponibles
| Rôle | Description | ID |
|------|--------------|----|
| 👨‍💼 Product Manager | Définit le périmètre du projet | `pm` |
| 👨‍💻 Developer | Implémente les fonctionnalités | `dev` |
| 🧪 Tester | Vérifie les livrables | `tester` |
| 🚀 Release Manager | Prépare le déploiement | `rel` |

### 🔁 Orchestration
Le `SimpleOrchestrator` fait dialoguer les agents de manière hiérarchique :

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

Chaque agent produit un message qui s’ajoute à la conversation globale.

---

## 💻 Lancer le projet

### 🔸 Option 1 — Avec Docker (recommandé)

#### 🧱 Prérequis
- Docker & Docker Compose installés  
- Port `5000` libre sur votre machine

#### 🚀 Démarrage
```bash
git clone https://github.com/drangoht/agency-multi-agent.git
cd agency-multi-agent
./scripts/setup.sh     # ou .\scripts\setup.ps1 sur Windows
```

#### 🌍 Accès
Ouvrez [http://localhost:5000](http://localhost:5000) dans votre navigateur.

Vous verrez une interface simple permettant :
- de saisir un **prompt initial**,
- de lancer la **conversation des agents**,
- et de suivre les **échanges en temps réel** via SignalR.

---

### 🔸 Option 2 — En local (sans Docker)
```bash
dotnet build
dotnet run --project src/Agency.Backend
```
Puis ouvrez [http://localhost:5000](http://localhost:5000).

---

## 🧪 Tests

Des tests unitaires simples sont fournis dans `Agency.Tests` :

```bash
dotnet test
```

Exemple :
- Vérifie que la conversation multi-agent produit bien des messages cohérents.
- Confirme que le Product Manager et le Developer ont bien participé à la session.

---

## 🧰 Technologies utilisées

| Catégorie | Outils / Frameworks |
|------------|---------------------|
| Backend | .NET 8, ASP.NET Core Minimal API |
| Communication | SignalR (temps réel) |
| Tests | xUnit |
| Conteneurisation | Docker, Docker Compose |
| Architecture | SOLID, Clean Architecture |
| Langage | C# 12 |

---

## 🧱 Schéma d’architecture

```
+------------------+         +-------------------+
|    Frontend      | <-----> |     Backend API   |
| (SignalR Client) |         | (ASP.NET Core)    |
+------------------+         +---------+---------+
                                      |
                                      ↓
                            +---------------------+
                            |   Orchestrator      |
                            |  (SimpleOrchestrator)|
                            +----------+----------+
                                       ↓
        --------------------------------------------------------
        ↓          ↓               ↓                ↓
  ProductMgr    Developer        Tester        ReleaseMgr
    (pm)          (dev)          (tester)        (rel)
```

---

## 🧩 Améliorations possibles

- [ ] Intégrer un LLM local (ex. **Ollama**, **LM Studio**) pour donner un vrai raisonnement aux agents.  
- [ ] Ajouter une **UI React ou Blazor** avec affichage hiérarchique des agents.  
- [ ] Faire communiquer les agents via **RabbitMQ** ou **Redis Pub/Sub**.  
- [ ] Séparer chaque agent dans son propre **conteneur Docker**.  
- [ ] Implémenter un orchestrateur plus intelligent (planification, feedback loops).  

---

## 🧑‍💻 Contribution

1. Forker le repo  
2. Créer une branche (`feature/ma-fonctionnalite`)  
3. Commit (`git commit -m "Ajout de la fonctionnalité X"`)  
4. Push (`git push origin feature/ma-fonctionnalite`)  
5. Créer une **Pull Request**

---

## 📄 Licence

Ce projet est distribué sous licence **MIT**.  
Vous êtes libre de l’utiliser, le modifier et le redistribuer avec mention d’auteur.

---

## ❤️ Remerciements

Merci aux développeurs .NET passionnés qui explorent les **systèmes multi-agents** et l’**IA distribuée**.  
Ce projet est un **socle éducatif et expérimental** pour bâtir des architectures IA locales, reproductibles et maîtrisées.

---

### 🧭 Auteur
**Grégory Thognard**  
💼 .NET Developer & AI Enthusiast  
📧 _contact via GitHub_

> “Build local, think distributed — empower your agents.” 🚀
