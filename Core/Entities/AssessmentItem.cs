namespace ASVS_Security_Auditor.Core.Entities;

public class AssessmentItem
{
    public int Id { get; set; }
    public int AssessmentId { get; set; }
    public Assessment Assessment { get; set; } = null!;
    
    public int AsvsRequirementId { get; set; }
    public AsvsRequirement AsvsRequirement { get; set; } = null!;
    
    public AssessmentStatus Status { get; set; } = AssessmentStatus.Pending;
    public string? Notes { get; set; }
}

