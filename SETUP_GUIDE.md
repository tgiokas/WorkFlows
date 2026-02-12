# Complete Setup & Demo Guide

## 📦 What You Have

A fully functional Document Workflow Management System with:
- ✅ 4 architectural layers (Clean Architecture)
- ✅ Repository Pattern implementation
- ✅ Working BPMN workflow engine
- ✅ Embedded visual workflow designer
- ✅ REST API with Swagger
- ✅ Sample data pre-loaded
- ✅ Dashboard UI

## 🚀 Running the Demo (2 Steps!)

### Step 1: Extract & Navigate
```bash
# Extract ZIP file
# Open terminal/command prompt
cd DocumentWorkflow/src/DocumentWorkflow.API
```

### Step 2: Run
```bash
dotnet run
```

### Step 3: Open Browser
```
http://localhost:5000
```

**✅ Done!** Database is created automatically with sample data.

## 🎯 Demo Scenarios

### Scenario A: Quick Demo (2 minutes)
```
1. Open http://localhost:5000
2. Click "Create Sample Document"
3. Click "Submit" on the new document
4. Click "My Approvals" tab
5. Click "Approve"
6. Back to Documents - see status "Published"
✅ Workflow executed successfully!
```

### Scenario B: Full Demo (5 minutes)
```
1. Dashboard - show statistics
2. Create document with specific amount ($1,500)
3. Submit for approval
4. Open Workflow Designer (separate tab)
   - Show visual workflow
   - Explain each step
5. Back to main app
6. My Approvals - show task created
7. Approve the document
8. Documents - show published
9. Swagger UI - show API endpoints
```

### Scenario C: Custom Workflow Demo (10 minutes)
```
1. Open Workflow Designer
2. Create new workflow:
   - Start Event
   - Validation Task
   - Approval Task
   - Publish Task
   - End Event
3. Save workflow
4. Create document matching that type
5. Submit and watch it execute!
```

## 📊 Key URLs

| URL | Purpose |
|-----|---------|
| `http://localhost:5000` | Main dashboard |
| `http://localhost:5000/workflow-designer.html` | BPMN designer |
| `http://localhost:5000/swagger` | API documentation |

## 🗂️ Project Structure

```
DocumentWorkflow/
├── src/
│   ├── DocumentWorkflow.Domain/
│   │   ├── Entities/
│   │   │   ├── Document.cs
│   │   │   ├── WorkflowDefinition.cs
│   │   │   ├── WorkflowInstance.cs
│   │   │   ├── WorkflowStep.cs
│   │   │   ├── ApprovalTask.cs
│   │   │   └── ApprovalPolicy.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs
│   │       ├── Repositories.cs
│   │       └── IUnitOfWork.cs
│   │
│   ├── DocumentWorkflow.Application/
│   │   ├── DTOs/
│   │   │   └── Dtos.cs
│   │   ├── Interfaces/
│   │   │   └── IServices.cs
│   │   └── Services/
│   │       ├── DocumentService.cs
│   │       ├── WorkflowService.cs
│   │       ├── WorkflowEngineService.cs ⭐
│   │       └── ApprovalService.cs
│   │
│   ├── DocumentWorkflow.Infrastructure/
│   │   ├── Data/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── Repository.cs
│   │   │   ├── SpecificRepositories.cs
│   │   │   └── UnitOfWork.cs
│   │   └── Seeders/
│   │       └── DataSeeder.cs
│   │
│   └── DocumentWorkflow.API/
│       ├── Controllers/
│       │   ├── DocumentsController.cs
│       │   ├── WorkflowsController.cs
│       │   └── ApprovalsController.cs
│       ├── wwwroot/
│       │   ├── index.html ⭐
│       │   └── workflow-designer.html ⭐
│       ├── Program.cs
│       └── appsettings.json
│
├── BPMN/
│   ├── DocumentApprovalWorkflow.bpmn
│   ├── WORKFLOW_GUIDE.md
│   └── workflow-structure.json
│
├── DocumentWorkflow.sln
└── README.md
```

⭐ = Key files to demo

## 📱 User Interface Guide

### Dashboard (index.html)

**Tabs:**
- Dashboard - Statistics and quick actions
- Documents - List all documents
- My Approvals - Pending approval tasks
- Workflow Designer - Link to designer

**Actions:**
- Create Sample Document
- Submit document for approval
- Approve/Reject tasks

### Workflow Designer (workflow-designer.html)

**Features:**
- Visual BPMN modeler (bpmn-js)
- Load existing workflows
- Create new workflows
- Save to database
- Export BPMN XML
- Import BPMN files

**Elements Available:**
- Start Event (circle)
- End Event (bold circle)
- Service Task (gear icon)
- User Task (person icon)
- Exclusive Gateway (diamond)
- Sequence Flow (arrows)

## 🔌 API Endpoints

### Documents API

```http
GET    /api/documents              # List all
GET    /api/documents/{id}         # Get by ID
POST   /api/documents              # Create
POST   /api/documents/{id}/submit  # Submit for approval
```

**Example Create:**
```json
POST /api/documents
{
  "title": "Office Supplies Invoice",
  "documentType": "Invoice",
  "department": "Finance",
  "amount": 750.00,
  "submittedBy": "user@company.com"
}
```

### Workflows API

```http
GET    /api/workflows                      # List all
GET    /api/workflows/{id}                 # Get by ID
POST   /api/workflows                      # Create/Update
GET    /api/workflows/active/{docType}     # Get active
DELETE /api/workflows/{id}                 # Delete
```

**Example Create:**
```json
POST /api/workflows
{
  "name": "Invoice Approval",
  "documentType": "Invoice",
  "bpmnXml": "<bpmn:definitions>...</bpmn:definitions>"
}
```

### Approvals API

```http
GET    /api/approvals/my-tasks           # My pending tasks
GET    /api/approvals/document/{id}      # Tasks for document
POST   /api/approvals/complete           # Complete task
```

**Example Complete:**
```json
POST /api/approvals/complete
{
  "taskId": "guid",
  "decision": "approve",
  "comments": "Looks good!"
}
```

## 🗄️ Database

**Technology:** SQLite  
**File:** `documentworkflow.db`  
**Location:** Same directory as API project

**Tables Created:**
- Documents
- WorkflowDefinitions
- WorkflowInstances
- WorkflowSteps
- ApprovalTasks
- ApprovalPolicies

**Sample Data:**
- 3 documents (Draft status)
- 1 workflow definition
- 2 approval policies

## 🎓 Code Highlights

### 1. BPMN Workflow Engine
**File:** `Application/Services/WorkflowEngineService.cs`

This is the heart of the system - parses BPMN XML and executes workflows step-by-step.

```csharp
public async Task<bool> ExecuteNextStepAsync(Guid instanceId)
{
    // 1. Get workflow instance
    // 2. Parse BPMN XML to find next step
    // 3. Execute based on step type:
    //    - ServiceTask: Auto-execute
    //    - UserTask: Create approval task
    //    - EndEvent: Complete workflow
    // 4. Update instance state
    // 5. Continue to next step (if auto)
}
```

### 2. Repository Pattern
**File:** `Infrastructure/Repositories/UnitOfWork.cs`

Clean separation of data access from business logic.

```csharp
public class UnitOfWork : IUnitOfWork
{
    public IDocumentRepository Documents { get; }
    public IWorkflowInstanceRepository WorkflowInstances { get; }
    // ... other repositories
    
    public async Task<int> SaveChangesAsync() { }
    public async Task BeginTransactionAsync() { }
}
```

### 3. Clean Architecture Layers

```
API Controller 
    → Application Service 
        → Domain Repository Interface 
            → Infrastructure Repository Implementation 
                → Database
```

Example flow for "Submit Document":
```
DocumentsController.SubmitForApproval()
  → DocumentService.SubmitDocumentForApprovalAsync()
    → WorkflowEngineService.StartWorkflowAsync()
      → UnitOfWork.WorkflowInstances.AddAsync()
        → DbContext.SaveChangesAsync()
```

## 🔧 Troubleshooting

### "Port 5000 already in use"
```bash
dotnet run --urls "http://localhost:5001"
```

### "Database locked"
```bash
# Stop the app (Ctrl+C)
# Delete database file
rm documentworkflow.db
# Restart
dotnet run
```

### "Workflow not executing"
- Check console for errors
- Verify workflow has active status
- Check document type matches workflow

### "Can't save workflows in designer"
- Verify API is running
- Check browser console (F12)
- Ensure CORS is enabled

## 📚 Learning Resources

### Understanding BPMN
- Start Event: Begins workflow
- Service Task: Automated action (validation, publishing)
- User Task: Human task (approval)
- Gateway: Decision point (approved? rejected?)
- End Event: Workflow completion

### Design Patterns
- **Repository Pattern:** Data access abstraction
- **Unit of Work:** Transaction management
- **Dependency Injection:** Loose coupling
- **DTO Pattern:** Data transfer
- **Service Layer:** Business logic

## 🎬 Presentation Tips

### Opening (1 min)
"This is a Document Management System with workflow automation. 
Users can design workflows visually and documents automatically 
follow those workflows for approval."

### Demo (3 mins)
1. Create document
2. Submit → Show auto-workflow start
3. Approve → Show auto-continue
4. Published → Complete!

### Designer (2 mins)
1. Show visual workflow
2. Drag new element
3. Explain: "Business users can do this!"

### Technical (2 mins)
1. Show code architecture
2. Explain Clean Architecture
3. Show Repository Pattern

### Closing (1 min)
"Full source code available. All layers implemented. 
Production-ready with additional auth, notifications, etc."

## ✅ Checklist Before Demo

- [ ] Application runs without errors
- [ ] Can access http://localhost:5000
- [ ] Dashboard shows statistics
- [ ] Can create sample document
- [ ] Can submit document
- [ ] Can see approval task
- [ ] Can approve/reject
- [ ] Workflow designer loads
- [ ] Can view existing workflow
- [ ] Swagger UI accessible

## 🚀 Going to Production

To make this production-ready, add:

1. **Security**
   - Authentication (ASP.NET Identity)
   - Authorization (role-based)
   - API key protection

2. **Features**
   - File upload/storage (Azure Blob, AWS S3)
   - Email notifications (SendGrid, SMTP)
   - Advanced BPMN support (parallel gateways, timers)
   - Workflow versioning

3. **Infrastructure**
   - Production database (SQL Server, PostgreSQL)
   - Docker containerization
   - CI/CD pipeline
   - Monitoring & logging

4. **Testing**
   - Unit tests
   - Integration tests
   - E2E tests

## 📞 Support

This is a complete, working proof-of-concept demonstrating:
- Clean Architecture
- Repository Pattern
- BPMN workflow automation
- Embedded workflow designer
- REST API
- SQLite database

All layers fully implemented and functional!
