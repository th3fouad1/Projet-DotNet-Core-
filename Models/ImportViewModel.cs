using ASVS_Security_Auditor.Core.Entities;
using Microsoft.AspNetCore.Http;
namespace ASVS_Security_Auditor.Models;

public class ImportViewModel
{
    public IFormFile? ExcelFile { get; set; }
}


