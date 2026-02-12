# 🚀 QUICK START - Document Workflow Demo

## Run the Demo (3 Steps)

### 1️⃣ Extract ZIP
Extract `DocumentWorkflow-Complete-Demo.zip` to your desired location

### 2️⃣ Navigate & Run
```bash
cd DocumentWorkflow/src/DocumentWorkflow.API
dotnet run
```

### 3️⃣ Open Browser
```
http://localhost:5000
```

**✅ Done!** Database auto-created with sample data.

---

## 🎯 Quick Demo (60 seconds)

1. **Click** "Create Sample Document" button
2. **Click** "Submit" on the new document
3. **Click** "My Approvals" tab
4. **Click** "Approve" button
5. **Go back** to Documents tab
6. **See** document status changed to "Published"

**🎉 Success!** You just executed a workflow!

---

## 📱 Key Pages

| URL | What It Does |
|-----|--------------|
| `http://localhost:5000` | Main dashboard - create & approve documents |
| `http://localhost:5000/workflow-designer.html` | Visual workflow designer |
| `http://localhost:5000/swagger` | API documentation |

---

## 🏗️ What's Included

✅ **4 Architecture Layers**
- Domain (entities & interfaces)
- Application (business logic)
- Infrastructure (data access)
- API (REST endpoints & UI)

✅ **Design Patterns**
- Clean Architecture
- Repository Pattern
- Unit of Work
- Dependency Injection

✅ **Features**
- Embedded BPMN visual designer
- Workflow execution engine
- Document approval system
- Audit trail
- Sample data pre-loaded

---

## 🎬 Demo Flow

### Basic Demo (2 min)
```
Dashboard → Create Document → Submit → Approve → Published ✓
```

### Full Demo (5 min)
```
1. Show Dashboard & Stats
2. Create Invoice ($1,500)
3. Submit for Approval
4. Open Workflow Designer (show visual workflow)
5. Back to app - approve task
6. Show published document
7. Show Swagger API docs
```

### Advanced Demo (10 min)
```
1-7. (Same as Full Demo)
8. Create custom workflow in designer
9. Save it
10. Create document with that type
11. Watch it execute through your custom workflow!
```

---

## 📚 Documentation

- `README.md` - Project overview
- `SETUP_GUIDE.md` - Complete setup & troubleshooting
- `BPMN/WORKFLOW_GUIDE.md` - How workflows work
- `Web/EMBEDDED_DESIGNER_GUIDE.md` - Designer details

---

## 🔧 Troubleshooting

**Port in use?**
```bash
dotnet run --urls "http://localhost:5001"
```

**Reset database?**
```bash
rm documentworkflow.db
dotnet run
```

**Need .NET 8?**
Download from: https://dotnet.microsoft.com/download

---

## 🎓 Key Files to Review

**Workflow Engine:** `Application/Services/WorkflowEngineService.cs`  
**BPMN Parser:** Reads XML and executes step-by-step  

**Repository Pattern:** `Infrastructure/Repositories/UnitOfWork.cs`  
**Clean data access abstraction**

**Controllers:** `API/Controllers/`  
**REST API endpoints**

**UI:** `API/wwwroot/index.html`  
**Dashboard & document management**

**Designer:** `API/wwwroot/workflow-designer.html`  
**Visual BPMN modeler**

---

## 💡 What Makes This Special

1. **No External Tools** - Workflow designer embedded in app
2. **Real Execution** - Workflows actually run (not just diagrams)
3. **Clean Code** - Proper architecture & patterns
4. **Complete Demo** - All layers implemented
5. **Production-Ready Base** - Add auth, files, email = production!

---

## 🎯 Presentation Points

**For Business Stakeholders:**
- "Users design workflows visually - no coding needed"
- "Documents automatically follow approval rules"
- "Complete audit trail of all actions"
- "Workflow changes don't require developer"

**For Technical Stakeholders:**
- "Clean Architecture with 4 layers"
- "Repository Pattern with Unit of Work"
- "BPMN 2.0 standard compliance"
- "RESTful API with Swagger docs"
- "SQLite for easy demo, supports SQL Server/PostgreSQL"

---

## ✅ Pre-Demo Checklist

- [ ] Extracted ZIP file
- [ ] .NET 8 SDK installed
- [ ] Can run `dotnet run` successfully
- [ ] Browser opens http://localhost:5000
- [ ] Dashboard loads with sample data
- [ ] Can create & submit document
- [ ] Workflow designer loads
- [ ] Ready to present!

---

**Time to Demo:** < 2 minutes setup  
**Wow Factor:** High! 🚀  
**Production Ready:** Add auth + files + email
