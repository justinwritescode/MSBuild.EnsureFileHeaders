using System.IO;
using System.Text;

namespace JustinWritesCode.MSBuild.EnsureFileHeaders;

public class CSharpFileHeaderProvider : FileHeaderLanguageProvider
{
    public override string[] Extensions => new[] { ".cs" };
    public override void WriteFileHeader(string filePath, string license, ITaskItem[] authors, FileHeaderFormat format = FileHeaderFormat.Short)
    {
        if (format == FileHeaderFormat.Short)
        {
            WriteShortFileHeader(filePath, license, authors);
        }
        else
        {
            WriteLongFileHeader(filePath, license, authors);
        }
    }

    private void WriteLongFileHeader(string filePath, string license, ITaskItem[] authors)
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
        fileHeader.AppendLine($"//    {Regex.Replace(license, "^", "^// ")}");
        fileHeader.AppendLine("//    ");
        fileContents = Regex.Replace(@"(^\/\/    .*){2,}", fileContents, ""); // Remove existing file headers
        fileContents = fileHeader.ToString() + fileContents;
        File.WriteAllText(filePath, fileContents);
    }

    private void WriteShortFileHeader(string filePath, string license, ITaskItem[] authors)
    {
        var fileHeaderContent = $"//  <copyright file=\"${Path.GetFileName(filePath)}\"> ©{DateTime.Now.Year} {authors.Select(author => $"{author.ItemSpec} <{(author.GetMetadata("Email"))}>")} (https://docs.justinwritescode.com) under MIT License. See https://opensource.org/licenses/MIT </copyright>";
        var fileHeader = $@"//  {new string('-', fileHeaderContent.Length - 4)}{Environment.NewLine}{fileHeaderContent}{Environment.NewLine}//  {new string('-', fileHeaderContent.Length - 4)}
 ";

        var fileContents = File.ReadAllText(filePath);
        fileContents = Regex.Replace(@"(^\/\/  \-+\n.*\n\/\/  \-+\n", fileContents, ""); // Remove existing file headers
        fileContents = fileHeader.ToString() + fileContents;
        File.WriteAllText(filePath, fileContents);
    }

}
