using System.Reflection;

[assembly: System.Reflection.AssemblyCompanyAttribute("MicroCube")]
[assembly: System.Reflection.AssemblyCopyrightAttribute("Copyright Alex Dillon, 2019")]
[assembly: System.Reflection.AssemblyDescriptionAttribute("GroupMe Desktop Client Avalonia is an open-source, cross platform, modular client" +
    " for GroupMe messenging. Support is provided for Linux, macOS, and Windows.")]
[assembly: System.Reflection.AssemblyProductAttribute("GroupMe Desktop Client Avalonia")]
[assembly: System.Reflection.AssemblyTitleAttribute("GroupMeClientAvalonia")]

// AssemblyVersion = full version info, major.minor.patch
[assembly: AssemblyVersion(ThisAssembly.SimpleVersion)]

// FileVersion = full version info, major.minor.patch
[assembly: AssemblyFileVersion(ThisAssembly.SimpleVersion)]

// InformationalVersion = full version + branch + commit sha.
[assembly: AssemblyInformationalVersion(ThisAssembly.InformationalVersion)]

/// <summary>
/// Defines Version Numbers used for automatic assembly versioning.
/// </summary>
public partial class ThisAssembly
{
    /// <summary>
    /// Simple release-like version number, like 4.0.1.
    /// </summary>
    public const string SimpleVersion = Git.BaseVersion.Major + "." + Git.BaseVersion.Minor + "." + Git.BaseVersion.Patch;

    /// <summary>
    /// Full version, plus branch and commit short sha, like 4.0.1-39cf84e-branch.
    /// </summary>
    public const string InformationalVersion = SimpleVersion + "-" + Git.Commit + "+" + Git.Branch;
}
