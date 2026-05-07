using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASVS_Security_Auditor.Models;
using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Core.Interfaces;
using ASVS_Security_Auditor.Core.Services;
using ASVS_Security_Auditor.Infrastructure.Parsers;
using ASVS_Security_Auditor.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ASVS_Security_Auditor.Controllers;

[Authorize]
public class AssessmentController : Controller
{
    private readonly IAssessmentRepository _assessmentRepo;
    private readonly IAsvsRequirementRepository _reqRepo;
    private readonly IGroqAiService _aiService;

    public AssessmentController(IAssessmentRepository assessmentRepo, IAsvsRequirementRepository reqRepo, IGroqAiService aiService)
    {
        _assessmentRepo = assessmentRepo;
        _reqRepo = reqRepo;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var assessments = await _assessmentRepo.GetAllWithItemsAsync();
        return View(assessments);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string name)
    {
        var requirements = await _reqRepo.GetAllAsync();
        if (!requirements.Any())
        {
            TempData["Error"] = "No ASVS requirements found in the database. Please import them first.";
            return RedirectToAction("Index");
        }

        var assessment = new Assessment
        {
            Name = string.IsNullOrWhiteSpace(name) ? "New Assessment" : name,
            Items = requirements.Select(r => new AssessmentItem
            {
                AsvsRequirementId = r.Id,
                Status = AssessmentStatus.Pending
            }).ToList()
        };

        await _assessmentRepo.AddAsync(assessment);

        return RedirectToAction("Details", new { id = assessment.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var assessment = await _assessmentRepo.GetByIdWithItemsAsync(id);

        if (assessment == null) return NotFound();

        return View(assessment);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int itemId, AssessmentStatus status, string notes)
    {
        var item = await _assessmentRepo.GetItemByIdAsync(itemId);
        if (item != null)
        {
            item.Status = status;
            item.Notes = notes;
            await _assessmentRepo.UpdateItemAsync(item);
        }
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ExplainRequirement(int reqId)
    {
        var req = await _reqRepo.GetByIdAsync(reqId);
        if (req == null) return NotFound();

        var explanation = await _aiService.GetRequirementExplanationAsync(req.Description);
        return Json(new { explanation });
    }

    [HttpPost]
    public async Task ChatStream([FromBody] ChatRequestDto request)
    {
        if (request == null || !request.Messages.Any()) return;
        
        var req = await _reqRepo.GetByIdAsync(request.RequirementId);
        if (req == null) return;

        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var fullMessages = new System.Collections.Generic.List<object>
        {
            new { role = "system", content = "Tu es un expert en cybersécurité francophone. Tu aides l'utilisateur à comprendre l'exigence ASVS suivante : " + req.Description + ". Réponds de manière concise et toujours en français." }
        };
        
        fullMessages.AddRange(request.Messages.Select(m => new { role = m.Role, content = m.Content }));

        await foreach (var chunk in _aiService.StreamChatAsync(fullMessages))
        {
            if (!string.IsNullOrEmpty(chunk))
            {
                // We send the chunk encoded to avoid breaking the SSE format
                var encoded = chunk.Replace("\n", "\\n").Replace("\r", "\\r");
                await Response.WriteAsync($"data: {encoded}\n\n");
                await Response.Body.FlushAsync();
            }
        }
        await Response.WriteAsync("data: [DONE]\n\n");
        await Response.Body.FlushAsync();
    }
}


