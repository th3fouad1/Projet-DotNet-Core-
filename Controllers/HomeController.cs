using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASVS_Security_Auditor.Models;
using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Core.Interfaces;
using System.Threading.Tasks;

namespace ASVS_Security_Auditor.Controllers;

public class HomeController : Controller
{
    private readonly IAsvsRequirementRepository _repository;

    public HomeController(IAsvsRequirementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        var requirements = await _repository.GetAllAsync();
        return View(requirements);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


