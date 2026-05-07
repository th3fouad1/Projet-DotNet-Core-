using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ASVS_Security_Auditor.Core.Entities;

public class Assessment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }
    
    public ICollection<AssessmentItem> Items { get; set; } = new List<AssessmentItem>();
}

