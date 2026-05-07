using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASVS_Security_Auditor.Core.Interfaces;
using ASVS_Security_Auditor.Core.Services;
using ASVS_Security_Auditor.Infrastructure.Parsers;
using ASVS_Security_Auditor.Models;
using ASVS_Security_Auditor.Models;
using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ASVS_Security_Auditor.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAsvsImportService _importService;
    private readonly IAsvsRequirementRepository _repository;

    public AdminController(IAsvsImportService importService, IAsvsRequirementRepository repository)
    {
        _importService = importService;
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new AdminIndexViewModel
        {
            Requirements = (await _repository.GetAllAsync()).ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Import(ImportViewModel importModel)
    {
        if (importModel.ExcelFile != null && importModel.ExcelFile.Length > 0)
        {
            using var stream = importModel.ExcelFile.OpenReadStream();
            var count = await _importService.ImportFromExcelAsync(stream);
            TempData["Message"] = $"Importation réussie de {count} exigences ASVS.";
        }
        else
        {
            TempData["Error"] = "Veuillez sélectionner un fichier Excel valide.";
        }
        return RedirectToAction("Index");
    }

    // CREATE
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AsvsRequirement req)
    {
        if (ModelState.IsValid)
        {
            await _repository.AddAsync(req);
            TempData["Message"] = "Exigence créée avec succès.";
            return RedirectToAction(nameof(Index));
        }
        return View(req);
    }

    // UPDATE (Edit)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var req = await _repository.GetByIdAsync(id.Value);
        if (req == null) return NotFound();
        return View(req);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AsvsRequirement req)
    {
        if (id != req.Id) return NotFound();

        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(req);
            TempData["Message"] = "Exigence modifiée avec succès.";
            return RedirectToAction(nameof(Index));
        }
        return View(req);
    }

    // DELETE
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var req = await _repository.GetByIdAsync(id);
        if (req != null)
        {
            await _repository.DeleteAsync(req);
            TempData["Message"] = "Exigence supprimée avec succès.";
        }
        return RedirectToAction(nameof(Index));
    }
}


