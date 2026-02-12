using DocumentWorkflow.Domain.Entities;
using DocumentWorkflow.Infrastructure.Data;

namespace DocumentWorkflow.Infrastructure.Seeders;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Clear existing data
        context.ApprovalTasks.RemoveRange(context.ApprovalTasks);
        context.WorkflowSteps.RemoveRange(context.WorkflowSteps);
        context.WorkflowInstances.RemoveRange(context.WorkflowInstances);
        context.Documents.RemoveRange(context.Documents);
        context.WorkflowDefinitions.RemoveRange(context.WorkflowDefinitions);
        context.ApprovalPolicies.RemoveRange(context.ApprovalPolicies);
        await context.SaveChangesAsync();

        // Seed Approval Policies
        var policies = new[]
        {
            new ApprovalPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Invoice - Manager Only",
                DocumentType = "Invoice",
                Department = "Finance",
                Conditions = "{\"amount\": {\"lessThan\": 1000}}",
                Approvers = "manager@company.com",
                RequireAllApprovals = true,
                Priority = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApprovalPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Invoice - Multi-Level",
                DocumentType = "Invoice",
                Department = "Finance",
                Conditions = "{\"amount\": {\"greaterThanOrEqual\": 1000}}",
                Approvers = "manager@company.com,finance.director@company.com",
                RequireAllApprovals = true,
                Priority = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        context.ApprovalPolicies.AddRange(policies);

        // Seed Workflow Definitions
        var simpleWorkflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Simple Document Approval",
            Description = "Basic approval workflow for demo purposes",
            DocumentType = "Invoice",
            Version = 1,
            IsActive = true,
            BpmnXml = GetSimpleWorkflowBpmn(),
            ParsedStructure = "{}",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system"
        };
        context.WorkflowDefinitions.Add(simpleWorkflow);

        // Seed Sample Documents
        var sampleDocs = new[]
        {
            new Document
            {
                Id = Guid.NewGuid(),
                Title = "Office Supplies Invoice - March 2024",
                FileName = "invoice-2024-03-15.pdf",
                FilePath = "/documents/invoice-2024-03-15.pdf",
                DocumentType = "Invoice",
                Department = "Finance",
                Amount = 450.00m,
                Status = "Draft",
                SubmittedBy = "john.doe@company.com",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Document
            {
                Id = Guid.NewGuid(),
                Title = "Software License Invoice - Q1 2024",
                FileName = "invoice-software-q1-2024.pdf",
                FilePath = "/documents/invoice-software-q1-2024.pdf",
                DocumentType = "Invoice",
                Department = "IT",
                Amount = 5500.00m,
                Status = "Draft",
                SubmittedBy = "jane.smith@company.com",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Document
            {
                Id = Guid.NewGuid(),
                Title = "Consulting Services Contract",
                FileName = "contract-consulting-2024.pdf",
                FilePath = "/documents/contract-consulting-2024.pdf",
                DocumentType = "Contract",
                Department = "Legal",
                Amount = 25000.00m,
                Status = "Draft",
                SubmittedBy = "bob.johnson@company.com",
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Documents.AddRange(sampleDocs);

        await context.SaveChangesAsync();
    }

    private static string GetSimpleWorkflowBpmn()
    {
        return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<bpmn:definitions xmlns:bpmn=""http://www.omg.org/spec/BPMN/20100524/MODEL"" 
                   xmlns:bpmndi=""http://www.omg.org/spec/BPMN/20100524/DI"" 
                   xmlns:dc=""http://www.omg.org/spec/DD/20100524/DC""
                   xmlns:di=""http://www.omg.org/spec/DD/20100524/DI""
                   id=""Definitions_Simple"" 
                   targetNamespace=""http://bpmn.io/schema/bpmn"">
  <bpmn:process id=""SimpleApprovalProcess"" name=""Simple Document Approval"" isExecutable=""true"">
    <bpmn:startEvent id=""StartEvent_1"" name=""Submit"">
      <bpmn:outgoing>Flow_1</bpmn:outgoing>
    </bpmn:startEvent>
    <bpmn:serviceTask id=""Task_Validate"" name=""Validate Document"">
      <bpmn:incoming>Flow_1</bpmn:incoming>
      <bpmn:outgoing>Flow_2</bpmn:outgoing>
    </bpmn:serviceTask>
    <bpmn:userTask id=""Task_Approve"" name=""Manager Approval"">
      <bpmn:incoming>Flow_2</bpmn:incoming>
      <bpmn:outgoing>Flow_3</bpmn:outgoing>
    </bpmn:userTask>
    <bpmn:serviceTask id=""Task_Publish"" name=""Publish Document"">
      <bpmn:incoming>Flow_3</bpmn:incoming>
      <bpmn:outgoing>Flow_4</bpmn:outgoing>
    </bpmn:serviceTask>
    <bpmn:endEvent id=""EndEvent_1"" name=""Complete"">
      <bpmn:incoming>Flow_4</bpmn:incoming>
    </bpmn:endEvent>
    <bpmn:sequenceFlow id=""Flow_1"" sourceRef=""StartEvent_1"" targetRef=""Task_Validate""/>
    <bpmn:sequenceFlow id=""Flow_2"" sourceRef=""Task_Validate"" targetRef=""Task_Approve""/>
    <bpmn:sequenceFlow id=""Flow_3"" sourceRef=""Task_Approve"" targetRef=""Task_Publish""/>
    <bpmn:sequenceFlow id=""Flow_4"" sourceRef=""Task_Publish"" targetRef=""EndEvent_1""/>
  </bpmn:process>
  <bpmndi:BPMNDiagram id=""BPMNDiagram_1"">
    <bpmndi:BPMNPlane id=""BPMNPlane_1"" bpmnElement=""SimpleApprovalProcess"">
      <bpmndi:BPMNShape id=""StartEvent_1_di"" bpmnElement=""StartEvent_1"">
        <dc:Bounds x=""179"" y=""159"" width=""36"" height=""36""/>
        <bpmndi:BPMNLabel>
          <dc:Bounds x=""180"" y=""202"" width=""35"" height=""14""/>
        </bpmndi:BPMNLabel>
      </bpmndi:BPMNShape>
      <bpmndi:BPMNShape id=""Task_Validate_di"" bpmnElement=""Task_Validate"">
        <dc:Bounds x=""270"" y=""137"" width=""100"" height=""80""/>
      </bpmndi:BPMNShape>
      <bpmndi:BPMNShape id=""Task_Approve_di"" bpmnElement=""Task_Approve"">
        <dc:Bounds x=""430"" y=""137"" width=""100"" height=""80""/>
      </bpmndi:BPMNShape>
      <bpmndi:BPMNShape id=""Task_Publish_di"" bpmnElement=""Task_Publish"">
        <dc:Bounds x=""590"" y=""137"" width=""100"" height=""80""/>
      </bpmndi:BPMNShape>
      <bpmndi:BPMNShape id=""EndEvent_1_di"" bpmnElement=""EndEvent_1"">
        <dc:Bounds x=""752"" y=""159"" width=""36"" height=""36""/>
        <bpmndi:BPMNLabel>
          <dc:Bounds x=""743"" y=""202"" width=""55"" height=""14""/>
        </bpmndi:BPMNLabel>
      </bpmndi:BPMNShape>
      <bpmndi:BPMNEdge id=""Flow_1_di"" bpmnElement=""Flow_1"">
        <di:waypoint x=""215"" y=""177""/>
        <di:waypoint x=""270"" y=""177""/>
      </bpmndi:BPMNEdge>
      <bpmndi:BPMNEdge id=""Flow_2_di"" bpmnElement=""Flow_2"">
        <di:waypoint x=""370"" y=""177""/>
        <di:waypoint x=""430"" y=""177""/>
      </bpmndi:BPMNEdge>
      <bpmndi:BPMNEdge id=""Flow_3_di"" bpmnElement=""Flow_3"">
        <di:waypoint x=""530"" y=""177""/>
        <di:waypoint x=""590"" y=""177""/>
      </bpmndi:BPMNEdge>
      <bpmndi:BPMNEdge id=""Flow_4_di"" bpmnElement=""Flow_4"">
        <di:waypoint x=""690"" y=""177""/>
        <di:waypoint x=""752"" y=""177""/>
      </bpmndi:BPMNEdge>
    </bpmndi:BPMNPlane>
  </bpmndi:BPMNDiagram>
</bpmn:definitions>";
    }
}
