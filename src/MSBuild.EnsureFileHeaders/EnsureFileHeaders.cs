namespace JustinWritesCode.MSBuild.EnsureFileHeaders;
using JustinWritesCode.GitHub;
using Microsoft.Build.Utilities;
using System.Linq;

public class EnsureFileHeaders : MSBTask
{
    [Required]
    public string SearchRoot { get; set; } = string.Empty;
    [Required]
    public string License { get; set; }
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
        var include = Include.Select(i => i.ItemSpec).ToList();
        var exclude = Exclude?.Select(i => i.ItemSpec).ToList() ?? new List<string>();

        var files = new DirectoryInfo(SearchRoot).GetFiles("*.*", SearchOption.AllDirectories)
            .Where(f => include.Any(i => f.FullName.EndsWith(i, StringComparison.OrdinalIgnoreCase)))
            .Where(f => !exclude.Any(i => f.FullName.EndsWith(i, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var languageProviders = new List<FileHeaderLanguageProvider>
        {
            new CSharpFileHeaderProvider(),
            new XmlFileHeaderProvider(),
            new YamlFileHeaderProvider()
        };

        foreach (var file in files)
        {
            var languageProvider = languageProviders.FirstOrDefault(lp => lp.CanPutFileHeaderOnFile(file.FullName));
            if (languageProvider == null)
            {
                Log.LogMessage($"No language provider found for file {file.FullName}");
                changedFiles.Add(new TaskItem(file.FullName, new Dictionary<string, string>
                {
                    { "Status", "Skipped" },
                    { "Reason", "No language provider found" }
                }));
                continue;
            }

            languageProvider.WriteFileHeader(file.FullName, license.Content, Authors);
            changedFiles.Add(new TaskItem(file.FullName, new Dictionary<string, string>
            {
                { "Status", "Updated" }
            }));
        }

        return true;
    }
}
