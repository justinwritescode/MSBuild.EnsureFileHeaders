/*
 * <file name="FileHeaderArgs.cs" created="Saturday, 29th October 2022 1:43:22 pm" modified="<<filemodified('dddd, Do MMMM YYYY h:mm:ss a')>>">
 *   <author name="Justin Chase" email="justin@justinwritescode.com" />
 *   <copyright year="projectCreationYear-2022" copyrightHolder="JustinWritesCode" license="<<license>>" />
 * </file>
 */

namespace MSBuild.EnsureFileHeaders;

public class FileHeaderArgs
{
    public string FilePath { get; set; }
    public SoftwareLicense License { get; set; }
    public ITaskItem[] Authors { get; set; }
    public string ProjectUrl { get; set; }
    public FileHeaderFormat Format { get; set; }
}
