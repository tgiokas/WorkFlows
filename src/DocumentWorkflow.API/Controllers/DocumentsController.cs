using Microsoft.AspNetCore.Mvc;
using DocumentWorkflow.Application.DTOs;
using DocumentWorkflow.Application.Interfaces;

namespace DocumentWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentService documentService,
        ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll([FromQuery] string? status = null)
    {
        try
        {
            var documents = status == null 
                ? await _documentService.GetDocumentsByStatusAsync("Draft")
                : await _documentService.GetDocumentsByStatusAsync(status);
            
            return Ok(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents");
            return StatusCode(500, "Error retrieving documents");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetById(Guid id)
    {
        try
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound();
            
            return Ok(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document {DocumentId}", id);
            return StatusCode(500, "Error retrieving document");
        }
    }

    [HttpPost]
    public async Task<ActionResult<DocumentDto>> Create([FromBody] CreateDocumentDto dto)
    {
        try
        {
            var document = await _documentService.CreateDocumentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document");
            return StatusCode(500, "Error creating document");
        }
    }

    [HttpPost("{id}/submit")]
    public async Task<ActionResult> SubmitForApproval(Guid id)
    {
        try
        {
            var success = await _documentService.SubmitDocumentForApprovalAsync(id);
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Document submitted for approval" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting document {DocumentId}", id);
            return StatusCode(500, ex.Message);
        }
    }
}
