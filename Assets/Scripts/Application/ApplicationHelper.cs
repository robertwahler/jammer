using UnityEngine;
using Newtonsoft.Json;
using SDD;

namespace Jammer {

  public static class ApplicationHelper {

    public static void Quit() {
      #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
      #elif UNITY_WEBPLAYER || UNITY_WEBGL
        Application.OpenURL(webplayerQuitURL);
      #else
        Application.Quit();
      #endif
    }

    public static bool IsMobile() {
      return ((Application.platform == RuntimePlatform.tvOS) || (Application.platform == RuntimePlatform.IPhonePlayer) || (Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.WP8Player));
    }

    public static bool IsDesktop() {
      return (Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer) ||
             (Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer) ||
             (Application.platform == RuntimePlatform.LinuxPlayer);
    }

    /// <summary>
    /// The folder where the application exe is stored. Assets can be shipped
    /// read-only in this location. i.e. mods, branding, etc.  We don't want to
    /// use the unity data folder, so return the folder above. This is an
    /// absolute path.
    /// </summary>
    #if UNITY_STANDALONE_OSX && !UNITY_EDITOR
      // macOS gives a useable location directly
      public static string AppPath = Application.dataPath;
    #else
      // Unity Editor
      //    ./
      //
      // macOS
      //    ./Jammer.app/Contents
      //
      // Linux
      //   ./Jammer/
      //
      // Windows
      //   ./Jammer/
      public static string AppPath = System.IO.Directory.GetParent(Application.dataPath).ToString();
    #endif

    /// <summary>
    /// Application assets can be pulled out of Assets/Resources for modding by users.
    /// </summary>
    public static string AppAssets = System.IO.Path.Combine(AppPath, "Assets/Resources");

    /// <summary>
    /// Application version deserialized from a JSON resource 'version.txt'
    /// </summary>
    public static Version Version {
      get {
        if (version == null) {
          try {
            TextAsset textData = (TextAsset) Resources.Load("Version", typeof(TextAsset));
            if (textData != null) {
              version = JsonConvert.DeserializeObject<Version>(textData.text);
            }
            else {
              Log.Error(string.Format("ApplicationHelper.Version failed to load Version.txt from resources"));
            }
          }
          catch (System.Exception e) {
            Log.Error(string.Format("ApplicationHelper.Version failed with {0}", e.ToString()));
          }
        }
        return version;
      }
    }
    private static Version version;

  }
}

