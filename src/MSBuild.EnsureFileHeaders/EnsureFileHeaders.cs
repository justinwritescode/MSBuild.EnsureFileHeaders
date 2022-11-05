/*
 * <file name="EnsureFileHeaders.cs" created="Thursday, 27th October 2022 5:35:36 am" modified="<<filemodified('dddd, Do MMMM YYYY h:mm:ss a')>>">
 *   <author name="Justin Chase" email="justin@justinwritescode.com" />
 *   <copyright year="projectCreationYear-2022" copyrightHolder="JustinWritesCode" license="<<license>>" />
 * </file>
 */

namespace MSBuild.EnsureFileHeaders;
using JustinWritesCode.GitHub;
using Microsoft.Build.Utilities;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

public class EnsureFileHeaders : MSBTask
{
    // [Required]
    // public string SearchRoot { get; set; } = string.Empty;
    [Required]
    public string License { get; set; } = SoftwareLicenseEnum.MIT.ToString();
    public string ProjectUrl { get; set; } = string.Empty;
    [Required]
    public ITaskItem[] Include { get; set; } = Array.Empty<ITaskItem>();
    public ITaskItem[] Exclude { get; set; } = Array.Empty<ITaskItem>();
    public ITaskItem[] Authors { get; set; } = Array.Empty<ITaskItem>();
    [Output]
    public ITaskItem[] Files { get; set; } = Array.Empty<ITaskItem>();

    public static readonly Array ValidLicenseValues = System.Enum.GetValues(typeof(SoftwareLicenseEnum));

    public override bool Execute()
    {
        var changedFiles = new List<ITaskItem>();
        var license = ((SoftwareLicenseEnum)Enum.Parse(typeof(SoftwareLicenseEnum), License)).GetLicenseAsync().GetAwaiter().GetResult();
        //Console.WriteLine(license.Name);
        //var license = new SoftwareLicense();
        var include = Include.Select(i => i.ItemSpec).ToList();
        var exclude = Exclude?.Select(i => i.ItemSpec).ToList() ?? new List<string>();
        var matcher = new Matcher();
        matcher.AddIncludePatterns(include);
        matcher.AddExcludePatterns(exclude);
        var files = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(Environment.CurrentDirectory))).Files.Select(f => f.Path).ToList();
        //var files = Directory.EnumerateFileSystemEntries() include.SelectMany(i => Directory.EnumerateFiles(i, "*.*", SearchOption.AllDirectories)).Where(f => !exclude.Contains(f)).ToList();

        Console.WriteLine($"Found {files.Count} files to check for headers.");
        Console.WriteLine($"Environment.CurrentDirectory: {Environment.CurrentDirectory}.");
        Console.WriteLine($"License: {license.Name}.");
        Console.WriteLine($"Include: {string.Join(", ", include)}.");
        Console.WriteLine($"Exclude: {string.Join(", ", exclude)}.");

        var languageProviders = new List<FileHeaderLanguageProvider>
        {
            new CSharpFileHeaderProvider(),
            new XmlFileHeaderProvider(),
            new YamlFileHeaderProvider()
        };

        foreach (var file in files)
        {
            Console.WriteLine(file);
            var languageProvider = languageProviders.FirstOrDefault(lp => lp.CanPutFileHeaderOnFile(file));
            if (languageProvider == null)
            {
                Log.LogMessage($"No language provider found for file {file}");
                changedFiles.Add(new TaskItem(file, new Dictionary<string, string>
                {
                    { "Status", "Skipped" },
                    { "Reason", "No language provider found" }
                }));
                continue;
            }

            languageProvider.WriteFileHeader(file, license, Authors, ProjectUrl);
            changedFiles.Add(new TaskItem(file, new Dictionary<string, string>
            {
                { "Status", "Updated" }
            }));
        }

        return true;
    }

    private class DirectoryInfoWrapper : DirectoryInfoBase
    {
        private readonly DirectoryInfo _directoryInfo;

        public DirectoryInfoWrapper(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public override DirectoryInfoBase ParentDirectory => new DirectoryInfoWrapper(_directoryInfo.Parent);

        public override string FullName => _directoryInfo.FullName;

        public override string Name => _directoryInfo.Name;

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
        {
            return _directoryInfo.EnumerateFileSystemInfos().Select(f => f is DirectoryInfo d ? (FileSystemInfoBase)new DirectoryInfoWrapper(d) : new FileInfoWrapper((FileInfo)f));
        }

        public override DirectoryInfoBase GetDirectory(string path)
        {
            return new DirectoryInfoWrapper(_directoryInfo.GetDirectories(path).Single());
        }

        public override FileInfoBase GetFile(string path)
        {
            return new FileInfoWrapper(_directoryInfo.GetFiles(path).Single());
        }
    }
}
