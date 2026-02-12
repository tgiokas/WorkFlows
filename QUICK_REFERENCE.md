# Embedded BPMN Designer - Quick Reference

## Where End Users Draw Workflows

### Answer: Inside Your Web Application!

```
┌────────────────────────────────────────────────────────┐
│  Your Document Management System (Browser)            │
├────────────────────────────────────────────────────────┤
│                                                        │
│  Navigation Menu:                                      │
│  📄 Documents                                          │
│  ✅ Approvals                                          │
│  ⚙️  Settings                                          │
│      └─→ 🎨 Workflow Designer  ← USER CLICKS HERE    │
│                                                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  ┌──────────────────────────────────────────────────┐ │
│  │  🎨 Workflow Designer Page                       │ │
│  │  ┌────────────────────────────────────────────┐  │ │
│  │  │                                            │  │ │
│  │  │   [New] [Load ▼] [Invoice Approval v2   ] │  │ │
│  │  │                                  [Save]   │  │ │
│  │  ├────────────────────────────────────────────┤  │ │
│  │  │                                            │  │ │
│  │  │  Palette:        Canvas:                  │  │ │
│  │  │  ┌─────┐        ┌───────────────────┐    │  │ │
│  │  │  │  O  │ Start  │    ( O )          │    │  │ │
│  │  │  │ [ ] │ Task   │      ↓            │    │  │ │
│  │  │  │  ◇  │ Gate   │  ┌────────┐       │    │  │ │
│  │  │  │ (O) │ End    │  │Validate│       │    │  │ │
│  │  │  └─────┘        │  └────────┘       │    │  │ │
│  │  │                 │      ↓            │    │  │ │
│  │  │   User drags →  │     ◇ Valid?     │    │  │ │
│  │  │   from palette  │    ↙  ↘          │    │  │ │
│  │  │   to canvas     │  Approve Reject  │    │  │ │
│  │  │                 │                   │    │  │ │
│  │  │                 └───────────────────┘    │  │ │
│  │  │                                            │  │ │
│  │  │   Technology: bpmn-js (JavaScript)        │  │ │
│  │  └────────────────────────────────────────────┘  │ │
│  └──────────────────────────────────────────────────┘ │
│                         ↓                              │
│                   Clicks [Save]                        │
│                         ↓                              │
└─────────────────────────┼──────────────────────────────┘
                          │
                          │ AJAX POST /api/workflows
                          │ {name: "Invoice Approval",
                          │  bpmnXml: "<bpmn>...</bpmn>"}
                          ↓
┌────────────────────────────────────────────────────────┐
│  Your ASP.NET Backend                                  │
├────────────────────────────────────────────────────────┤
│  WorkflowsController                                   │
│    ↓                                                   │
│  WorkflowService                                       │
│    ↓                                                   │
│  WorkflowDefinitionRepository                          │
│    ↓                                                   │
│  Database (WorkflowDefinition table)                   │
│    - Id: guid                                          │
│    - Name: "Invoice Approval"                          │
│    - BpmnXml: "<bpmn:definitions>...</bpmn>"          │
│    - DocumentType: "Invoice"                           │
│    - Version: 2                                        │
│    - IsActive: true                                    │
└────────────────────────────────────────────────────────┘
```

## User Journey: Creating a Workflow

```
Step 1: User logs into your DMS
   "https://yourdms.com"
   
Step 2: Navigate to Workflow Designer
   Settings → Workflow Designer
   
Step 3: Create New Workflow
   Click [New Workflow]
   Enter name: "Invoice Approval - Finance"
   
Step 4: Design Visually (Drag & Drop)
   Drag: Start Event
   Drag: Service Task "Validate Invoice"
   Drag: User Task "Manager Reviews"
   Drag: Gateway "Approved?"
   Drag: End Event "Complete"
   Connect them with arrows
   
Step 5: Configure Properties
   Click "Manager Reviews" task
   Set: Assignee = ${document.managerId}
   
Step 6: Save
   Click [Save Workflow]
   ✓ Saved to database
   ✓ Ready to use immediately!
   
Step 7: Activate
   Set as active for "Invoice" document type
   
Step 8: Test
   Submit an invoice
   Watch it flow through YOUR workflow!
```

## Key Files in Your Solution

### Frontend (What users see):
```
/Web/workflow-designer.html
   ↑
   This is the embedded designer page
   User opens this in browser
   No installation needed!
```

### Backend (API):
```
/API/Controllers/WorkflowsController.cs
   ↑
   Handles save/load/list operations
```

### Database (Storage):
```
WorkflowDefinition Table
   ↑
   Stores the BPMN XML
```

## Technologies Used

### bpmn-js (JavaScript Library)
- **What:** Open-source BPMN 2.0 modeler
- **From:** bpmn.io (same team that makes BPMN spec)
- **License:** Free & open source
- **CDN:** `https://unpkg.com/bpmn-js`
- **Integration:** Just include script tag in HTML

### No installation required:
- ✅ Runs in browser
- ✅ No plugins needed
- ✅ Works on any device
- ✅ Tablet/mobile friendly (view mode)

## Demo Setup (5 minutes)

1. **Add workflow-designer.html to your project**
   ```
   /wwwroot/workflow-designer.html
   ```

2. **Add menu item in your app**
   ```html
   <a href="/workflow-designer.html">Workflow Designer</a>
   ```

3. **Create API controller**
   ```
   /Controllers/WorkflowsController.cs
   ```

4. **Run migrations** (we'll create these)
   ```bash
   dotnet ef migrations add AddWorkflowTables
   dotnet ef database update
   ```

5. **Test it!**
   - Open designer
   - Drag some elements
   - Save
   - Check database - it's there!

## Comparison Chart

| Feature | External Tool (Camunda) | Embedded Designer |
|---------|------------------------|-------------------|
| Installation | ❌ Desktop app needed | ✅ Browser only |
| Context switching | ❌ Leave your app | ✅ Stay in app |
| Database integration | ❌ Manual import | ✅ Automatic |
| User-friendly | ⚠️ Technical users | ✅ Business users |
| Access control | ❌ Separate | ✅ Your auth |
| Versioning | ❌ Manual | ✅ Built-in |
| **For POC Demo** | ⚠️ Good for dev | ✅ **Perfect!** |
| **For Production** | ❌ Not ideal | ✅ **Recommended** |

## Quick Start Code

### Minimal HTML (Embedded Designer):
```html
<div id="canvas" style="height: 600px;"></div>
<button onclick="save()">Save</button>

<script src="https://unpkg.com/bpmn-js@17.0.2/dist/bpmn-modeler.development.js"></script>
<script>
  const modeler = new BpmnJS({ container: '#canvas' });
  
  async function save() {
    const { xml } = await modeler.saveXML();
    // Send to your API
    await fetch('/api/workflows', {
      method: 'POST',
      body: JSON.stringify({ name: 'My Workflow', bpmnXml: xml })
    });
  }
</script>
```

That's it! 30 lines of code for a full BPMN designer!

## Summary

**Question:** "Where will my end users draw workflows?"

**Answer:** In an embedded visual designer page inside your web application, accessed via browser, no external tools needed!

**Technology:** bpmn-js (open source JavaScript library)

**User experience:** Like using Google Docs, but for workflows

**Integration:** Saves directly to your database via your ASP.NET API

**For your POC:** This will WOW your stakeholders! 🚀
