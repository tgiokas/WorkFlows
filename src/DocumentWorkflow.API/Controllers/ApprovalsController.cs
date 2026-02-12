using Microsoft.AspNetCore.Mvc;
using DocumentWorkflow.Application.DTOs;
using DocumentWorkflow.Application.Interfaces;

namespace DocumentWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly IApprovalService _approvalService;
    private readonly ILogger<ApprovalsController> _logger;

    public ApprovalsController(
        IApprovalService approvalService,
        ILogger<ApprovalsController> logger)
    {
        _approvalService = approvalService;
        _logger = logger;
    }

    [HttpGet("my-tasks")]
    public async Task<ActionResult<IEnumerable<ApprovalTaskDto>>> GetMyTasks([FromQuery] string userId = "manager@company.com")
    {
        try
        {
            var tasks = await _approvalService.GetPendingTasksForUserAsync(userId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return StatusCode(500, "Error retrieving tasks");
        }
    }

    [HttpGet("document/{documentId}")]
    public async Task<ActionResult<IEnumerable<ApprovalTaskDto>>> GetTasksForDocument(Guid documentId)
    {
        try
        {
            var tasks = await _approvalService.GetTasksForDocumentAsync(documentId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for document");
            return StatusCode(500, "Error retrieving tasks");
        }
    }

    [HttpPost("complete")]
    public async Task<ActionResult> CompleteTask([FromBody] CompleteApprovalTaskDto dto)
    {
        try
        {
            var success = await _approvalService.CompleteApprovalTaskAsync(dto);
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Task completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing task");
            return StatusCode(500, "Error completing task");
        }
    }
}
