using DocumentWorkflow.Application.DTOs;
using DocumentWorkflow.Application.Interfaces;
using DocumentWorkflow.Domain.Entities;
using DocumentWorkflow.Domain.Interfaces;

namespace DocumentWorkflow.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWorkflowEngineService _workflowEngine;

    public DocumentService(IUnitOfWork unitOfWork, IWorkflowEngineService workflowEngine)
    {
        _unitOfWork = unitOfWork;
        _workflowEngine = workflowEngine;
    }

    public async Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto dto)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            FileName = dto.FileName,
            FilePath = dto.FilePath,
            DocumentType = dto.DocumentType,
            Department = dto.Department,
            Amount = dto.Amount,
            Status = "Draft",
            SubmittedBy = dto.SubmittedBy,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Documents.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(document);
    }

    public async Task<DocumentDto?> GetDocumentByIdAsync(Guid id)
    {
        var document = await _unitOfWork.Documents.GetByIdAsync(id);
        return document == null ? null : MapToDto(document);
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsByStatusAsync(string status)
    {
        var documents = await _unitOfWork.Documents.GetByStatusAsync(status);
        return documents.Select(MapToDto);
    }

    public async Task<bool> SubmitDocumentForApprovalAsync(Guid documentId)
    {
        var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
        if (document == null) return false;

        // Find active workflow for this document type
        var workflow = await _unitOfWork.WorkflowDefinitions
            .GetActiveByDocumentTypeAsync(document.DocumentType);

        if (workflow == null)
            throw new InvalidOperationException($"No active workflow found for document type: {document.DocumentType}");

        // Update document status
        document.Status = "Submitted";
        document.SubmittedAt = DateTime.UtcNow;
        await _unitOfWork.Documents.UpdateAsync(document);

        // Start workflow
        await _workflowEngine.StartWorkflowAsync(workflow.Id, documentId);
        
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static DocumentDto MapToDto(Document document)
    {
        return new DocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            FileName = document.FileName,
            DocumentType = document.DocumentType,
            Department = document.Department,
            Amount = document.Amount,
            Status = document.Status,
            SubmittedBy = document.SubmittedBy,
            CreatedAt = document.CreatedAt,
            SubmittedAt = document.SubmittedAt
        };
    }
}
