using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Core.Interfaces;
using ASVS_Security_Auditor.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASVS_Security_Auditor.Models;
using ClosedXML.Excel;

namespace ASVS_Security_Auditor.Infrastructure.Parsers;

public class AsvsImportService : IAsvsImportService
{
    private readonly ApplicationDbContext _context;

    public AsvsImportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> ImportFromExcelAsync(Stream excelStream)
    {
        using var workbook = new XLWorkbook(excelStream);
        var newRequirements = new List<AsvsRequirement>();

        foreach (var worksheet in workbook.Worksheets)
        {
            // Ignorer la feuille de résumé
            if (worksheet.Name.Contains("ASVS Status", StringComparison.OrdinalIgnoreCase) || 
                worksheet.Name.Contains("Dashboard", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            string currentChapterId = "";
            string currentChapterName = worksheet.Name;
            string currentSectionId = "";
            string currentSectionName = "";

            // On devine l'ID du chapitre à partir du nom (ex: "V1 Architecture" -> "V1")
            var chapterMatch = Regex.Match(worksheet.Name, @"V(\d+)");
            if (chapterMatch.Success)
            {
                currentChapterId = "V" + chapterMatch.Groups[1].Value;
            }

            foreach (var row in rows)
            {
                try
                {
                    var itemId = row.Cell(1).GetString().Trim();
                    var levelStr = row.Cell(3).GetString().Trim();
                    var description = row.Cell(4).GetString().Trim();
                    var cwe = row.Cell(5).GetString().Trim(); // Parfois colonne 2 ou 5

                    // Si pas d'item ID, c'est sûrement un titre de section
                    if (string.IsNullOrEmpty(itemId))
                    {
                        if (!string.IsNullOrEmpty(description) && description.StartsWith("V"))
                        {
                            var parts = description.Split(' ', 2);
                            if (parts.Length == 2)
                            {
                                currentSectionId = parts[0];
                                currentSectionName = parts[1];
                            }
                        }
                        continue;
                    }

                    bool l1 = levelStr.Contains("1");
                    bool l2 = levelStr.Contains("2") || l1; // Si L1 est requis, L2 et L3 le sont souvent (dépend des versions, mais on force l'inclusion)
                    bool l3 = levelStr.Contains("3") || levelStr.Contains("2") || l1;
                    
                    if (levelStr == "2") { l1 = false; l2 = true; l3 = true; }
                    if (levelStr == "3") { l1 = false; l2 = false; l3 = true; }

                    var item = new AsvsRequirement
                    {
                        ChapterId = string.IsNullOrEmpty(currentChapterId) ? "V" + itemId.Split('.')[0] : currentChapterId,
                        ChapterName = currentChapterName,
                        SectionId = currentSectionId,
                        SectionName = currentSectionName,
                        Item = itemId,
                        Description = description,
                        L1 = l1,
                        L2 = l2,
                        L3 = l3,
                        Cwe = cwe
                    };

                    newRequirements.Add(item);
                }
                catch (Exception)
                {
                    // Skip errors
                }
            }
        }

        if (newRequirements.Any())
        {
            _context.AsvsRequirements.RemoveRange(_context.AsvsRequirements);
            await _context.SaveChangesAsync();

            await _context.AsvsRequirements.AddRangeAsync(newRequirements);
            await _context.SaveChangesAsync();
        }

        return newRequirements.Count;
    }
}


