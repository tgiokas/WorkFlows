# Document Workflow Management System - Complete POC Demo

## 🎯 Overview
A complete, working proof-of-concept for a Document Management System with BPMN-based workflow automation.

## 🚀 Quick Start

### Run the Demo
```bash
cd src/DocumentWorkflow.API
dotnet run
```

### Open Browser
```
http://localhost:5000
```

That's it! Database created automatically with sample data.

## ✨ What's Included

✅ Complete working demo  
✅ Embedded BPMN Designer  
✅ Workflow execution engine  
✅ Sample documents & workflows  
✅ All 4 layers (Domain, Application, Infrastructure, API)  
✅ Clean Architecture + Repository Pattern  

## 📱 Demo Flow

1. **Dashboard** → Create Sample Document
2. **Submit** → Workflow starts automatically  
3. **My Approvals** → See approval task
4. **Approve/Reject** → Workflow continues
5. **Workflow Designer** → Create custom workflows

## 📚 Full Documentation

See detailed guides in BPMN/ folder:
- WORKFLOW_GUIDE.md - Complete workflow explanation
- EMBEDDED_DESIGNER_GUIDE.md - How the designer works
- QUICK_REFERENCE.md - Architecture overview

## 🏗️ Architecture

```
Domain → Application → Infrastructure → API
  ↓          ↓             ↓            ↓
Entities   Services    Repositories   Controllers
```

All layers follow Clean Architecture and Repository Pattern.

## 🎬 Demo Presentation

1. Show dashboard & create document
2. Submit for approval
3. Show workflow designer
4. Approve document
5. Show published result

**Time:** 5 minutes  
**Wow Factor:** High! 🚀
