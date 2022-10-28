using System.Text.RegularExpressions;

namespace MSBuild.EnsureFileHeaders;
using JustinWritesCode.GitHub;

public abstract class FileHeaderLanguageProvider
{
    public abstract string[] Extensions { get; }
    public abstract void WriteFileHeader(string filePath, SoftwareLicense license, ITaskItem[] authors, string projectUrl, FileHeaderFormat format = FileHeaderFormat.Short);
    public virtual bool CanPutFileHeaderOnFile(string fileName) => Extensions.Any(e => fileName.EndsWith(e, StringComparison.InvariantCultureIgnoreCase));
}
