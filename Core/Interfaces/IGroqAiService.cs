using ASVS_Security_Auditor.Models;
using ASVS_Security_Auditor.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASVS_Security_Auditor.Core.Interfaces;

public interface IGroqAiService
{
    Task<string> GetRequirementExplanationAsync(string requirementDescription);
    Task<string> ContinueChatAsync(IEnumerable<object> messages);
    IAsyncEnumerable<string> StreamChatAsync(IEnumerable<object> messages);
}


