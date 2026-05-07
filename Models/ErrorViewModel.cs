using ASVS_Security_Auditor.Core.Entities;
namespace ASVS_Security_Auditor.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

