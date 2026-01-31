# Task & Workflow Management API

Lightweight Task/Project Management API (.NET 8, EF Core, SQL Server, Swagger).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (e.g. **XIT037\\MSSQLSERVER2022**) — used in Development; see Section 2.

---

## Run and test

```bash
dotnet restore
dotnet build
dotnet run
```

Open **http://localhost:5000/swagger** (or the URL shown in the console) to explore and test the API.

---

## Section 2: Models + DbContext + Database

- **Database name:** `Ruchi_TaskWorkflowDb` (prefix `Ruchi_` as requested).
- **Connection:** Configured in `appsettings.Development.json` for Development (Server: `XIT037\MSSQLSERVER2022`, login: `test.practical`). See `docs/SECTION_2_SETUP.md` for details.

### Create the database

**Option A — SQL script (easiest):**  
1. Open **SSMS**, connect to **XIT037\MSSQLSERVER2022** as **test.practical**.  
2. Open **scripts/CreateDatabase_Ruchi_TaskWorkflowDb.sql** and run it (F5).  
3. Refresh Databases; **Ruchi_TaskWorkflowDb** should appear with **Projects** and **TaskItems**.

**Option B — EF Core:**  
```bash
dotnet restore
dotnet build
dotnet tool install --global dotnet-ef   # once, if needed
dotnet ef database update
```

Then run the API: `dotnet run`. The app should start and connect to **Ruchi_TaskWorkflowDb**.

### Verify Section 2

1. In SSMS (or similar), connect to your SQL Server and confirm database **Ruchi_TaskWorkflowDb** exists with tables **Projects** and **TaskItems**.
2. Run the API; no DB connection errors should appear.

---

---

## Section 3: Project APIs

- **POST /api/projects** — Create project (body: `{ "name": "...", "description": "..." }`).
- **GET /api/projects** — Get all projects.
- **GET /api/projects/{projectId}/tasks** — Get tasks by project.
- **GET /api/projects/{id}** — Get project by ID.
- **DELETE /api/projects/{id}** — Delete project. Returns 400 if project has active tasks (Todo or InProgress).

---

## Section 4: Task APIs

- **POST /api/tasks** — Create task (body: `projectId`, `title`, optional `description`, `priority`, `dueDate`). Status defaults to Todo. Returns 404 if project not found.
- **GET /api/projects/{projectId}/tasks** — Get all tasks for a project (on Projects controller).
- **GET /api/tasks/{id}** — Get task by ID.
- **DELETE /api/tasks/{id}** — Delete task (200 with message, or 404).

---

## Section 5: Update Task

- **PUT /api/tasks/{id}** — Update task (partial). Body: optional `title`, `description`, `status` (0=Todo, 1=InProgress, 2=Done), `priority` (0=Low, 1=Medium, 2=High), `dueDate`. Only sent fields are updated.

---

## Section 6: Business Rules

- **Status workflow:** Allowed: Todo→InProgress, InProgress→Done, InProgress→Todo. **Cannot move from Done** (returns 400 with message).
- **Due date:** Cannot be in the past on create or update (returns 400: "Due date cannot be in the past.").

---

## Section 7: Delete Project Rule

- **Delete project:** Project cannot be deleted if it has **active tasks** (Todo or InProgress). If any task is not Done, DELETE returns **400**. If all tasks are Done (or no tasks), project and its tasks are deleted.

---

## Section 8: Priority Cap (Optional Bonus)

- **Max 3 High-priority tasks per project:** On **Create** or **Update** task, if priority is set to **High** (2) and the project already has 3 High-priority tasks, the API returns **400** with message: "Only 3 High-priority tasks are allowed per project." (On Update, the current task is excluded from the count when it is already High.)

### Verify Section 8

1. Create a project. Create 3 tasks with `"priority": 2` (High) → all succeed.
2. Create a 4th task with `"priority": 2` for the same project → **400**.
3. Update one of the 3 High-priority tasks to Medium, then create or update another task to High → succeeds.

---

## Project structure

```
Task_and_Workflow_Management_API/
├── Controllers/
│   ├── ProjectsController.cs
│   └── TasksController.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   ├── ErrorDtos.cs
│   ├── ProjectDtos.cs
│   └── TaskDtos.cs
├── Services/
│   ├── IProjectService.cs
│   ├── ProjectService.cs
│   ├── ITaskService.cs
│   ├── TaskService.cs
│   └── Result.cs
├── Models/
│   ├── TaskStatus.cs      # enum
│   ├── TaskPriority.cs    # enum
│   ├── Project.cs         # entity
│   └── TaskItem.cs        # entity
├── Properties/
│   └── launchSettings.json
├── docs/
│   ├── WORKFLOW_AND_DATA_STRUCTURE.md
│   └── SECTION_2_SETUP.md
├── scripts/
│   ├── CreateDatabase_Ruchi_TaskWorkflowDb.sql
│   └── CreateTables_Only.sql
├── Migrations/
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── TaskWorkflowApi.csproj
```

---

---

## Conventions

- **Controllers:** Thin; delegate to services and map results to HTTP (404/400/200). `[ApiController]`, `[Route("api/[controller]")]`.
- **Services:** Business logic, validation, and data access. `IProjectService`/`ProjectService`, `ITaskService`/`TaskService` in `Services/`. Return `Result<T>` for operations that can fail (not found, validation).
- **DTOs:** Request/response records in `DTOs/`; validation via data annotations.
- **Models:** Entities and enums in `Models/`; EF Core and data annotations.
- **Data:** Single `AppDbContext` in `Data/`; SQL Server via connection string.

See **docs/API_DOCUMENTATION.md** for a short API summary, and `docs/` for workflow and data structure.
