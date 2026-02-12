# Document Approval Workflow - BPMN Guide

## Overview
This BPMN workflow handles the complete document approval process from submission to publication.

## How to Use This File

### Option 1: Camunda Modeler (Recommended)
1. Download Camunda Modeler from: https://camunda.com/download/modeler/
2. Open Camunda Modeler
3. File → Open → Select `DocumentApprovalWorkflow.bpmn`
4. You'll see the complete visual workflow!

### Option 2: Web-based (demo.bpmn.io)
1. Go to: https://demo.bpmn.io
2. Click "Open File"
3. Select `DocumentApprovalWorkflow.bpmn`
4. View and edit the workflow in your browser

## Workflow Steps Explained

### 1. Start Event: Submit for Approval
- Triggered when a user submits a document for approval
- Input: Document ID, Submitter info

### 2. Service Task: Validate Submission
**Purpose:** Check if the submission is valid
**Validation Checks:**
- Document exists
- User has permission to submit
- Required metadata is present
- Document type is configured

### 3. Gateway: Valid?
**Decision Point:** Is the submission valid?
- **No** → End (Validation Failed)
- **Yes** → Continue to approval routing

### 4. Service Task: Resolve Approval Policy & Approvers
**Purpose:** Determine who needs to approve based on:
- Document Type (Invoice, Contract, Report)
- Department
- Amount (for invoices)
- Custom business rules

**Example Policies:**
```
Invoice < $1,000 → Manager approval only
Invoice > $1,000 → Manager + Finance Director
Contract → Legal + Department Head
Report → Department Head only
```

### 5. Service Task: Create Approval Task(s)
**Purpose:** Create approval tasks in the database
- Assign to next approver
- Set due date
- Send notification

### 6. User Task: Review & Decide (Approver)
**Human Task:** Approver reviews document and makes decision
**Actions Available:**
- **Approve** - Accept the document
- **Reject** - Decline the document
- **Request Changes** - Ask submitter to revise

### 7. Gateway: Decision?
**Three Paths Based on Approver's Decision:**

#### Path A: Request Changes
- Set document status to "Changes Requested"
- Notify submitter with comments
- End workflow (submitter will resubmit)

#### Path B: Reject
- Set document status to "Rejected"
- Notify submitter
- End workflow

#### Path C: Approve
- Continue to check if more approvals needed

### 8. Gateway: More Approvals Needed?
**Decision Point:** Are there more approvers in the policy?
- **Yes** → Loop back to create next approval task
- **No** → All approvals complete, proceed to publishing

### 9. Service Task: Publish Document
**Purpose:** Finalize and publish the document
**Actions:**
- Lock the document version
- Add approval stamp/metadata
- Move to "Published" status
- Archive approval history

### 10. Service Task: Notify Stakeholders
**Purpose:** Inform relevant parties
- Notify submitter (success)
- Notify all approvers (completion)
- Notify watchers/subscribers
- Trigger any downstream processes

### 11. End Event: Approved & Published
**Success!** Document is now officially approved and published

## Workflow Variables Used

The workflow uses these variables for decision-making:

```javascript
{
  "valid": boolean,              // Validation result
  "decision": string,            // "approve" | "reject" | "changes"
  "hasMoreApprovers": boolean,   // Are there more approvers?
  "documentId": "guid",
  "documentType": string,
  "department": string,
  "amount": decimal,
  "approvers": ["email1", "email2"],
  "currentApproverIndex": number
}
```

## Service Task Implementations

When you implement this in .NET, each Service Task will map to a C# class:

1. **ValidateSubmissionActivity** - Document & permission validation
2. **ResolveApprovalPolicyActivity** - Query ApprovalPolicy table, match conditions
3. **CreateApprovalTaskActivity** - Insert into ApprovalTask table
4. **SetStatusActivity** - Update Document.Status
5. **NotifyActivity** - Send emails/notifications
6. **PublishDocumentActivity** - Finalize document

## Demo Scenarios

### Scenario 1: Simple Approval (Happy Path)
- Invoice for $500
- Manager approves
- Published ✓

### Scenario 2: Multi-Level Approval
- Invoice for $5,000
- Manager approves
- Finance Director approves
- Published ✓

### Scenario 3: Rejection
- Contract submitted
- Legal rejects (missing clauses)
- Workflow ends, submitter notified

### Scenario 4: Changes Requested
- Report submitted
- Department Head requests changes
- Submitter revises and resubmits

## Next Steps for POC

1. ✅ Visual workflow created
2. ⏭️ Implement BPMN parser in .NET
3. ⏭️ Map Service Tasks to C# activities
4. ⏭️ Build demo UI to show workflow execution
5. ⏭️ Create sample data for live demo

---

**Pro Tips for Demo Presentation:**
- Open this BPMN in Camunda Modeler during your presentation
- Walk through each step visually
- Use the "highlight" feature to show current execution point
- Have sample documents ready for each scenario
