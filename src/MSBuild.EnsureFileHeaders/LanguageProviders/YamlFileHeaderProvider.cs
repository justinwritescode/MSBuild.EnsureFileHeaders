using System.Text;

namespace JustinWritesCode.MSBuild.EnsureFileHeaders;

public class YamlFileHeaderProvider : FileHeaderLanguageProvider
{
    public override string[] Extensions => new[] { ".yml", ".yaml" };
    public override void WriteFileHeader(string filePath, string license, ITaskItem[] authors)
    {
        var fileContents = File.ReadAllText(filePath);
        var fileHeader = new StringBuilder();
        fileHeader.AppendLine("##   ");
        fileHeader.AppendLine($"##   {new FileInfo(filePath).Name}");
        fileHeader.AppendLine("##   ");
        if (authors.Length > 1)
        {
            fileHeader.AppendLine("##   Authors:");
            foreach (var author in authors)
            {
                fileHeader.AppendLine($"##   {author.ItemSpec} <{author.GetMetadata("Email")}>");
            }
        }
        else if (authors.Length == 1)
        {
            fileHeader.AppendLine($"##    Author: {authors[0].ItemSpec} <{authors[0].GetMetadata("Email")}>");
        }
        fileHeader.AppendLine("##   ");
        fileHeader.AppendLine($"##   Copyright © {DateTime.Now.Year} {string.Join(", ", authors.Select(a => a.ItemSpec))}, All rights reserved.");
        fileHeader.AppendLine("##   ");
        fileHeader.AppendLine($"##   {Regex.Replace(license, "^", "^##  ")}");
        fileHeader.AppendLine("##   ");
        fileContents = Regex.Replace(@"^##   .*$", fileContents, ""); // Remove existing file headers
        fileContents = fileHeader.ToString() + fileContents;
        File.WriteAllText(filePath, fileContents);
    }
}
