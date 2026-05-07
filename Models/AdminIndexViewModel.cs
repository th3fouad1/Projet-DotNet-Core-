using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Models;
using System.Collections.Generic;

namespace ASVS_Security_Auditor.Models;

public class AdminIndexViewModel
{
    public ImportViewModel ImportModel { get; set; } = new ImportViewModel();
    public List<AsvsRequirement> Requirements { get; set; } = new List<AsvsRequirement>();
}


