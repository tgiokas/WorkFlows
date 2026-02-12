# Embedded BPMN Designer - Implementation Guide

## Overview

Your end users will design workflows **directly in your web application** using an embedded visual designer. No need for external tools like Camunda Modeler for daily use!

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                 Your Web Application                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Workflow Designer Page (workflow-designer.html)     │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │                                                │  │  │
│  │  │     Embedded bpmn-js Designer                  │  │  │
│  │  │     (Drag & Drop Visual Editor)                │  │  │
│  │  │                                                │  │  │
│  │  │  [Start] → [Task] → [Gateway] → [End]        │  │  │
│  │  │                                                │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  │                                                       │  │
│  │  [Save Workflow Button] ──────────┐                  │  │
│  └───────────────────────────────────┼──────────────────┘  │
│                                      │                      │
│                                      ▼                      │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  ASP.NET API (WorkflowsController)                   │  │
│  │  - POST /api/workflows (Save BPMN XML)               │  │
│  │  - GET  /api/workflows (List all)                    │  │
│  │  - GET  /api/workflows/{id} (Load specific)          │  │
│  └───────────────────────────────┬──────────────────────┘  │
│                                  │                          │
│                                  ▼                          │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Database (WorkflowDefinition Table)                 │  │
│  │  - Id, Name, BpmnXml, DocumentType, Version...       │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## User Workflow

### For Business Users (Non-Technical)

1. **Navigate to Workflow Designer**
   - Click "Settings" → "Workflow Designer" in your app menu

2. **Create New Workflow**
   - Click "New Workflow" button
   - Give it a name: "Invoice Approval - Finance Department"

3. **Design Visually**
   - Drag "Start Event" to canvas
   - Drag "Service Task" → Name it "Validate Invoice"
   - Drag "User Task" → Name it "Manager Approval"
   - Drag "Gateway" → Configure conditions
   - Drag "End Event"
   - Connect them with arrows

4. **Save**
   - Click "Save Workflow"
   - XML is automatically generated and stored in database

5. **Activate**
   - Set as active for "Invoice" document type
   - Now all invoices use this workflow!

### For Developers/Power Users

- Same visual designer PLUS
- Option to view/edit raw BPMN XML
- Import/Export BPMN files
- Version control integration

## Implementation Steps

### Step 1: Add HTML Page to Your Web Project

Copy `workflow-designer.html` to your web project:

```
YourProject/
├── wwwroot/
│   └── workflow-designer.html  ← Place here
```

Or embed in a Razor Page / MVC View:

```csharp
// Pages/WorkflowDesigner.cshtml
@page
@model WorkflowDesignerModel

<div id="canvas"></div>

@section Scripts {
    <script src="https://unpkg.com/bpmn-js@17.0.2/dist/bpmn-modeler.development.js"></script>
    <script src="~/js/workflow-designer.js"></script>
}
```

### Step 2: Configure API Endpoint

Update the JavaScript in `workflow-designer.html`:

```javascript
// Change this line to match your API
const API_BASE_URL = '/api/workflows';
```

### Step 3: Add CORS (if needed)

In `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDesigner", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowDesigner");
```

### Step 4: Implement the Service Layer

```csharp
public interface IWorkflowService
{
    Task<WorkflowDefinitionDto> SaveWorkflowDefinitionAsync(CreateWorkflowDefinitionDto dto);
    Task<IEnumerable<WorkflowDefinitionDto>> GetAllWorkflowsAsync();
    Task<WorkflowDefinitionDto?> GetWorkflowByIdAsync(Guid id);
}
```

## Features of the Embedded Designer

### What Users Can Do:

✅ **Drag & Drop Elements**
- Start Events
- End Events
- Service Tasks (automated actions)
- User Tasks (human approvals)
- Gateways (decision points)
- Sequence Flows (arrows connecting elements)

✅ **Configure Properties**
- Click any element to edit properties
- Set task names
- Configure gateway conditions
- Add documentation

✅ **Save & Load**
- Save workflows to database
- Load existing workflows
- Create versions

✅ **Import/Export**
- Import BPMN from Camunda Modeler
- Export for external tools
- Share workflows across environments

✅ **Visual Validation**
- Real-time error highlighting
- Missing connections shown
- Invalid configurations marked

## BPMN Elements Available

### Events
- **Start Event** (Circle) - Where workflow begins
- **End Event** (Bold Circle) - Where workflow ends
- **Timer Event** - Scheduled triggers

### Tasks
- **Service Task** (Gear icon) - Automated actions
- **User Task** (Person icon) - Human tasks
- **Script Task** - Run code
- **Send Task** - Send messages

### Gateways
- **Exclusive Gateway** (Diamond with X) - Choose one path
- **Parallel Gateway** (Diamond with +) - Do all paths
- **Inclusive Gateway** - Do some paths

### Flows
- **Sequence Flow** (Solid arrow) - Normal flow
- **Conditional Flow** - With conditions

## Example: Business User Creating Invoice Approval

**Step-by-step in the designer:**

1. Drag **Start Event** → Label: "Invoice Submitted"

2. Drag **Service Task** → Label: "Validate Invoice"
   - Click task → Set implementation: "ValidateInvoiceActivity"

3. Drag **Gateway** (Exclusive) → Label: "Amount Check"
   - Click gateway → Add conditions:
     - `${amount < 1000}` → Manager only
     - `${amount >= 1000}` → Manager + Finance

4. Drag **User Task** → Label: "Manager Approval"
   - Set assignee: `${document.managerId}`

5. Drag **User Task** → Label: "Finance Approval"
   - Set assignee: "finance-team"

6. Drag **Service Task** → Label: "Publish Invoice"

7. Drag **End Event** → Label: "Approved"

8. **Connect** all with arrows

9. Click **Save** → Done! ✅

## Integration with Your DMS

### When User Creates Document:

```csharp
// In DocumentService.cs
public async Task SubmitDocumentAsync(Guid documentId)
{
    var document = await _documentRepo.GetByIdAsync(documentId);
    
    // Get active workflow for this document type
    var workflow = await _workflowService
        .GetActiveWorkflowForDocumentTypeAsync(document.DocumentType);
    
    if (workflow != null)
    {
        // Start workflow instance
        await _workflowEngine.StartWorkflowAsync(workflow.Id, documentId);
    }
}
```

### When Workflow Executes:

```csharp
// BpmnParser reads the XML
// Executes step-by-step
// Creates approval tasks
// Updates document status
```

## Security Considerations

### Who Can Design Workflows?

Add role-based access:

```csharp
[Authorize(Roles = "Admin,WorkflowDesigner")]
public class WorkflowsController : ControllerBase
{
    // Only admins and workflow designers can create/edit
}
```

### Validation

Always validate BPMN before saving:

```csharp
public async Task<BpmnValidationResult> ValidateBpmnAsync(string bpmnXml)
{
    // Check XML is valid
    // Check has Start and End events
    // Check all tasks are connected
    // Check no orphaned elements
}
```

## Benefits of Embedded Designer

✅ **No Context Switching** - Users stay in your app
✅ **Consistent UI/UX** - Matches your app's design
✅ **Access Control** - Integrated with your auth
✅ **Database Integration** - Workflows stored with your data
✅ **Versioning** - Track workflow changes
✅ **Audit Trail** - Who created/modified workflows
✅ **No External Tools Needed** - Everything in one place

## Demo Presentation Flow

1. **Show the designer page**
   - "This is where business users design workflows"

2. **Create a simple workflow live**
   - Drag elements
   - Connect them
   - Configure properties

3. **Save it**
   - Click save
   - Show it in the database

4. **Show it executing**
   - Submit a document
   - Show it following the visual workflow
   - Show approvers getting tasks

5. **Edit the workflow**
   - Load it back
   - Add a new step
   - Save new version
   - Show versioning

## Next Steps

Want me to create:

1. **Complete Blazor integration** - Instead of plain HTML
2. **The workflow execution engine** - That reads and runs these workflows
3. **The BPMN parser** - That converts XML to executable steps
4. **Demo data** - Sample workflows ready to import

Which would you like next?
