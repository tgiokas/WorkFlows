using Microsoft.EntityFrameworkCore.Storage;
using DocumentWorkflow.Domain.Interfaces;
using DocumentWorkflow.Infrastructure.Data;

namespace DocumentWorkflow.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        
        Documents = new DocumentRepository(_context);
        WorkflowDefinitions = new WorkflowDefinitionRepository(_context);
        WorkflowInstances = new WorkflowInstanceRepository(_context);
        WorkflowSteps = new WorkflowStepRepository(_context);
        ApprovalTasks = new ApprovalTaskRepository(_context);
        ApprovalPolicies = new ApprovalPolicyRepository(_context);
    }

    public IDocumentRepository Documents { get; }
    public IWorkflowDefinitionRepository WorkflowDefinitions { get; }
    public IWorkflowInstanceRepository WorkflowInstances { get; }
    public IWorkflowStepRepository WorkflowSteps { get; }
    public IApprovalTaskRepository ApprovalTasks { get; }
    public IApprovalPolicyRepository ApprovalPolicies { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
