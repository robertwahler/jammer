using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SDD;

namespace Jammer {

  /// <summary>
  /// Manages version information
  ///
  /// Format semver-label+code.build
  ///   Example: 1.0.0-dev+1.81a86ec
  /// </summary>
  public class Version {

    /// <summary>
    /// Base version string. This is written to the "Bundle Version" in the
    /// mobile player project settings.
    ///
    /// This can be automated by
    ///
    ///    git tag | tail -n 1
    ///
    /// </summary>
    public string version = "0.0.0";

    /// <summary>
    /// Used by Google play store to determine if one version is newer than
    /// another version.  This written to "Bundle Version Code" for Android in
    /// project settings.
    ///
    /// This can be automated by counting the git tags
    ///
    ///    git tag | grep -c [0-9]
    ///
    /// </summary>
    public int code = 1;

    /// <summary>
    /// Base version string. This is is displayed on the end of the version to
    /// distinguish builds.
    ///
    /// This can be automated by
    ///
    ///    git log --pretty=format:%h --abbrev-commit -1
    ///
    /// </summary>
    public string build = "0000000";

    /// <summary>
    /// Prerelease label metadata. Optional and manually added.
    /// </summary>
    public string label;

    /// <summary>
    /// Application.unityVersion, written at build time by Editor/Builders/Build.cs
    /// </summary>
    public string unity = "5.2";

    /// <summary>
    /// Single string of characters representing build flags. Determined at runtime.
    /// </summary>
    public string BuildFlags { get { return GetBuildFlags(); }}

    private string GetBuildFlags() {
      //            S                  L                   C
      // DebugFlags.Stats | DebugFlags.Logger | DebugFlags.Console
      // Duplicates will be removed
      List<string> flags = new List<string>();

      #if SDD_DEBUG
        flags.Add("D");
        #if SDD_LOG_DEBUG
          flags.Add("L0");
        #endif
        #if SDD_LOG_VERBOSE
          flags.Add("L1");
        #endif
      #endif

      #if SDD_CONSOLE
        flags.Add("C");
      #endif

      return string.Join("", flags.Distinct().ToArray());
    }

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString() {
      string _label = string.IsNullOrEmpty(label) ? "" : "-" + label;
      return string.Format("{0}{1}+{2}.{3}.{4}", version, _label, code.ToString(), build, unity);
    }
  }
}
