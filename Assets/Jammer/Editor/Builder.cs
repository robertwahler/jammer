using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;

using Jammer.Extensions;

namespace Jammer.Editor {

  // <https://docs.unity3d.com/Documentation/ScriptReference/BuildTarget.html>
  // <https://docs.unity3d.com/Documentation/ScriptReference/BuildOptions.html>

  public static class Builder {

    public static string Code {
      get {
        return ApplicationConstants.ProductCode;
      }
    }

    // public static BuildOptions developmentOptions = BuildOptions.SymlinkLibraries| BuildOptions.Development|BuildOptions.ConnectWithProfiler|BuildOptions.AllowDebugging;
    public static BuildOptions developmentOptions = BuildOptions.Development;

    public static BuildOptions releaseOptions = BuildOptions.None;

    /// <summary>
    /// Static construtor
    /// </summary>
    static Builder() {
      // not used
    }

    //
    // SWITCH TARGETS. NOTE: This is done automatically when a build is requested.
    //

    [MenuItem("Window/Jammer/Builder/Target/OSX", false, Shortcuts.MENU_BUILDER_TARGET + 10)]
    public static void SwitchBuildTargetOSX() {
      ConfigureCompilerDefines(target: BuildTarget.StandaloneOSXUniversal, opt: developmentOptions);
      EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSXUniversal);
    }

    [MenuItem("Window/Jammer/Builder/Target/Windows", false, Shortcuts.MENU_BUILDER_TARGET + 20)]
    public static void SwitchBuildTargetWindows() {
      ConfigureCompilerDefines(target: BuildTarget.StandaloneWindows, opt: developmentOptions);
      EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Window/Jammer/Builder/Target/Linux", false, Shortcuts.MENU_BUILDER_TARGET + 30)]
    public static void SwitchBuildTargetLinux() {
      ConfigureCompilerDefines(target: BuildTarget.StandaloneLinuxUniversal, opt: developmentOptions);
      EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneLinuxUniversal);
    }

    [MenuItem("Window/Jammer/Builder/Target/iOS", false, Shortcuts.MENU_BUILDER_TARGET + 40)]
    public static void SwitchBuildTargetIOS() {
      ConfigureCompilerDefines(target: BuildTarget.iOS, opt: developmentOptions);
      EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
    }

    [MenuItem("Window/Jammer/Builder/Target/Android", false, Shortcuts.MENU_BUILDER_TARGET + 50)]
    public static void SwitchBuildTargetAndroid() {
      ConfigureCompilerDefines(target: BuildTarget.Android, opt: developmentOptions);
      EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
    }

    [MenuItem("Window/Jammer/Builder/Target/WebGL", false, Shortcuts.MENU_BUILDER_TARGET + 200)]
    public static void SwitchBuildTargetWebGL() {
      ConfigureCompilerDefines(target: BuildTarget.WebGL, opt: developmentOptions);
      EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebGL);
    }

    [MenuItem("Window/Jammer/Builder/Target/tvOS", false, Shortcuts.MENU_BUILDER_TARGET + 210)]
    public static void SwitchBuildTargetTvOS() {
      ConfigureCompilerDefines(target: BuildTarget.tvOS, opt: developmentOptions);
      EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.tvOS);
    }

    //
    // DEVELOPMENT BUILDS
    //
    // NOTE: These can be run from the IDE but only the CLI allows saving,
    // loading, and customizing compiler defines
    //

    [MenuItem("Window/Jammer/Builder/Development/OSX", false, Shortcuts.MENU_BUILDER_DEVELOPMENT + 10)]
    public static void PerformOSXDevelopmentBuild() {
      Build(productCode: Code, target: BuildTarget.StandaloneOSXUniversal, opt: developmentOptions);
    }

    [MenuItem("Window/Jammer/Builder/Development/Windows", false, Shortcuts.MENU_BUILDER_DEVELOPMENT + 20)]
    public static void PerformWindowsDevelopmentBuild() {
      Build(productCode: Code, target: BuildTarget.StandaloneWindows, opt: developmentOptions);
    }

    [MenuItem("Window/Jammer/Builder/Development/Linux", false, Shortcuts.MENU_BUILDER_DEVELOPMENT + 30)]
    public static void PerformLinuxDevelopmentBuild() {
      Build(productCode: Code, target: BuildTarget.StandaloneLinuxUniversal, opt: developmentOptions);
    }

    [MenuItem("Window/Jammer/Builder/Development/iOS", false, Shortcuts.MENU_BUILDER_DEVELOPMENT + 40)]
    public static void PerformIOSDevelopmentBuild() {
      Build(productCode: Code, target: BuildTarget.iOS, opt: developmentOptions);
    }

    [MenuItem("Window/Jammer/Builder/Development/Android", false, Shortcuts.MENU_BUILDER_DEVELOPMENT + 50)]
    public static void PerformAndroidDevelopmentBuild() {
      Build(productCode: Code, target: BuildTarget.Android, opt: developmentOptions);
    }

    [MenuItem("Window/Jammer/Builder/Development/WebGL", false, Shortcuts.MENU_BUILDER_DEVELOPMENT + 200)]
    public static void PerformWebGLDevelopmentBuild() {
      Build(productCode: Code, target: BuildTarget.WebGL, opt: developmentOptions);
    }

    [MenuItem("Window/Jammer/Builder/Development/tvOS", false, Shortcuts.MENU_BUILDER_DEVELOPMENT + 210)]
    public static void PerformTvOSDevelopmentBuild() {
      Build(productCode: Code, target: BuildTarget.tvOS, opt: developmentOptions);
    }

    //
    // RELEASE BUILDS
    //
    // NOTE: These can be run from the IDE but only the CLI allows saving,
    // loading, and customizing compiler defines
    //

    [MenuItem("Window/Jammer/Builder/Release/OSX", false, Shortcuts.MENU_BUILDER_RELEASE + 10)]
    public static void PerformOSXReleaseBuild() {
      Build(productCode: Code, target: BuildTarget.StandaloneOSXUniversal, opt: releaseOptions);
    }

    [MenuItem("Window/Jammer/Builder/Release/Windows", false, Shortcuts.MENU_BUILDER_RELEASE + 20)]
    public static void PerformWindowsReleaseBuild() {
      Build(productCode: Code, target: BuildTarget.StandaloneWindows, opt: releaseOptions);
    }

    [MenuItem("Window/Jammer/Builder/Release/Linux", false, Shortcuts.MENU_BUILDER_RELEASE + 30)]
    public static void PerformLinuxReleaseBuild() {
      Build(productCode: Code, target: BuildTarget.StandaloneLinuxUniversal, opt: releaseOptions);
    }

    [MenuItem("Window/Jammer/Builder/Release/iOS", false, Shortcuts.MENU_BUILDER_RELEASE + 40)]
    public static void PerformIOSReleaseBuild() {
      Build(productCode: Code, target: BuildTarget.iOS, opt: releaseOptions);
    }

    [MenuItem("Window/Jammer/Builder/Release/Android", false, Shortcuts.MENU_BUILDER_RELEASE + 50)]
    public static void PerformAndroidReleaseBuild() {
      Build(productCode: Code, target: BuildTarget.Android, opt: releaseOptions);
    }

    [MenuItem("Window/Jammer/Builder/Release/WebGL", false, Shortcuts.MENU_BUILDER_RELEASE + 200)]
    public static void PerformWebGLReleaseBuild() {
      Build(productCode: Code, target: BuildTarget.WebGL, opt: releaseOptions);
    }

    [MenuItem("Window/Jammer/Builder/Release/tvOS", false, Shortcuts.MENU_BUILDER_RELEASE + 210)]
    public static void PerformTvOSReleaseBuild() {
      Build(productCode: Code, target: BuildTarget.tvOS, opt: releaseOptions);
    }

    /// <summary>
    /// Do the build
    /// </summary>
    public static void Build(string productCode, BuildTarget target, BuildOptions opt) {
      Log.Debug(string.Format("Builder.Build(productCode: {0}, target: {1}, opt: {2})", productCode, target, opt));

      // clean final destination build folder
      string buildFolder = BuildFolder(productCode, target);
      if (System.IO.Directory.Exists(buildFolder)) {
        Log.Debug(string.Format("Builder.Build() cleaning buildFolder {0}", buildFolder));
        Directory.Delete(buildFolder, recursive: true);
      }
      // Unity handles the switch here
      EditorUserBuildSettings.SwitchActiveBuildTarget(target);
      // command line overrides
      List<string> defines = ConfigureCommandLineOverrides();
      // configure product specific compiler defines
      ConfigureCompilerDefines(target, opt, defines);
      // update build sha and unity verison
      UpdateVersionResource(target);

      ConfigurePlayerSettings(target, opt);
      string filename = BuildFilename(productCode, target);
      string[] scenes = ScenePaths();

      try {
        Log.Debug(string.Format("Builder.Build() BuildPipeline.BuildPlayer(scenes: {0}, filename: {1}, target: {2}, opt: {3}", scenes, filename, target, opt));
        BuildPipeline.BuildPlayer(scenes, filename, target, opt);
        CopyRawResources(productCode, target, defines);
        CopyDocs(productCode, target);
      }
      finally {
        Log.Debug(string.Format("Builder.Build() finished"));
        // nothing to do yet
      }
    }

    public static string[] ScenePaths() {
      Log.Debug(string.Format("Builder.ScenePaths()"));

      string[] scenes = new string[EditorBuildSettings.scenes.Length];
      string path;

      for(int i = 0; i < scenes.Length; i++) {
        path = EditorBuildSettings.scenes[i].path;
        Log.Debug(string.Format("Builder.ScenePaths() path={0}", path));
        scenes[i] = path;
      }

      return scenes;
    }

    /// <summary>
    /// Configure compiler defines
    /// </summary>
    public static void ConfigureCompilerDefines(BuildTarget target, BuildOptions opt, List<string> defines = null) {
      Log.Debug(string.Format("Builder.ConfigureCompilerDefines(target: {0}, opt: {1}) Application.platform={2}", target, opt, Application.platform));

      // Basic compiler defines, override on a per target basis. Duplicates will be removed.
      if (defines == null) {
        defines = new List<string>();
      }

      // development defines
      if (opt.HasFlag(BuildOptions.Development)) {
        defines.Add("JAMMER_DEBUG");
        defines.Add("JAMMER_DEBUG_OVERLAY");
        defines.Add("JAMER_LOG_DEBUG");
        defines.Add("JAMMER_CONSOLE");
        defines.Add("JAMMER_DEBUG_RAYCASTS");
      }

      // remove current dupes, more dupes may still be added later
      defines = defines.Distinct().ToList();

      switch(target) {

        case BuildTarget.tvOS:
          Log.Debug(string.Format("Builder.ConfigureCompilerDefines() tvOS defines={0}", string.Join(";", defines.Distinct().ToArray())));
          PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.tvOS, string.Join(";", defines.Distinct().ToArray()));
          break;

        case BuildTarget.iOS:
          Log.Debug(string.Format("Builder.ConfigureCompilerDefines() iOS defines={0}", string.Join(";", defines.Distinct().ToArray())));
          PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, string.Join(";", defines.Distinct().ToArray()));
          break;

        case BuildTarget.StandaloneOSXIntel64:
        case BuildTarget.StandaloneOSXIntel:
        case BuildTarget.StandaloneOSXUniversal:
        case BuildTarget.StandaloneWindows:
        case BuildTarget.StandaloneWindows64:
        case BuildTarget.StandaloneLinux:
        case BuildTarget.StandaloneLinux64:
        case BuildTarget.StandaloneLinuxUniversal:
          Log.Debug(string.Format("Builder.ConfigureCompilerDefines() Standalone defines={0}", string.Join(";", defines.Distinct().ToArray())));
          PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", defines.Distinct().ToArray()));
          break;

        case BuildTarget.Android:
          Log.Debug(string.Format("Builder.ConfigureCompilerDefines() Android defines={0}", string.Join(";", defines.Distinct().ToArray())));
          PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, string.Join(";", defines.Distinct().ToArray()));
          break;

        case BuildTarget.WebGL:
          Log.Debug(string.Format("Builder.ConfigureCompilerDefines() WebGL defines={0}", string.Join(";", defines.Distinct().ToArray())));
          PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, string.Join(";", defines.Distinct().ToArray()));
          break;

        default:
          Log.Error(string.Format("Builder.ConfigureCompilerDefines(target: {1}, opt: {2}) Unknown target", target, opt));
          break;
      }
    }

    /// <summary>
    /// Configure non-persistent player settings
    /// </summary>
    public static void ConfigurePlayerSettings(BuildTarget target, BuildOptions opt) {
      Log.Debug(string.Format("Builder.ConfigurePlayerSettings(target: {0}, opt: {1}) Application.platform={2}", target, opt,  Application.platform));

      switch(target) {

        case BuildTarget.tvOS:
        case BuildTarget.iOS:
        case BuildTarget.StandaloneOSXUniversal:
        case BuildTarget.StandaloneWindows:
        case BuildTarget.StandaloneLinuxUniversal:
        case BuildTarget.WebGL:
          break;

        case BuildTarget.Android:
          string password = "";
          string key = "Jammer Android Keystore";

          if (Application.platform == RuntimePlatform.OSXEditor) {
            password = FindKeychainPassword(key);
          }
          if (!string.IsNullOrEmpty(password)) {
            PlayerSettings.Android.keystorePass = password;
            PlayerSettings.Android.keyaliasPass = password;
          }
          else {
            Log.Error(string.Format("Builder.ConfigurePlayerSettings(target: {0}) unable to find password for Android keystore key {1}", target, key));
          }
          break;

        default:
          Log.Error(string.Format("Builder.ConfigurePlayerSettings(target: {0}) target unknown", target));
          break;
      }
    }

    /// <summary>
    /// Configure overrides sent in via the CLI. NOTE: will not work from Unity
    /// IDE menus. Returns a list of compiler defines.
    /// </summary>
    public static List<string> ConfigureCommandLineOverrides() {
      List<string> defines = new List<string>();

      // read command line
      string[] args = System.Environment.GetCommandLineArgs();
      string commandLine = string.Join(" ", args);
      Log.Debug(string.Format("Builder.ConfigureCommandLineOverrides() commandLine={0}", commandLine));

      foreach (string arg in args) {
        if (Regex.IsMatch(arg, @"--defines")) {
          // This handles quoted paths with spaces but leaves the quotes in place
          Regex regex = new Regex(@"--defines\s+(?<defines>[\""].+?[\""]|[^ ]+)");
          Match match = regex.Match(commandLine);
          if (match.Success) {
            string d = match.Groups["defines"].Value;
            defines = d.Split(',').ToList();
          }
        }
      }

      return defines.Distinct().ToList();
    }

    /// <summary>
    /// Update version numbers in resources. Normally called before a build.
    /// </summary>
    public static void UpdateVersionResource(BuildTarget target) {
      string build = RunProcess(binary: "git", args: "log --pretty=format:%h --abbrev-commit -1");
      string unityVersion = Application.unityVersion;
      var paths = new List<string> {"Assets", "Resources", "Version.txt"};
      string filename = paths.Aggregate(System.IO.Path.Combine);
      ApplicationHelper.Version.unity = unityVersion;
      ApplicationHelper.Version.build = build;
      string json = JsonConvert.SerializeObject(value: ApplicationHelper.Version, formatting: Formatting.Indented);
      System.IO.File.WriteAllText(filename, json);
    }

    /// <summary>
    /// Return the build folder given code and target
    /// </summary>
    public static string BuildFolder(string productCode, BuildTarget target) {
      Log.Debug(string.Format("Builder.BuildFolder(productCode: {0}, target: {1})", productCode, target));

      switch (target) {

        case BuildTarget.StandaloneOSXUniversal:
          return string.Format("build/{0}.app", productCode);

        case BuildTarget.StandaloneWindows:
          return string.Format("build/{0}.Windows", productCode);

        case BuildTarget.StandaloneLinuxUniversal:
          return string.Format("build/{0}.Linux", productCode);

        case BuildTarget.tvOS:
          return string.Format("build/{0}.tvOS", productCode);

        case BuildTarget.iOS:
          return string.Format("build/{0}.iOS", productCode);

        case BuildTarget.Android:
          return string.Format("build/{0}.apk", productCode);

        case BuildTarget.WebGL:
          return string.Format("build/{0}.WebGL", productCode);

        default:
          throw new System.InvalidOperationException(string.Format("BuildFolder() unknown target={target}", target));
      }
    }

    /// <summary>
    /// Return build folder for docs,  mods, branding, workshop files, etc
    /// </summary>
    public static string BuildFolderApp(string productCode, BuildTarget target) {
      Log.Debug(string.Format("Builder.BuildFolderApp(productCode: {0}, target: {1})", productCode, target));

      string folder = BuildFolder(productCode, target);

      switch (target) {

        case BuildTarget.StandaloneOSXUniversal:
          return string.Format("{0}/Contents", folder);

        default:
          // all other targets use the main folder as the docs folder
          return folder;
      }
    }

    /// Return build folder for the executable binary. This is where steam_appid.txt lives
    /// </summary>
    public static string BuildFolderExe(string productCode, BuildTarget target) {
      Log.Debug(string.Format("Builder.BuildFolderApp(productCode: {0}, target: {1})", productCode, target));

      string folder = BuildFolder(productCode, target);

      switch (target) {

        case BuildTarget.StandaloneOSXUniversal:
          return string.Format("{0}/Contents/MacOS", folder);

        default:
          // all other targets use the main folder as the docs folder
          return folder;
      }
    }

    /// <summary>
    /// Return filename given code and target
    /// </summary>
    public static string BuildFilename(string productCode, BuildTarget target) {
      Log.Debug(string.Format("Builder.BuildFilename(productCode: {0}, target: {1})", productCode, target));

      string folder = BuildFolder(productCode, target);

      switch (target) {

        case BuildTarget.StandaloneWindows:
          return string.Format("{0}/{1}.exe", folder, productCode);

        case BuildTarget.StandaloneLinuxUniversal:
          return string.Format("{0}/{1}.x86", folder, productCode);

        case BuildTarget.Android:
          return string.Format("{0}/{1}.apk", folder, productCode);

        default:
          // all other targets use the folder as the filename
          return folder;
      }
    }

    /// <summary>
    /// Copy resources (version.txt JSON) to build folder
    /// </summary>
    public static void CopyRawResources(string productCode, BuildTarget target, List<string> defines) {
      Log.Debug(string.Format("Builder.CopyRawResources(productCode: {0}, target: {1})", productCode, target));

      // Version.txt
      string source = string.Format("{0}/Assets/Resources/Version.txt", System.IO.Directory.GetCurrentDirectory());
      string destination = string.Format("{0}/version.txt", BuildFolderApp(productCode, target));
      Log.Debug(string.Format("Builder.CopyRawResources(productCode: {0}, target: {1}) copying {2} to {3}", productCode, target, source, destination));
      System.IO.File.Copy(sourceFileName: source, destFileName: destination, overwrite: true);

      if (defines.Contains("JAMMER_STEAM")) {
        // steam_appid.txt (otherwise, app won't run properly unless started by steam directly
        source = string.Format("{0}/steam_appid.txt", System.IO.Directory.GetCurrentDirectory());
        destination = string.Format("{0}/steam_appid.txt", BuildFolderExe(productCode, target));
        Log.Debug(string.Format("Builder.CopyRawResources(productCode: {0}, target: {1}) copying {2} to {3}", productCode, target, source, destination));
        System.IO.File.Copy(sourceFileName: source, destFileName: destination, overwrite: true);
      }
    }

    /// <summary>
    /// Copy documents (HISTORY.txt, etc) to build folder
    /// </summary>
    public static void CopyDocs(string productCode, BuildTarget target) {
      Log.Debug(string.Format("Builder.CopyDocs(productCode: {0}, target: {1})", productCode, target));

      string sourceFolder = string.Format("{0}/doc", System.IO.Directory.GetCurrentDirectory());
      string destinationFolder = BuildFolderApp(productCode, target);

      string source = string.Format("{0}/HISTORY.txt", sourceFolder);
      string destination = string.Format("{0}/HISTORY.txt", destinationFolder);

      Log.Debug(string.Format("Builder.CopyDocs(productCode: {0}, target: {1}) copying {2} to {3}", productCode, target, source, destination));
      System.IO.File.Copy(sourceFileName: source, destFileName: destination, overwrite: true);
    }

    /// <summary>
    /// Run external process
    /// </summary>
    public static string RunProcess(string binary, string args) {
      Log.Debug(string.Format("Builder.RunProcess(binary: {0}, args: {1}", binary, args));

      System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
      pProcess.StartInfo.FileName = binary;
      pProcess.StartInfo.Arguments = args;
      pProcess.StartInfo.UseShellExecute = false;
      pProcess.StartInfo.RedirectStandardOutput = true;
      pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
      pProcess.StartInfo.CreateNoWindow = true;
      pProcess.Start();
      string output = pProcess.StandardOutput.ReadToEnd(); //The output result
      pProcess.WaitForExit();

      return output;
    }

    /// <summary>
    /// OSX builds use keychain to set Android keystore password. Use the
    /// KeyChain app to add the following password for Account "Jammer" and Name "Jammer
    /// Android KeyStore".
    ///
    /// Read on CLI
    ///
    ///     security -q find-generic-password  -l "Jammer Android Keystore" -w
    ///
    /// </summary>
    /// <remarks>
    ///   <http://stackoverflow.com/questions/285760/how-to-spawn-a-process-and-capture-its-stdout-in-net>
    /// </remarks>
    public static string FindKeychainPassword(string name) {
      Log.Debug(string.Format("Builder.FindKeychainPassword(name: {0})", name));

      StringBuilder outputBuilder;
      ProcessStartInfo processStartInfo;
      Process process;

      outputBuilder = new StringBuilder();

      processStartInfo = new ProcessStartInfo();
      processStartInfo.CreateNoWindow = true;
      processStartInfo.RedirectStandardOutput = true;
      processStartInfo.RedirectStandardInput = true;
      processStartInfo.UseShellExecute = false;
      processStartInfo.Arguments = string.Format("-q find-generic-password  -l '{0}' -w", name);
      processStartInfo.FileName = "security";

      process = new Process();
      process.StartInfo = processStartInfo;
      // enable raising events because Process does not raise events by default
      process.EnableRaisingEvents = true;
      // attach the event handler for OutputDataReceived before starting the process
      process.OutputDataReceived += new DataReceivedEventHandler (
          delegate(object sender, DataReceivedEventArgs e) {
              // append the new data to the data already read-in
              outputBuilder.Append(e.Data);
          }
      );

      // start the process, async read waiting for exit
      process.Start();
      process.BeginOutputReadLine();
      process.WaitForExit();
      process.CancelOutputRead();

      string output = outputBuilder.ToString();

      return output;
    }

  }
}
