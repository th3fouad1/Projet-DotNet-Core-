namespace ASVS_Security_Auditor.Core.Entities;

public class AsvsRequirement
{
    public int Id { get; set; }
    public string ChapterId { get; set; } = string.Empty;
    public string ChapterName { get; set; } = string.Empty;
    public string SectionId { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string Item { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool L1 { get; set; }
    public bool L2 { get; set; }
    public bool L3 { get; set; }
    public string? Cwe { get; set; }
    public string? Nist { get; set; }
}

