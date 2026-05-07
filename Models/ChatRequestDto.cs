using ASVS_Security_Auditor.Core.Entities;
using System.Collections.Generic;

namespace ASVS_Security_Auditor.Models;

public class ChatMessageDto
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class ChatRequestDto
{
    public int RequirementId { get; set; }
    public List<ChatMessageDto> Messages { get; set; } = new();
}


