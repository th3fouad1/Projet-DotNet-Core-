using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASVS_Security_Auditor.Core.Interfaces;
using ASVS_Security_Auditor.Models;
using ASVS_Security_Auditor.Core.Entities;

namespace ASVS_Security_Auditor.Controllers;

public class DashboardController : Controller
{
    private readonly IAssessmentRepository _assessmentRepo;

    public DashboardController(IAssessmentRepository assessmentRepo)
    {
        _assessmentRepo = assessmentRepo;
    }

    public async Task<IActionResult> Index(int? assessmentId)
    {
        var assessments = await _assessmentRepo.GetAllWithItemsAsync();
        ViewBag.Assessments = assessments;

        if (assessmentId == null && assessments.Any())
        {
            assessmentId = assessments.First().Id;
        }

        if (assessmentId != null)
        {
            var assessment = await _assessmentRepo.GetByIdWithItemsAsync(assessmentId.Value);

            if (assessment != null)
            {
                var total = assessment.Items.Count;
                var valid = assessment.Items.Count(i => i.Status == AssessmentStatus.Valid);
                var notValid = assessment.Items.Count(i => i.Status == AssessmentStatus.NotValid);
                var na = assessment.Items.Count(i => i.Status == AssessmentStatus.NotApplicable);
                var pending = assessment.Items.Count(i => i.Status == AssessmentStatus.Pending);
                
                var score = total - na > 0 ? (double)valid / (total - na) * 100 : 0;

                string riskLevel = "Inconnu";
                string riskBadge = "bg-secondary";
                if (total - pending - na == 0) {
                    riskLevel = "En cours";
                    riskBadge = "bg-secondary";
                } else if (score < 50) {
                    riskLevel = "Critique";
                    riskBadge = "bg-danger";
                } else if (score < 80) {
                    riskLevel = "Modéré";
                    riskBadge = "bg-warning text-dark";
                } else {
                    riskLevel = "Faible";
                    riskBadge = "bg-success";
                }

                ViewBag.Score = score;
                ViewBag.Stats = new[] { valid, notValid, na, pending };
                ViewBag.RiskLevel = riskLevel;
                ViewBag.RiskBadge = riskBadge;

                ViewBag.WeakAreas = assessment.Items
                    .Where(i => i.Status == AssessmentStatus.NotValid)
                    .GroupBy(i => i.AsvsRequirement.ChapterName)
                    .Select(g => new { Chapter = g.Key, Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .Take(3)
                    .ToDictionary(g => g.Chapter, g => g.Count);

                return View(assessment);
            }
        }

        return View(null);
    }
}


