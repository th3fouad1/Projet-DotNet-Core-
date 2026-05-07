using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ASVS_Security_Auditor.Infrastructure.Data;
using ASVS_Security_Auditor.Infrastructure.Parsers;
using ASVS_Security_Auditor.Core.Interfaces;
using ASVS_Security_Auditor.Core.Services;

namespace ASVS_Security_Auditor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Keep the connection string logic in Program.cs or move it here. For now, let's just register services:
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<IAsvsRequirementRepository, AsvsRequirementRepository>();
        services.AddScoped<IAsvsImportService, AsvsImportService>();
        services.AddScoped<IGroqAiService, GroqAiService>();
        
        return services;
    }
}
