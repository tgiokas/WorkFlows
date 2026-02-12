using Microsoft.AspNetCore.Mvc;
using DocumentWorkflow.Application.DTOs;
using DocumentWorkflow.Application.Interfaces;

namespace DocumentWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowsController : ControllerBase
{
    private readonly IWorkflowService _workflowService;
    private readonly ILogger<WorkflowsController> _logger;

    public WorkflowsController(
        IWorkflowService workflowService,
        ILogger<WorkflowsController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    /// <summary>
    /// Get all workflow definitions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkflowDefinitionDto>>> GetAll()
    {
        try
        {
            var workflows = await _workflowService.GetAllWorkflowsAsync();
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflows");
            return StatusCode(500, "Error retrieving workflows");
        }
    }

    /// <summary>
    /// Get workflow by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkflowDefinitionDto>> GetById(Guid id)
    {
        try
        {
            var workflow = await _workflowService.GetWorkflowByIdAsync(id);
            
            if (workflow == null)
                return NotFound($"Workflow with ID {id} not found");
            
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow {WorkflowId}", id);
            return StatusCode(500, "Error retrieving workflow");
        }
    }

    /// <summary>
    /// Create or update workflow definition
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkflowDefinitionDto>> CreateOrUpdate(
        [FromBody] CreateWorkflowDefinitionDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var workflow = await _workflowService.SaveWorkflowDefinitionAsync(dto);
            
            return CreatedAtAction(
                nameof(GetById), 
                new { id = workflow.Id }, 
                workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving workflow");
            return StatusCode(500, "Error saving workflow");
        }
    }

    /// <summary>
    /// Delete workflow definition
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _workflowService.DeleteWorkflowAsync(id);
            
            if (!result)
                return NotFound($"Workflow with ID {id} not found");
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workflow {WorkflowId}", id);
            return StatusCode(500, "Error deleting workflow");
        }
    }

    /// <summary>
    /// Get active workflow for a document type
    /// </summary>
    [HttpGet("active/{documentType}")]
    public async Task<ActionResult<WorkflowDefinitionDto>> GetActiveByDocumentType(string documentType)
    {
        try
        {
            var workflow = await _workflowService.GetActiveWorkflowForDocumentTypeAsync(documentType);
            
            if (workflow == null)
                return NotFound($"No active workflow found for document type: {documentType}");
            
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow for document type {DocumentType}", documentType);
            return StatusCode(500, "Error retrieving workflow");
        }
    }

    /// <summary>
    /// Validate BPMN XML
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<BpmnValidationResult>> ValidateBpmn([FromBody] string bpmnXml)
    {
        try
        {
            var result = await _workflowService.ValidateBpmnAsync(bpmnXml);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating BPMN");
            return StatusCode(500, "Error validating BPMN");
        }
    }

    /// <summary>
    /// Parse BPMN and get structure
    /// </summary>
    [HttpPost("parse")]
    public async Task<ActionResult<object>> ParseBpmn([FromBody] string bpmnXml)
    {
        try
        {
            var structure = await _workflowService.ParseBpmnStructureAsync(bpmnXml);
            return Ok(structure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing BPMN");
            return StatusCode(500, "Error parsing BPMN");
        }
    }
}
