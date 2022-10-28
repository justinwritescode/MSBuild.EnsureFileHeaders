//  -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//  <copyright file="CSharpFileHeaderProvider.cs" year="©2022" authors="Justin Chase <justin@justinwritescode.com>" projectUrl="https://docs.justinwritescode.com/MSBuild.EnsureFileHeaders" license="MIT License" licenseUrl="https://api.github.com/licenses/mit" />
//  -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
using System.IO;
using System.Text;

namespace MSBuild.EnsureFileHeaders;
using JustinWritesCode.GitHub;

public class CSharpFileHeaderProvider : FileHeaderLanguageProvider
{
    public override string[] Extensions => new[] { ".cs" };
    public override void WriteFileHeader(string filePath, SoftwareLicense license, ITaskItem[] authors, string projectUrl, FileHeaderFormat format = FileHeaderFormat.Short)
    {
        if (format == FileHeaderFormat.Short)
        {
            WriteShortFileHeader(filePath, license, authors, projectUrl);
        }
        else
        {
            WriteLongFileHeader(filePath, license, authors, projectUrl);
        }
    }

    private void WriteLongFileHeader(string filePath, SoftwareLicense license, ITaskItem[] authors, string projectUrl)
    {
        var fileContents = File.ReadAllText(filePath);
        var fileHeader = new StringBuilder();
        fileHeader.AppendLine("//    ");
        fileHeader.AppendLine($"//    {new FileInfo(filePath).Name}");
        fileHeader.AppendLine("//    ");
        if (authors.Length > 1)
        {
            fileHeader.AppendLine("//    Authors:");
            foreach (var author in authors)
            {
                fileHeader.AppendLine($"//    {author.ItemSpec} <{author.GetMetadata("Email")}>");
            }
        }
        else if (authors.Length == 1)
        {
            fileHeader.AppendLine($"//    Author: {authors[0].ItemSpec} <{authors[0].GetMetadata("Email")}>");
        }
        fileHeader.AppendLine("//    ");
        fileHeader.AppendLine($"//    Copyright © {DateTime.Now.Year} {string.Join(", ", authors.Select(a => a.ItemSpec))}, All rights reserved.");
        fileHeader.AppendLine("//    ");
        fileHeader.AppendLine($"//    {Regex.Replace(license.Content, "^", "^//    ")}");
        fileHeader.AppendLine("//    ");
        fileContents = Regex.Replace(@"(^\/\/    .*){2,}", fileContents, ""); // Remove existing file headers
        fileContents = fileHeader.ToString() + fileContents;
        File.WriteAllText(filePath, fileContents);
    }

    private void WriteShortFileHeader(string filePath, SoftwareLicense license, ITaskItem[] authors, string projectUrl)
    {
        var fileHeaderContent = $"//  <copyright file=\"{Path.GetFileName(filePath)}\" year=\"©{DateTime.Now.Year}\" authors=\"{string.Join(", ", authors.Select(author => $"{author.ItemSpec} <{(author.GetMetadata("Email"))}>"))}\" projectUrl=\"{projectUrl}\" license=\"{license.Name}\" licenseUrl=\"{license.Url}\" />";
        var fileHeader = $@"//  {new string('-', fileHeaderContent.Length - 4)}{Environment.NewLine}{fileHeaderContent}{Environment.NewLine}//  {new string('-', fileHeaderContent.Length - 4)}
 ";

        var inHeader = false;
        var fileContents = File.ReadAllText(filePath);
        var fileLines = fileContents.Split('\n');
        for (var i = 0; i < fileLines.Length; i++)
        {
            if (fileLines[i].StartsWith("//  ") && !inHeader)
            {
                inHeader = true;
            }
            else if (fileLines[i].StartsWith("//  ") && inHeader)
            {
                continue;
            }
            else if (!fileLines[i].StartsWith("//  ") && inHeader)
            {
                File.WriteAllText(filePath, fileHeader);
                File.AppendAllLines(filePath, fileLines.Skip(i));
            }
            return;
        }
    }
}
