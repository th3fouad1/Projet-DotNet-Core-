using ASVS_Security_Auditor.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ASVS_Security_Auditor.Models;

namespace ASVS_Security_Auditor.Core.Interfaces;

public interface IAsvsImportService
{
    Task<int> ImportFromExcelAsync(Stream excelStream);
}


