using Microsoft.EntityFrameworkCore;
using DocumentWorkflow.Domain.Entities;
using DocumentWorkflow.Domain.Interfaces;
using DocumentWorkflow.Infrastructure.Data;

namespace DocumentWorkflow.Infrastructure.Repositories;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Document>> GetByStatusAsync(string status)
    {
        return await _dbSet.Where(d => d.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetBySubmitterAsync(string submitterId)
    {
        return await _dbSet.Where(d => d.SubmittedBy == submitterId).ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByDocumentTypeAsync(string documentType)
    {
        return await _dbSet.Where(d => d.DocumentType == documentType).ToListAsync();
    }
}

public class WorkflowDefinitionRepository : Repository<WorkflowDefinition>, IWorkflowDefinitionRepository
{
    public WorkflowDefinitionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<WorkflowDefinition?> GetActiveByDocumentTypeAsync(string documentType)
    {
        return await _dbSet
            .Where(w => w.DocumentType == documentType && w.IsActive)
            .OrderByDescending(w => w.Version)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetByDocumentTypeAsync(string documentType)
    {
        return await _dbSet
            .Where(w => w.DocumentType == documentType)
            .OrderByDescending(w => w.Version)
            .ToListAsync();
    }

    public async Task<WorkflowDefinition?> GetLatestVersionAsync(string name)
    {
        return await _dbSet
            .Where(w => w.Name == name)
            .OrderByDescending(w => w.Version)
            .FirstOrDefaultAsync();
    }
}

public class WorkflowInstanceRepository : Repository<WorkflowInstance>, IWorkflowInstanceRepository
{
    public WorkflowInstanceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<WorkflowInstance>> GetByDocumentIdAsync(Guid documentId)
    {
        return await _dbSet
            .Where(wi => wi.DocumentId == documentId)
            .Include(wi => wi.Steps)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkflowInstance>> GetRunningInstancesAsync()
    {
        return await _dbSet
            .Where(wi => wi.Status == "Running")
            .ToListAsync();
    }

    public async Task<WorkflowInstance?> GetWithStepsAsync(Guid instanceId)
    {
        return await _dbSet
            .Include(wi => wi.Steps)
            .Include(wi => wi.ApprovalTasks)
            .FirstOrDefaultAsync(wi => wi.Id == instanceId);
    }
}

public class WorkflowStepRepository : Repository<WorkflowStep>, IWorkflowStepRepository
{
    public WorkflowStepRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<WorkflowStep>> GetByInstanceIdAsync(Guid instanceId)
    {
        return await _dbSet
            .Where(s => s.WorkflowInstanceId == instanceId)
            .OrderBy(s => s.StartedAt)
            .ToListAsync();
    }

    public async Task<WorkflowStep?> GetCurrentStepAsync(Guid instanceId)
    {
        return await _dbSet
            .Where(s => s.WorkflowInstanceId == instanceId)
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync();
    }
}

public class ApprovalTaskRepository : Repository<ApprovalTask>, IApprovalTaskRepository
{
    public ApprovalTaskRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ApprovalTask>> GetByAssigneeAsync(string assignee)
    {
        return await _dbSet
            .Where(t => t.AssignedTo == assignee)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApprovalTask>> GetPendingTasksAsync(string assignee)
    {
        return await _dbSet
            .Where(t => t.AssignedTo == assignee && t.Status == "Pending")
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApprovalTask>> GetByDocumentIdAsync(Guid documentId)
    {
        return await _dbSet
            .Where(t => t.DocumentId == documentId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }
}

public class ApprovalPolicyRepository : Repository<ApprovalPolicy>, IApprovalPolicyRepository
{
    public ApprovalPolicyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ApprovalPolicy>> GetActiveByDocumentTypeAsync(string documentType)
    {
        return await _dbSet
            .Where(p => p.DocumentType == documentType && p.IsActive)
            .OrderByDescending(p => p.Priority)
            .ToListAsync();
    }

    public async Task<ApprovalPolicy?> GetMatchingPolicyAsync(string documentType, string department, decimal? amount)
    {
        // Simplified - in real implementation would parse Conditions JSON
        return await _dbSet
            .Where(p => p.DocumentType == documentType && p.IsActive)
            .OrderByDescending(p => p.Priority)
            .FirstOrDefaultAsync();
    }
}
