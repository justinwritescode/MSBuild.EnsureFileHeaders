using System.Text.RegularExpressions;

namespace JustinWritesCode.MSBuild.EnsureFileHeaders;

public abstract class FileHeaderLanguageProvider
{
    public abstract string[] Extensions { get; }
    public abstract void WriteFileHeader(string filePath, string license, ITaskItem[] authors, FileHeaderFormat format = FileHeaderFormat.Short);
    public virtual bool CanPutFileHeaderOnFile(string fileName) => Extensions.Any(e => fileName.EndsWith(e, StringComparison.OrdinalIgnoreCase));
}
