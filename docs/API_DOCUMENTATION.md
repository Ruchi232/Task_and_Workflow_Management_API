# Task & Workflow Management API — Short Documentation

## Overview

Lightweight Task/Project Management API (mini Jira/Trello-style backend). Manages **Projects** and **Tasks** with status workflow, priority rules, and structured REST APIs.

---

## Technology Stack

| Layer        | Technology              |
|-------------|-------------------------|
| Framework   | .NET 8 Web API         |
| ORM         | Entity Framework Core 8 |
| Database    | SQL Server              |
| API docs    | Swagger / OpenAPI       |
| Logging     | ILogger (built-in)      |

---

## Data Model

- **Project:** Id, Name, Description, CreatedAt. One project has many tasks.
- **TaskItem:** Id, ProjectId, Title, Description, **Status** (Todo / InProgress / Done), **Priority** (Low / Medium / High), DueDate.

---

## API Endpoints

### Projects

| Method   | Endpoint                        | Description                          |
|----------|----------------------------------|--------------------------------------|
| POST     | /api/projects                   | Create project                       |
| GET      | /api/projects                   | Get all projects                     |
| GET      | /api/projects/{id}              | Get project by ID                    |
| GET      | /api/projects/{projectId}/tasks| Get tasks by project                 |
| DELETE   | /api/projects/{id}              | Delete project (blocked if active tasks) |

### Tasks

| Method   | Endpoint        | Description                |
|----------|-----------------|----------------------------|
| POST     | /api/tasks      | Create task (Status = Todo)|
| GET      | /api/tasks/{id} | Get task by ID             |
| PUT      | /api/tasks/{id} | Update task (partial)      |
| DELETE   | /api/tasks/{id} | Delete task                |

**Enums in request/response:** Status = 0 (Todo), 1 (InProgress), 2 (Done). Priority = 0 (Low), 1 (Medium), 2 (High).

---

## Business Rules

1. **Status workflow**  
   Allowed: Todo → InProgress, InProgress → Done, InProgress → Todo.  
   **Not allowed:** Done → InProgress or Done → Todo.  
   Invalid transition → **400** with message.

2. **Due date**  
   Due date cannot be in the past (on create or update).  
   Invalid → **400** with message.

3. **Delete project**  
   Project cannot be deleted if it has **active** tasks (Todo or InProgress).  
   If active tasks exist → **400**. If all tasks are Done (or no tasks), project and its tasks are deleted.

4. **Priority cap (optional)**  
   Maximum **3 High-priority** tasks per project.  
   Creating or updating a task to High when the project already has 3 High-priority tasks → **400** with message.

---

## Error Responses

- **404 Not Found:** `{ "message": "Not Found" }` (or "Project not found." where applicable).
- **400 Bad Request:** `{ "message": "..." }` for validation/rule violations (e.g. invalid status transition, due date in past, active tasks, priority limit).
- **200 OK (delete):** `{ "message": "Record deleted successfully" }`.

No `type`, `traceId`, or other Problem Details fields in these responses.

---

## How to Run

1. **Prerequisites:** .NET 8 SDK, SQL Server.
2. **Database:** Create database **Ruchi_TaskWorkflowDb** and run **scripts/CreateTables_Only.sql** (or use EF migration). Connection string in `appsettings.Development.json`.
3. **Run:**
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
4. **Test:** Open **http://localhost:5000/swagger** (or the URL in the console) and use Swagger UI.

---

## Project Structure

```
Controllers/     → ProjectsController, TasksController (thin; call services)
Services/        → IProjectService, ProjectService, ITaskService, TaskService, Result<T>
Data/            → AppDbContext
DTOs/            → ErrorDtos, ProjectDtos, TaskDtos
Models/          → Project, TaskItem, TaskStatus, TaskPriority
Migrations/      → EF Core migrations
Properties/      → launchSettings.json
docs/            → This file, WORKFLOW_AND_DATA_STRUCTURE.md, SECTION_2_SETUP.md
scripts/         → SQL scripts for database/tables
Program.cs       → App bootstrap, Swagger, DbContext, service registration
appsettings*.json
```

---

For detailed workflow and data-structure notes, see **WORKFLOW_AND_DATA_STRUCTURE.md**. For database setup, see **SECTION_2_SETUP.md**.
