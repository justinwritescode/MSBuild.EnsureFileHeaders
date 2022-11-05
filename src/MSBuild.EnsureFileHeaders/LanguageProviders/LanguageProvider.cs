// 
// 
// LanguageProvider.cs
// 
//   Created: 2022-10-27-05:35:36
//   Modified: 2022-10-29-02:44:10
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

namespace MSBuild.EnsureFileHeaders;
using System.Text.RegularExpressions;
using JustinWritesCode.GitHub;
public abstract class FileHeaderLanguageProvider
{
    public abstract string CommentBegin { get; }
    public abstract string CommentPrefix { get; }
    public abstract string CommentEnd { get; }
    public abstract string[] Extensions { get; }
    public virtual bool CanPutFileHeaderOnFile(string fileName) => Extensions.Any(e => fileName.EndsWith(e, StringComparison.InvariantCultureIgnoreCase));
    
    public virtual void WriteFileHeader(FileHeaderArgs args)
    {
        var fileHeader = args.Format == fileHeader = FileHeaderFormat.Short ? MakeShortFileHeader(args) : MakeLongFileHeader(args);
        var fileContents = File.ReadAllText(args.FilePath);
        var fileLines = fileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        var inHeader = false;
        for(var i = 0; i < fileLines.Length; i++)
        {
            if(fileLines[i].StartsWith(CommentBegin))
            {
                inHeader = true;
            }
            else if(inHeader && fileLines[i].StartsWith(CommentPrefix))
            {
                continue;
            }
            else if(inHeader && fileLines[i].StartsWith(CommentEnd))
            {
                fileLines = fileLines.Skip(i).ToArray();
            }
            else if(!inHeader)
            {
                continue;
            }
        }
        fileContents = fileHeader + string.Join(Environment.NewLine, fileLines);
        File.WriteAllText(args.FilePath, fileContents);
    }

    public virtual string MakeLongFileHeader(FileHeaderArgs args)
    {
        var fileHeader = new StringBuilder();
        fileHeader.AppendLine(CommentBegin);
        fileHeader.AppendLine($"{CommentPrefix} {new FileInfo(args.FilePath).Name}");
        fileHeader.AppendLine(CommentPrefix);
        if (authors.Length > 1)
        {
            fileHeader.AppendLine($"{CommentPrefix} Authors:");
            foreach (var author in authors)
            {
                fileHeader.AppendLine($"{CommentPrefix} {author.ItemSpec} <{author.GetMetadata("Email")}>");
            }
        }
        else if (authors.Length == 1)
        {
            fileHeader.AppendLine($"{CommentPrefix} Author: {authors[0].ItemSpec} <{authors[0].GetMetadata("Email")}>");
        }
        fileHeader.AppendLine(CommentPrefix);
        fileHeader.AppendLine($"{CommentPrefix} Copyright © {DateTime.Now.Year} {string.Join(", ", authors.Select(a => a.ItemSpec))}, All rights reserved.");
        fileHeader.AppendLine(CommentPrefix);
        fileHeader.AppendLine($"{CommentPrefix} {Regex.Replace(license.Content, "^", $"{CommentPrefix} ")}");
        fileHeader.AppendLine(CommentPrefix);
        fileHeader.AppendLine(CommentEnd);
        return fileHeader.ToString();
    }

    public virtual string MakeShortFileHeader(FileHeaderArgs args)
    {
        var fileHeader = new StringBuilder();
        fileHeader.AppendLine(CommentBegin);
        fileHeader.AppendLine($"{CommentPrefix} <file name=\"{new FileInfo(args.FilePath).Name)}\">");
        fileHeader.AppendLine($"{CommentPrefix}   <copyright year=\"{DateTime.Now.Year}\" license=\"{args.License}>");
        foreach(var author in args.Authors)
        {
            fileHeader.AppendLine($"{CommentPrefix}   <author name=\"{author.ItemSpec} email=\"{author.GetMetadata("Email")} />");
        }
        <file name=\"{new FileInfo(args.FilePath).Name)}\" year=\"{DateTime.Now.Year}\" license=\"{args.License}>");
        =\"{string.Join(", ", authors.Select(author => $"{author.ItemSpec} <{(author.GetMetadata("Email"))}>"))}\"
        fileHeader.AppendLine(CommentPrefix);
        if (authors.Length > 1)
        {
            fileHeader.AppendLine($"{CommentPrefix} Authors:");
            foreach (var author in authors)
            {
                fileHeader.AppendLine($"{CommentPrefix} {author.ItemSpec} <{author.GetMetadata("Email")}>");
            }
        }
        else if (authors.Length == 1)
        {
            fileHeader.AppendLine($"{CommentPrefix} Author: {authors[0].ItemSpec} <{authors[0].GetMetadata("Email")}>");
        }
        fileHeader.AppendLine(CommentPrefix);
        fileHeader.AppendLine($"{CommentPrefix} See {args.ProjectUrl} for license information.");
        fileHeader.AppendLine(CommentPrefix);
        fileHeader.AppendLine(CommentEnd);
        return fileHeader.ToString();
    }
}
